using Entity.DTOs.Implements.Business.Plaza;
using MediatR;

namespace Business.CQRS.Business.Plaza.Create
{
    public class CreatePlazaCommand : IRequest<PlazaSelectDto> 
    {
        public PlazaCreateDto Plaza { get; set; }
        public CreatePlazaCommand(PlazaCreateDto plaza)
        {
            Plaza = plaza;
        }
    }
}
