using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication1.Features.Content.Entities;

namespace WebApplication1.Shared.Data.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Content)
            .HasMaxLength(50000);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("draft");

        builder.Property(x => x.Tags)
            .HasMaxLength(500);

        builder.Property(x => x.Category)
            .HasMaxLength(100);

        builder.Property(x => x.PublishedAt)
            .HasMaxLength(30);

        builder.Property(x => x.Remark)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.Title);
        builder.HasIndex(x => x.CreatedAt);
    }
}

public class MediaItemConfiguration : IEntityTypeConfiguration<MediaItem>
{
    public void Configure(EntityTypeBuilder<MediaItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.FileUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.FileType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Tags)
            .HasMaxLength(500);

        builder.Property(x => x.Remark)
            .HasMaxLength(500);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.FileType);
        builder.HasIndex(x => x.FileName);
        builder.HasIndex(x => x.CreatedAt);
    }
}

public class PublishingCalendarConfiguration : IEntityTypeConfiguration<PublishingCalendar>
{
    public void Configure(EntityTypeBuilder<PublishingCalendar> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.PlannedDate)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Platform)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("pending");

        builder.Property(x => x.Remark)
            .HasMaxLength(500);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Platform);
        builder.HasIndex(x => x.PlannedDate);
        builder.HasIndex(x => x.Title);
    }
}
