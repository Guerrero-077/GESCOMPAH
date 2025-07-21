namespace Entity.DTOs.Implements.SecurityAuthentication
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }

        // Datos personales
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Identification { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
    }
}
