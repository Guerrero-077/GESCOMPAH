using Entity.Domain.Models.Implements.Business;

using Entity.DTOs.Implements.Business.ObligationMonth;

using Mapster;

namespace Business.Mapping.Registers
{
    public class BusinessObligationMonthMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ObligationMonth, ObligationMonthSelectDto>();
            config.NewConfig<ObligationMonth, ObligationMonthDto>();
            config.NewConfig<ObligationMonthDto, ObligationMonth>();
            config.NewConfig<ObligationMonthUpdateDto, ObligationMonth>();
        }
    }
}

