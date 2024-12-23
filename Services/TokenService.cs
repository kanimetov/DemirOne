using Demir.Data.Models;
using Demir.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Demir.Services;

public class TokenService : ITokenService
{
    private readonly ApplicationDbContext context;

    public TokenService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<TokenDto> CreateTokenAsync(string userId, string token, string userAgent)
    {
        var existedToken = await GetTokenByUserAgentAsync(userAgent);
        token = NormolizeToken(token);

        if(existedToken != null) {
            existedToken.Value = token;
            context.Tokens.Update(new Token{
                Id = existedToken.Id,
                UserAgent = existedToken.UserAgent,
                UserId = existedToken.UserId,
                Value = existedToken.Value
            });
            await context.SaveChangesAsync();
            return existedToken;
        }
        var newToken = new Token {
            UserId = userId,
            UserAgent = userAgent,
            Value = token
        };
        context.Tokens.Add(newToken);
        await context.SaveChangesAsync();
        return Mapper(newToken);
    }

    public bool IsTokenExist(string value)
    {
        return context.Tokens.FirstOrDefault(b => b.Value == NormolizeToken(value)) != null;
    }

    public async Task<TokenDto?> GetTokenByUserAgentAsync(string userAgent)
    {
        return Mapper(await context.Tokens.FirstOrDefaultAsync(b => b.UserAgent == userAgent));
    }

    private string NormolizeToken(string token) {
        token = token.StartsWith("Bearer ")
            ? token.Substring("Bearer ".Length)
            : token;
        return token;
    }

    private TokenDto? Mapper(Token? token) {
        return token != null ? new TokenDto {
            Id = token!.Id,
            UserAgent = token.UserAgent,
            Value = token.Value,
            UserId = token.UserId,
        } : null;
    }
}


public interface ITokenService
{
    bool IsTokenExist(string value);
    Task<TokenDto?> GetTokenByUserAgentAsync(string userAgent);
    Task<TokenDto> CreateTokenAsync(string userId, string token, string userAgent);
}