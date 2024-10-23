namespace WebApiWithRoles.DTOs.Response;

public class LoginResponse : GeneralResponse
{
    public string Token { get; set; } = string.Empty;
}