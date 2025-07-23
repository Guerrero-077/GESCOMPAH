using Entity.DTOs.Implements.Persons.Peron;

namespace Entity.DTOs.Implements.SecurityAuthentication.User
{
    public class UserSelectDto
    {
        public string Email { get; set; } = null!;
        public PersonDto Person { get; set; } = null!;

    }
}
