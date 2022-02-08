using Microsoft.AspNetCore.Mvc;

namespace Nedrech.WebApp.Models.ViewModels;

public class HomeModel
{
    public string CurrentUsername { get; set; } = string.Empty;
    
    public IEnumerable<ApplicationUser> Users { get; set; } = Enumerable.Empty<ApplicationUser>();

    [BindProperty]
    public HashSet<string> SelectedIds { get; set; } = new();
}