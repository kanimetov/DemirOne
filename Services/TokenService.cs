using Demir.Models;
using Microsoft.EntityFrameworkCore;

namespace Demir.Services;

public class TokenService : ITokenService
{
    private readonly ApplicationDbContext context;

    public TokenService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<Token> CreateTokenAsync(string userId, string token, string userAgent)
    {
        var existedToken = await GetTokenByUserAgentAsync(userAgent);
        token = NormolizeToken(token);

        if(existedToken != null) {
            existedToken.Value = token;
            context.Tokens.Update(existedToken);
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
        return newToken;
    }

    public bool IsTokenExist(string value)
    {
        return context.Tokens.FirstOrDefault(b => b.Value == NormolizeToken(value)) != null;
    }

    public async Task<Token?> GetTokenByUserAgentAsync(string userAgent)
    {
        return await context.Tokens.FirstOrDefaultAsync(b => b.UserAgent == userAgent);
    }

    private string NormolizeToken(string token) {
        token = token.StartsWith("Bearer ")
            ? token.Substring("Bearer ".Length)
            : token;
        return token;
    }
}


public interface ITokenService
{
    bool IsTokenExist(string value);
    Task<Token?> GetTokenByUserAgentAsync(string userAgent);
    Task<Token> CreateTokenAsync(string userId, string token, string userAgent);
}