using Entity.DTOs.Implements.Business.Plaza;
using MediatR;

namespace Business.CQRS.Business.Plaza.Update
{
    public class UpdatePlazaCommand : IRequest<PlazaSelectDto>
    {
        public PlazaUpdateDto Plaza { get; set; }

        public UpdatePlazaCommand(PlazaUpdateDto plaza)
        {
            Plaza = plaza;
        }
    }
}
