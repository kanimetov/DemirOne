namespace Demir.Models;

public class Token
{
    public int Id { get; set; }
    public string Value { get; set; }
    public string userAgent { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}