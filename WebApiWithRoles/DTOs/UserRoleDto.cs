using Microsoft.Build.Framework;

namespace WebApiWithRoles.DTOs;

public class UserRoleDto
{
    #region Public properties declaration

    [Required] public string Role { get; set; } = string.Empty;

    [Required] public string Username { get; set; } = string.Empty;

    #endregion
}