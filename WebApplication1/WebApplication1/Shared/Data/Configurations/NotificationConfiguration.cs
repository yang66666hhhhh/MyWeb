using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Notification;

namespace WebApplication1.Shared.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> entity)
    {
        entity.ToTable("Notifications");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.UserId).IsRequired();
        entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Content).HasMaxLength(2000);
        entity.Property(x => x.Type).HasConversion<int>().HasDefaultValue(NotificationType.System);
        entity.Property(x => x.IsRead).HasDefaultValue(false);
        entity.Property(x => x.ReadAt).HasColumnType("datetime");
        entity.Property(x => x.Link).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasIndex(x => x.UserId);
        entity.HasIndex(x => new { x.UserId, x.IsRead });
        entity.HasIndex(x => x.Type);
        entity.HasIndex(x => x.CreatedAt);
    }
}
