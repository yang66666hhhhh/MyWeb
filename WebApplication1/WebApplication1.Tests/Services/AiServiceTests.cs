using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Features.Ai.Dtos;
using WebApplication1.Features.Ai.Entities;
using WebApplication1.Features.Ai.Services;
using WebApplication1.Features.Tasks;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Tests.Services;

public class AiServiceTests : IDisposable
{
    private readonly AppDbContext _context;

    public AiServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task GenerateReportAsync_ShouldUseImplLogsAndRelatedProjectFilter()
    {
        var userId = Guid.NewGuid();
        var targetProject = new WorkProject
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProjectName = "实施项目A",
            Status = WorkProjectStatus.Active,
        };
        var otherProject = new WorkProject
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProjectName = "实施项目B",
            Status = WorkProjectStatus.Completed,
        };

        _context.WorkProjects.AddRange(targetProject, otherProject);
        _context.ImplLogs.AddRange(
            new ImplLog
            {
                UserId = userId,
                WorkDate = new DateOnly(2026, 5, 12),
                WeekDay = "Monday",
                Title = "目标项目实施巡检",
                ProjectId = targetProject.Id,
                ProjectName = targetProject.ProjectName,
                TotalHours = 5,
            },
            new ImplLog
            {
                UserId = userId,
                WorkDate = new DateOnly(2026, 5, 13),
                WeekDay = "Tuesday",
                Title = "其他项目问题排查",
                ProjectId = otherProject.Id,
                ProjectName = otherProject.ProjectName,
                TotalHours = 3,
            });
        _context.Tasks.AddRange(
            new TaskItem
            {
                UserId = userId,
                PlanDate = new DateOnly(2026, 5, 12),
                Title = "目标项目周计划",
                ProjectId = targetProject.Id,
                Status = TaskItemStatus.Completed,
            },
            new TaskItem
            {
                UserId = userId,
                PlanDate = new DateOnly(2026, 5, 13),
                Title = "其他项目周计划",
                ProjectId = otherProject.Id,
                Status = TaskItemStatus.Pending,
            });
        await _context.SaveChangesAsync();

        var service = CreateService(prompt => $"AI已生成:\n{prompt}");

        var result = await service.GenerateReportAsync(new GenerateReportRequest
        {
            Type = AiReportType.Weekly.ToString(),
            StartDate = "2026-05-12",
            EndDate = "2026-05-18",
            RelatedProjectId = targetProject.Id,
            IncludeStatistics = true,
        }, userId);

        Assert.NotNull(result);
        Assert.Contains("实施日志总数: 1", result.Content);
        Assert.Contains("目标项目实施巡检", result.Content);
        Assert.DoesNotContain("其他项目问题排查", result.Content);
        Assert.Contains("计划完成情况: 1/1 (100%)", result.Content);

        var persisted = await _context.AiReports.SingleAsync();
        Assert.Equal(result.Id, persisted.Id);
        Assert.Contains("目标项目实施巡检", persisted.Content);
        Assert.DoesNotContain("其他项目问题排查", persisted.Content);
        Assert.NotNull(result.RelatedProjectId);
        Assert.Equal(targetProject.Id, result.RelatedProjectId);
        Assert.Equal("实施项目A", result.RelatedProjectName);
    }

    [Fact]
    public async Task ChatAsync_ShouldIncludeRecentImplLogsInSystemContext()
    {
        var userId = Guid.NewGuid();
        var sessionlessRequest = new ChatRequest
        {
            Message = "帮我总结一下最近实施进展",
        };
        var project = new WorkProject
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProjectName = "实施项目上下文",
        };

        _context.WorkProjects.Add(project);
        _context.ImplLogs.Add(new ImplLog
        {
            UserId = userId,
            WorkDate = DateOnly.FromDateTime(DateTime.Today),
            WeekDay = DateTime.Today.DayOfWeek.ToString(),
            Title = "现场联调完成",
            ProjectId = project.Id,
            ProjectName = project.ProjectName,
            TotalHours = 4,
        });
        await _context.SaveChangesAsync();

        var service = CreateService(
            promptHandler: _ => throw new InvalidOperationException("本测试不应调用文本补全接口"),
            chatHandler: messages =>
            {
                var systemMessage = messages.FirstOrDefault(x => x.Role == "system");
                Assert.NotNull(systemMessage);
                Assert.Contains("最近实施日志", systemMessage!.Content);
                Assert.Contains("现场联调完成", systemMessage.Content);
                Assert.Contains("实施项目上下文", systemMessage.Content);
                return "已收到实施上下文";
            });

        var result = await service.ChatAsync(sessionlessRequest, userId);

        Assert.True(result.Success);
        Assert.Equal("已收到实施上下文", result.Content);
        Assert.Equal(2, await _context.AiChatMessages.CountAsync());
    }

    [Fact]
    public async Task GetReportsAsync_ShouldFilterByRelatedProjectId()
    {
        var userId = Guid.NewGuid();
        var projectA = Guid.NewGuid();
        var projectB = Guid.NewGuid();

        _context.AiReports.AddRange(
            new AiReport
            {
                UserId = userId,
                Title = "项目A周报",
                Type = AiReportType.Weekly,
                Remark = "{\"RelatedProjectId\":\"" + projectA + "\",\"RelatedProjectName\":\"项目A\"}",
                StartDate = new DateOnly(2026, 5, 12),
                EndDate = new DateOnly(2026, 5, 18),
            },
            new AiReport
            {
                UserId = userId,
                Title = "项目B周报",
                Type = AiReportType.Weekly,
                Remark = "{\"RelatedProjectId\":\"" + projectB + "\",\"RelatedProjectName\":\"项目B\"}",
                StartDate = new DateOnly(2026, 5, 12),
                EndDate = new DateOnly(2026, 5, 18),
            });
        await _context.SaveChangesAsync();

        var service = CreateService();

        var result = await service.GetReportsAsync(new AiQueryDto
        {
            Page = 1,
            PageSize = 10,
            RelatedProjectId = projectA,
            Type = "Weekly",
        }, userId);

        var report = Assert.Single(result.Items);
        Assert.Equal("项目A周报", report.Title);
        Assert.Equal(projectA, report.RelatedProjectId);
        Assert.Equal("项目A", report.RelatedProjectName);
    }

    private TestAiService CreateService(
        Func<string, string>? promptHandler = null,
        Func<List<ChatMessage>, string>? chatHandler = null)
    {
        var httpClientFactory = new Mock<IHttpClientFactory>();
        httpClientFactory
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(new StubHttpMessageHandler()));

        var logger = new Mock<ILogger<AiService>>();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenAI:ApiKey"] = "test-key",
                ["OpenAI:Model"] = "test-model",
            })
            .Build();

        return new TestAiService(
            _context,
            httpClientFactory.Object,
            logger.Object,
            configuration,
            promptHandler ?? (_ => "stub-report"),
            chatHandler ?? (_ => "stub-chat"));
    }

    private sealed class TestAiService(
        AppDbContext context,
        IHttpClientFactory httpClientFactory,
        ILogger<AiService> logger,
        IConfiguration configuration,
        Func<string, string> promptHandler,
        Func<List<ChatMessage>, string> chatHandler)
        : AiService(context, httpClientFactory, logger, configuration)
    {
        protected override Task<string> CallOpenAiAsync(string prompt, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptHandler(prompt));
        }

        protected override Task<string> CallOpenAiChatAsync(List<ChatMessage> messages, CancellationToken cancellationToken)
        {
            return Task.FromResult(chatHandler(messages));
        }
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"choices\":[{\"message\":{\"content\":\"stub\"}}]}", Encoding.UTF8, "application/json"),
            });
        }
    }
}
