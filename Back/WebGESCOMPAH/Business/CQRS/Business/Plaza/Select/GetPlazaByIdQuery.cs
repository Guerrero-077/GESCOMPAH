using Entity.DTOs.Implements.Business.Plaza;
using MediatR;

namespace Business.CQRS.Business.Plaza.Select
{
    public class GetPlazaByIdQuery : IRequest<PlazaSelectDto?>
    {
        public int Id { get; }

        public GetPlazaByIdQuery(int id)
        {
            Id = id;
        }
    }
}
