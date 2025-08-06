namespace Entity.DTOs.Implements.SecurityAuthentication.User
{
    public class UserCreateDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int PersonId { get; set; }
    }
}
