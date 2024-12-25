using Demir.Constants;
using Demir.Data.Models;
using Demir.Dtos;
using Demir.Services.Contracts;
using Microsoft.EntityFrameworkCore;
namespace Demir.Services;



public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        return MapUserToUserDto(user);
    }
    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        return MapUserToUserDto(await _context.Users.FirstOrDefaultAsync(u => u.Username == username));
    }

    public async Task<UserDto> CreateUserAsync(string username, string passwordHash)
    {
        if (await GetUserByUsernameAsync(username) != null)
        {
            throw new InvalidOperationException(Messages.UserAlreadyCreated);
        }

        try {
            var user = new User{
                Username = username,
                PasswordHash = passwordHash,
                Balance = new Balance()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return MapUserToUserDto(user);
        }
        catch(Exception){
            throw new InvalidOperationException(Messages.ContactSupport);
        }
    }

    public async Task<bool> UpdateUserAsync(UserDto user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser == null) return false;

        existingUser.LockoutEnd = user.LockoutEnd;
        existingUser.FailedLoginAttempts = user.FailedLoginAttempts;

        _context.Users.Update(existingUser);
        await _context.SaveChangesAsync();
        return true;
    }

    private UserDto? MapUserToUserDto(User? user) {
        return user != null ? new UserDto {
            Id = user!.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            FailedLoginAttempts = user.FailedLoginAttempts,
            LockoutEnd = user.LockoutEnd,
        } : null;
    }
}