namespace WebApiWithRoles.Models;

public class Register
{
    #region Public properties declaration

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;

    #endregion
}