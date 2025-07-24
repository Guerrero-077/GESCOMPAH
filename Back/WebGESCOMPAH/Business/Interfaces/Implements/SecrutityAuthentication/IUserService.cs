using Entity.DTOs.Implements.SecurityAuthentication.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces.Implements.SecrutityAuthentication
{
    public interface IUserService 
    {
        Task<IEnumerable<UserSelectDto>> GetAllUser();
    }
}
