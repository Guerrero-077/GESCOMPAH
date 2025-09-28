using Entity.Domain.Models.Implements.Utilities;

using Entity.DTOs.Implements.Utilities.Images;

using Mapster;

namespace Business.Mapping.Registers
{
    public class UtilitiesImagesMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Images, ImageSelectDto>();

            config.NewConfig<ImageCreateDto, Images>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Establishment)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.IsDeleted);

            config.NewConfig<Images, ImageCreateDto>();

            config.NewConfig<ImageUpdateDto, Images>()
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.FilePath)
                .Ignore(dest => dest.FileName)
                .Ignore(dest => dest.CreatedAt)
                .Ignore(dest => dest.IsDeleted)
                .IgnoreNullValues(true);
        }
    }
}

