using Entity.DTOs.Implements.Business.Plaza;
using MediatR;

namespace Business.CQRS.Business.Plaza.Update
{
    public class UpdatePlazaActiveStatusCommand : IRequest<PlazaSelectDto>
    {
        public int Id { get; }
        public bool Active { get; }

        public UpdatePlazaActiveStatusCommand(int id, bool active)
        {
            Id = id;
            Active = active;
        }
    }
}
