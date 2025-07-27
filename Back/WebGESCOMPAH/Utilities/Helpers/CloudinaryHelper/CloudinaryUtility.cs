


using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Utilities.Exceptions;

namespace Utilities.Helpers.CloudinaryHelper
{
    public class CloudinaryUtility
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryUtility(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, int establishmentId)
        {
            if (file.Length <= 0)
                throw new ArgumentException("Archivo vacío.");

            await using var stream = file.OpenReadStream();

            var uniqueSuffix = Guid.NewGuid().ToString("N"); // Evita colisiones entre imágenes del mismo establecimiento
            var fileExtension = Path.GetExtension(file.FileName);
            var sanitizedFileName = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "_");

            var publicId = $"establishments/{establishmentId}/{sanitizedFileName}_{uniqueSuffix}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId,
                Overwrite = false,
                UseFilename = false, // Ahora usamos publicId personalizado
                UniqueFilename = false,
                Folder = null // Ya está implícito en el public_id
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result;
        }



        public async Task DeleteAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new BusinessException("PublicId no puede estar vacío.");

            var deletionParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result != "ok")
                throw new BusinessException($"Error al eliminar imagen: {result.Error?.Message ?? result.Result}");
        }
    }
}
