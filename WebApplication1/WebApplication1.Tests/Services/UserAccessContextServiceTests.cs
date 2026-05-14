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
    public async Task GetAsync_ShouldMergePlanAndPersonaFeatures()
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
}
