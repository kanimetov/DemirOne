namespace Demir.Models;

public class Transaction
{
    public string Id { get; set; }
    public string Amount { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}