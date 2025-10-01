using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Utilities.Exceptions;

namespace Utilities.Helpers.CloudinaryHelper
{
    public class CloudinaryUtility
    {
        private readonly Cloudinary _cloudinary;

        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long _maxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

        public CloudinaryUtility(Cloudinary cloudinary) => _cloudinary = cloudinary;

        public async Task<ImageUploadResult> UploadImageAsync(IFormFile file, int establishmentId)
        {
            ValidateImage(file);

            await using var stream = file.OpenReadStream();

            var uniqueSuffix = Guid.NewGuid().ToString("N");
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var sanitizedFileName = Path.GetFileNameWithoutExtension(file.FileName)
                                            .Replace(" ", "_")
                                            .Replace(".", "_");

            var publicId = $"establishments/{establishmentId}/{sanitizedFileName}_{uniqueSuffix}";

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId,
                Overwrite = false,
                UseFilename = false,
                UniqueFilename = false,
                Transformation = new Transformation().Quality("auto").FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null || string.IsNullOrWhiteSpace(result.SecureUrl?.AbsoluteUri))
                throw new BusinessException($"Error al subir imagen: {result.Error?.Message ?? "Respuesta vacía o inválida"}");

            return result;
        }

        public async Task DeleteAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new BusinessException("PublicId no puede estar vacío.");

            var deletionParams = new DeletionParams(publicId)
            {
                ResourceType = ResourceType.Image,
                Invalidate = true,
                Type = "upload"
            };

            var result = await _cloudinary.DestroyAsync(deletionParams);

            // Acepta 'ok' y 'not found' (¡ojo al espacio!) como éxito (idempotente)
            var r = (result?.Result ?? string.Empty).Trim().ToLowerInvariant();
            if (r == "ok" || r == "not found" || r == "not_found")
                return;

            var msg = result?.Error?.Message ?? result?.Result ?? "unknown";
            throw new BusinessException($"Error al eliminar imagen: {msg}");
        }

        private void ValidateImage(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                throw new BusinessException("El archivo de imagen está vacío.");

            if (file.Length > _maxFileSizeInBytes)
                throw new BusinessException("El tamaño máximo permitido es de 5 MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                throw new BusinessException($"Extensión de archivo no permitida. Válidas: {string.Join(", ", _allowedExtensions)}");
        }
    }
}
