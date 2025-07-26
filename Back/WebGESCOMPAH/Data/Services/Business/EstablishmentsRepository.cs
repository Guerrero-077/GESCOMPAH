using Data.Interfaz.IDataImplemenent;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Business
{
    public class EstablishmentsRepository(ApplicationDbContext context) : DataGeneric<Establishment>(context), IEstablishments
    {

        public override async Task<IEnumerable<Establishment>> GetAllAsync() 
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .Include(e => e.Images)
                .ToListAsync();
        }


        public override async Task<Establishment?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Where(e => e.Id == id && !e.IsDeleted)
                .Include(e => e.Images)
                .FirstOrDefaultAsync();
        }
    }
}
