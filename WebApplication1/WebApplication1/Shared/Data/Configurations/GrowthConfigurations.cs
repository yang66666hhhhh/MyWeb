using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Features.Growth.Entities;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Shared.Data.Configurations;

public class HabitConfiguration : IEntityTypeConfiguration<Habit>
{
    public void Configure(EntityTypeBuilder<Habit> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("Habits");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        entity.Property(x => x.HabitType).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.TargetFrequency).HasMaxLength(20);
        entity.Property(x => x.Status).HasDefaultValue(1);
        entity.Property(x => x.CurrentStreak).HasDefaultValue(0);
        entity.Property(x => x.LongestStreak).HasDefaultValue(0);
        entity.Property(x => x.TotalCheckIns).HasDefaultValue(0);
        entity.Property(x => x.LastCheckInDate).HasColumnType("date").HasConversion(dateOnlyConverter);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasIndex(x => x.Name);
        entity.HasIndex(x => x.Status);
        entity.HasIndex(x => x.UserId);
    }
}

public class HabitCheckInConfiguration : IEntityTypeConfiguration<HabitCheckIn>
{
    public void Configure(EntityTypeBuilder<HabitCheckIn> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("HabitCheckIns");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.CheckInDate).HasColumnType("date").HasConversion(dateOnlyConverter).IsRequired();
        entity.Property(x => x.Remark).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Habit)
            .WithMany(x => x.CheckIns)
            .HasForeignKey(x => x.HabitId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => x.CheckInDate);
        entity.HasIndex(x => new { x.HabitId, x.CheckInDate }).IsUnique();
    }
}

public class GrowthProjectConfiguration : IEntityTypeConfiguration<GrowthProject>
{
    public void Configure(EntityTypeBuilder<GrowthProject> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("GrowthProjects");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(1000);
        entity.Property(x => x.StartDate).HasColumnType("date").HasConversion(dateOnlyConverter);
        entity.Property(x => x.EndDate).HasColumnType("date").HasConversion(dateOnlyConverter);
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(GrowthProjectStatus.InProgress);
        entity.Property(x => x.Type).HasConversion<int>().HasDefaultValue(GrowthProjectType.Personal);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Name);
        entity.HasIndex(x => x.Status);
        entity.HasIndex(x => x.UserId);
    }
}

public class KnowledgeArticleConfiguration : IEntityTypeConfiguration<KnowledgeArticle>
{
    public void Configure(EntityTypeBuilder<KnowledgeArticle> entity)
    {
        entity.ToTable("KnowledgeArticles");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Content).HasMaxLength(10000);
        entity.Property(x => x.Category).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Tags).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Title);
        entity.HasIndex(x => x.Category);
        entity.HasIndex(x => x.UserId);
    }
}

public class PostgraduateTaskConfiguration : IEntityTypeConfiguration<PostgraduateTask>
{
    public void Configure(EntityTypeBuilder<PostgraduateTask> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("PostgraduateTasks");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(1000);
        entity.Property(x => x.DueDate).HasColumnType("date").HasConversion(dateOnlyConverter);
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(PostgraduateTaskStatus.Pending);
        entity.Property(x => x.Priority).HasConversion<int>().HasDefaultValue(PostgraduateTaskPriority.Medium);
        entity.Property(x => x.Type).HasConversion<int>().HasDefaultValue(PostgraduateTaskType.Study);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Status);
        entity.HasIndex(x => x.DueDate);
        entity.HasIndex(x => x.UserId);
    }
}

public class ExamMistakeConfiguration : IEntityTypeConfiguration<ExamMistake>
{
    public void Configure(EntityTypeBuilder<ExamMistake> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("ExamMistakes");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Question).HasMaxLength(1000).IsRequired();
        entity.Property(x => x.Answer).HasMaxLength(2000);
        entity.Property(x => x.Explanation).HasMaxLength(2000);
        entity.Property(x => x.Subject).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Tags).HasMaxLength(500);
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(ExamMistakeStatus.Pending);
        entity.Property(x => x.LastReviewDate).HasColumnType("date").HasConversion(dateOnlyConverter);
        entity.Property(x => x.NextReviewDate).HasColumnType("date").HasConversion(dateOnlyConverter);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Subject);
        entity.HasIndex(x => x.UserId);
        entity.HasIndex(x => x.Status);
    }
}

public class ExamMaterialConfiguration : IEntityTypeConfiguration<ExamMaterial>
{
    public void Configure(EntityTypeBuilder<ExamMaterial> entity)
    {
        entity.ToTable("ExamMaterials");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Content).HasMaxLength(10000);
        entity.Property(x => x.Subject).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Tags).HasMaxLength(500);
        entity.Property(x => x.Type).HasConversion<int>().HasDefaultValue(ExamMaterialType.Note);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Subject);
        entity.HasIndex(x => x.Type);
        entity.HasIndex(x => x.UserId);
    }
}
