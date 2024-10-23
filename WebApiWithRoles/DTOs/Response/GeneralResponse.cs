namespace WebApiWithRoles.DTOs.Response;

public class GeneralResponse
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; } = string.Empty;
}