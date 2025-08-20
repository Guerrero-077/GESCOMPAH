using Data.Interfaz.DataBasic;
using Entity.Domain.Models.ModelBase;
using Entity.DTOs.Implements.Business.Plaza;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
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

        // ------------------------------------------------------------------------------------
        // 🔑 Punto de extensión de unicidad:
        //     - Devuelve la query filtrada por la “clave única” en base a 'candidate'.
        //     - Si devuelves null, se asume que NO hay regla de unicidad genérica a validar.
        //     - Úsalo en servicios concretos para definir Name único, claves compuestas, etc.
        // ------------------------------------------------------------------------------------
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

                // 1) Buscar duplicado (incluye inactivos porque GetAllQueryable no filtra IsDeleted)
                var query = ApplyUniquenessFilter(Data.GetAllQueryable(), candidate);
                if (query is not null)
                {
                    // Importante: FirstOrDefaultAsync ejecuta en BD sin trackeo (consulta segura)
                    var existing = await query.FirstOrDefaultAsync();
                    if (existing is not null)
                    {
                        if (!existing.IsDeleted)
                        {
                            // Duplicado activo ⇒ negocio decide fallar
                            throw new BusinessException("Ya existe un registro con los mismos datos.");
                        }

                        // 2) Duplicado inactivo ⇒ reactivar y actualizar campos desde el DTO
                        existing.IsDeleted = false;
                        _mapper.Map(dto, existing); // aplica cambios del DTO sobre la entidad “reactivada”
                        var updated = await Data.UpdateAsync(existing); // Data.UpdateAsync realiza el SaveChanges
                        return _mapper.Map<TDtoGet>(updated);
                    }
                }

                // 3) No existe duplicado ⇒ crear
                candidate.InitializeLogicalState(); // asegura IsDeleted = false (estado lógico inicial)
                var created = await Data.AddAsync(candidate);
                return _mapper.Map<TDtoGet>(created);
            }
            catch (DbUpdateException dbx)
            {
                // Si hay un índice único en BD, traducimos a excepción de negocio más clara
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
            BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");

            // Mapear a entidad con Id (asegúrate en tu BaseController de inyectar el Id de ruta en el DTO)
            var entity = _mapper.Map<TEntity>(dto);

            // No forzar IsDeleted = false aquí; deja que las reglas del servicio concreto lo controlen
            var updated = await Data.UpdateAsync(entity);
            return _mapper.Map<TDtoGet>(updated);
        }

        /// <summary>
        /// Eliminación física (borra el registro de la tabla).
        /// </summary>
        public override async Task<bool> DeleteAsync(int id)
        {
            try
            {
                BusinessValidationHelper.ThrowIfZeroOrLess(id, "El ID debe ser mayor que cero.");
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
                return await Data.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al eliminar lógicamente el registro con ID {id}.", ex);
            }
        }

        public override async Task<TDtoGet> UpdateActiveStatusAsync(int id, bool active)
        {
            var entity = await Data.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"No se encontró la plaza con ID {id}");

            if (entity.Active != active)
            {
                entity.Active = active;
                await Data.UpdateAsync(entity);
            }

            return _mapper.Map<TDtoGet>(entity);
        }

    }
}
