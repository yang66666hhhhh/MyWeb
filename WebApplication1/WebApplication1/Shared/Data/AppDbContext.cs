using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Features.DailyPlans;
using WebApplication1.Features.Growth.Entities;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Shared.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DailyPlan> DailyPlans => Set<DailyPlan>();

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuConfig> MenuConfigs => Set<MenuConfig>();
    public DbSet<MenuTag> MenuTags => Set<MenuTag>();
    public DbSet<UserTag> UserTags => Set<UserTag>();
    public DbSet<UserType> UserTypes => Set<UserType>();
    public DbSet<UserTypeTag> UserTypeTags => Set<UserTypeTag>();

    public DbSet<WorkProject> WorkProjects => Set<WorkProject>();
    public DbSet<WorkDevice> WorkDevices => Set<WorkDevice>();
    public DbSet<WorkTaskType> WorkTaskTypes => Set<WorkTaskType>();
    public DbSet<WorkLog> WorkLogs => Set<WorkLog>();
    public DbSet<WorkLogItem> WorkLogItems => Set<WorkLogItem>();
    public DbSet<WorkImportBatch> WorkImportBatches => Set<WorkImportBatch>();
    public DbSet<WorkImportRow> WorkImportRows => Set<WorkImportRow>();
    public DbSet<WorkDailyPlan> WorkDailyPlans => Set<WorkDailyPlan>();

    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitCheckIn> HabitCheckIns => Set<HabitCheckIn>();

    public DbSet<GrowthProject> GrowthProjects => Set<GrowthProject>();
    public DbSet<KnowledgeArticle> KnowledgeArticles => Set<KnowledgeArticle>();
    public DbSet<PostgraduateTask> PostgraduateTasks => Set<PostgraduateTask>();
    public DbSet<ExamMistake> ExamMistakes => Set<ExamMistake>();
    public DbSet<ExamMaterial> ExamMaterials => Set<ExamMaterial>();

    public DbSet<IndustryTemplate> IndustryTemplates => Set<IndustryTemplate>();
    public DbSet<TemplateField> TemplateFields => Set<TemplateField>();
    public DbSet<WorkLogDynamicValue> WorkLogDynamicValues => Set<WorkLogDynamicValue>();
    public DbSet<WorkCategory> WorkCategories => Set<WorkCategory>();

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

        ConfigureTenant(modelBuilder);
        ConfigureRole(modelBuilder);
        ConfigureMenu(modelBuilder);
        ConfigureMenuConfig(modelBuilder);
        ConfigureUserType(modelBuilder);
        ConfigureUser(modelBuilder);
        ConfigureDailyPlan(modelBuilder);
        ConfigureWorkProject(modelBuilder);
        ConfigureWorkDevice(modelBuilder);
        ConfigureWorkTaskType(modelBuilder);
        ConfigureWorkLog(modelBuilder);
        ConfigureWorkLogItem(modelBuilder);
        ConfigureWorkImport(modelBuilder);
        ConfigureWorkDailyPlan(modelBuilder);
        ConfigureHabit(modelBuilder);
        ConfigureGrowth(modelBuilder);
        ConfigureDynamicFields(modelBuilder);
        ConfigureWorkCategory(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.TenantId);
            entity.Property(x => x.Username).HasMaxLength(50).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(100).IsRequired();
            entity.Property(x => x.RealName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Avatar).HasMaxLength(500);
            entity.Property(x => x.Email).HasMaxLength(100);
            entity.Property(x => x.Phone).HasMaxLength(20);
            entity.Property(x => x.Roles).HasMaxLength(100).HasDefaultValue("user");
            entity.Property(x => x.Status).HasConversion<int>().HasDefaultValue(AppUserStatus.Active);
            entity.Property(x => x.LastLoginIp).HasMaxLength(50);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");

            entity.HasOne(x => x.Tenant)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.Username).IsUnique();
            entity.HasIndex(x => x.Email);
            entity.HasIndex(x => x.TenantId);
        });
    }

    private static void ConfigureTenant(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("Tenants");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(x => x.Code).IsUnique();
        });
    }

    private static void ConfigureRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Permissions).HasMaxLength(2000);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("UserRoles");
            entity.HasKey(x => new { x.UserId, x.RoleId });
            entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Role).WithMany().HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureMenu(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tags");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Color).HasMaxLength(20);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.ToTable("MenuItems");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Path).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Icon).HasMaxLength(100);
            entity.Property(x => x.RequiredPermissions).HasMaxLength(500);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");

            entity.HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.Path).IsUnique();
        });

        modelBuilder.Entity<MenuTag>(entity =>
        {
            entity.ToTable("MenuTags");
            entity.HasKey(x => new { x.MenuItemId, x.TagId });
            entity.HasOne(x => x.MenuItem).WithMany(x => x.MenuTags).HasForeignKey(x => x.MenuItemId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Tag).WithMany(x => x.MenuTags).HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserTag>(entity =>
        {
            entity.ToTable("UserTags");
            entity.HasKey(x => new { x.UserId, x.TagId });
            entity.HasOne(x => x.User).WithMany(x => x.UserTags).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Tag).WithMany(x => x.UserTags).HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureMenuConfig(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuConfig>(entity =>
        {
            entity.ToTable("MenuConfigs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Path).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Icon).HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(x => x.Path).IsUnique();
        });
    }

    private static void ConfigureUserType(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserType>(entity =>
        {
            entity.ToTable("UserTypes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Color).HasMaxLength(20);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<UserTypeTag>(entity =>
        {
            entity.ToTable("UserTypeTags");
            entity.HasKey(x => new { x.UserTypeId, x.TagId });
            entity.HasOne(x => x.UserType).WithMany(x => x.UserTypeTags).HasForeignKey(x => x.UserTypeId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(x => x.Tag).WithMany().HasForeignKey(x => x.TagId).OnDelete(DeleteBehavior.Cascade);
        });
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
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        modelBuilder.Entity<WorkProject>(entity =>
        {
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

            entity.HasOne(x => x.Template)
                .WithMany()
                .HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(x => x.WorkDate);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.ProjectId);
            entity.HasIndex(x => x.UserId);
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
            entity.Property(x => x.DuplicateStatus).HasDefaultValue(0);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");

            entity.HasOne(x => x.Batch)
                .WithMany(x => x.Rows)
                .HasForeignKey(x => x.BatchId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureHabit(ModelBuilder modelBuilder)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        modelBuilder.Entity<Habit>(entity =>
        {
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
        });

        modelBuilder.Entity<HabitCheckIn>(entity =>
        {
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
        });
    }

    private static void ConfigureGrowth(ModelBuilder modelBuilder)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            v => v.ToDateTime(TimeOnly.MinValue),
            v => DateOnly.FromDateTime(v));

        modelBuilder.Entity<GrowthProject>(entity =>
        {
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
        });

        modelBuilder.Entity<KnowledgeArticle>(entity =>
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
        });

        modelBuilder.Entity<PostgraduateTask>(entity =>
        {
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
        });

        modelBuilder.Entity<ExamMistake>(entity =>
        {
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
        });

        modelBuilder.Entity<ExamMaterial>(entity =>
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
        });
    }

    private static void ConfigureDynamicFields(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IndustryTemplate>(entity =>
        {
            entity.ToTable("IndustryTemplates");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500);
            entity.Property(x => x.Industry).HasMaxLength(50).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<TemplateField>(entity =>
        {
            entity.ToTable("TemplateFields");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FieldName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.FieldLabel).HasMaxLength(100).IsRequired();
            entity.Property(x => x.FieldType).HasConversion<int>().HasDefaultValue(FieldType.Text);
            entity.Property(x => x.Options).HasMaxLength(1000);
            entity.Property(x => x.DefaultValue).HasMaxLength(500);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");

            entity.HasOne(x => x.Template)
                .WithMany(x => x.Fields)
                .HasForeignKey(x => x.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.TemplateId, x.FieldName }).IsUnique();
        });

        modelBuilder.Entity<WorkLogDynamicValue>(entity =>
        {
            entity.ToTable("WorkLogDynamicValues");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.FieldName).HasMaxLength(50).IsRequired();
            entity.Property(x => x.StringValue).HasMaxLength(1000);
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");

            entity.HasOne(x => x.WorkLog)
                .WithMany(x => x.DynamicValues)
                .HasForeignKey(x => x.WorkLogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.WorkLogId, x.FieldName }).IsUnique();
        });
    }

    private static void ConfigureWorkCategory(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkCategory>(entity =>
        {
            entity.ToTable("WorkCategories");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(50).IsRequired();
            entity.Property(x => x.CreatedAt).HasColumnType("datetime");

            entity.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => x.ParentId);
        });
    }
}