using Data.Interfaz.IDataImplemenent.Utilities;
using Data.Repository;
using Entity.Domain.Models.Implements.Utilities;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Utilities
{
    public class ImagesRepository(ApplicationDbContext context) : DataGeneric<Images>(context), IImagesRepository
    {
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


        public async Task AddAsync(List<Images> images)
        {
            if (!images.Any()) return;

            var establishmentId = images.First().EstablishmentId;
            var existingCount = await _dbSet
                .CountAsync(i => i.EstablishmentId == establishmentId && !i.IsDeleted);

            if (existingCount + images.Count > 5)
                throw new InvalidOperationException("No se pueden asociar más de 5 imágenes a un establecimiento.");

            _dbSet.AddRange(images);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteByPublicIdAsync(string publicId)
        {
            var image = await _dbSet.FirstOrDefaultAsync(i => i.PublicId == publicId);
            if (image == null) return false;

            _dbSet.Remove(image);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}