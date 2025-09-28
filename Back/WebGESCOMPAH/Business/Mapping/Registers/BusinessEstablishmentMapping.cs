using System.Collections.Generic;

using Entity.Domain.Models.Implements.Business;

using Entity.DTOs.Implements.Business.EstablishmentDto;
using Entity.DTOs.Implements.Business.Plaza;
using Entity.DTOs.Implements.Utilities.Images;

using Mapster;

namespace Business.Mapping.Registers
{
    public class BusinessEstablishmentMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // Establishment -> Select
            config.NewConfig<Establishment, EstablishmentSelectDto>()
                  .Map(dest => dest.Images, src => src.Images.Adapt<List<ImageSelectDto>>());

            // Establishment Create -> Entity
            config.NewConfig<EstablishmentCreateDto, Establishment>()
                  .Ignore(dest => dest.Id)
                  .Ignore(dest => dest.Images)
                  .Ignore(dest => dest.Active)
                  .Ignore(dest => dest.IsDeleted)
                  .Ignore(dest => dest.CreatedAt)
                  .IgnoreNullValues(true)
                  .Map(dest => dest.Name, src => src.Name.Trim())
                  .Map(dest => dest.Description, src => src.Description.Trim())
                  .Map(dest => dest.Address, src => string.IsNullOrWhiteSpace(src.Address) ? null : src.Address.Trim())
                  .Map(dest => dest.AreaM2, src => src.AreaM2)
                  .Map(dest => dest.UvtQty, src => src.UvtQty)
                  .Map(dest => dest.PlazaId, src => src.PlazaId);

            // Establishment Update -> Entity
            config.NewConfig<EstablishmentUpdateDto, Establishment>()
                  .Ignore(dest => dest.Images)
                  .Ignore(dest => dest.Active)
                  .Ignore(dest => dest.IsDeleted)
                  .Ignore(dest => dest.CreatedAt)
                  .IgnoreNullValues(true)
                  .Map(dest => dest.Name, src => src.Name == null ? null : src.Name.Trim())
                  .Map(dest => dest.Description, src => src.Description == null ? null : src.Description.Trim())
                  .Map(dest => dest.Address, src => string.IsNullOrWhiteSpace(src.Address) ? null : src.Address.Trim())
                  .Map(dest => dest.AreaM2, src => src.AreaM2)
                  .Map(dest => dest.RentValueBase, src => src.RentValueBase)
                  .Map(dest => dest.UvtQty, src => src.UvtQty)
                  .Map(dest => dest.PlazaId, src => src.PlazaId);

            // Plaza
            config.NewConfig<Plaza, PlazaSelectDto>();
        }
    }
}

