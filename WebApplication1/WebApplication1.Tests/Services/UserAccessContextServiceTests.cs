using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Admin.Entities;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Features.Auth.Entities.Subscription;
using WebApplication1.Shared.Data;

namespace WebApplication1.Tests.Services;

public class UserAccessContextServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserAccessContextService _service;

    public UserAccessContextServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new UserAccessContextService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var result = await _service.GetAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAsync_ShouldUseFreePlan_WhenUserHasNoSubscription()
    {
        var user = new AppUser
        {
            Username = "member",
            RealName = "Member",
            PasswordHash = "test",
            Roles = "member"
        };
        var free = new Plan { Code = "Free", Name = "免费版", IsActive = true };
        var habit = new Feature { Code = "GROWTH_HABIT", Name = "习惯打卡", Category = "Growth" };

        _context.Users.Add(user);
        _context.Plans.Add(free);
        _context.Features.Add(habit);
        _context.PlanFeatures.Add(new PlanFeature { Plan = free, Feature = habit });
        await _context.SaveChangesAsync();

        var result = await _service.GetAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("member", result.RoleCode);
        Assert.Equal(1, result.RoleLevel);
        Assert.Equal("Free", result.PlanCode);
        Assert.Contains("GROWTH_HABIT", result.FeatureCodes);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnOwnerFeaturesAndPersonaCodes()
    {
        var user = new AppUser
        {
            Username = "owner",
            RealName = "Owner",
            PasswordHash = "test",
            Roles = "member,owner"
        };
        var pro = new Plan { Code = "Pro", Name = "专业版", IsActive = true };
        var workLog = new Feature { Code = "WORK_LOG", Name = "工作日志", Category = "Work" };
        var devRepo = new Feature { Code = "DEV_CODE_REPO", Name = "代码仓库", Category = "Persona" };
        var developer = new PersonaType { Code = "Developer", Name = "开发者", IsActive = true };

        _context.Users.Add(user);
        _context.Plans.Add(pro);
        _context.Features.AddRange(workLog, devRepo);
        _context.PlanFeatures.Add(new PlanFeature { Plan = pro, Feature = workLog });
        _context.PersonaTypes.Add(developer);
        _context.UserPersonas.Add(new UserPersona { User = user, PersonaType = developer, IsPrimary = true });
        _context.PersonaFeatures.Add(new PersonaFeature { PersonaCode = developer.Code, Feature = devRepo });
        _context.UserSubscriptions.Add(new UserSubscription
        {
            UserId = user.Id,
            Plan = pro,
            StartAt = DateTime.UtcNow.AddDays(-1),
            IsActive = true
        });
        await _context.SaveChangesAsync();

        var result = await _service.GetAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("owner", result.RoleCode);
        Assert.Equal(3, result.RoleLevel);
        Assert.Equal("Pro", result.PlanCode);
        Assert.Contains("Developer", result.PersonaCodes);
        Assert.Contains("WORK_LOG", result.FeatureCodes);
        Assert.Contains("DEV_CODE_REPO", result.FeatureCodes);
    }

    [Fact]
    public async Task GetAsync_ShouldNotGrantPersonaFeature_WhenPlanDoesNotIncludeIt()
    {
        var user = new AppUser
        {
            Username = "developer-member",
            RealName = "Developer Member",
            PasswordHash = "test",
            Roles = "member"
        };
        var free = new Plan { Code = "Free", Name = "免费版", IsActive = true };
        var workLog = new Feature { Code = "WORK_LOG", Name = "工作日志", Category = "Work" };
        var devRepo = new Feature { Code = "DEV_CODE_REPO", Name = "代码仓库", Category = "Persona" };
        var developer = new PersonaType { Code = "Developer", Name = "开发者", IsActive = true };

        _context.Users.Add(user);
        _context.Plans.Add(free);
        _context.Features.AddRange(workLog, devRepo);
        _context.PlanFeatures.Add(new PlanFeature { Plan = free, Feature = workLog });
        _context.PersonaTypes.Add(developer);
        _context.UserPersonas.Add(new UserPersona { User = user, PersonaType = developer, IsPrimary = true });
        _context.PersonaFeatures.Add(new PersonaFeature { PersonaCode = developer.Code, Feature = devRepo });
        await _context.SaveChangesAsync();

        var result = await _service.GetAsync(user.Id);

        Assert.NotNull(result);
        Assert.Contains("Developer", result.PersonaCodes);
        Assert.Contains("WORK_LOG", result.FeatureCodes);
        Assert.DoesNotContain("DEV_CODE_REPO", result.FeatureCodes);
    }

    [Fact]
    public async Task GetAsync_ShouldGrantPersonaFeature_WhenPlanAndPersonaBothAllowIt()
    {
        var user = new AppUser
        {
            Username = "student-member",
            RealName = "Student Member",
            PasswordHash = "test",
            Roles = "member"
        };
        var free = new Plan { Code = "Free", Name = "免费版", IsActive = true };
        var studentExam = new Feature { Code = "STUDENT_EXAM", Name = "考研备考", Category = "Persona" };
        var student = new PersonaType { Code = "Student", Name = "学生", IsActive = true };

        _context.Users.Add(user);
        _context.Plans.Add(free);
        _context.Features.Add(studentExam);
        _context.PlanFeatures.Add(new PlanFeature { Plan = free, Feature = studentExam });
        _context.PersonaTypes.Add(student);
        _context.UserPersonas.Add(new UserPersona { User = user, PersonaType = student, IsPrimary = true });
        _context.PersonaFeatures.Add(new PersonaFeature { PersonaCode = student.Code, Feature = studentExam });
        await _context.SaveChangesAsync();

        var result = await _service.GetAsync(user.Id);

        Assert.NotNull(result);
        Assert.Contains("Student", result.PersonaCodes);
        Assert.Contains("STUDENT_EXAM", result.FeatureCodes);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnDefaultFreeFeatures_WhenFreePlanHasNoConfiguredFeatures()
    {
        var user = new AppUser
        {
            Username = "free-user",
            RealName = "Free User",
            PasswordHash = "test",
            Roles = "member"
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _service.GetAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("Free", result.PlanCode);
        Assert.Contains("GROWTH_DAILY_PLAN", result.FeatureCodes);
        Assert.Contains("GROWTH_HABIT", result.FeatureCodes);
        Assert.Contains("WORK_LOG", result.FeatureCodes);
        Assert.Contains("WORK_TASK", result.FeatureCodes);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllEnabledFeatures_ForOwner()
    {
        var user = new AppUser
        {
            Username = "super-owner",
            RealName = "Super Owner",
            PasswordHash = "test",
            Roles = "owner"
        };
        var enabledFeature = new Feature { Code = "AI_SUMMARY", Name = "AI 总结", Category = "AI", IsEnabled = true };
        var disabledFeature = new Feature { Code = "LEGACY_HIDDEN", Name = "旧功能", Category = "Legacy", IsEnabled = false };

        _context.Users.Add(user);
        _context.Features.AddRange(enabledFeature, disabledFeature);
        await _context.SaveChangesAsync();

        var result = await _service.GetAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("owner", result.RoleCode);
        Assert.Contains("AI_SUMMARY", result.FeatureCodes);
        Assert.DoesNotContain("LEGACY_HIDDEN", result.FeatureCodes);
    }

    [Fact]
    public async Task GetAsync_ShouldFallbackToProFeatures_WhenCurrentPlanHasNoFeatures()
    {
        var user = new AppUser
        {
            Username = "pro-user",
            RealName = "Pro User",
            PasswordHash = "test",
            Roles = "pro"
        };
        var enterprise = new Plan { Code = "Enterprise", Name = "企业版", IsActive = true };
        var pro = new Plan { Code = "Pro", Name = "专业版", IsActive = true };
        var proFeature = new Feature { Code = "WORK_STATISTICS", Name = "工作统计", Category = "Work", IsEnabled = true };

        _context.Users.Add(user);
        _context.Plans.AddRange(enterprise, pro);
        _context.Features.Add(proFeature);
        _context.PlanFeatures.Add(new PlanFeature { Plan = pro, Feature = proFeature });
        _context.UserSubscriptions.Add(new UserSubscription
        {
            UserId = user.Id,
            Plan = enterprise,
            StartAt = DateTime.UtcNow.AddDays(-1),
            IsActive = true
        });
        await _context.SaveChangesAsync();

        var result = await _service.GetAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal("Enterprise", result.PlanCode);
        Assert.Contains("WORK_STATISTICS", result.FeatureCodes);
    }

    [Fact]
    public async Task GetAsync_ShouldIgnoreInactivePersonas_WhenCollectingPersonaFeatures()
    {
        var user = new AppUser
        {
            Username = "designer-user",
            RealName = "Designer User",
            PasswordHash = "test",
            Roles = "member"
        };
        var inactivePersona = new PersonaType { Code = "Designer", Name = "设计师", IsActive = false };
        var personaFeature = new Feature { Code = "DESIGN_ASSETS", Name = "设计资产", Category = "Persona", IsEnabled = true };

        _context.Users.Add(user);
        _context.PersonaTypes.Add(inactivePersona);
        _context.Features.Add(personaFeature);
        _context.UserPersonas.Add(new UserPersona { User = user, PersonaType = inactivePersona, IsPrimary = true });
        _context.PersonaFeatures.Add(new PersonaFeature { PersonaCode = inactivePersona.Code, Feature = personaFeature });
        await _context.SaveChangesAsync();

        var result = await _service.GetAsync(user.Id);

        Assert.NotNull(result);
        Assert.DoesNotContain("Designer", result.PersonaCodes);
        Assert.DoesNotContain("DESIGN_ASSETS", result.FeatureCodes);
    }
}
