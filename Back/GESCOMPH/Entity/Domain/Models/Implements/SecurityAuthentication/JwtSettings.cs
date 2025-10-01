namespace Entity.Domain.Models.Implements.SecurityAuthentication
{
    public class JwtSettings
    {
        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = "WebGESCOMPAH.API";
        public string Audience { get; set; } = "GESCOMPAH.Client";
        public int AccessTokenExpirationMinutes { get; set; } = 15;
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
