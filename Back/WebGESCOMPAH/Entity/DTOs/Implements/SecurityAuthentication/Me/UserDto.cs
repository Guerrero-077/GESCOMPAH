using Entity.DTOs.Implements.Persons.Person;

namespace Entity.DTOs.Implements.SecurityAuthentication.Me
{
    public class UserDto
    {
        public string Email { get; set; }
        public PersonDto Person { get; set; }
        public List<string> Roles { get; set; }

    }
}
