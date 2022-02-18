namespace Nedrech.WebApp.Models.ViewModels;

public class MessageModel
{
    public string[] ReceiverIds { get; set; } = Array.Empty<string>();

    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}