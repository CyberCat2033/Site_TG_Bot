using Microsoft.EntityFrameworkCore;

public class LostReportRepository
{
    private readonly BotContext _context;

    public LostReportRepository(BotContext context)
    {
        _context = context;
    }

    public async Task Add(long chatId, LostReport report)
    {
        var user =
            await _context.Users.FirstOrDefaultAsync(u => u.chatId == chatId)
            ?? throw new Exception();
        user.Reports.Add(report);
    }

    public async Task AddOrUpdate(long chatId, LostReport report)
    {
        var user = await _context
            .Users.Include(u => u.Reports) // Убедитесь, что загрузили коллекцию отчетов
            .FirstOrDefaultAsync(u => u.chatId == chatId);
        if (user is null)
            return;

        if (!user.Reports.Any(u => u.Id == report.Id))
        {
            report.UserId = user.Id;
            report.User = user;
            _context.Add(report);
            await _context.SaveChangesAsync();
            return;
        }
        else
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePartial(
        long chatId,
        Guid reportId,
        string? name = null,
        string? feedback = null,
        string? description = null,
        string? location = null,
        string? photoPath = null
    )
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.chatId == chatId);

        if (user is null)
            return;

        var existingReport = user.Reports.FirstOrDefault(r => r.Id == reportId);

        if (existingReport is null)
            return;

        if (!string.IsNullOrWhiteSpace(name))
            existingReport.Name = name;

        if (!string.IsNullOrWhiteSpace(feedback))
            existingReport.Feedback = feedback;

        if (!string.IsNullOrWhiteSpace(description))
            existingReport.Description = description;

        if (!string.IsNullOrWhiteSpace(location))
            existingReport.Location = location;

        if (!string.IsNullOrWhiteSpace(photoPath))
            existingReport.PhotoPath = photoPath;

        await _context.SaveChangesAsync();
    }

    public Task<List<LostReport>> GetAll(long chatId)
    {
        if (!_context.Users.Any(u => u.chatId == chatId))
        {
            throw new ArgumentException("Сначала запустите бота");
        }
        return _context
            .Users.Where(u => u.chatId == chatId)
            .AsNoTracking()
            .SelectMany(u => u.Reports)
            .ToListAsync();
    }

    public async Task<LostReport> GetForUpdate(long chatId, Guid reportId)
    {
        return await _context
                .LostReports.AsTracking()
                .FirstOrDefaultAsync(r => r.Id == reportId && r.User.chatId == chatId)
            ?? throw new Exception("Wrong Report Id");
    }

    public async Task<LostReport> Get(long chatId, Guid reportId)
    {
        var user =
            await _context
                .Users.AsNoTracking()
                .Include(u => u.Reports)
                .FirstOrDefaultAsync(u => u.chatId == chatId)
            ?? throw new Exception("Wrong chatId");
        return user.Reports.FirstOrDefault(r => r.Id == reportId)
            ?? throw new Exception("Wrong Report Id");
    }
}
