using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Location;

namespace Data.Interfaz.IDataImplement.Location
{
    public interface ICityRepository : IDataGeneric<City>
    {
        Task<IEnumerable<City>> GetCityByDepartmentAsync(int idDepartment);
    }
}