using Entity.Enum;
using System.Xml.Linq;

namespace Entity.DTOs.Implements.SecurityAuthentication.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        // Datos personales
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DocumentType DocumentType { get; set; }
        public string Document { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
    }
}
