using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ReportsConfiguration : IEntityTypeConfiguration<LostReport>
{
    public void Configure(EntityTypeBuilder<LostReport> builder)
    {
          builder.HasOne(r => r.User)
               .WithMany(u => u.Reports)
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
