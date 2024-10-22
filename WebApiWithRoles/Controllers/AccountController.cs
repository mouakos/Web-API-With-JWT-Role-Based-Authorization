using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiWithRoles.ActionsFilters;
using WebApiWithRoles.DTOs;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace WebApiWithRoles.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration) : ControllerBase
{
    #region Public methods declaration

    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody] string role)
    {
        if (string.IsNullOrWhiteSpace(role)) return BadRequest("Invalid role");
        if (await roleManager.RoleExistsAsync(role)) return BadRequest("Role already exist");
        var result = await roleManager.CreateAsync(new IdentityRole(role));
        if (result.Succeeded)
            return Ok(new { Message = "Role added successfully" });
        return BadRequest(result.Errors);
    }

    [HttpPost("assign-role")]
    [ServiceFilter(typeof(ModelValidationFilterAttribute))]
    public async Task<IActionResult> AssignRole([FromBody] UserRoleDto model)
    {
        var user = await userManager.FindByNameAsync(model.Username!);
        if (user == null) return BadRequest("User not found");

        if (!await roleManager.RoleExistsAsync(model.Role!))
            return BadRequest(new { Message = "Invalid role" });

        var result = await userManager.AddToRoleAsync(user, model.Role!);
        if (result.Succeeded)
            return Ok(new { Message = "Role assigned successfully" });
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ModelValidationFilterAttribute))]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByNameAsync(model.Username!);

        if (user == null || !await userManager.CheckPasswordAsync(user, model.Password!))
            return BadRequest(new { message = "Invalid username / password" });

        var token = await GenerateTokenAsync(user);
        return Ok(new { Mesage = "Login successfully", Token = new JwtSecurityTokenHandler().WriteToken(token) });
    }

    [HttpPost("register")]
    [ServiceFilter(typeof(ModelValidationFilterAttribute))]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        var user = new IdentityUser { UserName = model.Username, Email = model.Email };
        var result = await userManager.CreateAsync(user, model.Password!);
        if (result.Succeeded) return Ok(new { message = "User registered Successfully" });
        return BadRequest(result.Errors);
    }

    #endregion

    #region Private methods declaration

    private async Task<JwtSecurityToken> GenerateTokenAsync(IdentityUser user)
    {
        var userRoles = await userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName!),
            new(JwtRegisteredClaimNames.Jti, user.Id)
        };
        authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            expires: DateTime.Now.AddMinutes(double.Parse(configuration["Jwt:ExpiryMinutes"]!)),
            claims: authClaims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                SecurityAlgorithms.HmacSha256)
        );
        return token;
    }

    #endregion
}