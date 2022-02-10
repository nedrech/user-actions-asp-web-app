namespace Nedrech.WebApp.Models.ViewModels;

public class LoginModel
{
    public string Username { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public string? InfoMessage { get; set; }
    
    public string? DangerMessage { get; set; }
}