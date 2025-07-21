using Entity.DTOs.Implements.Persons;

namespace Entity.DTOs.Implements.SecurityAuthentication
{
    public class UserDto
    {
        public string Email { get; set; }
        public PersonDto Person { get; set; }
        public List<string> Roles { get; set; }

    }
}
