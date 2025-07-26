using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplemenent;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Establishment;
using MapsterMapper;

namespace Business.Services.Business
{
    public class EstablishmentService(IEstablishments data, IMapper mapper) : BusinessGeneric<EstablishmentSelectDto, EstablishmentCreateDto, EstablishmentUpdateDto, Establishment>(data, mapper), IEstablishmentService
    {
    }
}
