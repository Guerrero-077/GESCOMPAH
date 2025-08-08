using Entity.DTOs.Implements.Utilities.Images;
using Microsoft.AspNetCore.Http;

namespace Business.Interfaces.Implements.Utilities
{
    public interface IImagesService
    {
        /// <summary>
        /// Agrega nuevas imágenes a un establecimiento, respetando un máximo de 5 imágenes activas.
        /// </summary>
        /// <param name="establishmentId">ID del establecimiento</param>
        /// <param name="files">Colección de archivos para subir</param>
        /// <returns>Lista de imágenes agregadas</returns>
        Task<List<ImageSelectDto>> AddImagesAsync(int establishmentId, IFormFileCollection files);

        /// <summary>
        /// Reemplaza una imagen existente por una nueva.
        /// </summary>
        /// <param name="imageId">ID de la imagen a reemplazar</param>
        /// <param name="newFile">Nuevo archivo de imagen</param>
        /// <returns>DTO con la imagen actualizada</returns>
        Task<ImageSelectDto> ReplaceImageAsync(int imageId, IFormFile newFile);

        /// <summary>
        /// Elimina (soft delete) una imagen por su ID y la borra de Cloudinary.
        /// </summary>
        /// <param name="imageId">ID de la imagen a eliminar</param>
        Task DeleteImageByIdAsync(int imageId);

        /// <summary>
        /// Elimina (soft delete) múltiples imágenes por sus PublicIds y las borra de Cloudinary.
        /// </summary>
        /// <param name="publicIds">Lista de PublicIds de las imágenes a eliminar</param>
        Task DeleteImagesByPublicIdsAsync(List<string> publicIds);

        /// <summary>
        /// Obtiene las imágenes activas asociadas a un establecimiento.
        /// </summary>
        /// <param name="establishmentId">ID del establecimiento</param>
        /// <returns>Lista de DTOs con las imágenes</returns>
        Task<List<ImageSelectDto>> GetImagesByEstablishmentIdAsync(int establishmentId);
    }
}
