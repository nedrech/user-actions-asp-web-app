using System.ComponentModel.DataAnnotations.Schema;

namespace Nedrech.WebApp.Models;

public class ApplicationMessage
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public string SenderUserName { get; init; } = string.Empty;
    
    public string SenderUserId { get; init; } = string.Empty;

    public string ReceiverUserId { get; init; } = string.Empty;

    public string Subject { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public DateTimeOffset Date { get; init; } = DateTimeOffset.UtcNow;

    [NotMapped]
    public string DateTimeStr => Date.LocalDateTime.ToString("dd-MM-yyyy HH:mm:ss");
}