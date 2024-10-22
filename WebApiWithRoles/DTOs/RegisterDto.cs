using System.ComponentModel.DataAnnotations;

namespace WebApiWithRoles.DTOs;

public class RegisterDto
{
    #region Public properties declaration

    [Required] public string? Email { get; set; }

    [Required, DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required] public string? Username { get; set; }

    #endregion
}