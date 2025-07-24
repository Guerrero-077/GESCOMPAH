using Entity.DTOs.Implements.Utilities;
using MediatR;

namespace Business.CQRS.Utilities.Image.Select
{
    public class GetAllImageQuery : IRequest<IEnumerable<ImageSelectDto>>
    {
    }
}
