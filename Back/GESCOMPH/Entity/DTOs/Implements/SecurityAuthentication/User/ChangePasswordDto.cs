namespace Entity.DTOs.Implements.SecurityAuthentication.User
{
    public class ChangePasswordDto
    {
        public required int UserId { get; init; }
        public required string CurrentPassword { get; init; }
        public required string NewPassword { get; init; }
    }

}
