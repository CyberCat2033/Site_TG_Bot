using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

public class UserRepository
{
    private readonly BotContext _context;

    public UserRepository(BotContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByChatIdAsync(long chatId)
    {
        return await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.chatId == chatId);
    }

    public async Task<bool> Contains(long chatId) =>
        await _context.Users.AnyAsync(e => e.chatId == chatId);

    public async Task Add(Guid ID, long chatId)
    {
        var User = new User { Id = ID, chatId = chatId };
        await _context.AddAsync(User);
        await _context.SaveChangesAsync();
    }

    public async Task Add(Guid ID, Message message)
    {
        var User = new User
        {
            Id = ID,
            chatId = message.Chat.Id,
            UserName = message.Chat.Username,
        };
        await _context.AddAsync(User);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByChatId(long chatId)
    {
        await _context.Users.Where(u => u.chatId == chatId).ExecuteDeleteAsync();
        await _context.SaveChangesAsync();
    }
}
