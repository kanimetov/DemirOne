using Demir.Data.Models;
using Demir.Dtos;
using Demir.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Demir.Services;

public class TokenService : ITokenService
{
    private readonly ApplicationDbContext _context;

    public TokenService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TokenDto> CreateTokenAsync(int userId, string token, string userAgent)
    {
        var existedToken = await _context.Tokens.FirstOrDefaultAsync(b => b.UserAgent == userAgent);
        token = NormolizeToken(token);

        if(existedToken != null) {
            existedToken.Value = token;
            _context.Tokens.Update(existedToken);
            await _context.SaveChangesAsync();
            return MapTokenToTokenDto(existedToken);
        }
        var newToken = new Token {
            UserId = userId,
            UserAgent = userAgent,
            Value = token
        };
        _context.Tokens.Add(newToken);
        await _context.SaveChangesAsync();
        return MapTokenToTokenDto(newToken);
    }

    public bool IsTokenExist(string value)
    {
        return _context.Tokens.FirstOrDefault(b => b.Value == NormolizeToken(value)) != null;
    }

    public async Task<TokenDto?> GetTokenByUserAgentAsync(string userAgent)
    {
        return MapTokenToTokenDto(await _context.Tokens.FirstOrDefaultAsync(b => b.UserAgent == userAgent));
    }

    private string NormolizeToken(string token) {
        token = token.StartsWith("Bearer ")
            ? token.Substring("Bearer ".Length)
            : token;
        return token;
    }

    private TokenDto? MapTokenToTokenDto(Token? token) {
        return token != null ? new TokenDto {
            Id = token!.Id,
            UserAgent = token.UserAgent,
            Value = token.Value,
            UserId = token.UserId,
        } : null;
    }
}
