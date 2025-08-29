using Data.Interfaz.IDataImplement.Business;
using Data.Interfaz.IDataImplement.Utilities;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Domain.Models.Implements.Utilities;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Business
{
    public class EstablishmentsRepository : DataGeneric<Establishment>, IEstablishmentsRepository
    {
        private readonly IImagesRepository _imagesRepository;

        public EstablishmentsRepository(ApplicationDbContext context, IImagesRepository imagesRepository)
            : base(context)
        {
            _imagesRepository = imagesRepository ?? throw new ArgumentNullException(nameof(imagesRepository));
        }

        public override async Task<IEnumerable<Establishment>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(e =>
                    e.Active
                    && !e.IsDeleted
                    && e.Plaza != null
                    && e.Plaza.Active
                    && !e.Plaza.IsDeleted
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
                .Include(e => e.Images)
                .FirstOrDefaultAsync();
        }

        // 🔹 CORREGIDO: proyección a DTO liviano, no a entidad EF
        public async Task<IReadOnlyList<EstablishmentBasics>> GetBasicsByIdsAsync(IEnumerable<int> ids)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(e => ids.Contains(e.Id) && e.Active && !e.IsDeleted)
                .Select(e => new EstablishmentBasics(e.Id, e.RentValueBase, e.UvtQty))
                .ToListAsync();
        }

        public override async Task<Establishment> AddAsync(Establishment entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Add(entity); // sin SaveChanges
            return await Task.FromResult(entity);
        }

        public override async Task<Establishment> UpdateAsync(Establishment entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var existing = await _dbSet
                .Include(e => e.Images)
                .FirstOrDefaultAsync(e => e.Id == entity.Id && !e.IsDeleted);

            if (existing == null)
                throw new InvalidOperationException($"No se encontró el establecimiento con ID {entity.Id}.");

            _context.Entry(existing).CurrentValues.SetValues(entity);

            // Sincronizar imágenes
            var imagesToRemove = existing.Images.Where(img => !entity.Images.Any(eImg => eImg.Id == img.Id)).ToList();
            foreach (var img in imagesToRemove)
                _context.Set<Images>().Remove(img);

            var imagesToAdd = entity.Images.Where(img => img.Id == 0).ToList();
            foreach (var img in imagesToAdd)
            {
                img.EstablishmentId = existing.Id;
                existing.Images.Add(img);
            }

            // sin SaveChanges
            return existing;
        }
    }
}
