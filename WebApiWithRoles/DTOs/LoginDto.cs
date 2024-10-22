using System.ComponentModel.DataAnnotations;

namespace WebApiWithRoles.DTOs;

public class LoginDto
{
    #region Public properties declaration

    [Microsoft.Build.Framework.Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Microsoft.Build.Framework.Required] public string Username { get; set; } = string.Empty;

    #endregion
}