using System.Collections.Generic;
using System.Linq;

using Entity.Domain.Models.Implements.AdministrationSystem;

using Entity.DTOs.Implements.SecurityAuthentication.Me;

using Mapster;

namespace Business.Mapping.Registers
{
    public class MenuMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Module, MenuModuleDto>()
                .Map(dest => dest.Forms, src => src.FormModules.Select(fm => fm.Form).Adapt<List<FormDto>>());

            config.NewConfig<Form, FormDto>();
        }
    }
}
