using Demir.Models;
using Microsoft.EntityFrameworkCore;

namespace Demir.Services;

public class BalanceService : IBalanceService {
    const double DEFAULTWITHDRAWAMOUNT = 1.1;
    private readonly ApplicationDbContext context;

    public BalanceService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<Balance> CreateBalanceAsync(User user, double? amount = null)
    {
        if (await context.Balances.FirstOrDefaultAsync(b => b.UserId == user.Id) != null)
        {
            throw new InvalidOperationException("User already have a balance.");
        }

        Balance balance = new Balance {
            UserId = user.Id,
        };

        context.Balances.Add(balance);
        await context.SaveChangesAsync();
        return balance;
    }

    public async Task<Balance?> GetBalanceByUserIdAsync(string userId)
    {
        return await context.Balances.FirstOrDefaultAsync(b => b.UserId == userId);
    }

    public async Task<Balance> PaymentAsync(User user, double? withdraw = null)
    {
        var balance = await GetBalanceByUserIdAsync(user.Id);
        if (balance == null){
            throw new InvalidOperationException("Balance not found.");
        }

        double difference = balance.Amount - (withdraw ?? DEFAULTWITHDRAWAMOUNT);
        if(difference > 0) {
            balance.Amount = difference;
            context.Balances.Update(balance);
            await context.SaveChangesAsync();
        }else{
            throw new InvalidOperationException("There are not enough funds.");
        }

        return balance;
    }
}





public interface IBalanceService
{
    Task<Balance> CreateBalanceAsync(User user, double? amount = null);
    Task<Balance> PaymentAsync(User user, double? withdraw);
    Task<Balance?> GetBalanceByUserIdAsync(string userId);
}