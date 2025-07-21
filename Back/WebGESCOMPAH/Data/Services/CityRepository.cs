using Data.Interfaz.IDataImplemenent;
using Data.Repository;
using Entity.Domain.Models.Implements.Location;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services
{
    public class CityRepository : DataGeneric<City>, ICityRepository
    {
        public CityRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<City>> GetCityByDepartment(int idDepartment)
        {
            return await _dbSet.Where(c => c.DepartmentId == idDepartment && !c.IsDeleted)
                .ToListAsync();
        }
    }
}