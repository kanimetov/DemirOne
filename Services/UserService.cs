using Demir.Constants;
using Demir.Data.Models;
using Demir.Dtos;
using Microsoft.EntityFrameworkCore;
namespace Demir.Services;



public class UserService : IUserService
{
    private readonly ApplicationDbContext context;

    public UserService(ApplicationDbContext context)
    {
        this.context = context;
    }


    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        var user = await context.Users.FindAsync(id);

        return Mapper(user);
    }
    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        return Mapper(await context.Users.FirstOrDefaultAsync(u => u.Username == username));
    }

    public async Task<UserDto> CreateUserAsync(string username, string passwordHash)
    {
        if (await GetUserByUsernameAsync(username) != null)
        {
            throw new InvalidOperationException(Messages.UserAlreadyCreated);
        }

        User user = new User{
            Username = username,
            PasswordHash = passwordHash
        };

        context.Users.Add(user);
        context.Balances.Add(new Balance{
            UserId = user.Id
        });
        await context.SaveChangesAsync();
        return Mapper(user);
    }

    public async Task<bool> UpdateUserAsync(UserDto user)
    {
        var existingUser = await context.Users.FindAsync(user.Id);
        if (existingUser == null) return false;

        existingUser.LockoutEnd = user.LockoutEnd;
        existingUser.FailedLoginAttempts = user.FailedLoginAttempts;

        context.Users.Update(existingUser);
        await context.SaveChangesAsync();
        return true;
    }

    private UserDto? Mapper(User? user) {
        return user != null ? new UserDto {
            Id = user!.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            FailedLoginAttempts = user.FailedLoginAttempts,
            LockoutEnd = user.LockoutEnd,
        } : null;
    }
}


public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<UserDto> CreateUserAsync(string username, string passwordHash);
    Task<bool> UpdateUserAsync(UserDto user);
}