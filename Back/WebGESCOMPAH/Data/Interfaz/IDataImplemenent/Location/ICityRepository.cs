using Data.Interfaz.DataBasic;
using Entity.Domain.Models.Implements.Location;

namespace Data.Interfaz.IDataImplemenent.Location
{
    public interface ICityRepository : IDataGeneric<City>
    {
        Task<IEnumerable<City>> GetCityByDepartment(int idDepartment);
    }
}