using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Location.Select;

namespace Business.Interfaces.Implements
{
    public interface IDepartmentService : IBusiness<DepartmentSelectDto, DepartmentSelectDto, DepartmentSelectDto>
    {
    }
}
