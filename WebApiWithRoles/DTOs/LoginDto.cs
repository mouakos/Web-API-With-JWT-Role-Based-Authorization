namespace WebApiWithRoles.Models;

public class LoginDto
{
    #region Public properties declaration

    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    #endregion
}