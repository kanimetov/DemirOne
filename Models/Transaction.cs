namespace Demir.Models;

public class Transaction
{
    public int Id { get; set; }
    public double Withdraw { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
}