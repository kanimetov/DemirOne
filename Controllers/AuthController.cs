using Demir.Helpers;
using Demir.Models;
using Demir.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Demir.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration configuration;
    private readonly IUserService userService;

    public AuthController(IConfiguration configuration, IUserService userService)
    {
        this.configuration = configuration;
        this.userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(Model model)
    {
        User? user = await userService.GetUserByUsernameAsync(model.Username);
        // Validate user credentials (this is just an example)
        if (user != null)
            return Conflict(new {message= "A user with this email already exists."});
        var passwordHelper = new PasswordHelper();
        string passwordHash = passwordHelper.HashPassword(model.Password);
        User createUser = await userService.CreateUserAsync(model.Username, passwordHash);

        TokenResult result = GenerateJwtToken(configuration, createUser.Username, createUser.Id);

        return Ok(new
        {
            result.Token,
            result.Expiration,
            username = createUser.Username
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Model model)
    {
        User? user = await userService.GetUserByUsernameAsync(model.Username);

        if (user == null) 
            return NotFound(new {message = "User not registered."});
        
        if (user.LockoutEnd > DateTime.UtcNow)
            return BadRequest($"Account is locked until {user.LockoutEnd} UTC.");

        var passwordHelper = new PasswordHelper();
        if(!passwordHelper.VerifyPassword(user.PasswordHash, model.Password)) {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(10); // Lock account for 10 minutes
                await userService.UpdateUserAsync(user);
                return BadRequest("Account locked due to multiple failed login attempts.");
            }

            await userService.UpdateUserAsync(user);
            return Unauthorized(new { message = "Invalid username or password." });
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        await userService.UpdateUserAsync(user);


        TokenResult result = GenerateJwtToken(configuration, user.Username, user.Id);

        return Ok(result);
    }


    private TokenResult GenerateJwtToken(IConfiguration configuration, string username, string userId)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiresInMinutes"])),
            signingCredentials: signingCredentials
        );

        return new TokenResult
        {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
        };
    }
}

public class TokenResult {
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}

public class Model
{
    public string Username { get; set; }
    public string Password { get; set; }
}