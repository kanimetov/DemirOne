using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Demir.Dtos;
using Demir.Constants;
using Demir.Services.Contracts;


namespace Demir.Controllers;


[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase{
    private readonly IBalanceService _balanceService;
    private readonly IUserService _userService;

    public PaymentController(IUserService userService, IBalanceService balanceService)
    {
        _balanceService = balanceService;
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> Payment(){
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        if (username == null || !int.TryParse(userIdString, out int userId))
        {
            return BadRequest(Messages.InvalidUserInfo);
        }

        UserDto? user = await _userService.GetUserByIdAsync(userId);

        if(user == null)
            return BadRequest(Messages.ContactSupport);

        
        try {
            BalanceDto balance = await _balanceService.PaymentAsync(user, null);

            return Ok(new {
                username,
                balance = balance.Amount,
                status = Messages.PaymentSuccess
            });
        }
        catch(InvalidOperationException ex){
            return BadRequest(new { message = ex.Message });
        }
    }

}