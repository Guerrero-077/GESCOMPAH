    using Data.Interfaz.IDataImplemenent.Business;
    using Data.Interfaz.IDataImplemenent.Utilities;
    using Data.Repository;
    using Entity.Domain.Models.Implements.Business;
    using Entity.Domain.Models.Implements.Utilities;
    using Entity.Infrastructure.Context;
    using Microsoft.EntityFrameworkCore;

    namespace Data.Services.Business
    {
        public class EstablishmentsRepository : DataGeneric<Establishment>, IEstablishmentsRepository
        {
            private readonly ApplicationDbContext _context;
            private readonly IImagesRepository _imagesRepository;

            public EstablishmentsRepository(ApplicationDbContext context, IImagesRepository imagesRepository)
                : base(context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
                _imagesRepository = imagesRepository ?? throw new ArgumentNullException(nameof(imagesRepository));
            }

            public override async Task<IEnumerable<Establishment>> GetAllAsync()
            {
                return await _dbSet
                    .Where(e => !e.IsDeleted)
                    .Include(e => e.Plaza)
                    .Include(e => e.Images)
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

            public override async Task<Establishment> AddAsync(Establishment entity)
            {
                if (entity == null) throw new ArgumentNullException(nameof(entity));

                _dbSet.Add(entity);
                // No SaveChanges aquí

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

                // Sincronización imágenes
                var imagesToRemove = existing.Images.Where(img => !entity.Images.Any(eImg => eImg.Id == img.Id)).ToList();
                foreach (var img in imagesToRemove)
                    _context.Set<Images>().Remove(img);

                var imagesToAdd = entity.Images.Where(img => img.Id == 0).ToList();
                foreach (var img in imagesToAdd)
                {
                    img.EstablishmentId = existing.Id;
                    existing.Images.Add(img);
                }

                // No SaveChanges aquí
                return existing;
            }
        }
    }
