using System.Security.Claims;
using Demir.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Demir.Dtos;


namespace Demir.Controllers;


[Authorize]
[ApiController]
[Route("api")]
public class BalanceController : ControllerBase{
    private readonly IBalanceService balanceService;
    private readonly IUserService userService;

    public BalanceController(IUserService userService, IBalanceService balanceService)
    {
        this.balanceService = balanceService;
        this.userService = userService;
    }

    [HttpPost("payment")]
    public async Task<IActionResult> Payment(){
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        if (username == null || userId == null)
        {
            return Unauthorized(new { message = "User is not authorized." });
        }

        UserDto? user = await userService.GetUserByIdAsync(userId);

        if(user == null)
            return BadRequest($"Please contact our support team");

        
        try {
            BalanceDto balance = await balanceService.PaymentAsync(user, null);

            return Ok(new {
                username,
                balance = balance.Amount,
            });
        }
        catch(InvalidOperationException ex){
            return BadRequest(new { message = ex.Message });
        }
    }

}