namespace WebApiWithRoles.Models;

public class RegisterDto
{
    #region Public properties declaration

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    #endregion
}