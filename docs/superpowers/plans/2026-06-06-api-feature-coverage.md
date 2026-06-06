# API Feature Coverage Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ensure protected business API actions in the targeted controllers have explicit `RequireFeature` coverage aligned with seeded FeatureCodes.

**Architecture:** Keep the existing authorization path: `RequireFeatureAttribute` resolves `IUserAccessContextService` and checks `UserAccessContext.FeatureCodes`. Add reflection tests to detect missing feature annotations on targeted business controllers, then add the minimum method-level attributes and seed definitions required by those attributes.

**Tech Stack:** ASP.NET Core 10, C# 12, xUnit, EF Core InMemory, PowerShell commands through `rtk`.

---

## File Structure

- Modify `WebApplication1/WebApplication1/Features/Work/Controllers/WorkExtendedController.cs`
  - Add `using WebApplication1.Features.Auth.Authorization;`
  - Add method-level `[RequireFeature]` attributes for OKR, risk, and files actions.
- Modify `WebApplication1/WebApplication1/Features/Ai/Controllers/AiExtendedController.cs`
  - Add `using WebApplication1.Features.Auth.Authorization;`
  - Add method-level `[RequireFeature]` attributes for automation, knowledge chat, and insights actions.
- Modify `WebApplication1/WebApplication1/Features/Analytics/Controllers/AnalyticsExtendedController.cs`
  - Add `using WebApplication1.Features.Auth.Authorization;`
  - Add method-level `[RequireFeature]` attributes for time, habits, finance, custom reports, and AI insights actions.
- Modify `WebApplication1/WebApplication1/Features/Persona/Controllers/PersonaController.cs`
  - Replace existing dev `WORK_PROJECT`/`WORK_TASK` feature attributes with `DEV_*`.
  - Add method-level `[RequireFeature]` attributes for design and teacher actions.
- Modify `WebApplication1/WebApplication1/Shared/Data/DbSeeder.cs`
  - Add standard feature definitions for `AI_AUTOMATION`, `AI_KNOWLEDGE_CHAT`, `ANALYTICS_TIME`, `ANALYTICS_HABITS`, `ANALYTICS_CUSTOM`, `ANALYTICS_AI`.
  - Ensure Pro and Team plans receive all enabled features after missing definitions are added.
  - Ensure Developer, Designer, Teacher, Implementation, and Student persona feature mappings are repaired for existing databases.
- Modify `WebApplication1/WebApplication1.Tests/Services/DbSeederTests.cs`
  - Add tests proving the new FeatureCodes are seeded, Pro gets them, Free does not, persona mappings are repaired, and `UserAccessContextService` exposes the seeded paid features to Pro users.
- Create `WebApplication1/WebApplication1.Tests/Services/ApiFeatureCoverageTests.cs`
  - Add reflection tests for the targeted controllers and expected method-level FeatureCode mapping.

## Task 1: Reflection Coverage Tests

**Files:**
- Create: `WebApplication1/WebApplication1.Tests/Services/ApiFeatureCoverageTests.cs`

- [ ] **Step 1: Write the failing reflection tests**

Create `WebApplication1/WebApplication1.Tests/Services/ApiFeatureCoverageTests.cs`:

```csharp
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Ai;
using WebApplication1.Features.Analytics;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Persona.Controllers;
using WebApplication1.Features.Work.Controllers;

namespace WebApplication1.Tests.Services;

public class ApiFeatureCoverageTests
{
    private static readonly Dictionary<Type, Dictionary<string, string>> ExpectedFeatures = new()
    {
        [typeof(WorkExtendedController)] = new()
        {
            [nameof(WorkExtendedController.GetOkrs)] = "WORK_OKR",
            [nameof(WorkExtendedController.CreateOkr)] = "WORK_OKR",
            [nameof(WorkExtendedController.UpdateOkr)] = "WORK_OKR",
            [nameof(WorkExtendedController.DeleteOkr)] = "WORK_OKR",
            [nameof(WorkExtendedController.GetRisks)] = "WORK_RISK",
            [nameof(WorkExtendedController.CreateRisk)] = "WORK_RISK",
            [nameof(WorkExtendedController.UpdateRisk)] = "WORK_RISK",
            [nameof(WorkExtendedController.DeleteRisk)] = "WORK_RISK",
            [nameof(WorkExtendedController.GetFiles)] = "WORK_TASK",
            [nameof(WorkExtendedController.CreateFile)] = "WORK_TASK",
            [nameof(WorkExtendedController.UpdateFile)] = "WORK_TASK",
            [nameof(WorkExtendedController.DeleteFile)] = "WORK_TASK",
        },
        [typeof(AiExtendedController)] = new()
        {
            [nameof(AiExtendedController.GetWorkflows)] = "AI_AUTOMATION",
            [nameof(AiExtendedController.CreateWorkflow)] = "AI_AUTOMATION",
            [nameof(AiExtendedController.UpdateWorkflow)] = "AI_AUTOMATION",
            [nameof(AiExtendedController.DeleteWorkflow)] = "AI_AUTOMATION",
            [nameof(AiExtendedController.GetKnowledgeChatSessions)] = "AI_KNOWLEDGE_CHAT",
            [nameof(AiExtendedController.SendKnowledgeChatMessage)] = "AI_KNOWLEDGE_CHAT",
            [nameof(AiExtendedController.DeleteKnowledgeChatSession)] = "AI_KNOWLEDGE_CHAT",
            [nameof(AiExtendedController.GetInsights)] = "AI_INSIGHTS",
            [nameof(AiExtendedController.GenerateInsight)] = "AI_INSIGHTS",
            [nameof(AiExtendedController.DeleteInsight)] = "AI_INSIGHTS",
        },
        [typeof(AnalyticsExtendedController)] = new()
        {
            [nameof(AnalyticsExtendedController.GetTimeOverview)] = "ANALYTICS_TIME",
            [nameof(AnalyticsExtendedController.GetHourlyDistribution)] = "ANALYTICS_TIME",
            [nameof(AnalyticsExtendedController.GetWeeklyTrend)] = "ANALYTICS_TIME",
            [nameof(AnalyticsExtendedController.GetHabitsOverview)] = "ANALYTICS_HABITS",
            [nameof(AnalyticsExtendedController.GetHabitTrends)] = "ANALYTICS_HABITS",
            [nameof(AnalyticsExtendedController.GetFinanceOverview)] = "ANALYTICS_FINANCE",
            [nameof(AnalyticsExtendedController.GetMonthlyFinanceTrend)] = "ANALYTICS_FINANCE",
            [nameof(AnalyticsExtendedController.GetExpenseBreakdown)] = "ANALYTICS_FINANCE",
            [nameof(AnalyticsExtendedController.GetCustomReports)] = "ANALYTICS_CUSTOM",
            [nameof(AnalyticsExtendedController.CreateCustomReport)] = "ANALYTICS_CUSTOM",
            [nameof(AnalyticsExtendedController.DeleteCustomReport)] = "ANALYTICS_CUSTOM",
            [nameof(AnalyticsExtendedController.GetAiInsights)] = "ANALYTICS_AI",
            [nameof(AnalyticsExtendedController.GenerateAiInsight)] = "ANALYTICS_AI",
        },
        [typeof(PersonaController)] = new()
        {
            [nameof(PersonaController.GetRepositories)] = "DEV_CODE_REPO",
            [nameof(PersonaController.GetRepositoryById)] = "DEV_CODE_REPO",
            [nameof(PersonaController.CreateRepository)] = "DEV_CODE_REPO",
            [nameof(PersonaController.UpdateRepository)] = "DEV_CODE_REPO",
            [nameof(PersonaController.DeleteRepository)] = "DEV_CODE_REPO",
            [nameof(PersonaController.GetIssues)] = "DEV_ISSUES",
            [nameof(PersonaController.GetIssueById)] = "DEV_ISSUES",
            [nameof(PersonaController.CreateIssue)] = "DEV_ISSUES",
            [nameof(PersonaController.UpdateIssue)] = "DEV_ISSUES",
            [nameof(PersonaController.DeleteIssue)] = "DEV_ISSUES",
            [nameof(PersonaController.GetPipelines)] = "DEV_PIPELINES",
            [nameof(PersonaController.GetPipelineById)] = "DEV_PIPELINES",
            [nameof(PersonaController.CreatePipeline)] = "DEV_PIPELINES",
            [nameof(PersonaController.UpdatePipeline)] = "DEV_PIPELINES",
            [nameof(PersonaController.DeletePipeline)] = "DEV_PIPELINES",
            [nameof(PersonaController.GetDesignAssets)] = "DESIGN_ASSETS",
            [nameof(PersonaController.GetDesignAssetById)] = "DESIGN_ASSETS",
            [nameof(PersonaController.CreateDesignAsset)] = "DESIGN_ASSETS",
            [nameof(PersonaController.UpdateDesignAsset)] = "DESIGN_ASSETS",
            [nameof(PersonaController.DeleteDesignAsset)] = "DESIGN_ASSETS",
            [nameof(PersonaController.GetPrototypes)] = "DESIGN_PROTOTYPE",
            [nameof(PersonaController.GetPrototypeById)] = "DESIGN_PROTOTYPE",
            [nameof(PersonaController.CreatePrototype)] = "DESIGN_PROTOTYPE",
            [nameof(PersonaController.UpdatePrototype)] = "DESIGN_PROTOTYPE",
            [nameof(PersonaController.DeletePrototype)] = "DESIGN_PROTOTYPE",
            [nameof(PersonaController.GetTeacherCourses)] = "TEACHER_COURSE",
            [nameof(PersonaController.GetTeacherCourseById)] = "TEACHER_COURSE",
            [nameof(PersonaController.CreateTeacherCourse)] = "TEACHER_COURSE",
            [nameof(PersonaController.UpdateTeacherCourse)] = "TEACHER_COURSE",
            [nameof(PersonaController.DeleteTeacherCourse)] = "TEACHER_COURSE",
            [nameof(PersonaController.GetTeacherStudents)] = "TEACHER_STUDENT",
            [nameof(PersonaController.GetTeacherStudentById)] = "TEACHER_STUDENT",
            [nameof(PersonaController.CreateTeacherStudent)] = "TEACHER_STUDENT",
            [nameof(PersonaController.UpdateTeacherStudent)] = "TEACHER_STUDENT",
            [nameof(PersonaController.DeleteTeacherStudent)] = "TEACHER_STUDENT",
        },
    };

    [Fact]
    public void TargetBusinessActions_ShouldDeclareExpectedRequireFeature()
    {
        var failures = ExpectedFeatures
            .SelectMany(controller =>
                controller.Value.Select(expected =>
                    new
                    {
                        Controller = controller.Key.Name,
                        Action = expected.Key,
                        Expected = expected.Value,
                        Actual = GetRequireFeature(controller.Key, expected.Key)?.FeatureCode
                    }))
            .Where(x => !string.Equals(x.Actual, x.Expected, StringComparison.OrdinalIgnoreCase))
            .Select(x => $"{x.Controller}.{x.Action}: expected {x.Expected}, actual {x.Actual ?? "<none>"}")
            .ToList();

        Assert.True(failures.Count == 0, "Missing or incorrect RequireFeature attributes:" + Environment.NewLine + string.Join(Environment.NewLine, failures));
    }

    [Fact]
    public void TargetBusinessActions_ShouldAllBeListedInExpectedFeatureMap()
    {
        var failures = ExpectedFeatures
            .SelectMany(controller =>
            {
                var expectedActionNames = controller.Value.Keys.ToHashSet(StringComparer.Ordinal);
                return controller.Key
                    .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    .Where(HasHttpMethodAttribute)
                    .Where(method => !expectedActionNames.Contains(method.Name))
                    .Select(method => $"{controller.Key.Name}.{method.Name}");
            })
            .ToList();

        Assert.True(failures.Count == 0, "HTTP actions missing from expected Feature map:" + Environment.NewLine + string.Join(Environment.NewLine, failures));
    }

    private static RequireFeatureAttribute? GetRequireFeature(Type controllerType, string actionName)
    {
        var method = controllerType.GetMethod(actionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
        return method?.GetCustomAttribute<RequireFeatureAttribute>()
            ?? controllerType.GetCustomAttribute<RequireFeatureAttribute>();
    }

    private static bool HasHttpMethodAttribute(MethodInfo method)
    {
        return method.GetCustomAttributes().Any(attribute => attribute is HttpMethodAttribute);
    }
}
```

- [ ] **Step 2: Run the new tests and verify they fail**

Run:

```powershell
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore --filter ApiFeatureCoverageTests
```

Expected: FAIL. The output should list missing or incorrect `RequireFeature` attributes for `WorkExtendedController`, `AiExtendedController`, `AnalyticsExtendedController`, and `PersonaController`.

- [ ] **Step 3: Commit the failing tests**

```powershell
rtk git add WebApplication1\WebApplication1.Tests\Services\ApiFeatureCoverageTests.cs
rtk git commit -m "test: cover api feature annotations"
```

## Task 2: Controller Feature Attributes

**Files:**
- Modify: `WebApplication1/WebApplication1/Features/Work/Controllers/WorkExtendedController.cs`
- Modify: `WebApplication1/WebApplication1/Features/Ai/Controllers/AiExtendedController.cs`
- Modify: `WebApplication1/WebApplication1/Features/Analytics/Controllers/AnalyticsExtendedController.cs`
- Modify: `WebApplication1/WebApplication1/Features/Persona/Controllers/PersonaController.cs`
- Test: `WebApplication1/WebApplication1.Tests/Services/ApiFeatureCoverageTests.cs`

- [ ] **Step 1: Add the missing authorization namespace imports**

In each of these files, add the import if it is missing:

```csharp
using WebApplication1.Features.Auth.Authorization;
```

Files:

- `WebApplication1/WebApplication1/Features/Work/Controllers/WorkExtendedController.cs`
- `WebApplication1/WebApplication1/Features/Ai/Controllers/AiExtendedController.cs`
- `WebApplication1/WebApplication1/Features/Analytics/Controllers/AnalyticsExtendedController.cs`

`PersonaController.cs` already has this import.

- [ ] **Step 2: Add Work extended Feature attributes**

In `WebApplication1/WebApplication1/Features/Work/Controllers/WorkExtendedController.cs`, add these attributes immediately below each `[Http...]` attribute:

```csharp
[RequireFeature("WORK_OKR")]
```

Apply to:

- `GetOkrs`
- `CreateOkr`
- `UpdateOkr`
- `DeleteOkr`

```csharp
[RequireFeature("WORK_RISK")]
```

Apply to:

- `GetRisks`
- `CreateRisk`
- `UpdateRisk`
- `DeleteRisk`

```csharp
[RequireFeature("WORK_TASK")]
```

Apply to:

- `GetFiles`
- `CreateFile`
- `UpdateFile`
- `DeleteFile`

- [ ] **Step 3: Add AI extended Feature attributes**

In `WebApplication1/WebApplication1/Features/Ai/Controllers/AiExtendedController.cs`, add:

```csharp
[RequireFeature("AI_AUTOMATION")]
```

Apply to:

- `GetWorkflows`
- `CreateWorkflow`
- `UpdateWorkflow`
- `DeleteWorkflow`

```csharp
[RequireFeature("AI_KNOWLEDGE_CHAT")]
```

Apply to:

- `GetKnowledgeChatSessions`
- `SendKnowledgeChatMessage`
- `DeleteKnowledgeChatSession`

```csharp
[RequireFeature("AI_INSIGHTS")]
```

Apply to:

- `GetInsights`
- `GenerateInsight`
- `DeleteInsight`

- [ ] **Step 4: Add Analytics extended Feature attributes**

In `WebApplication1/WebApplication1/Features/Analytics/Controllers/AnalyticsExtendedController.cs`, add:

```csharp
[RequireFeature("ANALYTICS_TIME")]
```

Apply to:

- `GetTimeOverview`
- `GetHourlyDistribution`
- `GetWeeklyTrend`

```csharp
[RequireFeature("ANALYTICS_HABITS")]
```

Apply to:

- `GetHabitsOverview`
- `GetHabitTrends`

```csharp
[RequireFeature("ANALYTICS_FINANCE")]
```

Apply to:

- `GetFinanceOverview`
- `GetMonthlyFinanceTrend`
- `GetExpenseBreakdown`

```csharp
[RequireFeature("ANALYTICS_CUSTOM")]
```

Apply to:

- `GetCustomReports`
- `CreateCustomReport`
- `DeleteCustomReport`

```csharp
[RequireFeature("ANALYTICS_AI")]
```

Apply to:

- `GetAiInsights`
- `GenerateAiInsight`

- [ ] **Step 5: Replace Persona Feature attributes**

In `WebApplication1/WebApplication1/Features/Persona/Controllers/PersonaController.cs`, replace existing dev attributes and add missing design/teacher attributes:

```csharp
[RequireFeature("DEV_CODE_REPO")]
```

Apply to:

- `GetRepositories`
- `GetRepositoryById`
- `CreateRepository`
- `UpdateRepository`
- `DeleteRepository`

```csharp
[RequireFeature("DEV_ISSUES")]
```

Apply to:

- `GetIssues`
- `GetIssueById`
- `CreateIssue`
- `UpdateIssue`
- `DeleteIssue`

```csharp
[RequireFeature("DEV_PIPELINES")]
```

Apply to:

- `GetPipelines`
- `GetPipelineById`
- `CreatePipeline`
- `UpdatePipeline`
- `DeletePipeline`

```csharp
[RequireFeature("DESIGN_ASSETS")]
```

Apply to:

- `GetDesignAssets`
- `GetDesignAssetById`
- `CreateDesignAsset`
- `UpdateDesignAsset`
- `DeleteDesignAsset`

```csharp
[RequireFeature("DESIGN_PROTOTYPE")]
```

Apply to:

- `GetPrototypes`
- `GetPrototypeById`
- `CreatePrototype`
- `UpdatePrototype`
- `DeletePrototype`

```csharp
[RequireFeature("TEACHER_COURSE")]
```

Apply to:

- `GetTeacherCourses`
- `GetTeacherCourseById`
- `CreateTeacherCourse`
- `UpdateTeacherCourse`
- `DeleteTeacherCourse`

```csharp
[RequireFeature("TEACHER_STUDENT")]
```

Apply to:

- `GetTeacherStudents`
- `GetTeacherStudentById`
- `CreateTeacherStudent`
- `UpdateTeacherStudent`
- `DeleteTeacherStudent`

- [ ] **Step 6: Run reflection tests and verify they pass**

Run:

```powershell
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore --filter ApiFeatureCoverageTests
```

Expected: PASS.

- [ ] **Step 7: Commit Controller authorization changes**

```powershell
rtk git add WebApplication1\WebApplication1\Features\Work\Controllers\WorkExtendedController.cs WebApplication1\WebApplication1\Features\Ai\Controllers\AiExtendedController.cs WebApplication1\WebApplication1\Features\Analytics\Controllers\AnalyticsExtendedController.cs WebApplication1\WebApplication1\Features\Persona\Controllers\PersonaController.cs
rtk git commit -m "fix: align business api feature attributes"
```

## Task 3: Seeder Feature Definitions and Plan Grants

**Files:**
- Modify: `WebApplication1/WebApplication1.Tests/Services/DbSeederTests.cs`
- Modify: `WebApplication1/WebApplication1/Shared/Data/DbSeeder.cs`

- [ ] **Step 1: Add failing Seeder tests**

Add these imports to `WebApplication1/WebApplication1.Tests/Services/DbSeederTests.cs`:

```csharp
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Features.Auth.Entities.Subscription;
```

Append these tests to `WebApplication1/WebApplication1.Tests/Services/DbSeederTests.cs` before `AssertMenuWithChildren`:

```csharp
    [Fact]
    public async Task SeedAsync_ShouldCreateApiFeatureCoverageCodes()
    {
        await DbSeeder.SeedAsync(_context);

        var featureCodes = await _context.Features
            .AsNoTracking()
            .Select(x => x.Code)
            .ToListAsync();

        var requiredCodes = new[]
        {
            "WORK_OKR",
            "WORK_RISK",
            "AI_AUTOMATION",
            "AI_KNOWLEDGE_CHAT",
            "AI_INSIGHTS",
            "ANALYTICS_TIME",
            "ANALYTICS_HABITS",
            "ANALYTICS_FINANCE",
            "ANALYTICS_CUSTOM",
            "ANALYTICS_AI",
            "DEV_CODE_REPO",
            "DEV_ISSUES",
            "DEV_PIPELINES",
            "DESIGN_ASSETS",
            "DESIGN_PROTOTYPE",
            "TEACHER_COURSE",
            "TEACHER_STUDENT",
        };

        foreach (var code in requiredCodes)
        {
            Assert.Contains(code, featureCodes);
        }
    }

    [Fact]
    public async Task SeedAsync_ShouldGrantNewNonFreeFeaturesToProAndTeamOnly()
    {
        await DbSeeder.SeedAsync(_context);

        var planFeatures = await _context.PlanFeatures
            .AsNoTracking()
            .Select(x => new { x.Plan.Code, FeatureCode = x.Feature.Code })
            .ToListAsync();

        var newNonFreeCodes = new[]
        {
            "AI_AUTOMATION",
            "AI_KNOWLEDGE_CHAT",
            "ANALYTICS_TIME",
            "ANALYTICS_HABITS",
            "ANALYTICS_CUSTOM",
            "ANALYTICS_AI",
            "WORK_OKR",
            "WORK_RISK",
        };

        foreach (var code in newNonFreeCodes)
        {
            Assert.DoesNotContain(planFeatures, x => x.Code == "Free" && x.FeatureCode == code);
            Assert.Contains(planFeatures, x => x.Code == "Pro" && x.FeatureCode == code);
            Assert.Contains(planFeatures, x => x.Code == "Team" && x.FeatureCode == code);
        }
    }

    [Fact]
    public async Task SeedAsync_ShouldRepairPersonaFeatureMappings()
    {
        await DbSeeder.SeedAsync(_context);

        var personaFeatures = await _context.PersonaFeatures
            .AsNoTracking()
            .Select(x => new { x.PersonaCode, FeatureCode = x.Feature.Code })
            .ToListAsync();

        Assert.Contains(personaFeatures, x => x.PersonaCode == "Developer" && x.FeatureCode == "DEV_CODE_REPO");
        Assert.Contains(personaFeatures, x => x.PersonaCode == "Developer" && x.FeatureCode == "DEV_ISSUES");
        Assert.Contains(personaFeatures, x => x.PersonaCode == "Developer" && x.FeatureCode == "DEV_PIPELINES");
        Assert.Contains(personaFeatures, x => x.PersonaCode == "Designer" && x.FeatureCode == "DESIGN_ASSETS");
        Assert.Contains(personaFeatures, x => x.PersonaCode == "Designer" && x.FeatureCode == "DESIGN_PROTOTYPE");
        Assert.Contains(personaFeatures, x => x.PersonaCode == "Teacher" && x.FeatureCode == "TEACHER_COURSE");
        Assert.Contains(personaFeatures, x => x.PersonaCode == "Teacher" && x.FeatureCode == "TEACHER_STUDENT");
    }

    [Fact]
    public async Task SeedAsync_ShouldExposeSeededPaidFeaturesThroughAccessContext()
    {
        await DbSeeder.SeedAsync(_context);

        var proPlanId = await _context.Plans
            .Where(x => x.Code == "Pro")
            .Select(x => x.Id)
            .SingleAsync();

        var user = new AppUser
        {
            Username = "analytics-pro",
            RealName = "Analytics Pro",
            PasswordHash = "test",
            Roles = "pro"
        };
        _context.Users.Add(user);
        _context.UserSubscriptions.Add(new UserSubscription
        {
            UserId = user.Id,
            PlanId = proPlanId,
            StartAt = DateTime.UtcNow.AddDays(-1),
            IsActive = true
        });
        await _context.SaveChangesAsync();

        var service = new UserAccessContextService(_context);
        var access = await service.GetAsync(user.Id);

        Assert.NotNull(access);
        Assert.Contains("AI_AUTOMATION", access.FeatureCodes);
        Assert.Contains("AI_KNOWLEDGE_CHAT", access.FeatureCodes);
        Assert.Contains("ANALYTICS_TIME", access.FeatureCodes);
        Assert.Contains("ANALYTICS_AI", access.FeatureCodes);
        Assert.DoesNotContain("DEV_ISSUES", access.FeatureCodes);
    }
```

- [ ] **Step 2: Run Seeder tests and verify they fail**

Run:

```powershell
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore --filter DbSeederTests
```

Expected: FAIL. At minimum, `AI_AUTOMATION`, `AI_KNOWLEDGE_CHAT`, `ANALYTICS_TIME`, `ANALYTICS_HABITS`, `ANALYTICS_CUSTOM`, and `ANALYTICS_AI` should be missing.

- [ ] **Step 3: Add standard feature definitions**

In `WebApplication1/WebApplication1/Shared/Data/DbSeeder.cs`, add a new array after `DefaultFreeFeatureCodes`:

```csharp
    private static readonly (string Code, string Name, string Category, string Description)[] StandardFeatureDefinitions =
    [
        ("WORK_LOG", "工作日志", "Work", "工作日志管理"),
        ("WORK_PROJECT", "工作项目", "Work", "工作项目管理"),
        ("WORK_DEVICE", "设备管理", "Work", "设备管理"),
        ("WORK_TASK", "工作任务", "Work", "工作任务管理"),
        ("WORK_IMPORT", "数据导入", "Work", "Excel 数据导入"),
        ("WORK_STATISTICS", "工作统计", "Work", "工作日志与任务统计看板"),
        ("WORK_TEMPLATE", "模板管理", "Work", "行业模板管理"),
        ("WORK_OKR", "OKR管理", "Work", "目标与关键结果管理"),
        ("WORK_GANTT", "甘特图", "Work", "项目甘特图"),
        ("WORK_RISK", "风险管理", "Work", "项目风险管理"),
        ("TASK_UNIFIED", "统一任务", "Growth", "统一任务系统"),
        ("GROWTH_DAILY_PLAN", "每日计划", "Growth", "每日计划管理"),
        ("GROWTH_HABIT", "习惯打卡", "Growth", "习惯养成和打卡"),
        ("GROWTH_KNOWLEDGE", "知识库", "Growth", "知识文章管理"),
        ("GROWTH_SKILL", "技能管理", "Growth", "技能追踪和提升"),
        ("GROWTH_FITNESS", "健身管理", "Growth", "健身记录和分析"),
        ("AI_ASSISTANT", "AI助手", "AI", "AI对话助手"),
        ("AI_PLANNER", "AI规划器", "AI", "AI智能规划"),
        ("AI_REPORT", "AI报告", "AI", "AI生成报告"),
        ("AI_INSIGHTS", "数据洞察", "AI", "AI数据分析洞察"),
        ("AI_AUTOMATION", "AI自动化", "AI", "AI工作流自动化"),
        ("AI_KNOWLEDGE_CHAT", "知识库问答", "AI", "基于知识库的 AI 问答"),
        ("ASSET_DASHBOARD", "资产看板", "Assets", "财务资产总览"),
        ("ASSET_INCOME", "收入管理", "Assets", "收入记录和分析"),
        ("ASSET_EXPENSE", "支出管理", "Assets", "支出记录和分析"),
        ("ASSET_BUDGET", "预算管理", "Assets", "预算设定和追踪"),
        ("ASSET_INVEST", "投资管理", "Assets", "投资组合管理"),
        ("ANALYTICS_GROWTH", "成长分析", "Analytics", "个人成长数据分析"),
        ("ANALYTICS_WORK", "工作分析", "Analytics", "工作效率分析"),
        ("ANALYTICS_FINANCE", "财务分析", "Analytics", "财务数据分析"),
        ("ANALYTICS_TIME", "时间分析", "Analytics", "时间使用数据分析"),
        ("ANALYTICS_HABITS", "习惯分析", "Analytics", "习惯打卡数据分析"),
        ("ANALYTICS_CUSTOM", "自定义报表", "Analytics", "自定义分析报表"),
        ("ANALYTICS_AI", "AI洞察分析", "Analytics", "AI 洞察分析"),
        ("DEV_CODE_REPO", "代码仓库", "Persona", "代码仓库管理"),
        ("DEV_ISSUES", "问题跟踪", "Persona", "Bug和任务跟踪"),
        ("DEV_PIPELINES", "流水线", "Persona", "CI/CD流水线"),
        ("DESIGN_ASSETS", "设计资产", "Persona", "设计资源管理"),
        ("DESIGN_PROTOTYPE", "原型管理", "Persona", "产品原型设计"),
        ("TEACHER_COURSE", "课程管理", "Persona", "在线课程管理"),
        ("TEACHER_STUDENT", "学生管理", "Persona", "学生信息管理"),
        ("IMPL_KANBAN", "项目看板", "Persona", "实施项目看板"),
        ("IMPL_CUSTOMER", "客户管理", "Persona", "客户信息管理"),
        ("STUDENT_LEARNING", "学习计划", "Persona", "学习计划制定"),
        ("STUDENT_MISTAKES", "错题本", "Persona", "错题记录和复习"),
        ("STUDENT_REVIEW", "复习日程", "Persona", "到期复习安排"),
        ("STUDENT_MATERIALS", "学习资料", "Persona", "学习资料管理"),
        ("STUDENT_RECORDS", "学习记录", "Persona", "学习过程记录"),
        ("STUDENT_SUBJECTS", "科目目标", "Persona", "科目目标配置"),
        ("STUDENT_EXAM", "考研备考", "Persona", "考研备考管理"),
    ];
```

- [ ] **Step 4: Reuse standard definitions in the initial seed list**

Replace the hard-coded `var features = new List<Feature> { ... };` feature block with:

```csharp
            var features = StandardFeatureDefinitions
                .Select(x => new Feature
                {
                    Code = x.Code,
                    Name = x.Name,
                    Category = x.Category,
                    Description = x.Description
                })
                .ToList();
```

Keep the existing `context.Features.AddRange(features);` and save logic.

- [ ] **Step 5: Ensure all standard definitions and paid plan grants are repaired**

Replace:

```csharp
        await EnsureFeatureDefinitionsAsync(context, DefaultFreeFeatureDefinitions, logger);
        await EnsurePlanFeaturesAsync(context, "Free", DefaultFreeFeatureCodes, logger);
        await EnsurePersonaFeaturesAsync(context, "Student", "STUDENT_", logger);
```

With:

```csharp
        await EnsureFeatureDefinitionsAsync(context, StandardFeatureDefinitions, logger);
        await EnsurePlanFeaturesAsync(context, "Free", DefaultFreeFeatureCodes, logger);
        await EnsureAllEnabledPlanFeaturesAsync(context, "Pro", logger);
        await EnsureAllEnabledPlanFeaturesAsync(context, "Team", logger);
        await EnsurePersonaFeaturesAsync(context, "Developer", "DEV_", logger);
        await EnsurePersonaFeaturesAsync(context, "Designer", "DESIGN_", logger);
        await EnsurePersonaFeaturesAsync(context, "Teacher", "TEACHER_", logger);
        await EnsurePersonaFeaturesAsync(context, "Implementation", "IMPL_", logger);
        await EnsurePersonaFeaturesAsync(context, "Student", "STUDENT_", logger);
```

Then remove `DefaultFreeFeatureDefinitions`.

- [ ] **Step 6: Add helper for paid plan repair**

Add this helper after `EnsurePlanFeaturesAsync`:

```csharp
    private static async Task EnsureAllEnabledPlanFeaturesAsync(
        AppDbContext context,
        string planCode,
        ILogger? logger)
    {
        var enabledFeatureCodes = await context.Features
            .Where(x => x.IsEnabled)
            .Select(x => x.Code)
            .ToListAsync();

        await EnsurePlanFeaturesAsync(context, planCode, enabledFeatureCodes, logger);
    }
```

- [ ] **Step 7: Run Seeder tests and verify they pass**

Run:

```powershell
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore --filter DbSeederTests
```

Expected: PASS.

- [ ] **Step 8: Commit Seeder and tests**

```powershell
rtk git add WebApplication1\WebApplication1.Tests\Services\DbSeederTests.cs WebApplication1\WebApplication1\Shared\Data\DbSeeder.cs
rtk git commit -m "fix: seed api feature coverage codes"
```

## Task 4: Full Verification

**Files:**
- No new source changes expected.

- [ ] **Step 1: Run all backend tests**

Run:

```powershell
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore
```

Expected: PASS with all tests passing.

- [ ] **Step 2: Check working tree**

Run:

```powershell
rtk git status --short
```

Expected: no uncommitted source changes. If test artifacts appear, remove only generated artifacts and keep user changes.

- [ ] **Step 3: Review final diff against the design**

Run:

```powershell
rtk git log --oneline -5
```

Expected: includes commits for the reflection tests, Controller feature alignment, Seeder feature codes, and access-context tests.

```powershell
rtk git diff HEAD~3..HEAD --stat
```

Expected: only the planned Controller, Seeder, and test files changed.
