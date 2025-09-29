using Data.Interfaz.DataBasic;
using Entity.Domain.Models.ModelBase;
using Entity.DTOs.Base;
using Entity.DTOs.Implements.Business.Plaza;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Utilities.Exceptions;
using Utilities.Helpers.Business;

namespace Business.Repository
{
    /// <summary>
    /// Servicio genérico de negocio para CRUD con soporte de:
    /// - Mapeo DTO &lt;=&gt; Entidad (Mapster)
    /// - Eliminado lógico (reactivación en Create si aplica)
    /// - Validación mínima de argumentos
    /// - Verificación de duplicados a través de ApplyUniquenessFilter (opcional por entidad)
    /// 
    /// NOTA: GetAllAsync/GetByIdAsync en Data filtran IsDeleted, mientras que GetAllQueryable NO lo filtra,
    /// lo que permite buscar duplicados incluyendo inactivos (soft-deleted).
    /// </summary>
    public class BusinessGeneric<TDtoGet, TDtoCreate, TDtoUpdate, TEntity>
        : ABusinessGeneric<TDtoGet, TDtoCreate, TDtoUpdate, TEntity> where TEntity : BaseModel
    {
        protected readonly IDataGeneric<TEntity> Data;
        protected readonly IMapper _mapper;

        public BusinessGeneric(IDataGeneric<TEntity> data, IMapper mapper)
        {
            Data = data;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtiene todos los registros ACTIVOS (Data.GetAllAsync ya excluye IsDeleted).
        /// </summary>
        public override async Task<IEnumerable<TDtoGet>> GetAllAsync()
        {
            try
            {
                var entities = await Data.GetAllAsync();
                return _mapper.Map<IEnumerable<TDtoGet>>(entities);
            }
            catch (Exception ex)
            {
                // Envuelve cualquier fallo para mantener una capa de negocio coherente
                throw new BusinessException("Error al obtener todos los registros.", ex);
            }
        }

        /// <summary>
        /// Obtiene un registro por ID si está ACTIVO (Data.GetByIdAsync ya excluye IsDeleted).
        /// </summary>
        public override async Task<TDtoGet?> GetByIdAsync(int id)
        {
            try
            {
                BusinessValidationHelper.ThrowIfZeroOrLess(id, "El ID debe ser mayor que cero.");

                var entity = await Data.GetByIdAsync(id);
                return entity == null ? default : _mapper.Map<TDtoGet>(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener el registro con ID {id}.", ex);
            }
        }

        // Punto de extensión: aplica filtro de unicidad para detectar duplicados.
        protected virtual IQueryable<TEntity>? ApplyUniquenessFilter(IQueryable<TEntity> query, TEntity candidate)
            => null;

        /// <summary>
        /// Crea un nuevo registro:
        /// - Si ApplyUniquenessFilter no es null y existe un duplicado ACTIVO ⇒ error.
        /// - Si existe un duplicado INACTIVO ⇒ se reactiva y se actualiza con el DTO.
        /// - Si no existe ⇒ se crea desde cero.
        /// </summary>
        public override async Task<TDtoGet> CreateAsync(TDtoCreate dto)
        {
            try
            {
                BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");
                var candidate = _mapper.Map<TEntity>(dto);

                // Buscar duplicado (incluye inactivos; GetAllQueryable no filtra IsDeleted)
                var query = ApplyUniquenessFilter(Data.GetAllQueryable(), candidate);
                if (query is not null)
                {
                    var existing = query.FirstOrDefault();
                    if (existing is not null)
                    {
                        if (!existing.IsDeleted)
                        {
                            // Duplicado activo ⇒ falla
                            throw new BusinessException("Ya existe un registro con los mismos datos.");
                        }

                        // Duplicado inactivo ⇒ reactivar y actualizar
                        existing.IsDeleted = false;
                        _mapper.Map(dto, existing);
                        var updated = await Data.UpdateAsync(existing);
                        return _mapper.Map<TDtoGet>(updated);
                    }
                }

                // No existe duplicado ⇒ crear
                candidate.InitializeLogicalState();
                var created = await Data.AddAsync(candidate);
                return _mapper.Map<TDtoGet>(created);
            }
            catch (DbUpdateException dbx)
            {
                // Índice único en BD ⇒ mensaje de negocio claro
                throw new BusinessException("Violación de unicidad al crear el registro. Revisa valores únicos.", dbx);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al crear el registro.", ex);
            }
        }

        /// <summary>
        /// Actualiza un registro (respeta IsDeleted tal cual venga en la entidad mapeada).
        /// Recomendación: en servicios concretos valida estados (ej. no permitir update si está IsDeleted).
        /// </summary>
        public override async Task<TDtoGet> UpdateAsync(TDtoUpdate dto)
        {
            try
            {
                BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");
                var entity = _mapper.Map<TEntity>(dto);
                var updated = await Data.UpdateAsync(entity);
                return _mapper.Map<TDtoGet>(updated);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al actualizar el registro.", ex);
            }
        }

        /// <summary>
        /// Eliminación física (borra el registro de la tabla).
        /// </summary>
        public override async Task<bool> DeleteAsync(int id)
        {
            try
            {
                BusinessValidationHelper.ThrowIfZeroOrLess(id, "El ID debe ser mayor que cero.");

                var entity = await Data.GetByIdAsync(id);
                if (entity == null)
                {
                    return false; // Or throw a KeyNotFoundException
                }

                if (entity.Active)
                {
                    throw new BusinessException("No se puede eliminar un registro que se encuentra activo.");
                }

                return await Data.DeleteAsync(id);
            }
            catch (DbUpdateException dbx)
            {
                // Constraints FK, etc.
                throw new BusinessException($"No se pudo eliminar el registro con ID {id} por restricciones de datos.", dbx);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al eliminar el registro con ID {id}.", ex);
            }
        }

        /// <summary>
        /// Eliminación lógica (marca IsDeleted = true).
        /// </summary>
        public override async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                BusinessValidationHelper.ThrowIfZeroOrLess(id, "El ID debe ser mayor que cero.");

                var entity = await Data.GetByIdAsync(id);
                if (entity == null)
                {
                    return false; // Or throw a KeyNotFoundException
                }

                if (entity.Active)
                {
                    throw new BusinessException("No se puede eliminar un registro que se encuentra activo.");
                }

                return await Data.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al eliminar lógicamente el registro con ID {id}.", ex);
            }
        }

        


        public override async Task UpdateActiveStatusAsync(int id, bool active)
        {
            try
            {
                BusinessValidationHelper.ThrowIfZeroOrLess(id, "El ID debe ser mayor que cero.");
                var entity = await Data.GetByIdAsync(id)
                    ?? throw new KeyNotFoundException($"No se encontró el registro con ID {id}.");

                if (entity.Active == active) return;

                entity.Active = active;
                await Data.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al actualizar el estado del registro con ID {id}.", ex);
            }
        }


        /// <summary>
        /// Campos de TEntity sobre los que aplicar "Search" (Contains).
        /// Ej.: return new[] { nameof(Plaza.Name), nameof(Plaza.Code) };
        /// </summary>
        protected virtual Expression<Func<TEntity, string>>[] SearchableFields() => Array.Empty<Expression<Func<TEntity, string>>>();


        /// <summary>
        /// Lista blanca de campos sobre los que se permite ordenar.
        /// </summary>
        protected virtual string[] SortableFields() => Array.Empty<string>();

        /// <summary>
        /// Mapa opcional de claves de orden -> selector fuertemente tipado.
        /// Si se provee, Data usará estas expresiones en lugar de EF.Property.
        /// Las claves deben coincidir con las aceptadas desde el front.
        /// </summary>
        protected virtual IDictionary<string, LambdaExpression> SortMap()
            => new Dictionary<string, LambdaExpression>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Lista blanca de filtros (propiedad -> builder de expresión) para "Filters".
        /// Controla QUÉ se puede filtrar desde la Web (hardening).
        /// </summary>
        protected virtual IDictionary<string, Func<string, Expression<Func<TEntity, bool>>>> AllowedFilters()
            => new Dictionary<string, Func<string, Expression<Func<TEntity, bool>>>>();

        /// <summary>
        /// Ejecuta consulta genérica: valida filtros permitidos y delega a Data.
        /// </summary>
        public override async Task<PagedResult<TDtoGet>> QueryAsync(PageQuery query)
        {
            try
            {
                // 1) Traducir filtros permitidos a expresiones tipadas
                var safeFilters = new List<Expression<Func<TEntity, bool>>>();
                if (query.Filters is not null)
                {
                    var allow = AllowedFilters();
                    foreach (var (k, v) in query.Filters)
                    {
                        if (allow.TryGetValue(k, out var builder))
                            safeFilters.Add(builder(v));
                        // Filtros no permitidos: se ignoran (o lanzar excepción si lo prefieres)
                    }
                }

                // Validate sort field contra SortMap (si existe) o SortableFields
                var sortOk = SortMap().ContainsKey(query.Sort ?? string.Empty)
                             || SortableFields().Contains(query.Sort, StringComparer.OrdinalIgnoreCase);
                if (!sortOk)
                {
                    query = query with { Sort = null };
                }

                // 2) Data ejecuta server-side con Search + Sort + Paginado
                var result = await Data.QueryAsync(
                    query,
                    SearchableFields(),
                    safeFilters.ToArray(),
                    SortMap()
                );

                // 3) Mapear entidades -> DTOs de salida
                return new PagedResult<TDtoGet>(
                    Items: _mapper.Map<IEnumerable<TDtoGet>>(result.Items).ToList(),
                    Total: result.Total,
                    Page: result.Page,
                    Size: result.Size
                );
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error en consulta genérica.", ex);
            }
        }

    }
}
