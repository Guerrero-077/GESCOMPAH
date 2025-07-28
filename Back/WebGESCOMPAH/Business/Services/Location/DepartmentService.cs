using Business.Interfaces.Implements.Location;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Location;
using Entity.DTOs.Implements.Location.Department;
using MapsterMapper;

namespace Business.Services.Location
{
    public class DepartmentService : BusinessGeneric<DepartmentSelectDto, DepartmentCreateDto, DepartmentUpdateDto, Department>, IDepartmentService
    {

        public DepartmentService(IDataGeneric<Department> data, IMapper mapper) : base(data, mapper)
        {
        }

    }       
}