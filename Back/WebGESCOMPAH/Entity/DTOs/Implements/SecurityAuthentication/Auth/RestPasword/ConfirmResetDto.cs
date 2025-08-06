namespace Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword
{
    public class ConfirmResetDto
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
