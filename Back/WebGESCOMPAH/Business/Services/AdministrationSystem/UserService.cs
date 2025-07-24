using Business.Interfaces.Implements;
using Business.Repository;
using Data.Interfaz.DataBasic;
using Data.Interfaz.IDataImplemenent;
using Entity.Domain.Models.Implements.SecurityAuthentication;
using Entity.DTOs.Implements.SecurityAuthentication.User;
using MapsterMapper;

namespace Business.Services.AdministrationSystem
{
    public class UserService : BusinessGeneric<UserDto, UserSelectDto, UserSelectDto, User>, IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IDataGeneric<User> data, IUserRepository userRepository, IMapper mapper )
            :base(data, mapper)
        {

            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserSelectDto>> GetAllUser()
        {
            try
            {
                var entities = await _userRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<UserSelectDto>>(entities);
            }
            catch (Exception ex)
            {

                throw new Exception("An error occurred while retrieving users.", ex);
            }
        }
    }
}
