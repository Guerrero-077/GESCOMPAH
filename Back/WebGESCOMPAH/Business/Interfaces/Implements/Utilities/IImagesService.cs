using Business.Interfaces.IBusiness;
using Entity.Domain.Models.Implements.Utilities;
using Entity.DTOs.Implements.Utilities.Images;

namespace Business.Interfaces.Implements.Utilities
{
    public interface IImagesService : IBusiness<ImageSelectDto, ImageCreateDto, ImageUpdateDto>
    {
        Task<IEnumerable<ImageSelectDto>> GetByEstablishmentIdAsync(int establishmentId);
        Task AddAsync(ImageCreateDto entity);
    }
}
