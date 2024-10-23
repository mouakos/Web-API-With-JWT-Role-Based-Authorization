using WebApiWithRoles.DTOs;
using WebApiWithRoles.DTOs.Response;

namespace WebApiWithRoles.Interfaces;

public interface IAccountService
{
    Task<GeneralResponse> CreateAsync(RegisterDto registerDto);
    Task<GeneralResponse> AssignRoleAsync(UserRoleDto userRoleDto);
    Task<GeneralResponse> AddRoleAsync(string role);
    Task<LoginResponse> LoginAsync(LoginDto loginDto);
}