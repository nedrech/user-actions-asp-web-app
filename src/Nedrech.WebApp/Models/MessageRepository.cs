using Microsoft.EntityFrameworkCore;

namespace Nedrech.WebApp.Models;

public class MessageRepository : IMessageRepository
{
    private readonly ApplicationDbContext _context;

    public MessageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<ApplicationMessage> Messages =>
        _context.Messages;

    public async Task CreateMessageRangeAsync(IEnumerable<ApplicationMessage> messages)
    {
        _context.AddRange(messages);
        await _context.SaveChangesAsync();
    }
}