using Microsoft.AspNetCore.Mvc;

namespace Nedrech.WebApp.Models.ViewModels;

public class HomeModel
{
    public string CurrentUsername { get; set; } = string.Empty;
    
    public IEnumerable<UserModel> Users { get; set; } = Enumerable.Empty<UserModel>();

    [BindProperty]
    public HomeModelAction Action { get; set; }

    [BindProperty]
    public HashSet<string> SelectedIds { get; set; } = new();
}

public class UserModel
{
    public UserModel(string id, string username, string email,
        DateTimeOffset regDate, DateTimeOffset? lastLoginDate, DateTimeOffset? lockOutEnd)
    {
        Id = id;
        Username = username;
        Email = email;
        RegDateStr = regDate.LocalDateTime.ToString("dd-MM-yyyy HH:mm:ss");
        LastLoginDateStr = lastLoginDate?.LocalDateTime.ToString("dd-MM-yyyy HH:mm:ss");
        Blocked = lockOutEnd != null;
    }
    
    public string Id { get; }
    
    public string Username { get; }
    
    public string Email { get; }
    
    public string RegDateStr { get; }
    
    public string? LastLoginDateStr { get; }
    
    public bool Blocked { get; }
}

public enum HomeModelAction
{
    Block, Unblock, Delete, Refresh
}