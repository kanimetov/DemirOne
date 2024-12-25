namespace Demir.Data.Models;

public class Transaction
{
    public int Id { get; set; }
    public decimal Withdraw { get; set; }
    public int BalanceId { get; set; }
    public Balance Balance { get; set; }
}