using Business.Interfaces.Implements.Business;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Business;
using Entity.DTOs.Implements.Business.Clause;
using MapsterMapper;

namespace Business.Services.Business
{
    public class ClauseService : BusinessGeneric<ClauseSelectDto, ClauseDto, ClauseUpdateDto, Clause>, IClauseService
    {
        public ClauseService(IDataGeneric<Clause> data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}
