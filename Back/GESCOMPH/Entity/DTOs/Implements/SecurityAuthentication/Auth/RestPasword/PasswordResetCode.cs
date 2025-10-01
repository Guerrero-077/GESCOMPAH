using Entity.Domain.Models.ModelBase;

namespace Entity.DTOs.Implements.SecurityAuthentication.Auth.RestPasword
{
    public class PasswordResetCode : BaseModel
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public DateTime Expiration { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}
