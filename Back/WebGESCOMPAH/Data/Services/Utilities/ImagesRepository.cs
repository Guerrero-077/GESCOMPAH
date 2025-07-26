using Data.Interfaz.IDataImplemenent;
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
        public async Task AddAsync(List<Images> entity)
        {
            _dbSet.AddRange(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(List<Images> images)
        {
            if (images == null || !images.Any())
            throw new ArgumentNullException(nameof(images), "La lista de imágenes no puede ser nula o vacía.");
            foreach (var image in images)
            {
                var existingImage = await _dbSet.FindAsync(image.Id);
                if (existingImage != null)
                {
                    existingImage.IsDeleted = true; // Marcar como eliminado lógicamente
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
