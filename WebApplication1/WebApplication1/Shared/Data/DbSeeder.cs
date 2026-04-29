using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Features.Work.Entities;

namespace WebApplication1.Shared.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.WorkProjects.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(DateTime.Now);

        var projects = new List<WorkProject>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProjectName = "生产线升级项目",
                ProjectCode = "PRJ-001",
                ProjectType = WorkProjectType.Internal,
                CustomerName = "内部",
                Description = "生产线自动化升级改造",
                StartDate = today.AddDays(-30),
                Status = WorkProjectStatus.Active,
                Sort = 1,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectName = "质量改进项目",
                ProjectCode = "PRJ-002",
                ProjectType = WorkProjectType.Internal,
                CustomerName = "内部",
                Description = "提升产品质量和良品率",
                StartDate = today.AddDays(-20),
                Status = WorkProjectStatus.Active,
                Sort = 2,
                CreatedAt = now
            }
        };
        await context.WorkProjects.AddRangeAsync(projects);

        var taskTypes = new List<WorkTaskType>
        {
            new()
            {
                Id = Guid.NewGuid(),
                TypeName = "调试",
                TypeCode = "TT-001",
                Description = "设备调试工作",
                Sort = 1,
                Enabled = true,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                TypeName = "问题处理",
                TypeCode = "TT-002",
                Description = "问题处理和解决",
                Sort = 2,
                Enabled = true,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                TypeName = "维护",
                TypeCode = "TT-003",
                Description = "日常维护保养",
                Sort = 3,
                Enabled = true,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                TypeName = "生产",
                TypeCode = "TT-004",
                Description = "生产作业工作",
                Sort = 4,
                Enabled = true,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                TypeName = "检测",
                TypeCode = "TT-005",
                Description = "质量检测工作",
                Sort = 5,
                Enabled = true,
                CreatedAt = now
            }
        };
        await context.WorkTaskTypes.AddRangeAsync(taskTypes);

        var devices = new List<WorkDevice>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProjectId = projects[0].Id,
                DeviceName = "A线体",
                DeviceCode = "DEVICE-A",
                DeviceType = WorkDeviceType.ProductionLine,
                Description = "主生产线A",
                Status = WorkDeviceStatus.Active,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectId = projects[0].Id,
                DeviceName = "B线体",
                DeviceCode = "DEVICE-B",
                DeviceType = WorkDeviceType.ProductionLine,
                Description = "主生产线B",
                Status = WorkDeviceStatus.Active,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectId = projects[0].Id,
                DeviceName = "C线体",
                DeviceCode = "DEVICE-C",
                DeviceType = WorkDeviceType.ProductionLine,
                Description = "主生产线C",
                Status = WorkDeviceStatus.Active,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectId = projects[1].Id,
                DeviceName = "测试设备1",
                DeviceCode = "TEST-001",
                DeviceType = WorkDeviceType.TestingDevice,
                Description = "测试设备1",
                Status = WorkDeviceStatus.Active,
                CreatedAt = now
            }
        };
        await context.WorkDevices.AddRangeAsync(devices);

        var weekDay = today.DayOfWeek switch
        {
            DayOfWeek.Sunday => "周日",
            DayOfWeek.Monday => "周一",
            DayOfWeek.Tuesday => "周二",
            DayOfWeek.Wednesday => "周三",
            DayOfWeek.Thursday => "周四",
            DayOfWeek.Friday => "周五",
            DayOfWeek.Saturday => "周六",
            _ => "周一"
        };

        var workLogs = new List<WorkLog>
        {
            new()
            {
                Id = Guid.NewGuid(),
                WorkDate = today,
                WeekDay = weekDay,
                ProjectId = projects[0].Id,
                Title = "A线体设备调试",
                OriginalContent = "完成设备调试和参数优化",
                Summary = "今日主要完成A线体调试，产出达标",
                TotalHours = 4,
                Status = WorkLogStatus.Normal,
                SourceType = WorkLogSourceType.Manual,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                WorkDate = today,
                WeekDay = weekDay,
                ProjectId = projects[0].Id,
                Title = "B线体数据记录",
                OriginalContent = "",
                Summary = "",
                TotalHours = 0,
                Status = WorkLogStatus.PendingSupplement,
                SourceType = WorkLogSourceType.Manual,
                Remark = "缺失数据",
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                WorkDate = today,
                WeekDay = weekDay,
                ProjectId = projects[1].Id,
                Title = "质量问题处理",
                OriginalContent = "生产异常问题处理",
                Summary = "解决了2个质量问题",
                TotalHours = 3,
                Status = WorkLogStatus.Normal,
                SourceType = WorkLogSourceType.Manual,
                CreatedAt = now
            }
        };
        await context.WorkLogs.AddRangeAsync(workLogs);

        var dailyPlans = new List<WorkDailyPlan>
        {
            new()
            {
                Id = Guid.NewGuid(),
                PlanDate = today,
                Title = "A线体设备调试",
                Content = "完成A线体设备调试任务",
                ProjectId = projects[0].Id,
                Priority = WorkDailyPlanPriority.High,
                Status = WorkDailyPlanStatus.Pending,
                StartTime = "08:00",
                EndTime = "12:00",
                EstimatedHours = 4,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                PlanDate = today,
                Title = "B线体问题处理",
                Content = "处理B线体异常问题",
                ProjectId = projects[0].Id,
                Priority = WorkDailyPlanPriority.Urgent,
                Status = WorkDailyPlanStatus.InProgress,
                StartTime = "09:00",
                EndTime = "17:00",
                EstimatedHours = 8,
                Remark = "紧急",
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                PlanDate = today,
                Title = "设备日常巡检",
                Content = "日常巡检设备",
                ProjectId = projects[1].Id,
                Priority = WorkDailyPlanPriority.Low,
                Status = WorkDailyPlanStatus.Pending,
                StartTime = "17:00",
                EndTime = "18:00",
                EstimatedHours = 1,
                CreatedAt = now
            }
        };
        await context.WorkDailyPlans.AddRangeAsync(dailyPlans);

        await context.SaveChangesAsync();
    }
}