using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasMany(u => u.Reports)
        .WithOne(r => r.User) // если в Report есть ссылка на User
        .HasForeignKey(r => r.UserId) // имя внешнего ключа в Report
        .OnDelete(DeleteBehavior.Cascade);
    }
}
