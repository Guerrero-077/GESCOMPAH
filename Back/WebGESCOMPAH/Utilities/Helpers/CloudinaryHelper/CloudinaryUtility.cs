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

        public async Task<(string FilePath, string FileName)> UploadAsync(IFormFile file, string folder)
        {
            if (file.Length == 0)
                throw new BusinessException("Archivo vacío.");

            var allowedTypes = new[] { "image/jpeg", "image/png" };
            if (!allowedTypes.Contains(file.ContentType))
                throw new BusinessException("Formato de imagen no soportado. Solo JPG y PNG están permitidos.");

            var publicId = $"img_{Guid.NewGuid()}";
            var fullFolder = folder.TrimEnd('/');

            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = publicId,
                Folder = fullFolder
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new BusinessException($"Error al subir imagen a Cloudinary: {result.Error.Message}");

            return (result.SecureUrl.ToString(), $"{fullFolder}/{publicId}");
        }

        public async Task DeleteAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Result != "ok")
                throw new BusinessException($"Error al eliminar imagen en Cloudinary: {result.Error?.Message}");
        }
    }
}
