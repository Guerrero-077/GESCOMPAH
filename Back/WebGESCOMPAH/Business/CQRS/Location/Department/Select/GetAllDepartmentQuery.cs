using Entity.DTOs.Implements.Location.Department;
using MediatR;

namespace Business.CQRS.Location.Department.Select
{
    public class GetAllDepartmentQuery : IRequest<IEnumerable<DepartmentSelectDto>>
    {
    }
}
