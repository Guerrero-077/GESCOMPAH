using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Utilities.Images;
using Microsoft.AspNetCore.Http;

namespace Business.Interfaces.Implements.Utilities
{
    /// <summary>
    /// Servicio de imágenes desacoplado del servicio de Establecimientos.
    /// </summary>
    public interface IImagesService : IBusiness<ImageSelectDto, ImageCreateDto, ImageUpdateDto>
    {
        /// <summary>Sube y asocia imágenes a un establecimiento.</summary>
        Task<List<ImageSelectDto>> AddImagesAsync(int establishmentId, IFormFileCollection files);

        /// <summary>Elimina una imagen por su PublicId en Cloudinary.</summary>
        Task DeleteByPublicIdAsync(string publicId);

        /// <summary>Obtiene imágenes por establecimiento.</summary>
        Task<List<ImageSelectDto>> GetImagesByEstablishmentIdAsync(int establishmentId);
    }
}
