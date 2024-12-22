using Demir.Models;
using Microsoft.EntityFrameworkCore;

namespace Demir.Services;



public class UserService : IUserService
{
    private readonly ApplicationDbContext context;

    public UserService(ApplicationDbContext context)
    {
        this.context = context;
    }


    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User> CreateUserAsync(string username, string passwordHash)
    {
        if (await GetUserByUsernameAsync(username) != null)
        {
            throw new InvalidOperationException("A user with the same username already exists.");
        }

        User user = new User{
            Username = username,
            PasswordHash = passwordHash
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

}


public interface IUserService
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User> CreateUserAsync(string username, string passwordHash);
}