namespace Entity.Domain.Models.Implements.SecurityAuthentication
{
    /// <summary>
    /// Representa un refresh token persistido (guardamos hash, no el token en texto plano).
    /// </summary>
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TokenHash { get; set; } = null!; // SHA256 hex
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; } = false;
        public string? ReplacedByTokenHash { get; set; }
        //public string? RemoteIpAddress { get; set; } // opcional para auditoría
    }
}
