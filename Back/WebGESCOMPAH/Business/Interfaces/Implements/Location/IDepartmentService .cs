using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Location.Department;

namespace Business.Interfaces.Implements.Location
{
    public interface IDepartmentService : IBusiness<DepartmentSelectDto, DepartmentCreateDto, DepartmentUpdateDto>
    {
    }
}
