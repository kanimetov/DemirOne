using Demir.Dtos;

namespace Demir.Services.Contracts;
public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<UserDto> CreateUserAsync(string username, string passwordHash);
    Task<bool> UpdateUserAsync(UserDto user);
}