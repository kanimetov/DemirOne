namespace Demir.Data.Models;

public class Balance
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}