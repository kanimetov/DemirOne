namespace Demir.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }

    public Balance Balance { get; set; }
    public Token[] Tokens { get; set; }
}