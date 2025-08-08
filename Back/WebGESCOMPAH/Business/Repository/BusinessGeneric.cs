using Data.Interfaz.DataBasic;
using Entity.Domain.Models.ModelBase;
using MapsterMapper;
using Utilities.Exceptions;
using Utilities.Helpers.Business;

namespace Business.Repository
{
    public class BusinessGeneric<TDtoGet, TDtoCreate, TDtoUpdate, TEntity> : ABusinessGeneric<TDtoGet, TDtoCreate, TDtoUpdate, TEntity> where TEntity : BaseModel
    {
        protected readonly IDataGeneric<TEntity> Data;
        protected readonly IMapper _mapper;

        public BusinessGeneric(IDataGeneric<TEntity> data, IMapper mapper)
        {
            Data = data;
            _mapper = mapper;
        }

        public override async Task<IEnumerable<TDtoGet>> GetAllAsync()
        {
            try
            {
                var entities = await Data.GetAllAsync();
                return _mapper.Map<IEnumerable<TDtoGet>>(entities);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al obtener todos los registros.", ex);
            }

        }
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
        public override async Task<TDtoGet> CreateAsync(TDtoCreate dto)
        {
            try
            {
                BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");

                var entity = _mapper.Map<TEntity>(dto);
                entity.InitializeLogicalState(); // Inicializa estado lógico (is_deleted = false)

                var created = await Data.AddAsync(entity);
                return _mapper.Map<TDtoGet>(created);
            }
            catch (Exception ex)
            {
                throw new BusinessException("Error al crear el registro.", ex);
            }
        }
        public override async Task<TDtoGet> UpdateAsync(TDtoUpdate dto)
        {
            BusinessValidationHelper.ThrowIfNull(dto, "El DTO no puede ser nulo.");

            var entity = _mapper.Map<TEntity>(dto);
            entity.InitializeLogicalState();

            var updated = await Data.UpdateAsync(entity);
            return _mapper.Map<TDtoGet>(updated);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            try
            {
                BusinessValidationHelper.ThrowIfZeroOrLess(id, "El ID debe ser mayor que cero.");


                return await Data.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al eliminar el registro con ID {id}.", ex);
            }
        }

        public override async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                BusinessValidationHelper.ThrowIfZeroOrLess(id, "El ID debe ser mayor que cero.");


                return await Data.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al eliminar logicamente el registro con ID {id}.", ex);
            }
        }



    }
}
