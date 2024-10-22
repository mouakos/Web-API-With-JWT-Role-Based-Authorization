using System.ComponentModel.DataAnnotations;

namespace WebApiWithRoles.DTOs;

public class LoginDto
{
    #region Public properties declaration

    [Required, DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required] public string? Username { get; set; }

    #endregion
}