using Business.Interfaces.Implements.Utilities;
using Business.Repository;
using Data.Interfaz.IDataImplemenent;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.Utilities.Images;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using Utilities.Helpers.CloudinaryHelper;

namespace Business.Services.Utilities
{
    public class ImageService(IImagesRepository data, IMapper mapper, CloudinaryUtility cloudinaryUtility, ILogger<ImageService> logger) : BusinessGeneric<ImageSelectDto, ImageCreateDto, ImageUpdateDto, Images>(data, mapper), IImagesService
    {
        private readonly IImagesRepository _dataRepository = data;
        private readonly CloudinaryUtility _cloudinaryUtility = cloudinaryUtility;
        private readonly ILogger<ImageService> _logger = logger;


        public override async Task<IEnumerable<ImageSelectDto>> GetAllAsync()
        {
            try
            {
                var entities = await _dataRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<ImageSelectDto>>(entities);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las imágenes", ex);
            }
        }
        public async Task<IEnumerable<ImageSelectDto>> GetByEstablishmentIdAsync(int establishmentId)
        {
            try
            {
                var entities = await _dataRepository.GetByEstablishmentIdAsync(establishmentId);
                return _mapper.Map<IEnumerable<ImageSelectDto>>(entities);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las imágenes por establecimiento", ex);
            }
        }

        public async Task AddAsync(ImageCreateDto dto)
        {
            if (dto == null || dto.Files == null || dto.Files.Count == 0)
                throw new ArgumentException("La lista de imágenes no puede ser nula o vacía.");

            var images = new List<Images>();

            foreach (var file in dto.Files)
            {
                var (url, fileName) = await _cloudinaryUtility.UploadAsync(file, $"establishments/{dto.EstablishmentId}");

                images.Add(new Images
                {
                    FileName = fileName, // Este es el publicId completo: folder/img_guid
                    FilePath = url,
                    EstablishmentId = dto.EstablishmentId
                });
            }

            await _dataRepository.AddAsync(images);
        }

        public override async Task<ImageSelectDto> UpdateAsync(ImageUpdateDto dto)
        {
            var image = await _dataRepository.GetByIdAsync(dto.Id);
            if (image == null || image.IsDeleted)
                throw new ArgumentException("Imagen no encontrada.");

            var oldPublicId = image.FileName;

            // ✅ Mapear el DTO sobre la entidad usando el IMapper de Mapster
            _mapper.Map(dto, image);

            // Subir nueva imagen (si viene)
            if (dto.File is not null)
            {
                var (newUrl, newPublicId) = await _cloudinaryUtility.UploadAsync(
                    dto.File,
                    $"establishments/{image.EstablishmentId}"
                );

                image.FilePath = newUrl;
                image.FileName = newPublicId;
            }

            await _dataRepository.UpdateAsync(image);

            // Eliminar la imagen anterior si cambió
            if (dto.File is not null && !string.Equals(oldPublicId, image.FileName, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    await _cloudinaryUtility.DeleteAsync(oldPublicId);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "No se pudo eliminar la imagen anterior en Cloudinary: {PublicId}", oldPublicId);
                }
            }

            return _mapper.Map<ImageSelectDto>(image);
        }


        public override async Task<bool> DeleteLogicAsync(int id)
        {
            var image = await _dataRepository.GetByIdAsync(id);
            if (image == null || image.IsDeleted)
                throw new ArgumentException("Imagen no encontrada.");

            try
            {
                // Eliminar de Cloudinary primero
                await _cloudinaryUtility.DeleteAsync(image.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al eliminar imagen en Cloudinary. FileName: {FileName}", image.FileName);
                // Continúa con la eliminación lógica aunque falle en cloud
            }

            image.IsDeleted = true;
            //image.UpdateAt = DateTime.UtcNow.AddHours(-5); // según tu zona

            await _dataRepository.UpdateAsync(image);

            return true;
        }


        public override async Task<bool> DeleteAsync(int id)
        {
            var image = await _dataRepository.GetByIdAsync(id) ?? throw new ArgumentException("Imagen no encontrada.");
            try
            {
                await _cloudinaryUtility.DeleteAsync(image.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al eliminar imagen en Cloudinary. FileName: {FileName}", image.FileName);
                // O podés relanzar si es crítico
            }

            return await _dataRepository.DeleteAsync(id);
        }




    }
}
