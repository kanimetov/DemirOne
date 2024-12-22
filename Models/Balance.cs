namespace Demir.Models;

public class Balance
{
    public string Id { get; set; }
    public double Amount { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}