using Business.Interfaces.IBusiness;
using Entity.DTOs.Implements.Utilities;

namespace Business.Interfaces.Implements.Utilities
{
    public interface IImagesService : IBusiness<ImageSelectDto, ImageDto, ImageDto>
    {
    }
}
