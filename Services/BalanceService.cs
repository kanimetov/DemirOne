using Demir.Constants;
using Demir.Data.Models;
using Demir.Dtos;
using Demir.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Demir.Services;

public class BalanceService : IBalanceService {
    private const decimal DEFAULT_WITHDRAW_AMOUNT = 1.1M;
    private readonly ApplicationDbContext _context;

    public BalanceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BalanceDto> PaymentAsync(UserDto userDto, decimal? withdraw = null)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try {
            var user = await _context.Users.Include(u => u.Balance).FirstAsync(b => b.Id == userDto.Id);
            var withdrawAmount = withdraw ?? DEFAULT_WITHDRAW_AMOUNT;

            decimal difference = user.Balance.Amount - withdrawAmount;
            

            if(difference > 0) {
                user.Balance.Amount = Math.Round(difference, 1);
                
                _context.Transactions.Add(new Transaction {
                    BalanceId = user.Balance.Id,
                    Withdraw = withdrawAmount,
                });
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }else{
                throw new InvalidOperationException(Messages.NotEnoughFunds);
            }

            return MapBalancaToBalanceDto(user.Balance);
        }
        catch(Exception){
            await transaction.RollbackAsync();
            throw;
        }
    }

    private BalanceDto? MapBalancaToBalanceDto(Balance? balance) {
        return balance != null ? new BalanceDto {
            Id = balance!.Id,
            Amount = balance.Amount,
            UserId = balance.UserId,
        } : null;
    }
}