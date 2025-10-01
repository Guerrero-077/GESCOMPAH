using Business.Interfaces.Implements.Location;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Location;
using Entity.DTOs.Implements.Location.Department;
using MapsterMapper;
using System.Linq.Expressions;

namespace Business.Services.Location
{
    public class DepartmentService : BusinessGeneric<DepartmentSelectDto, DepartmentCreateDto, DepartmentUpdateDto, Department>, IDepartmentService
    {

        public DepartmentService(IDataGeneric<Department> data, IMapper mapper) : base(data, mapper)
        {
        }

        protected override IQueryable<Department>? ApplyUniquenessFilter(IQueryable<Department> query, Department candidate)
            => query.Where(d => d.Name == candidate.Name);

        protected override Expression<Func<Department, string>>[] SearchableFields() => 
        [
            d => d.Name
        ];

        protected override string[] SortableFields() => new[]
        {
            nameof(Department.Name),
            nameof(Department.Id),
            nameof(Department.CreatedAt),
            nameof(Department.Active)
        };

        protected override IDictionary<string, Func<string, Expression<Func<Department, bool>>>> AllowedFilters() =>
            new Dictionary<string, Func<string, Expression<Func<Department, bool>>>>(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(Department.Name)] = value => d => d.Name == value,
                [nameof(Department.Active)] = value => d => d.Active == bool.Parse(value)
            };

    }
}