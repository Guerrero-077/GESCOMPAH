using Data.Interfaz.IDataImplement.Utilities;
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

        // Lectura estándar: solo activas y no borradas, sin tracking
        public override async Task<IEnumerable<Images>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Where(i => i.Active && !i.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Images>> GetByEstablishmentIdAsync(int establishmentId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(i => i.EstablishmentId == establishmentId && i.Active && !i.IsDeleted)
                .OrderByDescending(i => i.Id) // portada = la última subida (opcional)
                .ToListAsync();
        }

        /// <summary>
        /// Inserta en lote y confirma con UN solo SaveChangesAsync().
        /// (No confundirse con AddAsync(T) del genérico.)
        /// </summary>
        public async Task AddRangeAsync(IEnumerable<Images> images)
        {
            if (images == null) throw new ArgumentNullException(nameof(images));
            var list = images.Where(x => x != null).ToList();
            if (list.Count == 0) return;

            await _dbSet.AddRangeAsync(list);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Borrado físico por PublicId. El borrado en Cloudinary lo maneja la capa Business.
        /// </summary>
        public async Task<bool> DeleteByPublicIdAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId)) return false;

            var entity = await _dbSet.FirstOrDefaultAsync(i => i.PublicId == publicId && !i.IsDeleted);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Borrado lógico por PublicId (Active=false, IsDeleted=true).
        /// </summary>
        public async Task<bool> DeleteLogicalByPublicIdAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId)) return false;

            var entity = await _dbSet.FirstOrDefaultAsync(i => i.PublicId == publicId && !i.IsDeleted);
            if (entity == null) return false;

            entity.Active = false;
            entity.IsDeleted = true;
            _dbSet.Update(entity);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
