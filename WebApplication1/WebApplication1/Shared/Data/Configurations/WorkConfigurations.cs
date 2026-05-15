using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Shared.Data.Configurations;

public class WorkProjectConfiguration : IEntityTypeConfiguration<WorkProject>
{
    public void Configure(EntityTypeBuilder<WorkProject> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("WorkProjects");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.ProjectName).HasMaxLength(100).IsRequired();
        entity.Property(x => x.ProjectCode).HasMaxLength(50);
        entity.Property(x => x.ProjectType).HasConversion<int>().HasDefaultValue(WorkProjectType.Internal);
        entity.Property(x => x.CustomerName).HasMaxLength(100);
        entity.Property(x => x.Description).HasMaxLength(1000);
        entity.Property(x => x.StartDate).HasColumnType("date").HasConversion(dateOnlyConverter);
        entity.Property(x => x.EndDate).HasColumnType("date").HasConversion(dateOnlyConverter);
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(WorkProjectStatus.Active);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.ProjectName);
        entity.HasIndex(x => x.Status);
    }
}

public class WorkDeviceConfiguration : IEntityTypeConfiguration<WorkDevice>
{
    public void Configure(EntityTypeBuilder<WorkDevice> entity)
    {
        entity.ToTable("WorkDevices");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.DeviceName).HasMaxLength(100).IsRequired();
        entity.Property(x => x.DeviceCode).HasMaxLength(50);
        entity.Property(x => x.DeviceType).HasConversion<int>().HasDefaultValue(WorkDeviceType.Equipment);
        entity.Property(x => x.Description).HasMaxLength(1000);
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(WorkDeviceStatus.Active);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasIndex(x => x.DeviceName);
        entity.HasIndex(x => x.Status);
    }
}

public class WorkTaskTypeConfiguration : IEntityTypeConfiguration<WorkTaskType>
{
    public void Configure(EntityTypeBuilder<WorkTaskType> entity)
    {
        entity.ToTable("WorkTaskTypes");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.TypeName).HasMaxLength(50).IsRequired();
        entity.Property(x => x.TypeCode).HasMaxLength(50);
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.Enabled).HasDefaultValue(true);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.TypeName).IsUnique();
    }
}

public class WorkLogConfiguration : IEntityTypeConfiguration<WorkLog>
{
    public void Configure(EntityTypeBuilder<WorkLog> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("WorkLogs");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.WorkDate).HasColumnType("date").HasConversion(dateOnlyConverter).IsRequired();
        entity.Property(x => x.WeekDay).HasMaxLength(10);
        entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
        entity.Property(x => x.OriginalContent).HasMaxLength(4000);
        entity.Property(x => x.Summary).HasMaxLength(1000);
        entity.Property(x => x.TotalHours).HasPrecision(10, 2);
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(WorkLogStatus.Normal);
        entity.Property(x => x.SourceType).HasConversion<int>().HasDefaultValue(WorkLogSourceType.Manual);
        entity.Property(x => x.Remark).HasMaxLength(1000);
        entity.Property(x => x.PersonaCode).HasMaxLength(50);
        entity.Property(x => x.ExtraData).HasMaxLength(4000);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(x => x.Template)
            .WithMany()
            .HasForeignKey(x => x.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasIndex(x => x.WorkDate);
        entity.HasIndex(x => x.Status);
        entity.HasIndex(x => x.ProjectId);
        entity.HasIndex(x => x.UserId);
        entity.HasIndex(x => x.PersonaCode);
    }
}

public class WorkLogItemConfiguration : IEntityTypeConfiguration<WorkLogItem>
{
    public void Configure(EntityTypeBuilder<WorkLogItem> entity)
    {
        entity.ToTable("WorkLogItems");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Content).HasMaxLength(2000).IsRequired();
        entity.Property(x => x.Hours).HasPrecision(10, 2);
        entity.Property(x => x.ProgressPercent);
        entity.Property(x => x.Remark).HasMaxLength(500);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.WorkLog)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.WorkLogId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.TaskType)
            .WithMany()
            .HasForeignKey(x => x.TaskTypeId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(x => x.Device)
            .WithMany()
            .HasForeignKey(x => x.DeviceId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class WorkImportBatchConfiguration : IEntityTypeConfiguration<WorkImportBatch>
{
    public void Configure(EntityTypeBuilder<WorkImportBatch> entity)
    {
        entity.ToTable("WorkImportBatches");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.FileName).HasMaxLength(200).IsRequired();
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(WorkImportStatus.Pending);
        entity.Property(x => x.ImportStrategy).HasConversion<int>().HasDefaultValue(WorkImportStrategy.SkipDuplicate);
        entity.Property(x => x.ErrorMessage).HasMaxLength(2000);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.HasIndex(x => x.Status);
    }
}

public class WorkImportRowConfiguration : IEntityTypeConfiguration<WorkImportRow>
{
    public void Configure(EntityTypeBuilder<WorkImportRow> entity)
    {
        entity.ToTable("WorkImportRows");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.RawDate).HasMaxLength(50);
        entity.Property(x => x.RawWeekDay).HasMaxLength(10);
        entity.Property(x => x.RawProject).HasMaxLength(100);
        entity.Property(x => x.RawDevice).HasMaxLength(200);
        entity.Property(x => x.RawTaskType).HasMaxLength(200);
        entity.Property(x => x.RawContent).HasMaxLength(2000);
        entity.Property(x => x.RawHours).HasMaxLength(20);
        entity.Property(x => x.RawRemark).HasMaxLength(500);
        entity.Property(x => x.ErrorMessage).HasMaxLength(500);
        entity.Property(x => x.ValidationStatus).HasConversion<int>().HasDefaultValue(WorkImportValidationStatus.Valid);
        entity.Property(x => x.DuplicateStatus).HasDefaultValue(0);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Batch)
            .WithMany(x => x.Rows)
            .HasForeignKey(x => x.BatchId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WorkDailyPlanConfiguration : IEntityTypeConfiguration<WorkDailyPlan>
{
    public void Configure(EntityTypeBuilder<WorkDailyPlan> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("WorkDailyPlans");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.Title).HasMaxLength(120).IsRequired();
        entity.Property(x => x.Content).HasMaxLength(1000);
        entity.Property(x => x.Priority).HasConversion<int>().HasDefaultValue(WorkDailyPlanPriority.Medium);
        entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(WorkDailyPlanStatus.Pending);
        entity.Property(x => x.StartTime).HasMaxLength(10);
        entity.Property(x => x.EndTime).HasMaxLength(10);
        entity.Property(x => x.Remark).HasMaxLength(1000);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");
        entity.Property(x => x.PlanDate).HasColumnType("date").HasConversion(dateOnlyConverter);

        entity.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasIndex(x => x.PlanDate);
        entity.HasIndex(x => x.Status);
    }
}

public class WorkLogTemplateConfiguration : IEntityTypeConfiguration<WorkLogTemplate>
{
    public void Configure(EntityTypeBuilder<WorkLogTemplate> entity)
    {
        entity.ToTable("WorkLogTemplates");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.PersonaCode).HasMaxLength(50).IsRequired();
        entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
        entity.Property(x => x.Description).HasMaxLength(500);
        entity.Property(x => x.FieldDefinitions).HasMaxLength(4000).IsRequired();
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasIndex(x => x.PersonaCode).IsUnique();
    }
}

public class ImplLogConfiguration : IEntityTypeConfiguration<ImplLog>
{
    public void Configure(EntityTypeBuilder<ImplLog> entity)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        entity.ToTable("ImplLogs");
        entity.HasKey(x => x.Id);
        entity.Property(x => x.WorkDate).HasColumnType("date").HasConversion(dateOnlyConverter).IsRequired();
        entity.Property(x => x.WeekDay).HasMaxLength(10);
        entity.Property(x => x.Title).HasMaxLength(200).IsRequired();
        entity.Property(x => x.ProjectName).HasMaxLength(200);
        entity.Property(x => x.TotalHours).HasPrecision(10, 2);
        entity.Property(x => x.PersonaCode).HasMaxLength(50);
        entity.Property(x => x.ExtraData).HasMaxLength(4000);
        entity.Property(x => x.CreatedAt).HasColumnType("datetime");

        entity.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasOne(x => x.Template)
            .WithMany()
            .HasForeignKey(x => x.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasIndex(x => x.ProjectId);
        entity.HasIndex(x => x.TemplateId);
        entity.HasIndex(x => x.UserId);
        entity.HasIndex(x => x.WorkDate);
    }
}
