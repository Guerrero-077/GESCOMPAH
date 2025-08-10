using Entity.DTOs.Implements.SecurityAuthentication.Auth;
using Entity.DTOs.Implements.SecurityAuthentication.Me;

namespace Business.Interfaces
{
    public interface IToken
    {
        //Task<string> GenerateToken(LoginDto dto);
        //Task<(string AccessToken, string RefreshToken, string CsrfToken)> GenerateTokensAsync(LoginDto dto, string remoteIp = null);
        Task<(string AccessToken, string RefreshToken, string CsrfToken, UserMeDto user)> GenerateTokensAsync(LoginDto dto);
        Task<(string NewAccessToken, string NewRefreshToken)> RefreshAsync(string refreshTokenPlain, string remoteIp = null);
        Task RevokeRefreshTokenAsync(string refreshTokenPlain);
    }
}
