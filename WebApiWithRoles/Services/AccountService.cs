using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiWithRoles.DTOs;
using WebApiWithRoles.DTOs.Response;
using WebApiWithRoles.Interfaces;

namespace WebApiWithRoles.Services;

public class AccountService(
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration) : IAccountService
{
    /// <inheritdoc />
    public async Task<GeneralResponse> CreateAsync(RegisterDto registerDto)
    {
        var user = new IdentityUser { UserName = registerDto.Username, Email = registerDto.Email };
        var result = await userManager.CreateAsync(user, registerDto.Password!);
        return result.Succeeded
            ? new LoginResponse { Message = "User registered Successfully", IsSuccess = true }
            : new LoginResponse { Message = result.Errors.First().Description, IsSuccess = false };
    }

    /// <inheritdoc />
    public async Task<GeneralResponse> AssignRoleAsync(UserRoleDto userRoleDto)
    {
        var user = await userManager.FindByNameAsync(userRoleDto.Username!);
        if (user == null) return new GeneralResponse { Message = "User not found" };

        if (!await roleManager.RoleExistsAsync(userRoleDto.Role!))
            return new GeneralResponse { Message = "Invalid role" };

        var result = await userManager.AddToRoleAsync(user, userRoleDto.Role!);

        return result.Succeeded
            ? new GeneralResponse { Message = "Role assigned successfully", IsSuccess = true }
            : new GeneralResponse { Message = result.Errors.First().Description };
    }

    /// <inheritdoc />
    public async Task<GeneralResponse> AddRoleAsync(string role)
    {
        if (await roleManager.RoleExistsAsync(role)) return new GeneralResponse { Message = "Role already exists" };
        var result = await roleManager.CreateAsync(new IdentityRole(role));

        return result.Succeeded
            ? new GeneralResponse { Message = "Role added successfully", IsSuccess = true }
            : new GeneralResponse { Message = result.Errors.First().Description };
    }

    /// <inheritdoc />
    public async Task<LoginResponse> LoginAsync(LoginDto loginDto)
    {
        var response = new LoginResponse();
        var user = await userManager.FindByNameAsync(loginDto.Username!);

        if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password!))
        {
            response.Message = "Invalid username / password";
            return response;
        }

        var token = await GenerateTokenAsync(user);
        response.Message = "Login successfully";
        response.Token = token;
        response.IsSuccess = true;
        return response;
    }

    private async Task<string> GenerateTokenAsync(IdentityUser user)
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
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}