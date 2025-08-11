using FluentValidation;
using Entity.DTOs.Implements.Location.City;

namespace Entity.DTOs.Validations.City
{
    public class CityUpdateDtoValidator : CityBaseDtoValidator<CityUpdateDto>
    {
        public CityUpdateDtoValidator()
        {
        }
    }
}
