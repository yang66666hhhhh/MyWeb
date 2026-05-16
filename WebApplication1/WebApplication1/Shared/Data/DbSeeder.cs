using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Features.Auth.Entities.Subscription;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Admin.Entities;
using WebApplication1.Features.DailyPlans;
using WebApplication1.Features.Tasks;
using FieldType = WebApplication1.Features.Work.Entities.FieldType;

namespace WebApplication1.Shared.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, ILogger? logger = null)
    {
        logger?.LogInformation("[DbSeeder] Starting seed...");

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(DateTime.Now);

        // Users
        if (!await context.Users.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding Users...");
            var users = new List<AppUser>
            {
                new()
                {
                    Username = "vben",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RealName = "平台所有者",
                    Avatar = $"https://api.dicebear.com/7.x/avataaars/svg?seed=vben",
                    Email = "vben@example.com",
                    Status = AppUserStatus.Active,
                    Roles = "owner",
                    CreatedAt = now
                },
                new()
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RealName = "专业用户",
                    Avatar = $"https://api.dicebear.com/7.x/avataaars/svg?seed=admin",
                    Email = "admin@example.com",
                    Status = AppUserStatus.Active,
                    Roles = "pro",
                    CreatedAt = now
                },
                new()
                {
                    Username = "jack",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RealName = "Jack Chen",
                    Avatar = $"https://api.dicebear.com/7.x/avataaars/svg?seed=jack",
                    Email = "jack@example.com",
                    Status = AppUserStatus.Active,
                    Roles = "member",
                    CreatedAt = now
                },
                new()
                {
                    Username = "lisa",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RealName = "Lisa Wang",
                    Avatar = $"https://api.dicebear.com/7.x/avataaars/svg?seed=lisa",
                    Email = "lisa@example.com",
                    Status = AppUserStatus.Active,
                    Roles = "member",
                    CreatedAt = now
                },
                new()
                {
                    Username = "tom",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RealName = "Tom Zhang",
                    Avatar = $"https://api.dicebear.com/7.x/avataaars/svg?seed=tom",
                    Email = "tom@example.com",
                    Status = AppUserStatus.Inactive,
                    Roles = "member",
                    CreatedAt = now
                },
            };
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] Users seeded.");
        }

        // Tags
        if (!await context.Tags.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding Tags...");
            var tags = new List<Tag>
            {
                new() { Id = Guid.NewGuid(), Name = "日志管理", Description = "工作日志相关功能", Color = "#1890ff", Sort = 1, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "系统配置", Description = "系统管理和配置", Color = "#722ed1", Sort = 2, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "考研学习", Description = "考研相关功能", Color = "#52c41a", Sort = 3, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "习惯养成", Description = "习惯打卡功能", Color = "#fa8c16", Sort = 4, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "知识库", Description = "知识管理功能", Color = "#13c2c2", Sort = 5, IsActive = true, CreatedAt = now }
            };
            context.Tags.AddRange(tags);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] Tags seeded.");
        }

        // MenuConfigs (下拉框选项)
        if (!await context.MenuConfigs.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding MenuConfigs...");
            var configs = new List<MenuConfig>
            {
                new() { Id = Guid.NewGuid(), Path = "/dashboard", Name = "仪表盘", Icon = "ant-design:dashboard", Sort = 0, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/work/logs", Name = "工作日志", Icon = "ant-design:file-text", Sort = 10, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/work/projects", Name = "项目列表", Icon = "ant-design:project", Sort = 11, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/work/templates", Name = "模板中心", Icon = "ant-design:layout", Sort = 12, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/work/daily-plan", Name = "每日计划", Icon = "ant-design:calendar", Sort = 13, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/work/import", Name = "数据导入", Icon = "ant-design:upload", Sort = 14, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/growth/habits", Name = "习惯打卡", Icon = "ant-design:check-circle", Sort = 20, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/student/postgraduate", Name = "考研任务", Icon = "ant-design:book", Sort = 21, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/student/mistakes", Name = "错题本", Icon = "ant-design:warning", Sort = 22, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/growth/knowledge-base", Name = "知识库", Icon = "ant-design:database", Sort = 23, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/growth/projects", Name = "成长项目", Icon = "ant-design:rocket", Sort = 24, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/system/user", Name = "用户管理", Icon = "lucide:users", Sort = 90, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/system/menu-tag", Name = "菜单标签", Icon = "lucide:menu", Sort = 91, IsActive = true, CreatedAt = now },
            };
            context.MenuConfigs.AddRange(configs);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] MenuConfigs seeded.");
        }

        // MenuItems (用户看到的菜单 - 仅业务菜单，系统管理菜单由 RoleMenu 控制)
        if (!await context.MenuItems.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding MenuItems...");
            var tagDict = await context.Tags.ToDictionaryAsync(t => t.Name);
            var menuItems = new List<MenuItem>
            {
                new() { Id = Guid.NewGuid(), Name = "工作日志", Path = "/work/logs", Icon = "ant-design:file-text", Sort = 10, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "项目列表", Path = "/work/projects", Icon = "ant-design:project", Sort = 11, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "模板中心", Path = "/work/templates", Icon = "ant-design:layout", Sort = 12, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "每日计划", Path = "/work/daily-plan", Icon = "ant-design:calendar", Sort = 13, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "数据导入", Path = "/work/import", Icon = "ant-design:upload", Sort = 14, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "习惯打卡", Path = "/growth/habits", Icon = "ant-design:check-circle", Sort = 20, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "考研任务", Path = "/student/postgraduate", Icon = "ant-design:book", Sort = 21, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "错题本", Path = "/student/mistakes", Icon = "ant-design:warning", Sort = 22, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "知识库", Path = "/growth/knowledge-base", Icon = "ant-design:database", Sort = 23, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "成长项目", Path = "/growth/projects", Icon = "ant-design:rocket", Sort = 24, IsActive = true, CreatedAt = now },
            };
            context.MenuItems.AddRange(menuItems);
            await context.SaveChangesAsync();

            // MenuTags
            logger?.LogInformation("[DbSeeder] Seeding MenuTags...");
            var menuItemDict = menuItems.ToDictionary(m => m.Path);
            var menuTags = new List<MenuTag>
            {
                new() { MenuItemId = menuItemDict["/work/logs"].Id, TagId = tagDict["日志管理"].Id },
                new() { MenuItemId = menuItemDict["/work/projects"].Id, TagId = tagDict["日志管理"].Id },
                new() { MenuItemId = menuItemDict["/work/templates"].Id, TagId = tagDict["日志管理"].Id },
                new() { MenuItemId = menuItemDict["/work/daily-plan"].Id, TagId = tagDict["日志管理"].Id },
                new() { MenuItemId = menuItemDict["/work/import"].Id, TagId = tagDict["日志管理"].Id },
                new() { MenuItemId = menuItemDict["/growth/habits"].Id, TagId = tagDict["习惯养成"].Id },
                new() { MenuItemId = menuItemDict["/student/postgraduate"].Id, TagId = tagDict["考研学习"].Id },
                new() { MenuItemId = menuItemDict["/student/mistakes"].Id, TagId = tagDict["考研学习"].Id },
                new() { MenuItemId = menuItemDict["/growth/knowledge-base"].Id, TagId = tagDict["知识库"].Id },
                new() { MenuItemId = menuItemDict["/growth/projects"].Id, TagId = tagDict["考研学习"].Id },
            };
            context.MenuTags.AddRange(menuTags);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] MenuItems and MenuTags seeded.");
        }

        // UserTypes
        if (!await context.UserTypes.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding UserTypes...");
            var userTypes = new List<UserType>
            {
                new() { Id = Guid.NewGuid(), Name = "普通用户", Code = "user", Description = "普通用户", Color = "#1890ff", Sort = 1, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "考研学生", Code = "exam_student", Description = "准备考研的学生", Color = "#52c41a", Sort = 2, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "PCB工程师", Code = "pcb_engineer", Description = "PCB设计工程师", Color = "#fa8c16", Sort = 3, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "教师", Code = "teacher", Description = "学校教师", Color = "#722ed1", Sort = 4, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "IT工程师", Code = "it_engineer", Description = "IT开发工程师", Color = "#13c2c2", Sort = 5, IsActive = true, CreatedAt = now },
            };
            context.UserTypes.AddRange(userTypes);
            await context.SaveChangesAsync();

            // UserTypeTags
            var tagDict = await context.Tags.ToDictionaryAsync(t => t.Name);
            var typeDict = userTypes.ToDictionary(t => t.Code);
            var userTypeTags = new List<UserTypeTag>
            {
                new() { UserTypeId = typeDict["user"].Id, TagId = tagDict["日志管理"].Id },
                new() { UserTypeId = typeDict["user"].Id, TagId = tagDict["习惯养成"].Id },
                new() { UserTypeId = typeDict["exam_student"].Id, TagId = tagDict["考研学习"].Id },
                new() { UserTypeId = typeDict["exam_student"].Id, TagId = tagDict["习惯养成"].Id },
                new() { UserTypeId = typeDict["exam_student"].Id, TagId = tagDict["知识库"].Id },
                new() { UserTypeId = typeDict["pcb_engineer"].Id, TagId = tagDict["日志管理"].Id },
                new() { UserTypeId = typeDict["teacher"].Id, TagId = tagDict["日志管理"].Id },
                new() { UserTypeId = typeDict["teacher"].Id, TagId = tagDict["习惯养成"].Id },
                new() { UserTypeId = typeDict["it_engineer"].Id, TagId = tagDict["日志管理"].Id },
                new() { UserTypeId = typeDict["it_engineer"].Id, TagId = tagDict["系统配置"].Id },
            };
            context.UserTypeTags.AddRange(userTypeTags);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] UserTypes and UserTypeTags seeded.");
        }

        // PersonaTypes - 身份类型
        if (!await context.PersonaTypes.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding PersonaTypes...");
            var personaTypes = new List<PersonaType>
            {
                new() { Code = "Developer", Name = "开发者", Icon = "💻", Description = "软件开发和编程相关工作", DefaultHomeRoute = "/dashboard/workspace", Sort = 1, IsActive = true, CreatedAt = now },
                new() { Code = "Implementation", Name = "实施工程师", Icon = "🔧", Description = "项目实施和技术支持", DefaultHomeRoute = "/work/dashboard", Sort = 2, IsActive = true, CreatedAt = now },
                new() { Code = "Sales", Name = "销售", Icon = "💼", Description = "销售和客户管理", DefaultHomeRoute = "/growth/dashboard", Sort = 3, IsActive = true, CreatedAt = now },
                new() { Code = "Teacher", Name = "教师", Icon = "📚", Description = "教学和课程管理", DefaultHomeRoute = "/growth/dashboard", Sort = 4, IsActive = true, CreatedAt = now },
                new() { Code = "Designer", Name = "设计师", Icon = "🎨", Description = "设计和创意工作", DefaultHomeRoute = "/work/dashboard", Sort = 5, IsActive = true, CreatedAt = now },
                new() { Code = "Student", Name = "学生", Icon = "📖", Description = "学习和课程学习", DefaultHomeRoute = "/growth/dashboard", Sort = 6, IsActive = true, CreatedAt = now },
                new() { Code = "Freelancer", Name = "自由职业", Icon = "🌟", Description = "自由职业和外包工作", DefaultHomeRoute = "/work/dashboard", Sort = 7, IsActive = true, CreatedAt = now },
                new() { Code = "General", Name = "通用", Icon = "👤", Description = "通用模式，使用所有功能", DefaultHomeRoute = "/dashboard/workspace", Sort = 99, IsActive = true, CreatedAt = now },
            };
            context.PersonaTypes.AddRange(personaTypes);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] PersonaTypes seeded.");
        }

        // UserPersonas - assign personas to users (多对多)
        if (!await context.UserPersonas.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding UserPersonas...");
            var personaDict = await context.PersonaTypes.ToDictionaryAsync(p => p.Code);
            var users = await context.Users.ToListAsync();
            var vben = users.First(u => u.Username == "vben");
            var admin = users.First(u => u.Username == "admin");
            var jack = users.First(u => u.Username == "jack");
            var lisa = users.First(u => u.Username == "lisa");

            var userPersonas = new List<UserPersona>
            {
                new() { UserId = vben.Id, PersonaTypeId = personaDict["Developer"].Id, IsPrimary = true },
                new() { UserId = vben.Id, PersonaTypeId = personaDict["Implementation"].Id, IsPrimary = false },
                new() { UserId = admin.Id, PersonaTypeId = personaDict["General"].Id, IsPrimary = true },
                new() { UserId = jack.Id, PersonaTypeId = personaDict["Developer"].Id, IsPrimary = true },
                new() { UserId = lisa.Id, PersonaTypeId = personaDict["Student"].Id, IsPrimary = true },
            };
            context.UserPersonas.AddRange(userPersonas);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] UserPersonas seeded.");
        }

        // UserTags - assign tags to users
        if (!await context.UserTags.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding UserTags...");
            var tagDict = await context.Tags.ToDictionaryAsync(t => t.Name);
            var users = await context.Users.ToListAsync();
            var vben = users.First(u => u.Username == "vben");
            var admin = users.First(u => u.Username == "admin");
            var jack = users.First(u => u.Username == "jack");
            var lisa = users.First(u => u.Username == "lisa");

            var userTags = new List<UserTag>
            {
                new() { UserId = vben.Id, TagId = tagDict["日志管理"].Id },
                new() { UserId = vben.Id, TagId = tagDict["系统配置"].Id },
                new() { UserId = vben.Id, TagId = tagDict["考研学习"].Id },
                new() { UserId = vben.Id, TagId = tagDict["习惯养成"].Id },
                new() { UserId = vben.Id, TagId = tagDict["知识库"].Id },
                new() { UserId = admin.Id, TagId = tagDict["日志管理"].Id },
                new() { UserId = admin.Id, TagId = tagDict["系统配置"].Id },
                new() { UserId = jack.Id, TagId = tagDict["日志管理"].Id },
                new() { UserId = jack.Id, TagId = tagDict["习惯养成"].Id },
                new() { UserId = lisa.Id, TagId = tagDict["考研学习"].Id },
                new() { UserId = lisa.Id, TagId = tagDict["习惯养成"].Id },
                new() { UserId = lisa.Id, TagId = tagDict["知识库"].Id },
            };
            context.UserTags.AddRange(userTags);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] UserTags seeded.");
        }

        // RoleMenus
        logger?.LogInformation("[DbSeeder] Syncing RoleMenus...");
        await SyncRoleMenusAsync(context, now);
        logger?.LogInformation("[DbSeeder] RoleMenus synced.");

        // WorkTaskTypes - 任务类型
        if (!await context.WorkTaskTypes.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding WorkTaskTypes...");
            var taskTypes = new List<WorkTaskType>
            {
                new() { TypeName = "设备调试", TypeCode = "DEVICE_DEBUG", Description = "设备安装调试工作", Sort = 1, Enabled = true, CreatedAt = now },
                new() { TypeName = "问题处理", TypeCode = "TROUBLESHOOT", Description = "故障处理和问题解决", Sort = 2, Enabled = true, CreatedAt = now },
                new() { TypeName = "日常维护", TypeCode = "MAINTENANCE", Description = "日常维护保养工作", Sort = 3, Enabled = true, CreatedAt = now },
                new() { TypeName = "生产作业", TypeCode = "PRODUCTION", Description = "生产线作业任务", Sort = 4, Enabled = true, CreatedAt = now },
                new() { TypeName = "质量检测", TypeCode = "QC", Description = "质量检查和测试", Sort = 5, Enabled = true, CreatedAt = now },
                new() { TypeName = "数据记录", TypeCode = "DATA_ENTRY", Description = "生产数据记录统计", Sort = 6, Enabled = true, CreatedAt = now },
                new() { TypeName = "培训学习", TypeCode = "TRAINING", Description = "培训和学习任务", Sort = 7, Enabled = true, CreatedAt = now },
                new() { TypeName = "会议讨论", TypeCode = "MEETING", Description = "会议和沟通协调", Sort = 8, Enabled = true, CreatedAt = now },
                new() { TypeName = "其他", TypeCode = "OTHER", Description = "其他工作", Sort = 9, Enabled = true, CreatedAt = now },
            };
            context.WorkTaskTypes.AddRange(taskTypes);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] WorkTaskTypes seeded.");
        }

        // WorkProjects - 工作项目
        if (!await context.WorkProjects.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding WorkProjects...");
            var projects = new List<WorkProject>
            {
                new() { ProjectName = "生产线升级项目", ProjectCode = "PROJ001", ProjectType = WorkProjectType.RAndD, CustomerName = "内部", Location = "惠州", Description = "生产线设备升级改造", StartDate = today.AddDays(-60), EndDate = today.AddDays(30), Status = WorkProjectStatus.Active, Sort = 1, CreatedAt = now },
                new() { ProjectName = "质量改进项目", ProjectCode = "PROJ002", ProjectType = WorkProjectType.Support, CustomerName = "内部", Location = "东莞", Description = "提升产品质量和良品率", StartDate = today.AddDays(-45), EndDate = today.AddDays(45), Status = WorkProjectStatus.Active, Sort = 2, CreatedAt = now },
                new() { ProjectName = "设备维护项目", ProjectCode = "PROJ003", ProjectType = WorkProjectType.Support, CustomerName = "内部", Location = "深圳", Description = "设备定期维护保养", StartDate = today.AddDays(-30), EndDate = today.AddDays(60), Status = WorkProjectStatus.Active, Sort = 3, CreatedAt = now },
                new() { ProjectName = "新产品导入项目", ProjectCode = "PROJ004", ProjectType = WorkProjectType.RAndD, CustomerName = "客户A", Location = "苏州", Description = "新产品NPI导入", StartDate = today.AddDays(-20), EndDate = today.AddDays(90), Status = WorkProjectStatus.Active, Sort = 4, CreatedAt = now },
                new() { ProjectName = "能耗优化项目", ProjectCode = "PROJ005", ProjectType = WorkProjectType.Internal, CustomerName = "内部", Location = "佛山", Description = "降低生产能耗成本", StartDate = today.AddDays(-15), EndDate = today.AddDays(75), Status = WorkProjectStatus.Active, Sort = 5, CreatedAt = now },
                new() { ProjectName = "安全生产项目", ProjectCode = "PROJ006", ProjectType = WorkProjectType.Internal, CustomerName = "内部", Location = "中山", Description = "安全生产管理提升", StartDate = today.AddDays(-10), EndDate = today.AddDays(100), Status = WorkProjectStatus.Active, Sort = 6, CreatedAt = now },
            };
            context.WorkProjects.AddRange(projects);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] WorkProjects seeded.");
        }

        // WorkDevices - 工作设备
        if (!await context.WorkDevices.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding WorkDevices...");
            var projectDict = await context.WorkProjects.ToDictionaryAsync(p => p.ProjectCode);
            var devices = new List<WorkDevice>
            {
                new() { ProjectId = projectDict["PROJ001"].Id, DeviceName = "A线体", DeviceCode = "DEVICE_A", DeviceType = WorkDeviceType.ProductionLine, Description = "A产品生产线", Status = WorkDeviceStatus.Active, CreatedAt = now },
                new() { ProjectId = projectDict["PROJ001"].Id, DeviceName = "B线体", DeviceCode = "DEVICE_B", DeviceType = WorkDeviceType.ProductionLine, Description = "B产品生产线", Status = WorkDeviceStatus.Active, CreatedAt = now },
                new() { ProjectId = projectDict["PROJ001"].Id, DeviceName = "C线体", DeviceCode = "DEVICE_C", DeviceType = WorkDeviceType.ProductionLine, Description = "C产品生产线", Status = WorkDeviceStatus.Active, CreatedAt = now },
                new() { ProjectId = projectDict["PROJ002"].Id, DeviceName = "D线体", DeviceCode = "DEVICE_D", DeviceType = WorkDeviceType.ProductionLine, Description = "D产品生产线", Status = WorkDeviceStatus.Active, CreatedAt = now },
                new() { ProjectId = projectDict["PROJ003"].Id, DeviceName = "测试设备1", DeviceCode = "TEST_001", DeviceType = WorkDeviceType.TestingDevice, Description = "功能测试设备", Status = WorkDeviceStatus.Active, CreatedAt = now },
                new() { ProjectId = projectDict["PROJ003"].Id, DeviceName = "测试设备2", DeviceCode = "TEST_002", DeviceType = WorkDeviceType.TestingDevice, Description = "性能测试设备", Status = WorkDeviceStatus.Active, CreatedAt = now },
                new() { ProjectId = projectDict["PROJ001"].Id, DeviceName = "机器人手臂1", DeviceCode = "ROBOT_001", DeviceType = WorkDeviceType.Equipment, Description = "自动化装配机器人", Status = WorkDeviceStatus.Active, CreatedAt = now },
                new() { ProjectId = projectDict["PROJ001"].Id, DeviceName = "机器人手臂2", DeviceCode = "ROBOT_002", DeviceType = WorkDeviceType.Equipment, Description = "自动化焊接机器人", Status = WorkDeviceStatus.Active, CreatedAt = now },
                new() { ProjectId = projectDict["PROJ004"].Id, DeviceName = "E线体", DeviceCode = "DEVICE_E", DeviceType = WorkDeviceType.ProductionLine, Description = "新产品试产线", Status = WorkDeviceStatus.Active, CreatedAt = now },
                new() { ProjectId = projectDict["PROJ005"].Id, DeviceName = "能源监控设备", DeviceCode = "ENERGY_001", DeviceType = WorkDeviceType.TestingDevice, Description = "能耗监测系统", Status = WorkDeviceStatus.Active, CreatedAt = now },
                new() { ProjectId = projectDict["PROJ006"].Id, DeviceName = "安全监控设备", DeviceCode = "SAFE_001", DeviceType = WorkDeviceType.TestingDevice, Description = "安全生产监控", Status = WorkDeviceStatus.Active, CreatedAt = now },
            };
            context.WorkDevices.AddRange(devices);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] WorkDevices seeded.");
        }

        // WorkLogs - 工作日志（约90条，覆盖14天）
        if (!await context.WorkLogs.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding WorkLogs...");
            var projectDict = await context.WorkProjects.ToDictionaryAsync(p => p.ProjectCode);
            var deviceDict = await context.WorkDevices.ToDictionaryAsync(d => d.DeviceCode);
            var taskTypeDict = await context.WorkTaskTypes.ToDictionaryAsync(t => t.TypeCode);
            var user = await context.Users.FirstAsync(u => u.Username == "vben");

            var logs = new List<WorkLog>();
            var random = new Random(42); // 固定种子保证数据一致

            // 14天内每天3-5条日志
            for (int day = -14; day <= 0; day++)
            {
                var workDate = today.AddDays(day);
                var weekDay = workDate.DayOfWeek.ToString();
                int logsPerDay = random.Next(3, 6);

                for (int i = 0; i < logsPerDay; i++)
                {
                    var projectCodes = new[] { "PROJ001", "PROJ002", "PROJ003", "PROJ004" };
                    var deviceCodes = new[] { "DEVICE_A", "DEVICE_B", "DEVICE_C", "TEST_001", "ROBOT_001" };
                    var taskTypeCodes = new[] { "DEVICE_DEBUG", "TROUBLESHOOT", "MAINTENANCE", "PRODUCTION", "QC", "DATA_ENTRY", "TRAINING", "MEETING" };

                    var projectCode = projectCodes[random.Next(projectCodes.Length)];
                    var deviceCode = deviceCodes[random.Next(deviceCodes.Length)];
                    var taskTypeCode = taskTypeCodes[random.Next(taskTypeCodes.Length)];

                    var hours = random.NextDouble() * 6 + 2; // 2-8小时
                    var log = new WorkLog
                    {
                        UserId = user.Id,
                        WorkDate = workDate,
                        WeekDay = weekDay,
                        ProjectId = projectDict[projectCode].Id,
                        Title = GetWorkLogTitle(taskTypeCode, projectCode),
                        OriginalContent = GetWorkLogContent(taskTypeCode),
                        Summary = GetWorkLogSummary(taskTypeCode),
                        TotalHours = Math.Round((decimal)hours, 2),
                        Status = day < -3 ? WorkLogStatus.Normal : (random.Next(10) < 8 ? WorkLogStatus.Normal : (WorkLogStatus)random.Next(1, 3)),
                        SourceType = WorkLogSourceType.Manual,
                        CreatedAt = now.AddDays(day).AddHours(random.Next(8, 18)).AddMinutes(random.Next(0, 60))
                    };
                    logs.Add(log);
                }
            }

            context.WorkLogs.AddRange(logs);
            await context.SaveChangesAsync();

            // WorkLogItems - 工作日志明细
            logger?.LogInformation("[DbSeeder] Seeding WorkLogItems...");
            var logItems = new List<WorkLogItem>();
            var logList = await context.WorkLogs.Include(l => l.Project).ToListAsync();
            foreach (var log in logList)
            {
                var itemCount = random.Next(1, 4);
                for (int i = 0; i < itemCount; i++)
                {
                    var deviceCodes = new[] { "DEVICE_A", "DEVICE_B", "DEVICE_C", "TEST_001", "ROBOT_001", "DEVICE_D", "DEVICE_E" };
                    var taskTypeCodes = new[] { "DEVICE_DEBUG", "TROUBLESHOOT", "MAINTENANCE", "PRODUCTION", "QC" };

                    var item = new WorkLogItem
                    {
                        WorkLogId = log.Id,
                        Content = GetWorkItemContent(taskTypeCodes[random.Next(taskTypeCodes.Length)]),
                        TaskTypeId = taskTypeDict[taskTypeCodes[random.Next(taskTypeCodes.Length)]].Id,
                        DeviceId = deviceDict[deviceCodes[random.Next(deviceCodes.Length)]].Id,
                        Hours = Math.Round((decimal)(log.TotalHours / itemCount), 2),
                        ProgressPercent = random.Next(60, 101),
                        Sort = i
                    };
                    logItems.Add(item);
                }
            }
            context.WorkLogItems.AddRange(logItems);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] WorkLogs and WorkLogItems seeded.");
        }

        // DailyPlans - 个人每日计划
        if (!await context.DailyPlans.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding DailyPlans...");
            var user = await context.Users.FirstAsync(u => u.Username == "vben");
            var plans = new List<DailyPlan>();

            // 今天和明天的计划
            var todayPlans = new[]
            {
                ("完成生产线A的调试工作", "正在调试中发现参数异常", 3),
                ("参加质量分析会议", "准备汇报材料", 2),
                ("设备日常点检", null, 1),
            };
            foreach (var (title, remark, priority) in todayPlans)
            {
                plans.Add(new DailyPlan
                {
                    PlanDate = today,
                    Title = title,
                    Remark = remark,
                    Priority = priority,
                    Status = priority == 3 ? DailyPlanStatus.Completed : DailyPlanStatus.Pending,
                    CreatedAt = now
                });
            }

            // 明天的计划
            var tomorrowPlans = new[]
            {
                ("完成设备B的维护保养", "准备维护工具和配件", 3),
                ("编写设备操作SOP", "参考现有文档", 2),
                ("参加项目例会", "准备进度汇报", 2),
                ("整理月度数据报告", null, 1),
            };
            foreach (var (title, remark, priority) in tomorrowPlans)
            {
                plans.Add(new DailyPlan
                {
                    PlanDate = today.AddDays(1),
                    Title = title,
                    Remark = remark,
                    Priority = priority,
                    Status = DailyPlanStatus.Pending,
                    CreatedAt = now
                });
            }

            // 过去3天的已完成计划
            for (int day = -3; day <= -1; day++)
            {
                plans.Add(new DailyPlan
                {
                    PlanDate = today.AddDays(day),
                    Title = $"已完成任务 - {Math.Abs(day)}天前",
                    Remark = "已完成的工作内容",
                    Priority = 2,
                    Status = DailyPlanStatus.Completed,
                    CreatedAt = now.AddDays(day)
                });
            }

            context.DailyPlans.AddRange(plans);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] DailyPlans seeded.");
        }

        // Tasks - 统一任务系统
        if (!await context.Tasks.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding Tasks...");
            var user = await context.Users.FirstAsync(u => u.Username == "vben");
            var tasks = new List<TaskItem>();

            var todayTasks = new[]
            {
                ("完成生产线A的调试工作", "正在调试中发现参数异常", TaskPriority.High, TaskType.Work, TaskSource.Work),
                ("参加质量分析会议", "准备汇报材料", TaskPriority.Medium, TaskType.Work, TaskSource.Work),
                ("设备日常点检", null, TaskPriority.Low, TaskType.Work, TaskSource.Work),
                ("阅读《设计模式》章节", "复习单例模式", TaskPriority.Medium, TaskType.Personal, TaskSource.Growth),
                ("30分钟有氧运动", null, TaskPriority.Medium, TaskType.Personal, TaskSource.Growth),
            };
            foreach (var (title, remark, priority, type, source) in todayTasks)
            {
                tasks.Add(new TaskItem
                {
                    UserId = user.Id,
                    PlanDate = today,
                    Title = title,
                    Description = remark,
                    Remark = remark,
                    Priority = priority,
                    Type = type,
                    Source = source,
                    Status = priority == TaskPriority.High ? TaskItemStatus.Completed : TaskItemStatus.Pending,
                    CreatedAt = now
                });
            }

            var tomorrowTasks = new[]
            {
                ("完成设备B的维护保养", "准备维护工具和配件", TaskPriority.High, TaskType.Work, TaskSource.Work),
                ("编写设备操作SOP", "参考现有文档", TaskPriority.Medium, TaskType.Work, TaskSource.Work),
                ("参加项目例会", "准备进度汇报", TaskPriority.Medium, TaskType.Work, TaskSource.Work),
                ("整理月度数据报告", null, TaskPriority.Low, TaskType.Work, TaskSource.Work),
                ("学习英语口语", "30分钟听力练习", TaskPriority.Medium, TaskType.Personal, TaskSource.Growth),
            };
            foreach (var (title, remark, priority, type, source) in tomorrowTasks)
            {
                tasks.Add(new TaskItem
                {
                    UserId = user.Id,
                    PlanDate = today.AddDays(1),
                    Title = title,
                    Description = remark,
                    Remark = remark,
                    Priority = priority,
                    Type = type,
                    Source = source,
                    Status = TaskItemStatus.Pending,
                    CreatedAt = now
                });
            }

            for (int day = -3; day <= -1; day++)
            {
                tasks.Add(new TaskItem
                {
                    UserId = user.Id,
                    PlanDate = today.AddDays(day),
                    Title = $"已完成任务 - {Math.Abs(day)}天前",
                    Description = "已完成的工作内容",
                    Priority = TaskPriority.Medium,
                    Type = TaskType.Work,
                    Source = TaskSource.Work,
                    Status = TaskItemStatus.Completed,
                    CreatedAt = now.AddDays(day)
                });
            }

            context.Tasks.AddRange(tasks);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] Tasks seeded.");
        }

        // WorkLogTemplates
        if (!await context.WorkLogTemplates.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding WorkLogTemplates...");

            var eapTemplate = new WorkLogTemplate
            {
                PersonaCode = "Implementation",
                Name = "EAP实施日志",
                Description = "EAP设备自动化平台实施工程师专用日志模板",
                FieldDefinitions = @"{
                    ""fields"": [
                        {
                            ""key"": ""location"",
                            ""label"": ""项目地"",
                            ""type"": ""select"",
                            ""options"": [""惠州"", ""深圳"", ""东莞"", ""苏州"", ""上海""],
                            ""required"": true,
                            ""placeholder"": ""请选择项目地""
                        },
                        {
                            ""key"": ""equipment"",
                            ""label"": ""线体/设备"",
                            ""type"": ""multi-select"",
                            ""required"": true,
                            ""placeholder"": ""选择涉及的设备""
                        },
                        {
                            ""key"": ""taskTypes"",
                            ""label"": ""任务类型"",
                            ""type"": ""multi-select"",
                            ""options"": [""学习"", ""梳理"", ""调试/协助"", ""配置"", ""对接"", ""测试""],
                            ""required"": true
                        },
                        {
                            ""key"": ""msapItems"",
                            ""label"": ""MSAP工作项"",
                            ""type"": ""task-list"",
                            ""fields"": [""content"", ""percent""],
                            ""placeholder"": ""添加MSAP工作项""
                        },
                        {
                            ""key"": ""hdiItems"",
                            ""label"": ""HDI工作项"",
                            ""type"": ""task-list"",
                            ""fields"": [""content"", ""percent""],
                            ""placeholder"": ""添加HDI工作项""
                        }
                    ]
                }",
                IsActive = true,
                Sort = 1,
                CreatedAt = now
            };

            var devTemplate = new WorkLogTemplate
            {
                PersonaCode = "Developer",
                Name = "开发工作日志",
                Description = "软件开发者专用日志模板",
                FieldDefinitions = @"{
                    ""fields"": [
                        {
                            ""key"": ""repository"",
                            ""label"": ""代码仓库"",
                            ""type"": ""text"",
                            ""required"": true
                        },
                        {
                            ""key"": ""branch"",
                            ""label"": ""分支"",
                            ""type"": ""text""
                        },
                        {
                            ""key"": ""commits"",
                            ""label"": ""提交次数"",
                            ""type"": ""number""
                        },
                        {
                            ""key"": ""linesChanged"",
                            ""label"": ""代码行数变更"",
                            ""type"": ""number""
                        },
                        {
                            ""key"": ""bugsFixed"",
                            ""label"": ""修复Bug数"",
                            ""type"": ""number""
                        }
                    ]
                }",
                IsActive = true,
                Sort = 2,
                CreatedAt = now
            };

            var teacherTemplate = new WorkLogTemplate
            {
                PersonaCode = "Teacher",
                Name = "教学工作日志",
                Description = "教师专用日志模板",
                FieldDefinitions = @"{
                    ""fields"": [
                        {
                            ""key"": ""course"",
                            ""label"": ""课程名称"",
                            ""type"": ""text"",
                            ""required"": true
                        },
                        {
                            ""key"": ""className"",
                            ""label"": ""班级"",
                            ""type"": ""text""
                        },
                        {
                            ""key"": ""studentCount"",
                            ""label"": ""学生人数"",
                            ""type"": ""number""
                        },
                        {
                            ""key"": ""teachingHours"",
                            ""label"": ""授课时长"",
                            ""type"": ""number""
                        },
                        {
                            ""key"": ""homework"",
                            ""label"": ""布置作业"",
                            ""type"": ""textarea""
                        }
                    ]
                }",
                IsActive = true,
                Sort = 3,
                CreatedAt = now
            };

            context.WorkLogTemplates.AddRange(eapTemplate, devTemplate, teacherTemplate);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] WorkLogTemplates seeded.");
        }

        // Features
        if (!await context.Features.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding Features...");
            var features = new List<Feature>
            {
                // 工作模块
                new() { Code = "WORK_LOG", Name = "工作日志", Category = "Work", Description = "记录和管理工作日志" },
                new() { Code = "WORK_PROJECT", Name = "工作项目", Category = "Work", Description = "管理工作项目" },
                new() { Code = "WORK_DEVICE", Name = "设备管理", Category = "Work", Description = "管理工作中使用的设备" },
                new() { Code = "WORK_TASK", Name = "工作任务", Category = "Work", Description = "管理工作任务" },
                new() { Code = "WORK_IMPORT", Name = "数据导入", Category = "Work", Description = "Excel数据导入" },
                new() { Code = "WORK_STATISTICS", Name = "工作统计", Category = "Work", Description = "工作日志与任务统计看板" },
                new() { Code = "WORK_TEMPLATE", Name = "模板管理", Category = "Work", Description = "行业模板管理" },
                new() { Code = "WORK_OKR", Name = "OKR管理", Category = "Work", Description = "目标与关键结果管理" },
                new() { Code = "WORK_GANTT", Name = "甘特图", Category = "Work", Description = "项目甘特图" },
                new() { Code = "WORK_RISK", Name = "风险管理", Category = "Work", Description = "项目风险管理" },

                // 成长模块
                new() { Code = "TASK_UNIFIED", Name = "统一任务", Category = "Growth", Description = "统一任务系统（个人+工作）" },
                new() { Code = "GROWTH_DAILY_PLAN", Name = "每日计划", Category = "Growth", Description = "每日计划管理" },
                new() { Code = "GROWTH_HABIT", Name = "习惯打卡", Category = "Growth", Description = "习惯养成和打卡" },
                new() { Code = "GROWTH_KNOWLEDGE", Name = "知识库", Category = "Growth", Description = "知识文章管理" },
                new() { Code = "GROWTH_SKILL", Name = "技能管理", Category = "Growth", Description = "技能追踪和提升" },
                new() { Code = "GROWTH_FITNESS", Name = "健身管理", Category = "Growth", Description = "健身记录和分析" },

                // AI模块
                new() { Code = "AI_ASSISTANT", Name = "AI助手", Category = "AI", Description = "AI对话助手" },
                new() { Code = "AI_PLANNER", Name = "AI规划器", Category = "AI", Description = "AI智能规划" },
                new() { Code = "AI_REPORT", Name = "AI报告", Category = "AI", Description = "AI生成报告" },
                new() { Code = "AI_INSIGHTS", Name = "数据洞察", Category = "AI", Description = "AI数据分析洞察" },

                // 财务模块
                new() { Code = "ASSET_DASHBOARD", Name = "资产看板", Category = "Assets", Description = "财务资产总览" },
                new() { Code = "ASSET_INCOME", Name = "收入管理", Category = "Assets", Description = "收入记录和分析" },
                new() { Code = "ASSET_EXPENSE", Name = "支出管理", Category = "Assets", Description = "支出记录和分析" },
                new() { Code = "ASSET_BUDGET", Name = "预算管理", Category = "Assets", Description = "预算设定和追踪" },
                new() { Code = "ASSET_INVEST", Name = "投资管理", Category = "Assets", Description = "投资组合管理" },

                // 分析模块
                new() { Code = "ANALYTICS_GROWTH", Name = "成长分析", Category = "Analytics", Description = "个人成长数据分析" },
                new() { Code = "ANALYTICS_WORK", Name = "工作分析", Category = "Analytics", Description = "工作效率分析" },
                new() { Code = "ANALYTICS_FINANCE", Name = "财务分析", Category = "Analytics", Description = "财务数据分析" },

                // Persona专属
                new() { Code = "DEV_CODE_REPO", Name = "代码仓库", Category = "Persona", Description = "代码仓库管理" },
                new() { Code = "DEV_ISSUES", Name = "问题跟踪", Category = "Persona", Description = "Bug和任务跟踪" },
                new() { Code = "DEV_PIPELINES", Name = "流水线", Category = "Persona", Description = "CI/CD流水线" },
                new() { Code = "DESIGN_ASSETS", Name = "设计资产", Category = "Persona", Description = "设计资源管理" },
                new() { Code = "DESIGN_PROTOTYPE", Name = "原型管理", Category = "Persona", Description = "产品原型设计" },
                new() { Code = "TEACHER_COURSE", Name = "课程管理", Category = "Persona", Description = "在线课程管理" },
                new() { Code = "TEACHER_STUDENT", Name = "学生管理", Category = "Persona", Description = "学生信息管理" },
                new() { Code = "IMPL_KANBAN", Name = "项目看板", Category = "Persona", Description = "实施项目看板" },
                new() { Code = "IMPL_CUSTOMER", Name = "客户管理", Category = "Persona", Description = "客户信息管理" },
                new() { Code = "STUDENT_LEARNING", Name = "学习计划", Category = "Persona", Description = "学习计划制定" },
                new() { Code = "STUDENT_MISTAKES", Name = "错题本", Category = "Persona", Description = "错题记录和复习" },
                new() { Code = "STUDENT_EXAM", Name = "考研备考", Category = "Persona", Description = "考研备考管理" },
            };
            context.Features.AddRange(features);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] Features seeded.");
        }

        // Plans
        if (!await context.Plans.AnyAsync())
        {
            logger?.LogInformation("[DbSeeder] Seeding Plans...");
            var plans = new List<Plan>
            {
                new() { Code = "Free", Name = "免费版", Description = "基础功能，适合个人使用", Price = 0, BillingCycle = 0, Sort = 1 },
                new() { Code = "Pro", Name = "专业版", Description = "高级功能，适合专业用户", Price = 29, BillingCycle = 30, Sort = 2 },
                new() { Code = "Team", Name = "团队版", Description = "团队协作功能", Price = 99, BillingCycle = 30, Sort = 3 },
            };
            context.Plans.AddRange(plans);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] Plans seeded.");

            // PlanFeatures - Free plan features
            var allFeatures = await context.Features.ToListAsync();
            var planDict = await context.Plans.ToDictionaryAsync(p => p.Code);
            var featureDict = allFeatures.ToDictionary(f => f.Code);

            var planFeatures = new List<PlanFeature>();

            // Free: basic growth + work log
            var freeFeatures = new[] { "GROWTH_DAILY_PLAN", "GROWTH_HABIT", "WORK_LOG", "WORK_TASK" };
            foreach (var code in freeFeatures)
            {
                if (featureDict.TryGetValue(code, out var feature))
                {
                    planFeatures.Add(new PlanFeature { PlanId = planDict["Free"].Id, FeatureId = feature.Id });
                }
            }

            // Pro: all features
            foreach (var feature in allFeatures)
            {
                planFeatures.Add(new PlanFeature { PlanId = planDict["Pro"].Id, FeatureId = feature.Id });
            }

            // Team: all features (same as Pro for now)
            foreach (var feature in allFeatures)
            {
                planFeatures.Add(new PlanFeature { PlanId = planDict["Team"].Id, FeatureId = feature.Id });
            }

            context.PlanFeatures.AddRange(planFeatures);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] PlanFeatures seeded.");

            // PersonaFeatures
            var personaFeatures = new List<PersonaFeature>();
            var personaPrefixMap = new Dictionary<string, string>
            {
                ["Developer"] = "DEV_",
                ["Designer"] = "DESIGN_",
                ["Teacher"] = "TEACHER_",
                ["Implementation"] = "IMPL_",
                ["Student"] = "STUDENT_",
            };
            foreach (var (persona, prefix) in personaPrefixMap)
            {
                foreach (var feature in allFeatures.Where(f => f.Code.StartsWith(prefix)))
                {
                    personaFeatures.Add(new PersonaFeature { PersonaCode = persona, FeatureId = feature.Id });
                }
            }
            context.PersonaFeatures.AddRange(personaFeatures);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] PersonaFeatures seeded.");
        }

        logger?.LogInformation("[DbSeeder] Seed completed successfully!");
    }

    private static async Task SyncRoleMenusAsync(AppDbContext context, DateTime now)
    {
        var menusToAdd = new List<RoleMenu>();

        RoleMenu L(string name, string path, string? icon, string? comp, int sort, int minRoleLevel, bool isBase, string? personaTag, string category = "General", string? featureCode = null)
        {
            var m = new RoleMenu
            {
                Id = Guid.NewGuid(),
                Name = name,
                Path = path,
                Icon = icon,
                Component = comp,
                Sort = sort,
                MinRoleLevel = minRoleLevel,
                IsBaseMenu = isBase,
                PersonaTag = personaTag,
                MenuCategory = category,
                FeatureCode = featureCode,
                IsVisible = true,
                IsEnabled = true,
                CreatedAt = now
            };
            menusToAdd.Add(m);
            return m;
        }

        void C(RoleMenu parent, string name, string path, string? icon, string? comp, int sort, int minRoleLevel, bool isBase, string? personaTag, string? category = null, string? featureCode = null)
        {
            menusToAdd.Add(new RoleMenu
            {
                Id = Guid.NewGuid(),
                ParentId = parent.Id,
                Name = name,
                Path = path,
                Icon = icon,
                Component = comp,
                Sort = sort,
                MinRoleLevel = minRoleLevel,
                IsBaseMenu = isBase,
                PersonaTag = personaTag,
                MenuCategory = category ?? parent.MenuCategory,
                FeatureCode = featureCode,
                IsVisible = true,
                IsEnabled = true,
                CreatedAt = now
            });
        }

        // ===== 工作台 - 所有人可用 =====
        var dash = L("工作台", "/dashboard/workspace", "lucide:layout-dashboard", "/views/dashboard/workspace/index.vue", 0, 1, true, null, "Dashboard");

        // ===== 个人成长 - 默认首页 =====
        var growth = L("个人成长", "/growth", "lucide:sprout", null, 10, 1, true, null, "Growth");
        C(growth, "成长看板", "/growth/dashboard", "lucide:gauge", "/views/growth/dashboard/index.vue", 0, 1, true, null);
        C(growth, "每日计划", "/growth/daily-plans", "lucide:calendar-check", "/views/growth/daily-plans/index.vue", 1, 1, true, null, null, "GROWTH_DAILY_PLAN");
        C(growth, "习惯打卡", "/growth/habits", "lucide:badge-check", "/views/growth/habits/index.vue", 2, 1, true, null, null, "GROWTH_HABIT");
        C(growth, "知识库", "/growth/knowledge-base", "lucide:library", "/views/growth/knowledge-base/index.vue", 3, 1, true, null, null, "GROWTH_KNOWLEDGE");

        // ===== 工作中心 - 真实主线 =====
        var work = L("工作中心", "/work", "lucide:briefcase", null, 20, 1, true, null, "Work");
        C(work, "工作看板", "/work/dashboard", "lucide:layout-dashboard", "/views/work/dashboard/index.vue", 0, 1, true, null);
        C(work, "工作日志", "/work/work-log", "lucide:clipboard-list", "/views/work/log/index.vue", 1, 1, true, null, null, "WORK_LOG");
        C(work, "工作任务", "/work/tasks", "lucide:check-square", "/views/work/tasks/index.vue", 2, 1, true, null, null, "WORK_TASK");
        C(work, "工作项目", "/work/project", "lucide:folder-kanban", "/views/work/project/index.vue", 3, 2, false, null, null, "WORK_PROJECT");
        C(work, "设备管理", "/work/device", "lucide:monitor", "/views/work/device/index.vue", 4, 2, false, null, null, "WORK_DEVICE");
        C(work, "数据导入", "/work/import", "lucide:upload", "/views/work/import/index.vue", 5, 2, false, null, null, "WORK_IMPORT");
        C(work, "统计看板", "/work/statistics", "lucide:bar-chart-3", "/views/work/statistics/index.vue", 6, 2, false, null, null, "WORK_STATISTICS");

        // ===== AI 总结 - Pro =====
        var ai = L("AI 助手", "/ai", "lucide:brain", null, 30, 2, true, null, "AI");
        C(ai, "AI 助手", "/ai/assistant", "lucide:message-square", "/views/ai/assistant/index.vue", 0, 2, true, null, null, "AI_ASSISTANT");
        C(ai, "AI 规划器", "/ai/planner", "lucide:sparkles", "/views/ai/planner/index.vue", 1, 2, true, null, null, "AI_PLANNER");

        // ===== Persona 身份菜单 =====
        var developer = L("开发中心", "/dev", "lucide:code-2", null, 40, 1, false, "Developer", "Persona");
        C(developer, "代码仓库", "/dev/code-repository", "lucide:git-branch", "/views/dev/code-repository/index.vue", 0, 1, false, "Developer", null, "DEV_CODE_REPO");
        C(developer, "问题跟踪", "/dev/issues", "lucide:bug", "/views/dev/issues/index.vue", 1, 1, false, "Developer", null, "DEV_ISSUES");
        C(developer, "流水线", "/dev/pipelines", "lucide:workflow", "/views/dev/pipelines/index.vue", 2, 1, false, "Developer", null, "DEV_PIPELINES");

        var implementation = L("实施中心", "/implementation", "lucide:wrench", null, 41, 1, false, "Implementation", "Persona");
        C(implementation, "项目看板", "/implementation/kanban", "lucide:kanban", "/views/implementation/kanban/index.vue", 0, 1, false, "Implementation", null, "IMPL_KANBAN");
        C(implementation, "客户管理", "/implementation/customers", "lucide:contact", "/views/implementation/customers/index.vue", 1, 1, false, "Implementation", null, "IMPL_CUSTOMER");
        C(implementation, "实施任务", "/implementation/tasks", "lucide:check-square", "/views/implementation/tasks/index.vue", 2, 1, false, "Implementation", null, "WORK_TASK");
        C(implementation, "实施日志", "/implementation/impl-log", "lucide:clipboard-list", "/views/implementation/impl-log/index.vue", 3, 1, false, "Implementation", null, "WORK_LOG");
        C(implementation, "周计划", "/implementation/weekly-plan", "lucide:calendar-range", "/views/implementation/weekly-plan/index.vue", 4, 1, false, "Implementation", null, "WORK_TASK");
        C(implementation, "实施周报", "/implementation/weekly-report", "lucide:file-text", "/views/implementation/weekly-report/index.vue", 5, 2, false, "Implementation", null, "AI_REPORT");

        var designer = L("设计中心", "/design", "lucide:palette", null, 42, 1, false, "Designer", "Persona");
        C(designer, "设计资产", "/design/assets", "lucide:images", "/views/design/assets/index.vue", 0, 1, false, "Designer", null, "DESIGN_ASSETS");
        C(designer, "原型管理", "/design/prototypes", "lucide:panels-top-left", "/views/design/prototypes/index.vue", 1, 1, false, "Designer", null, "DESIGN_PROTOTYPE");

        var teacher = L("教学中心", "/teacher", "lucide:graduation-cap", null, 43, 1, false, "Teacher", "Persona");
        C(teacher, "课程管理", "/teacher/courses", "lucide:book-open", "/views/teacher/courses/index.vue", 0, 1, false, "Teacher", null, "TEACHER_COURSE");
        C(teacher, "学生管理", "/teacher/students", "lucide:users", "/views/teacher/students/index.vue", 1, 1, false, "Teacher", null, "TEACHER_STUDENT");

        var student = L("学习中心", "/student", "lucide:book-marked", null, 44, 1, false, "Student", "Persona");
        C(student, "学习计划", "/student/learning", "lucide:book-open-check", "/views/student/learning/index.vue", 0, 1, false, "Student", null, "STUDENT_LEARNING");
        C(student, "错题本", "/student/mistakes", "lucide:notebook-pen", "/views/student/mistakes/index.vue", 1, 1, false, "Student", null, "STUDENT_MISTAKES");
        C(student, "考研备考", "/student/postgraduate", "lucide:school", "/views/student/postgraduate/index.vue", 2, 1, false, "Student", null, "STUDENT_EXAM");

        // ===== 平台管理 =====
        var platform = L("平台管理", "/system", "lucide:settings", null, 90, 2, true, null, "System");
        C(platform, "用户管理", "/system/user", "lucide:users", "/views/system/user/index.vue", 0, 2, true, null);
        C(platform, "角色菜单", "/system/role-menu", "lucide:shield", "/views/system/role-menu/index.vue", 1, 3, false, null, "System");
        C(platform, "身份类型", "/system/persona", "lucide:user-check", "/views/system/persona/index.vue", 2, 3, false, null, "System");

        var desiredByPath = menusToAdd
            .GroupBy(x => x.Path, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .ToDictionary(x => x.Path, StringComparer.OrdinalIgnoreCase);

        var existingMenus = await context.RoleMenus.ToListAsync();
        var existingByPath = existingMenus
            .GroupBy(x => x.Path, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        var finalIdsByPath = desiredByPath.ToDictionary(
            x => x.Key,
            x => existingByPath.TryGetValue(x.Key, out var existing) ? existing.Id : x.Value.Id,
            StringComparer.OrdinalIgnoreCase);
        var desiredPathByInitialId = desiredByPath.Values.ToDictionary(x => x.Id, x => x.Path);

        foreach (var desired in desiredByPath.Values)
        {
            if (existingByPath.TryGetValue(desired.Path, out var existing))
            {
                desired.Id = existing.Id;
            }

            desired.ParentId = desired.ParentId.HasValue
                ? ResolveFinalParentId(desired.ParentId.Value, desiredPathByInitialId, finalIdsByPath)
                : null;
        }

        var finalIds = finalIdsByPath.Values.ToHashSet();
        foreach (var desired in desiredByPath.Values)
        {
            if (desired.ParentId.HasValue && !finalIds.Contains(desired.ParentId.Value))
            {
                desired.ParentId = null;
            }
        }

        var newMenuParentIds = new Dictionary<Guid, Guid?>();
        foreach (var desired in desiredByPath.Values)
        {
            if (existingByPath.TryGetValue(desired.Path, out var existing))
            {
                continue;
            }

            newMenuParentIds[desired.Id] = desired.ParentId;
            desired.ParentId = null;
            context.RoleMenus.Add(desired);
        }

        if (newMenuParentIds.Count > 0)
        {
            await context.SaveChangesAsync();
        }

        foreach (var desired in desiredByPath.Values)
        {
            if (newMenuParentIds.TryGetValue(desired.Id, out var parentId))
            {
                desired.ParentId = parentId;
            }

            if (existingByPath.TryGetValue(desired.Path, out var existing))
            {
                ApplyRoleMenu(existing, desired, resetParent: true);
            }
        }

        await context.SaveChangesAsync();

        foreach (var desired in desiredByPath.Values)
        {
            if (existingByPath.TryGetValue(desired.Path, out var existing))
            {
                existing.ParentId = desired.ParentId;
            }
            else
            {
                desired.ParentId = desired.ParentId;
            }
        }

        var desiredPaths = desiredByPath.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var stale in existingMenus.Where(x => !desiredPaths.Contains(x.Path)))
        {
            stale.IsVisible = false;
            stale.IsEnabled = false;
        }

        await context.SaveChangesAsync();
    }

    private static Guid? ResolveFinalParentId(
        Guid desiredParentId,
        Dictionary<Guid, string> desiredPathByInitialId,
        Dictionary<string, Guid> finalIdsByPath)
    {
        if (!desiredPathByInitialId.TryGetValue(desiredParentId, out var parentPath))
        {
            return desiredParentId;
        }

        return finalIdsByPath.TryGetValue(parentPath, out var finalId)
            ? finalId
            : desiredParentId;
    }

    private static void ApplyRoleMenu(RoleMenu target, RoleMenu source, bool resetParent = false)
    {
        target.ParentId = resetParent ? null : source.ParentId;
        target.Name = source.Name;
        target.Icon = source.Icon;
        target.Component = source.Component;
        target.Sort = source.Sort;
        target.MinRoleLevel = source.MinRoleLevel;
        target.IsBaseMenu = source.IsBaseMenu;
        target.PersonaTag = source.PersonaTag;
        target.MenuCategory = source.MenuCategory;
        target.FeatureCode = source.FeatureCode;
        target.IsVisible = source.IsVisible;
        target.IsEnabled = source.IsEnabled;
    }

    private static string GetWorkLogTitle(string taskTypeCode, string projectCode)
    {
        return taskTypeCode switch
        {
            "DEVICE_DEBUG" => $"{projectCode}设备调试完成",
            "TROUBLESHOOT" => $"{projectCode}问题处理",
            "MAINTENANCE" => $"{projectCode}日常维护",
            "PRODUCTION" => $"{projectCode}生产作业",
            "QC" => $"{projectCode}质量检测",
            "DATA_ENTRY" => $"数据记录与分析",
            "TRAINING" => "培训学习记录",
            "MEETING" => "项目会议讨论",
            _ => "其他工作"
        };
    }

    private static string GetWorkLogContent(string taskTypeCode)
    {
        return taskTypeCode switch
        {
            "DEVICE_DEBUG" => "1. 设备参数调整\n2. 性能测试验证\n3. 文档更新",
            "TROUBLESHOOT" => "1. 问题分析\n2. 原因排查\n3. 解决方案实施",
            "MAINTENANCE" => "1. 设备点检\n2. 保养记录\n3. 异常确认",
            "PRODUCTION" => "1. 按照作业指导书操作\n2. 产品质检\n3. 产量统计",
            "QC" => "1. 取样检测\n2. 数据记录\n3. 异常反馈",
            "DATA_ENTRY" => "1. 数据采集\n2. 报表填写\n3. 系统录入",
            "TRAINING" => "1. 课程学习\n2. 笔记整理\n3. 考核完成",
            "MEETING" => "1. 会议参与\n2. 议题讨论\n3. 纪要整理",
            _ => "完成当日工作任务"
        };
    }

    private static string GetWorkLogSummary(string taskTypeCode)
    {
        return taskTypeCode switch
        {
            "DEVICE_DEBUG" => "设备调试完成，性能达标",
            "TROUBLESHOOT" => "问题已解决，恢复正常",
            "MAINTENANCE" => "日常维护完成，无异常",
            "PRODUCTION" => "生产任务完成",
            "QC" => "检测合格，数据正常",
            "DATA_ENTRY" => "数据已录入系统",
            "TRAINING" => "培训完成，收获颇丰",
            "MEETING" => "会议完成，达成共识",
            _ => "工作完成"
        };
    }

    private static string GetWorkItemContent(string taskTypeCode)
    {
        return taskTypeCode switch
        {
            "DEVICE_DEBUG" => "完成设备调试和参数优化",
            "TROUBLESHOOT" => "完成问题分析和处理",
            "MAINTENANCE" => "完成设备日常维护",
            "PRODUCTION" => "完成生产作业任务",
            "QC" => "完成质量检测工作",
            _ => "完成任务"
        };
    }
}
