using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Plaza;
using MapsterMapper;

namespace Business.Services.Business
{
    public class PlazasService : BusinessGeneric<PlazaSelectDto, PlazaCreateDto, PlazaUpdateDto, Plaza>, IPlazaService
    {
        private readonly IDataGeneric<Plaza> _data;
        public PlazasService(IDataGeneric<Plaza> data, IMapper mapper) : base(data, mapper)
        {
            _data = data;
        }
     
        public async Task<PlazaSelectDto> UpdateActiveStatusAsync(int id, bool active)
        {
            var entity = await _data.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"No se encontró la plaza con ID {id}");

            if (entity.Active != active)
            {
                entity.Active = active;
                await _data.UpdateAsync(entity);
            }

            return _mapper.Map<PlazaSelectDto>(entity);
        }



    }
}
