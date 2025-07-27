using Business.Interfaces.IBusiness;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.Utilities.Images;
using Microsoft.AspNetCore.Http;

namespace Business.Interfaces.Implements.Utilities
{
    public interface IImagesService : IBusiness<ImageSelectDto, ImageCreateDto, ImageUpdateDto>
    {
        //Task<List<ImageSelectDto>> AddImagesAsync(int establishmentId, ICollection<IFormFile> files);
        //Task DeleteImagesByPublicIdsAsync(ICollection<string> publicIds);
        //Task<List<ImageSelectDto>> GetImagesByEstablishmentIdAsync(int establishmentId);


        Task<List<ImageSelectDto>> AddImagesAsync(int establishmentId, ICollection<IFormFile> files);
        Task DeleteImageByIdAsync(int imageId);
        Task DeleteImagesByPublicIdsAsync(ICollection<string> publicIds);
        Task<List<ImageSelectDto>> GetImagesByEstablishmentIdAsync(int establishmentId);
    }

}

//Task<IEnumerable<ImageSelectDto>> GetByEstablishmentIdAsync(int establishmentId);
//Task AddAsync(ImageCreateDto entity);
//Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file);
//Task DeleteImageAsync(string publicId);