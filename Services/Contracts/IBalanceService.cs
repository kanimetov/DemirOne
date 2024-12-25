using Demir.Dtos;

namespace Demir.Services.Contracts;

public interface IBalanceService
{
    Task<BalanceDto> PaymentAsync(UserDto user, decimal? withdraw);
}