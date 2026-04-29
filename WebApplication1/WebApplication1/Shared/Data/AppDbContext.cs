using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Features.DailyPlans;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Shared.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DailyPlan> DailyPlans => Set<DailyPlan>();

    public DbSet<WorkProject> WorkProjects => Set<WorkProject>();
    public DbSet<WorkDevice> WorkDevices => Set<WorkDevice>();
    public DbSet<WorkTaskType> WorkTaskTypes => Set<WorkTaskType>();
    public DbSet<WorkLog> WorkLogs => Set<WorkLog>();
    public DbSet<WorkLogItem> WorkLogItems => Set<WorkLogItem>();
    public DbSet<WorkImportBatch> WorkImportBatches => Set<WorkImportBatch>();
    public DbSet<WorkImportRow> WorkImportRows => Set<WorkImportRow>();
    public DbSet<WorkDailyPlan> WorkDailyPlans => Set<WorkDailyPlan>();

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is EntityBase baseEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    baseEntity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    baseEntity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureDailyPlan(modelBuilder);
        ConfigureWorkProject(modelBuilder);
        ConfigureWorkDevice(modelBuilder);
        ConfigureWorkTaskType(modelBuilder);
        ConfigureWorkLog(modelBuilder);
        ConfigureWorkLogItem(modelBuilder);
        ConfigureWorkImport(modelBuilder);
        ConfigureWorkDailyPlan(modelBuilder);
    }

    private static void ConfigureWorkDailyPlan(ModelBuilder modelBuilder)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        modelBuilder.Entity<WorkDailyPlan>(entity =>
        {
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
        });
    }

    private static void ConfigureDailyPlan(ModelBuilder modelBuilder)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        modelBuilder.Entity<DailyPlan>(entity =>
        {
            entity.ToTable("DailyPlans");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.PlanDate).HasColumnType("date").HasConversion(dateOnlyConverter).IsRequired();
            entity.Property(x => x.Title).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.Priority).HasDefaultValue(3);
            entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(DailyPlanStatus.Pending);
            entity.Property(x => x.Remark).HasMaxLength(1000);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(x => x.PlanDate);
            entity.HasIndex(x => x.Status);
        });
    }

    private static void ConfigureWorkProject(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkProject>(entity =>
        {
            entity.ToTable("WorkProjects");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ProjectName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.ProjectCode).HasMaxLength(50);
            entity.Property(x => x.ProjectType).HasConversion<int>().HasDefaultValue(WorkProjectType.Internal);
            entity.Property(x => x.CustomerName).HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(WorkProjectStatus.Active);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(x => x.ProjectName);
            entity.HasIndex(x => x.Status);
        });
    }

    private static void ConfigureWorkDevice(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkDevice>(entity =>
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
        });
    }

    private static void ConfigureWorkTaskType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkTaskType>(entity =>
        {
            entity.ToTable("WorkTaskTypes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TypeName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.TypeCode).HasMaxLength(50);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Enabled).HasDefaultValue(true);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(x => x.TypeName).IsUnique();
        });
    }

    private static void ConfigureWorkLog(ModelBuilder modelBuilder)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        modelBuilder.Entity<WorkLog>(entity =>
        {
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
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");

            entity.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.WorkDate);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.ProjectId);
        });
    }

    private static void ConfigureWorkLogItem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkLogItem>(entity =>
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
        });
    }

    private static void ConfigureWorkImport(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkImportBatch>(entity =>
        {
            entity.ToTable("WorkImportBatches");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FileName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(WorkImportStatus.Pending);
            entity.Property(x => x.ImportStrategy).HasConversion<int>().HasDefaultValue(WorkImportStrategy.SkipDuplicate);
            entity.Property(x => x.ErrorMessage).HasMaxLength(2000);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(x => x.Status);
        });

        modelBuilder.Entity<WorkImportRow>(entity =>
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
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");

            entity.HasOne(x => x.Batch)
                .WithMany(x => x.Rows)
                .HasForeignKey(x => x.BatchId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}