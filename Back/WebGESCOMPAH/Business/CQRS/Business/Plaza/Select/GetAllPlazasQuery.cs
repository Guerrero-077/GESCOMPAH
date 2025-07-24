using Entity.DTOs.Implements.Business.Plaza;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.CQRS.Business.Plaza.Select
{
    public class GetAllPlazasQuery : IRequest<IEnumerable<PlazaSelectDto>>
    {
    }
}
