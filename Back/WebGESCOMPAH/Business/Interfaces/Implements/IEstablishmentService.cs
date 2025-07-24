using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.Establishment;

namespace Business.Interfaces.Implements
{
    public interface IEstablishmentService : IBusiness<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto>
    {
        Task DeleteAsync(int id, bool forceDelete);


    }

}
