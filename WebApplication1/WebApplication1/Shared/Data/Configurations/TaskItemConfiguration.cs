using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Features.Tasks;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Shared.Data.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("Tasks");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.UserId);
        entity.Property(x => x.PlanDate).HasColumnType("date").HasConversion(dateOnlyConverter).IsRequired();
        entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(2000);
        entity.Property(x => x.Type).HasConversion<int>().HasDefaultValue(TaskType.Personal);
        entity.Property(x => x.Source).HasConversion<int>().HasDefaultValue(TaskSource.Growth);
        entity.Property(x => x.ProjectId);
        entity.Property(x => x.Priority).HasConversion<int>().HasDefaultValue(TaskPriority.Medium);
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(TaskItemStatus.Pending);
        entity.Property(x => x.StartTime).HasMaxLength(10);
        entity.Property(x => x.EndTime).HasMaxLength(10);
        entity.Property(x => x.EstimatedHours).HasColumnType("decimal(10,2)");
        entity.Property(x => x.ActualHours).HasColumnType("decimal(10,2)");
        entity.Property(x => x.ConvertedWorkLogId);
        entity.Property(x => x.Remark).HasMaxLength(1000);
        entity.Property(x => x.CompletedAt).HasColumnType("datetime");
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasIndex(x => x.UserId);
        entity.HasIndex(x => x.PlanDate);
        entity.HasIndex(x => x.Type);
        entity.HasIndex(x => x.Status);
        entity.HasIndex(x => x.Source);
    }
}