using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Ai.Entities;
using WebApplication1.Features.Admin.Entities;
using WebApplication1.Features.Assets.Entities;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Features.Auth.Entities.Subscription;
using WebApplication1.Features.Content.Entities;
using WebApplication1.Features.DailyPlans;
using WebApplication1.Features.Growth.Entities;
using WebApplication1.Features.Network.Entities;
using WebApplication1.Features.Notification;
using WebApplication1.Features.Tasks;
using WebApplication1.Features.Persona.Entities;
using WebApplication1.Features.Analytics.Entities;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared;

namespace WebApplication1.Shared.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DailyPlan> DailyPlans => Set<DailyPlan>();

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    public DbSet<Notification> Notifications => Set<Notification>();

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
    public DbSet<WorkLogTemplate> WorkLogTemplates => Set<WorkLogTemplate>();
    public DbSet<WorkImportBatch> WorkImportBatches => Set<WorkImportBatch>();
    public DbSet<WorkImportRow> WorkImportRows => Set<WorkImportRow>();
    public DbSet<WorkDailyPlan> WorkDailyPlans => Set<WorkDailyPlan>();
    public DbSet<Okr> Okrs => Set<Okr>();
    public DbSet<WorkRisk> WorkRisks => Set<WorkRisk>();
    public DbSet<WorkFile> WorkFiles => Set<WorkFile>();
    public DbSet<ImplLog> ImplLogs => Set<ImplLog>();
    public DbSet<WeeklyPlan> WeeklyPlans => Set<WeeklyPlan>();
    public DbSet<WeeklyPlanTask> WeeklyPlanTasks => Set<WeeklyPlanTask>();
    public DbSet<SoftwareAsset> SoftwareAssets => Set<SoftwareAsset>();

    public DbSet<Habit> Habits => Set<Habit>();
    public DbSet<HabitCheckIn> HabitCheckIns => Set<HabitCheckIn>();

    public DbSet<GrowthProject> GrowthProjects => Set<GrowthProject>();
    public DbSet<KnowledgeArticle> KnowledgeArticles => Set<KnowledgeArticle>();
    public DbSet<PostgraduateTask> PostgraduateTasks => Set<PostgraduateTask>();
    public DbSet<ExamMistake> ExamMistakes => Set<ExamMistake>();
    public DbSet<ExamMaterial> ExamMaterials => Set<ExamMaterial>();
    public DbSet<StudentSubject> StudentSubjects => Set<StudentSubject>();
    public DbSet<StudentStudyRecord> StudentStudyRecords => Set<StudentStudyRecord>();

    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<YearPlan> YearPlans => Set<YearPlan>();
    public DbSet<MonthlyReview> MonthlyReviews => Set<MonthlyReview>();
    public DbSet<LearningPath> LearningPaths => Set<LearningPath>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<FitnessRecord> FitnessRecords => Set<FitnessRecord>();
    public DbSet<SleepRecord> SleepRecords => Set<SleepRecord>();
    public DbSet<MoodRecord> MoodRecords => Set<MoodRecord>();
    public DbSet<ReadingBook> ReadingBooks => Set<ReadingBook>();
    public DbSet<FocusSession> FocusSessions => Set<FocusSession>();

    public DbSet<IndustryTemplate> IndustryTemplates => Set<IndustryTemplate>();
    public DbSet<TemplateField> TemplateFields => Set<TemplateField>();
    public DbSet<WorkLogDynamicValue> WorkLogDynamicValues => Set<WorkLogDynamicValue>();
    public DbSet<WorkCategory> WorkCategories => Set<WorkCategory>();

    public DbSet<AiPlan> AiPlans => Set<AiPlan>();
    public DbSet<AiReport> AiReports => Set<AiReport>();
    public DbSet<AiChatSession> AiChatSessions => Set<AiChatSession>();
    public DbSet<AiChatMessage> AiChatMessages => Set<AiChatMessage>();
    public DbSet<AutomationWorkflow> AutomationWorkflows => Set<AutomationWorkflow>();
    public DbSet<AiInsight> AiInsights => Set<AiInsight>();
    public DbSet<KnowledgeChatSessionItem> KnowledgeChatSessionItems => Set<KnowledgeChatSessionItem>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<PersonaType> PersonaTypes => Set<PersonaType>();
    public DbSet<UserPersonaRecord> UserPersonaRecords => Set<UserPersonaRecord>();
    public DbSet<PersonaMenuItem> PersonaMenuItems => Set<PersonaMenuItem>();

    public DbSet<RoleMenu> RoleMenus => Set<RoleMenu>();
    public DbSet<UserPersona> UserPersonas => Set<UserPersona>();
    public DbSet<MenuAction> MenuActions => Set<MenuAction>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<PersonaFeature> PersonaFeatures => Set<PersonaFeature>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

    public DbSet<Investment> Investments => Set<Investment>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Income> Incomes => Set<Income>();

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<MediaItem> MediaItems => Set<MediaItem>();
    public DbSet<PublishingCalendar> PublishingCalendars => Set<PublishingCalendar>();

    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Interaction> Interactions => Set<Interaction>();

    // Persona
    public DbSet<CodeRepository> CodeRepositories => Set<CodeRepository>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<Pipeline> Pipelines => Set<Pipeline>();
    public DbSet<DesignAsset> DesignAssets => Set<DesignAsset>();
    public DbSet<Prototype> Prototypes => Set<Prototype>();
    public DbSet<TeacherCourse> TeacherCourses => Set<TeacherCourse>();
    public DbSet<TeacherStudent> TeacherStudents => Set<TeacherStudent>();

    public DbSet<CustomReport> CustomReports => Set<CustomReport>();

    public override int SaveChanges()
    {
        ApplySoftDelete();
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplySoftDelete();
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplySoftDelete()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Deleted && e.Entity is EntityBase);

        foreach (var entry in entries)
        {
            if (entry.Entity is EntityBase baseEntity)
            {
                entry.State = EntityState.Modified;
                baseEntity.IsDeleted = true;
                baseEntity.DeletedAt = DateTime.UtcNow;
            }
        }
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(EntityBase).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(ApplySoftDeleteFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);
                method.Invoke(null, [modelBuilder]);
            }
        }
    }

    private static void ApplySoftDeleteFilter<T>(ModelBuilder modelBuilder) where T : EntityBase
    {
        modelBuilder.Entity<T>().HasQueryFilter(x => !x.IsDeleted);
    }
}
