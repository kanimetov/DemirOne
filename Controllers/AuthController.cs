using Demir.Constants;
using Demir.Dtos;
using Demir.Helpers;
using Demir.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    private const int MAX_LOGIN_ATTEMPTS = 5;
    private const int LOCKOUT_MINUTES = 10;

    public AuthController(IConfiguration configuration, IUserService userService, ITokenService tokenService)
    {
        _configuration = configuration;
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(Model model)
    {
        UserDto? user = await _userService.GetUserByUsernameAsync(model.Username);
        if (user != null)
            return Conflict(new {message= Messages.UserAlreadyCreated});
        
        var passwordHelper = new PasswordHelper();
        string passwordHash = passwordHelper.HashPassword(model.Password);
        UserDto createUser = await _userService.CreateUserAsync(model.Username, passwordHash);

        TokenResult result = GenerateJwtToken(_configuration, createUser.Username, createUser.Id);

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
        UserDto? user = await _userService.GetUserByUsernameAsync(model.Username);

        if (user == null) 
            return NotFound(new {message = Messages.UserNotRegistered});
        
        if (user.LockoutEnd > DateTime.UtcNow)
            return BadRequest($"{Messages.AccountLocked} {user.LockoutEnd} UTC.");

        var passwordHelper = new PasswordHelper();
        if(!passwordHelper.VerifyPassword(user.PasswordHash, model.Password)) {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= MAX_LOGIN_ATTEMPTS)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(LOCKOUT_MINUTES); // Lock account for 10 minutes
                await _userService.UpdateUserAsync(user);
                return BadRequest(Messages.AccountLocked);
            }

            await _userService.UpdateUserAsync(user);
            return Unauthorized(new { message = Messages.InvalidUsernameOrPassword });
        }

        user.FailedLoginAttempts = 0;
        user.LockoutEnd = null;
        await _userService.UpdateUserAsync(user);


        TokenResult result = GenerateJwtToken(_configuration, user.Username, user.Id);

        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        if (username == null || !int.TryParse(userIdString, out int userId))
        {
            return BadRequest(Messages.InvalidUserInfo);
        }

        var token = Request.Headers["Authorization"].ToString();
        var userAgent = Request.Headers["User-Agen"].ToString();
        await _tokenService.CreateTokenAsync(userId, token, userAgent);


        return Ok(new { message = Messages.Logout });
    }


    private TokenResult GenerateJwtToken(IConfiguration configuration, string username, int userId)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
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
