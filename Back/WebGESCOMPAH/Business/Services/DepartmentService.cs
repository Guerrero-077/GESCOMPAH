using Business.Interfaces.Implements;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Location;
using Entity.DTOs.Implements.Location.Select;
using MapsterMapper;

namespace Business.Services
{
    public class DepartmentService : BusinessGeneric<DepartmentSelectDto, DepartmentSelectDto, Department>, IDepartmentService
    {
        public DepartmentService(IDataGeneric<Department> data, IMapper mapper) : base(data, mapper)
        {
        }
    }
}