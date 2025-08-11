using FluentValidation;
using Entity.DTOs.Implements.Business.Plaza;

namespace Entity.DTOs.Validations.Plaza
{
    public class PlazaUpdateDtoValidator : PlazaBaseDtoValidator<PlazaUpdateDto>
    {
        public PlazaUpdateDtoValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Active).NotNull();
        }
    }
}
