using Entity.DTOs.Base;

namespace Entity.DTOs.Implements.SecurityAuthentication.User
{
    public class UserSelectDto : BaseDto
    {
        public string PersonName { get; set; }
        public string PersonDocument {get; set;}
        public string PersonAddress{get; set;}
        public string PersonPhone{get; set;}
        public string Email { get; set; }
        public string CityName { get; set; }
        public bool Active { get; set; }
        public IEnumerable<string> Roles { get; set; } = [];
    }
}
