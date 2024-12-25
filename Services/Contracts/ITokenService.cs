using Demir.Dtos;

namespace Demir.Services.Contracts;

public interface ITokenService
{
    bool IsTokenExist(string value);
    Task<TokenDto?> GetTokenByUserAgentAsync(string userAgent);
    Task<TokenDto> CreateTokenAsync(int userId, string token, string userAgent);
}