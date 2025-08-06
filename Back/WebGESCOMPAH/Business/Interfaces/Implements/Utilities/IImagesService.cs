using Microsoft.AspNetCore.Http;
using Entity.DTOs.Implements.Utilities.Images;

namespace Business.Interfaces.Implements.Utilities
{
    public interface IImagesService
    {
        /// <summary>
        /// Agrega múltiples imágenes a un establecimiento.
        /// </summary>
        Task<List<ImageSelectDto>> AddImagesAsync(int establishmentId, IFormFileCollection files);

        /// <summary>
        /// Reemplaza una imagen existente (por ID).
        /// </summary>
        Task<ImageSelectDto> ReplaceImageAsync(int imageId, IFormFile newFile);

        /// <summary>
        /// Elimina una imagen individual por ID.
        /// </summary>
        Task DeleteImageByIdAsync(int imageId);

        /// <summary>
        /// Elimina múltiples imágenes por sus publicIds en Cloudinary.
        /// </summary>
        Task DeleteImagesByPublicIdsAsync(List<string> publicIds);

        /// <summary>
        /// Obtiene todas las imágenes asociadas a un establecimiento.
        /// </summary>
        Task<List<ImageSelectDto>> GetImagesByEstablishmentIdAsync(int establishmentId);
    }
}
