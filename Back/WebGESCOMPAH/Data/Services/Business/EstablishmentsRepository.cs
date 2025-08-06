using Data.Interfaz.IDataImplemenent.Business;
using Data.Interfaz.IDataImplemenent.Utilities;
using Data.Repository;
using Entity.Domain.Models.Implements.Business;
using Entity.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Services.Business
{
    public class EstablishmentsRepository(
        ApplicationDbContext context,
        IImagesRepository imagesRepository)
        : DataGeneric<Establishment>(context), IEstablishments
    {
        private readonly IImagesRepository _imagesRepository = imagesRepository;

        public override async Task<IEnumerable<Establishment>> GetAllAsync()
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .Include(e => e.Images)
                .Include(e => e.Plaza)
                .ToListAsync();
        }

        public override async Task<Establishment?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Where(e => e.Id == id && !e.IsDeleted)
                .Include(e => e.Images)
                 .Include(e => e.Plaza)
                .FirstOrDefaultAsync();
        }

        public override async Task<Establishment> AddAsync(Establishment entity)
        {
            // Paso 1: Agregar el establecimiento
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();

            // Paso 2: Asignar imágenes si las hay
            if (entity.Images.Any())
            {
                foreach (var img in entity.Images)
                {
                    img.EstablishmentId = entity.Id;
                }

                await _imagesRepository.AddAsync(entity.Images);
            }

            return entity;
        }

        public override async Task<Establishment> UpdateAsync(Establishment entity)
        {
            // Paso 1: Obtener el establecimiento actual con imágenes
            var existing = await _dbSet
                .Include(e => e.Images)
                .FirstOrDefaultAsync(e => e.Id == entity.Id && !e.IsDeleted);

            if (existing == null)
                throw new InvalidOperationException("No se encontró el establecimiento.");

            // Paso 2: Actualizar campos base
            _context.Entry(existing).CurrentValues.SetValues(entity);

            // Paso 3: Si hay nuevas imágenes, gestionarlas
            if (entity.Images.Any())
            {
                // Lógica conservadora: eliminar imágenes existentes y reemplazar
                var existingImages = await _imagesRepository.GetByEstablishmentIdAsync(entity.Id);
                foreach (var img in existingImages)
                {
                    await _imagesRepository.DeleteByPublicIdAsync(img.PublicId);
                }

                foreach (var img in entity.Images)
                {
                    img.EstablishmentId = entity.Id;
                }

                await _imagesRepository.AddAsync(entity.Images);
            }

            await _context.SaveChangesAsync();
            return existing;
        }
    }
}
