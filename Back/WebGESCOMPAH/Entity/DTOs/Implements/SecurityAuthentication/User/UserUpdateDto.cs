namespace Entity.DTOs.Implements.SecurityAuthentication.User
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public int PersonId { get; set; }
    }
}
