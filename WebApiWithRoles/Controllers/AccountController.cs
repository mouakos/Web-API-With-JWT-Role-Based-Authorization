using Microsoft.AspNetCore.Mvc;
using WebApiWithRoles.ActionsFilters;
using WebApiWithRoles.DTO;
using WebApiWithRoles.Interfaces;

namespace WebApiWithRoles.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAccountService accountService) : ControllerBase
{
    #region Public methods declaration

    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody] string role)
    {
        if (string.IsNullOrWhiteSpace(role)) return BadRequest("Invalid role");
        var result = await accountService.AddRoleAsync(role);

        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("assign-role")]
    [ServiceFilter(typeof(ModelValidationFilterAttribute))]
    public async Task<IActionResult> AssignRole([FromBody] UserRoleDto model)
    {
        var result = await accountService.AssignRoleAsync(model);

        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ModelValidationFilterAttribute))]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var result = await accountService.AuthenticateAsync(model);

        if (result.IsSuccess)
            return Ok(result);
        return Unauthorized(result);
    }

    [HttpPost("register")]
    [ServiceFilter(typeof(ModelValidationFilterAttribute))]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var result = await accountService.RegisterAsync(model);

        if (result.IsSuccess)
            return Ok(result);
        return BadRequest(result);
    }

    #endregion
}