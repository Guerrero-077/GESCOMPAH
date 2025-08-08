using Data.Interfaz.IDataImplemenent.Utilities;
using Data.Repository;
using Entity.Domain.Models.Implements.Utilities;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Utilities
{
    public class ImagesRepository : DataGeneric<Images>, IImagesRepository
    {
        private readonly ApplicationDbContext _context;

        public ImagesRepository(ApplicationDbContext context) : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<IEnumerable<Images>> GetAllAsync()
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Images>> GetByEstablishmentIdAsync(int establishmentId)
        {
            return await _dbSet
                .Where(e => e.EstablishmentId == establishmentId && !e.IsDeleted)
                .ToListAsync();
        }

        public Task AddAsync(List<Images> images)
        {
            if (images == null || !images.Any())
                return Task.CompletedTask;

            // No transacciones ni SaveChanges aquí
            _dbSet.AddRange(images);
            return Task.CompletedTask;
        }

        public async Task<bool> DeleteByPublicIdAsync(string publicId)
        {
            var image = await _dbSet.FirstOrDefaultAsync(i => i.PublicId == publicId);
            if (image == null) return false;

            _dbSet.Remove(image);
            return true;
        }

        public async Task<bool> DeleteLogicalByPublicIdAsync(string publicId)
        {
            var entity = await _context.Images.FirstOrDefaultAsync(i => i.PublicId == publicId);
            if (entity == null) return false;

            entity.Active = false;
            entity.IsDeleted = true; // o el nombre del flag soft delete
            _context.Images.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
