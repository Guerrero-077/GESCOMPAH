using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Business.Clause;

namespace Business.Interfaces.Implements.Business
{
    public interface IClauseService : IBusiness<ClauseSelectDto, ClauseDto, ClauseUpdateDto>
    {
    }
}
