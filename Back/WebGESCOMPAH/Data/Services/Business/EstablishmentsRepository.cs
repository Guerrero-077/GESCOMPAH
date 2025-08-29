using Data.Interfaz.IDataImplement.Business;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Business
{
    public class EstablishmentsRepository : DataGeneric<Establishment>, IEstablishmentsRepository
    {
        public EstablishmentsRepository(ApplicationDbContext context) : base(context) { }

        public override async Task<IEnumerable<Establishment>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(e =>
                    e.Active && !e.IsDeleted &&
                    e.Plaza != null && e.Plaza.Active && !e.Plaza.IsDeleted
                )
                .Include(e => e.Plaza)
                .Include(e => e.Images.Where(img => img.Active && !img.IsDeleted))
                .ToListAsync();
        }

        public override async Task<Establishment?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Where(e => e.Id == id && !e.IsDeleted)
                .Include(e => e.Plaza)
                .Include(e => e.Images.Where(img => img.Active && !img.IsDeleted))
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Proyección liviana exacta a la interfaz: (Id, RentValueBase, UvtQty).
        /// </summary>
        public async Task<IReadOnlyList<EstablishmentBasics>> GetBasicsByIdsAsync(IEnumerable<int> ids)
        {
            var set = ids?.Distinct().ToList() ?? [];
            if (set.Count == 0) return Array.Empty<EstablishmentBasics>();

            var list = await _dbSet
                .AsNoTracking()
                .Where(e => set.Contains(e.Id) && e.Active && !e.IsDeleted)
                .Select(e => new { e.Id, e.RentValueBase, e.UvtQty })
                .ToListAsync();

            return list.Select(x => new EstablishmentBasics(x.Id, x.RentValueBase, x.UvtQty))
                       .ToList()
                       .AsReadOnly();
        }
    }
}
