using Entity.DTOs.Implements.Persons.Peron;

namespace Entity.DTOs.Implements.SecurityAuthentication.User
{
    public class UserDto
    {
        public string Email { get; set; }
        public PersonDto Person { get; set; }
        public List<string> Roles { get; set; }

    }
}
