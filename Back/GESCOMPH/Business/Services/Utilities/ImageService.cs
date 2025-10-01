using Business.Interfaces.Implements.Utilities;
using Business.Repository;
using Data.Interfaz.IDataImplement.Utilities;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.Utilities.Images;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;
using Utilities.Helpers.CloudinaryHelper;

namespace Business.Services.Utilities
{
    /// <summary>
    /// Servicio para manejar imágenes con Cloudinary. Incluye:
    /// - Validación de archivos (delegada a CloudinaryUtility).
    /// - Subida en paralelo con control de concurrencia.
    /// - Rollback en Cloudinary si la persistencia falla.
    /// </summary>
    public sealed class ImageService :
        BusinessGeneric<ImageSelectDto, ImageCreateDto, ImageUpdateDto, Images>, IImagesService
    {
        private readonly IImagesRepository _imagesRepository;
        private readonly CloudinaryUtility _cloudinary;
        private readonly ILogger<ImageService> _logger;

        // Ajusta según tu infraestructura: 3 paralelos es buen punto medio
        private const int MaxParallelUploads = 3;
        private const int MaxFilesPerRequest = 5;

        public ImageService(
            IImagesRepository imagesRepository,
            CloudinaryUtility cloudinary,
            IMapper mapper,
            ILogger<ImageService> logger
        ) : base(imagesRepository, mapper)
        {
            _imagesRepository = imagesRepository;
            _cloudinary = cloudinary;
            _logger = logger;
        }

        public async Task<List<ImageSelectDto>> AddImagesAsync(int establishmentId, IFormFileCollection files)
        {
            if (files is null || files.Count == 0)
                throw new BusinessException("Debe adjuntar al menos un archivo.");

            var filesToUpload = files.Take(MaxFilesPerRequest).Where(f => f?.Length > 0).ToList();
            if (filesToUpload.Count == 0)
                throw new BusinessException("No se recibieron archivos válidos.");

            var uploadedEntities = new List<Images>();
            var uploadedPublicIds = new List<string>();
            using var semaphore = new SemaphoreSlim(MaxParallelUploads);

            try
            {
                var tasks = filesToUpload.Select(async file =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        var result = await _cloudinary.UploadImageAsync(file, establishmentId);
                        lock (uploadedEntities)
                        {
                            uploadedEntities.Add(new Images
                            {
                                FileName = file.FileName,
                                FilePath = result.SecureUrl.AbsoluteUri,
                                PublicId = result.PublicId,
                                EstablishmentId = establishmentId
                            });
                            uploadedPublicIds.Add(result.PublicId);
                        }
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                await Task.WhenAll(tasks);

                // Persistir en BD
                await _imagesRepository.AddRangeAsync(uploadedEntities);

                _logger.LogInformation("Subidas {Count} imágenes para establecimiento {Id}", uploadedEntities.Count, establishmentId);
                return _mapper.Map<List<ImageSelectDto>>(uploadedEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falló la subida/persistencia de imágenes para establecimiento {Id}. Ejecutando rollback...", establishmentId);

                // Rollback en Cloudinary para lo que sí subió
                var deletes = uploadedPublicIds.Select(pid => _cloudinary.DeleteAsync(pid));
                await Task.WhenAll(deletes);

                throw new BusinessException("Error al adjuntar imágenes al establecimiento.", ex);
            }
        }

        public async Task DeleteByPublicIdAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new BusinessException("PublicId requerido.");

            await _cloudinary.DeleteAsync(publicId);
            await _imagesRepository.DeleteByPublicIdAsync(publicId);
        }

        public async Task DeleteByIdAsync(int id)
        {
            // Cargar desde DB
            var img = await _imagesRepository.GetByIdAsync(id);
            if (img is null)
                return; // idempotente: nada que borrar

            // Borrar en Cloudinary (idempotente)
            await _cloudinary.DeleteAsync(img.PublicId);

            // Borrar fisicamente en DB (o soft si prefieres)
            await _imagesRepository.DeleteAsync(id);
        }

        public async Task<List<ImageSelectDto>> GetImagesByEstablishmentIdAsync(int establishmentId)
        {
            var images = await _imagesRepository.GetByEstablishmentIdAsync(establishmentId);
            return _mapper.Map<List<ImageSelectDto>>(images);
        }
    }
}
