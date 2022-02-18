namespace Nedrech.WebApp.Models;

public interface IMessageRepository
{
    IQueryable<ApplicationMessage> Messages { get; }

    Task CreateMessageRangeAsync(IEnumerable<ApplicationMessage> messages);
}