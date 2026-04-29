using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Features.Work.Entities;

namespace WebApplication1.Shared.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        Console.WriteLine("[DbSeeder] Starting seed...");

        var hasData = await context.WorkProjects.AnyAsync();
        Console.WriteLine($"[DbSeeder] WorkProjects has data: {hasData}");

        if (hasData)
        {
            Console.WriteLine("[DbSeeder] Data already exists, skipping seed.");
            return;
        }

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(DateTime.Now);
        Console.WriteLine($"[DbSeeder] Seeding WorkProjects... Today: {today}");

        var projects = new List<WorkProject>
        {
            new()
            {
                Id = Guid.NewGuid(),
                ProjectName = "生产线升级项目",
                ProjectCode = "PRJ-001",
                ProjectType = WorkProjectType.Internal,
                CustomerName = "内部",
                Description = "生产线自动化升级改造，提升生产效率",
                StartDate = today.AddDays(-60),
                EndDate = today.AddDays(30),
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
                StartDate = today.AddDays(-30),
                EndDate = today.AddDays(60),
                Status = WorkProjectStatus.Active,
                Sort = 2,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectName = "设备维护项目",
                ProjectCode = "PRJ-003",
                ProjectType = WorkProjectType.Internal,
                CustomerName = "内部",
                Description = "生产设备日常维护和保养",
                StartDate = today.AddDays(-90),
                EndDate = today.AddDays(90),
                Status = WorkProjectStatus.Active,
                Sort = 3,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectName = "新产品导入项目",
                ProjectCode = "PRJ-004",
                ProjectType = WorkProjectType.Internal,
                CustomerName = "内部",
                Description = "新产品线导入和验证",
                StartDate = today.AddDays(-15),
                EndDate = today.AddDays(45),
                Status = WorkProjectStatus.Active,
                Sort = 4,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectName = "能耗优化项目",
                ProjectCode = "PRJ-005",
                ProjectType = WorkProjectType.Internal,
                CustomerName = "内部",
                Description = "生产线能耗优化和成本降低",
                StartDate = today.AddDays(-45),
                Status = WorkProjectStatus.Active,
                Sort = 5,
                CreatedAt = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                ProjectName = "安全生产项目",
                ProjectCode = "PRJ-006",
                ProjectType = WorkProjectType.Internal,
                CustomerName = "内部",
                Description = "安全生产管理提升",
                StartDate = today.AddDays(-120),
                Status = WorkProjectStatus.Active,
                Sort = 6,
                CreatedAt = now
            }
        };
        await context.WorkProjects.AddRangeAsync(projects);

        var taskTypes = new List<WorkTaskType>
        {
            new() { Id = Guid.NewGuid(), TypeName = "设备调试", TypeCode = "TT-001", Description = "设备调试和参数优化", Sort = 1, Enabled = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), TypeName = "问题处理", TypeCode = "TT-002", Description = "问题处理和异常解决", Sort = 2, Enabled = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), TypeName = "日常维护", TypeCode = "TT-003", Description = "设备日常维护保养", Sort = 3, Enabled = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), TypeName = "生产作业", TypeCode = "TT-004", Description = "生产作业执行", Sort = 4, Enabled = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), TypeName = "质量检测", TypeCode = "TT-005", Description = "产品质量检测", Sort = 5, Enabled = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), TypeName = "数据记录", TypeCode = "TT-006", Description = "生产数据记录", Sort = 6, Enabled = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), TypeName = "培训学习", TypeCode = "TT-007", Description = "技能培训和学习", Sort = 7, Enabled = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), TypeName = "会议讨论", TypeCode = "TT-008", Description = "会议和沟通协调", Sort = 8, Enabled = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), TypeName = "其他", TypeCode = "TT-009", Description = "其他工作事项", Sort = 9, Enabled = true, CreatedAt = now }
        };
        await context.WorkTaskTypes.AddRangeAsync(taskTypes);

        var devices = new List<WorkDevice>
        {
            new() { Id = Guid.NewGuid(), ProjectId = projects[0].Id, DeviceName = "A线体", DeviceCode = "DEVICE-A01", DeviceType = WorkDeviceType.ProductionLine, Description = "主生产线A", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[0].Id, DeviceName = "B线体", DeviceCode = "DEVICE-B01", DeviceType = WorkDeviceType.ProductionLine, Description = "主生产线B", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[0].Id, DeviceName = "C线体", DeviceCode = "DEVICE-C01", DeviceType = WorkDeviceType.ProductionLine, Description = "主生产线C", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[1].Id, DeviceName = "D线体", DeviceCode = "DEVICE-D01", DeviceType = WorkDeviceType.ProductionLine, Description = "质量检测线", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[1].Id, DeviceName = "E线体", DeviceCode = "DEVICE-E01", DeviceType = WorkDeviceType.ProductionLine, Description = "包装线", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[2].Id, DeviceName = "测试设备1号", DeviceCode = "TEST-001", DeviceType = WorkDeviceType.TestingDevice, Description = "功能测试设备", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[2].Id, DeviceName = "测试设备2号", DeviceCode = "TEST-002", DeviceType = WorkDeviceType.TestingDevice, Description = "性能测试设备", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[2].Id, DeviceName = "测量设备1", DeviceCode = "MEAS-001", DeviceType = WorkDeviceType.TestingDevice, Description = "精密测量设备", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[0].Id, DeviceName = "机器人手臂1", DeviceCode = "ROBOT-001", DeviceType = WorkDeviceType.Equipment, Description = "装配机器人", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[0].Id, DeviceName = "机器人手臂2", DeviceCode = "ROBOT-002", DeviceType = WorkDeviceType.Equipment, Description = "焊接机器人", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[3].Id, DeviceName = "新产品测试线", DeviceCode = "NPI-001", DeviceType = WorkDeviceType.ProductionLine, Description = "新产品导入测试线", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[4].Id, DeviceName = "能耗监控设备", DeviceCode = "ENERGY-001", DeviceType = WorkDeviceType.TestingDevice, Description = "能耗监测设备", Status = WorkDeviceStatus.Active, CreatedAt = now },
            new() { Id = Guid.NewGuid(), ProjectId = projects[2].Id, DeviceName = "维护设备", DeviceCode = "MAINT-001", DeviceType = WorkDeviceType.Equipment, Description = "维修保养设备", Status = WorkDeviceStatus.Maintenance, CreatedAt = now }
        };
        await context.WorkDevices.AddRangeAsync(devices);

        var workLogs = new List<WorkLog>();
        var random = new Random(42);
        string[] weekDays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
        string[] logTitles = { "A线体设备调试", "B线体参数优化", "生产线日常巡检", "产品质量检测", "设备故障维修", "生产数据记录", "作业指导书更新", "新员工培训", "生产例会召开", "异常问题处理", "设备保养维护", "效率提升改善", "安全生产检查", "5S现场管理", "工艺文件编制" };
        string[] contents = { "完成设备调试，参数已优化到位，产出达标", "处理设备异常，更换磨损部件，恢复正常生产", "进行日常巡检，发现隐患及时处理", "完成产品质量检测，不良品率在可控范围内", "分析故障原因，制定预防措施", "记录生产数据，填写作业记录表", "更新作业指导书，优化作业流程", "组织新员工培训，讲解操作要点", "参加生产例会，讨论产能提升方案", "处理生产线异常，快速恢复生产", "完成设备定期保养，延长设备寿命", "分析生产效率数据，提出改善建议", "进行安全生产检查，整改安全隐患", "整理现场5S，营造整洁工作环境", "编制新工艺文件，规范化作业标准" };

        for (int dayOffset = -14; dayOffset <= 0; dayOffset++)
        {
            var date = today.AddDays(dayOffset);
            var weekDay = weekDays[(int)date.DayOfWeek];
            var logsPerDay = random.Next(3, 6);

            for (int i = 0; i < logsPerDay; i++)
            {
                var projectIndex = random.Next(projects.Count);
                var titleIndex = random.Next(logTitles.Length);
                var hours = random.NextDouble() * 4 + 0.5;
                hours = Math.Round(hours * 2) / 2;
                var status = random.Next(10) < 8 ? WorkLogStatus.Normal : random.Next(2) == 0 ? WorkLogStatus.MissingData : WorkLogStatus.PendingSupplement;

                workLogs.Add(new WorkLog
                {
                    Id = Guid.NewGuid(),
                    WorkDate = date,
                    WeekDay = weekDay,
                    ProjectId = projects[projectIndex].Id,
                    Title = logTitles[titleIndex],
                    OriginalContent = contents[titleIndex],
                    Summary = contents[titleIndex] + "，工作完成",
                    TotalHours = (decimal)hours,
                    Status = status,
                    SourceType = WorkLogSourceType.Manual,
                    Remark = status == WorkLogStatus.MissingData ? "数据待补充" : null,
                    CreatedAt = now
                });
            }
        }

        workLogs.Add(new WorkLog
        {
            Id = Guid.NewGuid(),
            WorkDate = today.AddDays(-1),
            WeekDay = weekDays[(int)today.AddDays(-1).DayOfWeek],
            ProjectId = projects[0].Id,
            Title = "计划转换-设备调试",
            OriginalContent = "根据昨日计划完成设备调试工作",
            Summary = "计划转换完成",
            TotalHours = 3,
            Status = WorkLogStatus.Normal,
            SourceType = WorkLogSourceType.PlanConversion,
            CreatedAt = now
        });

        await context.WorkLogs.AddRangeAsync(workLogs);

        var dailyPlans = new List<WorkDailyPlan>();
        string[] planTitles = { "A线体设备巡检", "B线体参数优化", "C线体清洁保养", "D线体质量检测", "设备故障处理", "生产数据记录", "新员工培训", "生产例会", "设备维护保养", "安全隐患排查", "效率提升会议", "5S检查", "产品质量评审", "工艺文件审核", "备件库存盘点" };
        string[] planContents = { "对A线体进行全面巡检，检查各部件运行状态", "优化B线体生产参数，提升产能", "清洁C线体各部件，更换润滑油", "对D线体产品进行质量抽检", "处理设备突发故障，恢复生产", "记录当日生产数据，整理报表", "组织新员工操作培训", "参加每日生产例会", "对关键设备进行维护保养", "排查生产线安全隐患", "分析效率数据，制定提升方案", "检查现场5S执行情况", "参与产品质量评审会议", "审核工艺文件完整性", "盘点备件库存，补货" };

        for (int i = 0; i < 5; i++)
        {
            var projectIndex = random.Next(projects.Count);
            var titleIndex = random.Next(planTitles.Length);
            var priority = (WorkDailyPlanPriority)random.Next(1, 5);
            var status = i < 2 ? WorkDailyPlanStatus.Completed : i == 2 ? WorkDailyPlanStatus.InProgress : random.Next(2) == 0 ? WorkDailyPlanStatus.Pending : WorkDailyPlanStatus.Cancelled;
            var startHour = 8 + i * 2;
            var endHour = startHour + random.Next(1, 3) + 1;

            dailyPlans.Add(new WorkDailyPlan
            {
                Id = Guid.NewGuid(),
                PlanDate = today,
                Title = planTitles[titleIndex],
                Content = planContents[titleIndex],
                ProjectId = projects[projectIndex].Id,
                Priority = priority,
                Status = status,
                StartTime = $"{startHour:D2}:00",
                EndTime = $"{endHour:D2}:00",
                EstimatedHours = endHour - startHour,
                ActualHours = status == WorkDailyPlanStatus.Completed ? (decimal)(endHour - startHour - 0.5) : null,
                Remark = priority == WorkDailyPlanPriority.Urgent ? "紧急任务" : null,
                CreatedAt = now
            });
        }

        var tomorrow = today.AddDays(1);
        for (int i = 0; i < 4; i++)
        {
            var projectIndex = random.Next(projects.Count);
            var titleIndex = random.Next(planTitles.Length);
            var priority = (WorkDailyPlanPriority)random.Next(1, 5);
            var startHour = 8 + i * 2;
            var endHour = startHour + random.Next(1, 4);

            dailyPlans.Add(new WorkDailyPlan
            {
                Id = Guid.NewGuid(),
                PlanDate = tomorrow,
                Title = planTitles[titleIndex],
                Content = planContents[titleIndex],
                ProjectId = projects[projectIndex].Id,
                Priority = priority,
                Status = WorkDailyPlanStatus.Pending,
                StartTime = $"{startHour:D2}:00",
                EndTime = $"{endHour:D2}:00",
                EstimatedHours = endHour - startHour,
                Remark = null,
                CreatedAt = now
            });
        }

        for (int dayOffset = -3; dayOffset < 0; dayOffset++)
        {
            var date = today.AddDays(dayOffset);
            for (int i = 0; i < 3; i++)
            {
                var projectIndex = random.Next(projects.Count);
                var titleIndex = random.Next(planTitles.Length);
                var startHour = 8 + i * 3;
                var endHour = startHour + random.Next(1, 3);

                dailyPlans.Add(new WorkDailyPlan
                {
                    Id = Guid.NewGuid(),
                    PlanDate = date,
                    Title = planTitles[titleIndex],
                    Content = planContents[titleIndex],
                    ProjectId = projects[projectIndex].Id,
                    Priority = (WorkDailyPlanPriority)random.Next(1, 5),
                    Status = WorkDailyPlanStatus.Completed,
                    StartTime = $"{startHour:D2}:00",
                    EndTime = $"{endHour:D2}:00",
                    EstimatedHours = endHour - startHour,
                    ActualHours = (decimal)(endHour - startHour - random.NextDouble()),
                    Remark = null,
                    CreatedAt = now
                });
            }
        }

        await context.WorkDailyPlans.AddRangeAsync(dailyPlans);

        var importBatch = new WorkImportBatch
        {
            Id = Guid.NewGuid(),
            FileName = "2024年5月工作日志.xlsx",
            Status = WorkImportStatus.Completed,
            ImportStrategy = WorkImportStrategy.SkipDuplicate,
            TotalRows = 156,
            SuccessRows = 152,
            FailedRows = 4,
            ErrorMessage = "4行数据格式错误",
            CreatedAt = now.AddDays(-7)
        };
        await context.WorkImportBatches.AddAsync(importBatch);

        Console.WriteLine("[DbSeeder] Saving changes...");
        await context.SaveChangesAsync();
        Console.WriteLine("[DbSeeder] Seed completed successfully!");
    }
}