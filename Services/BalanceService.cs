using Demir.Data.Models;
using Demir.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Demir.Services;

public class BalanceService : IBalanceService {
    const double DEFAULTWITHDRAWAMOUNT = 1.1;
    private readonly ApplicationDbContext context;

    public BalanceService(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<BalanceDto> CreateBalanceAsync(UserDto user, double? amount = null)
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
        return Mapper(balance);
    }

    public async Task<BalanceDto?> GetBalanceByUserIdAsync(string userId)
    {
        return Mapper(await context.Balances.FirstOrDefaultAsync(b => b.UserId == userId));
    }

    public async Task<BalanceDto> PaymentAsync(UserDto user, double? withdraw = null)
    {
        using var transaction = await context.Database.BeginTransactionAsync();
        try {
            var balance = await context.Balances.FirstOrDefaultAsync(b => b.UserId == user.Id);
            if (balance == null){
                throw new InvalidOperationException("Balance not found.");
            }
            var withdrawAmount = withdraw ?? DEFAULTWITHDRAWAMOUNT;

            double difference = balance.Amount - withdrawAmount;
            

            if(difference > 0) {
                var updatedProduct = new Balance {
                    Id = balance.Id,
                    UserId = balance.UserId,
                    Amount = Math.Round(difference, 1)
                };
                
                context.Transactions.Add(new Transaction {
                    UserId = user.Id,
                    Withdraw = withdrawAmount,
                });
                context.Entry(balance).CurrentValues.SetValues(updatedProduct);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }else{
                throw new InvalidOperationException("There are not enough funds.");
            }

            return Mapper(balance);
        }
        catch(Exception){
            await transaction.RollbackAsync();
            throw;
        }
    }

    private BalanceDto? Mapper(Balance? user) {
        return user != null ? new BalanceDto {
            Id = user!.Id,
            Amount = user.Amount,
            UserId = user.UserId,
        } : null;
    }
}





public interface IBalanceService
{
    Task<BalanceDto> CreateBalanceAsync(UserDto user, double? amount = null);
    Task<BalanceDto> PaymentAsync(UserDto user, double? withdraw);
    Task<BalanceDto?> GetBalanceByUserIdAsync(string userId);
}