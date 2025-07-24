using Business.Interfaces.Implements.Location;
using Entity.DTOs.Implements.Location.Department;
using MediatR;

namespace Business.CQRS.Location.Department.Select
{
    public class GetAllDepartmentQueryHandler : IRequestHandler<GetAllDepartmentQuery, IEnumerable<DepartmentSelectDto>>
    {
        private readonly IDepartmentService _departmentService;
        public GetAllDepartmentQueryHandler(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        public async Task<IEnumerable<DepartmentSelectDto>> Handle(GetAllDepartmentQuery request, CancellationToken cancellationToken)
        {
            return await _departmentService.GetAllAsync();
        }
    }
}
