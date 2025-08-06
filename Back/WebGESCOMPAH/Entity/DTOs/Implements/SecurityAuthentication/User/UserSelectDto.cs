using Entity.DTOs.Implements.Persons.Peron;

namespace Entity.DTOs.Implements.SecurityAuthentication.User
{
    public class UserSelectDto
    {
        public string PersonName { get; set; } 
        public string Email { get; set; } = null!;
        public bool Active { get; set; }


    }
}
