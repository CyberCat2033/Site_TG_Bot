using Microsoft.EntityFrameworkCore;

public class BotContext : DbContext
{
    public BotContext(DbContextOptions<BotContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<LostReport> LostReports { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=/home/cybercat/Site_TG_Bot/DataBases/bot.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new ReportsConfiguration());
    }
}
