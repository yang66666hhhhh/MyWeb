using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Features.Auth.Entities.Subscription;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Growth.Entities;
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
                new() { Id = Guid.NewGuid(), Path = "/student/dashboard", Name = "学习总览", Icon = "ant-design:dashboard", Sort = 21, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/student/review", Name = "复习日程", Icon = "ant-design:clock-circle", Sort = 22, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/student/mistakes", Name = "错题本", Icon = "ant-design:warning", Sort = 23, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/student/materials", Name = "学习资料", Icon = "ant-design:database", Sort = 24, IsActive = true, CreatedAt = now },
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
                new() { Id = Guid.NewGuid(), Name = "学习总览", Path = "/student/dashboard", Icon = "ant-design:dashboard", Sort = 21, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "复习日程", Path = "/student/review", Icon = "ant-design:clock-circle", Sort = 22, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "错题本", Path = "/student/mistakes", Icon = "ant-design:warning", Sort = 23, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "学习资料", Path = "/student/materials", Icon = "ant-design:database", Sort = 24, IsActive = true, CreatedAt = now },
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
                new() { MenuItemId = menuItemDict["/student/dashboard"].Id, TagId = tagDict["考研学习"].Id },
                new() { MenuItemId = menuItemDict["/student/review"].Id, TagId = tagDict["考研学习"].Id },
                new() { MenuItemId = menuItemDict["/student/mistakes"].Id, TagId = tagDict["考研学习"].Id },
                new() { MenuItemId = menuItemDict["/student/materials"].Id, TagId = tagDict["考研学习"].Id },
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

        var lisaSeedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "lisa");
        if (lisaSeedUser is not null && !await context.StudentSubjects.AnyAsync(x => x.UserId == lisaSeedUser.Id))
        {
            logger?.LogInformation("[DbSeeder] Seeding Lisa student subjects...");
            var subjects = new List<StudentSubject>
            {
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Name = "数据结构", Description = "算法与数据结构核心专题", Color = "blue", TargetHours = 120, Sort = 0, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Name = "操作系统", Description = "进程、内存、文件系统与 I/O", Color = "cyan", TargetHours = 100, Sort = 1, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Name = "计算机网络", Description = "协议栈、路由、传输层与应用层", Color = "green", TargetHours = 90, Sort = 2, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Name = "数学", Description = "高数、线代与概率统计", Color = "purple", TargetHours = 180, Sort = 3, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Name = "英语", Description = "阅读、翻译、写作与词汇", Color = "orange", TargetHours = 120, Sort = 4, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Name = "政治", Description = "选择题、分析题与时政复习", Color = "red", TargetHours = 80, Sort = 5, IsActive = true, CreatedAt = now },
            };
            context.StudentSubjects.AddRange(subjects);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] Lisa student subjects seeded.");
        }

        if (lisaSeedUser is not null && !await context.PostgraduateTasks.AnyAsync(x => x.UserId == lisaSeedUser.Id))
        {
            logger?.LogInformation("[DbSeeder] Seeding Lisa student tasks...");
            var tasks = new List<PostgraduateTask>
            {
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "完成数据结构树与图专题", Description = "刷完图遍历、最短路径和最小生成树核心题，整理易错模板。", DueDate = today, Status = PostgraduateTaskStatus.InProgress, Priority = PostgraduateTaskPriority.Urgent, Type = PostgraduateTaskType.Practice, CreatedAt = now.AddHours(-8) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "英语阅读精读 2 篇", Description = "记录长难句、替换表达和错题原因。", DueDate = today, Status = PostgraduateTaskStatus.Pending, Priority = PostgraduateTaskPriority.High, Type = PostgraduateTaskType.Study, CreatedAt = now.AddHours(-7) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "数学线代矩阵相似复盘", Description = "复习特征值、相似对角化和二次型判定。", DueDate = today.AddDays(1), Status = PostgraduateTaskStatus.Pending, Priority = PostgraduateTaskPriority.High, Type = PostgraduateTaskType.Review, CreatedAt = now.AddDays(-1) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "政治马原选择题 50 题", Description = "重点看错选项背后的概念混淆。", DueDate = today.AddDays(-1), Status = PostgraduateTaskStatus.Overdue, Priority = PostgraduateTaskPriority.Medium, Type = PostgraduateTaskType.Practice, CreatedAt = now.AddDays(-3) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "计算机网络 TCP 专题", Description = "梳理握手、挥手、拥塞控制和可靠传输。", DueDate = today.AddDays(2), Status = PostgraduateTaskStatus.Pending, Priority = PostgraduateTaskPriority.Medium, Type = PostgraduateTaskType.Study, CreatedAt = now.AddDays(-2) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "操作系统进程调度小测", Description = "完成一套调度算法练习并核对平均周转时间。", DueDate = today.AddDays(-2), Status = PostgraduateTaskStatus.Completed, Priority = PostgraduateTaskPriority.Medium, Type = PostgraduateTaskType.Mock, CreatedAt = now.AddDays(-4), UpdatedAt = now.AddDays(-2) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "本周错题二次复盘", Description = "把仍然卡住的错题加入下周复习清单。", DueDate = today.AddDays(3), Status = PostgraduateTaskStatus.Pending, Priority = PostgraduateTaskPriority.Low, Type = PostgraduateTaskType.Review, CreatedAt = now.AddDays(-1) },
            };
            context.PostgraduateTasks.AddRange(tasks);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] Lisa student tasks seeded.");
        }

        if (lisaSeedUser is not null && !await context.ExamMistakes.AnyAsync(x => x.UserId == lisaSeedUser.Id))
        {
            logger?.LogInformation("[DbSeeder] Seeding Lisa exam mistakes...");
            var mistakes = new List<ExamMistake>
            {
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "数据结构", Question = "Dijkstra 算法在存在负权边时为什么不适用？", Answer = "负权边会破坏已确定最短距离不再变小的贪心前提。", Explanation = "可用反例验证：已出队节点仍可能被负权边松弛。", Tags = "图论,最短路径,贪心", ReviewCount = 1, LastReviewDate = today.AddDays(-3), NextReviewDate = today, Status = ExamMistakeStatus.Reviewed, CreatedAt = now.AddDays(-8) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "数学", Question = "矩阵可对角化的充分必要条件如何判断？", Answer = "n 阶矩阵有 n 个线性无关特征向量。", Explanation = "不要只看特征值个数，还要比较几何重数与代数重数。", Tags = "线代,特征值,对角化", ReviewCount = 0, NextReviewDate = today.AddDays(-1), Status = ExamMistakeStatus.Pending, CreatedAt = now.AddDays(-6) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "英语", Question = "长难句中插入语导致主干识别错误。", Answer = "先划主谓宾，再回填修饰成分。", Explanation = "遇到逗号、破折号、同位语时先临时跳过。", Tags = "阅读,长难句", ReviewCount = 2, LastReviewDate = today.AddDays(-1), NextReviewDate = today.AddDays(2), Status = ExamMistakeStatus.Reviewed, CreatedAt = now.AddDays(-10) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "操作系统", Question = "银行家算法安全序列判断漏掉 Available 回收。", Answer = "每完成一个进程，都要把 Allocation 加回 Available。", Explanation = "按 Need <= Available 逐步模拟，不能只比较初始资源。", Tags = "死锁,银行家算法", ReviewCount = 1, LastReviewDate = today.AddDays(-4), NextReviewDate = today, Status = ExamMistakeStatus.Pending, CreatedAt = now.AddDays(-9) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "计算机网络", Question = "TCP 拥塞控制和流量控制概念混淆。", Answer = "拥塞控制面向网络，流量控制面向接收端。", Explanation = "分别对应拥塞窗口 cwnd 和接收窗口 rwnd。", Tags = "TCP,拥塞控制,流量控制", ReviewCount = 3, LastReviewDate = today.AddDays(-2), NextReviewDate = today.AddDays(5), Status = ExamMistakeStatus.Mastered, CreatedAt = now.AddDays(-14), UpdatedAt = now.AddDays(-2) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "政治", Question = "实践和认识的辩证关系表述不完整。", Answer = "实践决定认识，认识反作用于实践。", Explanation = "分析题要写出来源、动力、目的、检验标准四个角度。", Tags = "马原,认识论", ReviewCount = 0, NextReviewDate = today.AddDays(1), Status = ExamMistakeStatus.Pending, CreatedAt = now.AddDays(-5) },
            };
            context.ExamMistakes.AddRange(mistakes);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] Lisa exam mistakes seeded.");
        }

        if (lisaSeedUser is not null && !await context.ExamMaterials.AnyAsync(x => x.UserId == lisaSeedUser.Id))
        {
            logger?.LogInformation("[DbSeeder] Seeding Lisa exam materials...");
            var materials = new List<ExamMaterial>
            {
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "数据结构图论模板", Subject = "数据结构", Type = ExamMaterialType.Template, Tags = "图论,模板,邻接表", Content = "DFS/BFS、拓扑排序、Dijkstra、Floyd 的适用条件和复杂度对照。", CreatedAt = now.AddDays(-12) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "线代相似对角化总结", Subject = "数学", Type = ExamMaterialType.Summary, Tags = "线代,矩阵,二次型", Content = "按特征值、特征向量、相似对角化、正交变换四块整理。", CreatedAt = now.AddDays(-9) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "英语阅读替换表达", Subject = "英语", Type = ExamMaterialType.Note, Tags = "阅读,词汇,写作", Content = "记录真题中常见同义替换，按态度词、逻辑词、动作词分类。", CreatedAt = now.AddDays(-7) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "操作系统调度公式卡片", Subject = "操作系统", Type = ExamMaterialType.Formula, Tags = "调度,周转时间,等待时间", Content = "周转时间=完成时间-到达时间；带权周转时间=周转时间/服务时间。", CreatedAt = now.AddDays(-6) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "TCP 重点问答", Subject = "计算机网络", Type = ExamMaterialType.Summary, Tags = "TCP,可靠传输,拥塞控制", Content = "三次握手、四次挥手、滑动窗口、超时重传、快重传快恢复。", CreatedAt = now.AddDays(-4) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Title = "政治马原分析题框架", Subject = "政治", Type = ExamMaterialType.Template, Tags = "马原,分析题", Content = "概念解释、原理展开、材料扣题、方法论落点四步答题。", CreatedAt = now.AddDays(-3) },
            };
            context.ExamMaterials.AddRange(materials);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] Lisa exam materials seeded.");
        }

        if (lisaSeedUser is not null && !await context.StudentStudyRecords.AnyAsync(x => x.UserId == lisaSeedUser.Id))
        {
            logger?.LogInformation("[DbSeeder] Seeding Lisa study records...");
            var records = new List<StudentStudyRecord>
            {
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "数据结构", Summary = "完成图遍历与最短路径专题练习", RecordDate = today, DurationMinutes = 95, TaskTitle = "完成数据结构树与图专题", Remark = "Dijkstra 负权边条件还需要二刷。", CreatedAt = now.AddHours(-3) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "英语", Summary = "精读阅读真题并整理长难句", RecordDate = today, DurationMinutes = 70, TaskTitle = "英语阅读精读 2 篇", Remark = "主干识别速度比上周好。", CreatedAt = now.AddHours(-2) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "数学", Summary = "复习矩阵相似与二次型", RecordDate = today.AddDays(-1), DurationMinutes = 120, TaskTitle = "数学线代矩阵相似复盘", Remark = "相似对角化判断仍需加强。", CreatedAt = now.AddDays(-1).AddHours(2) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "操作系统", Summary = "完成进程调度算法小测", RecordDate = today.AddDays(-2), DurationMinutes = 80, TaskTitle = "操作系统进程调度小测", Remark = "平均等待时间计算已掌握。", CreatedAt = now.AddDays(-2).AddHours(1) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "计算机网络", Summary = "梳理 TCP 可靠传输与拥塞控制", RecordDate = today.AddDays(-3), DurationMinutes = 90, TaskTitle = "计算机网络 TCP 专题", Remark = "cwnd 与 rwnd 对比清楚了。", CreatedAt = now.AddDays(-3).AddHours(2) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "政治", Summary = "马原认识论选择题复盘", RecordDate = today.AddDays(-4), DurationMinutes = 55, TaskTitle = "政治马原选择题 50 题", Remark = "分析题框架需要背熟。", CreatedAt = now.AddDays(-4).AddHours(3) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "数据结构", Summary = "二叉树遍历和线索二叉树", RecordDate = today.AddDays(-5), DurationMinutes = 75, TaskTitle = "数据结构基础回顾", Remark = "递归转非递归还要继续练。", CreatedAt = now.AddDays(-5).AddHours(2) },
                new() { Id = Guid.NewGuid(), UserId = lisaSeedUser.Id, Subject = "英语", Summary = "作文功能句积累", RecordDate = today.AddDays(-6), DurationMinutes = 45, TaskTitle = "英语作文素材整理", Remark = "整理了图表作文开头句。", CreatedAt = now.AddDays(-6).AddHours(1) },
            };
            context.StudentStudyRecords.AddRange(records);
            await context.SaveChangesAsync();
            logger?.LogInformation("[DbSeeder] Lisa study records seeded.");
        }

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
        var vbenDailyPlanUser = await context.Users.FirstAsync(u => u.Username == "vben");
        if (!await context.DailyPlans.AnyAsync(x => x.UserId == vbenDailyPlanUser.Id))
        {
            logger?.LogInformation("[DbSeeder] Seeding DailyPlans...");
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
                    UserId = vbenDailyPlanUser.Id,
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
                    UserId = vbenDailyPlanUser.Id,
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
                    UserId = vbenDailyPlanUser.Id,
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

        await SeedUserExperienceDataAsync(context, logger, now, today);

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
                new() { Code = "STUDENT_REVIEW", Name = "复习日程", Category = "Persona", Description = "到期复习安排" },
                new() { Code = "STUDENT_MATERIALS", Name = "学习资料", Category = "Persona", Description = "学习资料管理" },
                new() { Code = "STUDENT_RECORDS", Name = "学习记录", Category = "Persona", Description = "学习过程记录" },
                new() { Code = "STUDENT_SUBJECTS", Name = "科目目标", Category = "Persona", Description = "科目目标配置" },
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
            var freeFeatures = DefaultFreeFeatureCodes;
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

        await EnsureFeatureDefinitionsAsync(context, DefaultFreeFeatureDefinitions, logger);
        await EnsurePlanFeaturesAsync(context, "Free", DefaultFreeFeatureCodes, logger);
        await EnsurePersonaFeaturesAsync(context, "Student", "STUDENT_", logger);

        logger?.LogInformation("[DbSeeder] Seed completed successfully!");
    }

    private static readonly string[] DefaultFreeFeatureCodes =
    [
        "GROWTH_DAILY_PLAN",
        "GROWTH_HABIT",
        "GROWTH_KNOWLEDGE",
        "WORK_LOG",
        "WORK_TASK",
        "STUDENT_EXAM",
        "STUDENT_LEARNING",
        "STUDENT_MISTAKES",
        "STUDENT_REVIEW",
        "STUDENT_MATERIALS",
        "STUDENT_RECORDS",
        "STUDENT_SUBJECTS",
    ];

    private static readonly (string Code, string Name, string Category, string Description)[] DefaultFreeFeatureDefinitions =
    [
        ("GROWTH_DAILY_PLAN", "每日计划", "Growth", "每日计划管理"),
        ("GROWTH_HABIT", "习惯打卡", "Growth", "习惯养成和打卡"),
        ("GROWTH_KNOWLEDGE", "知识库", "Growth", "知识文章管理"),
        ("WORK_LOG", "工作日志", "Work", "工作日志管理"),
        ("WORK_TASK", "工作任务", "Work", "工作任务管理"),
        ("STUDENT_EXAM", "考研备考", "Persona", "考研备考管理"),
        ("STUDENT_LEARNING", "学习计划", "Persona", "学习计划制定"),
        ("STUDENT_MISTAKES", "错题本", "Persona", "错题记录和复习"),
        ("STUDENT_REVIEW", "复习日程", "Persona", "到期复习安排"),
        ("STUDENT_MATERIALS", "学习资料", "Persona", "学习资料管理"),
        ("STUDENT_RECORDS", "学习记录", "Persona", "学习过程记录"),
        ("STUDENT_SUBJECTS", "科目目标", "Persona", "科目目标配置"),
    ];

    private static async Task EnsureFeatureDefinitionsAsync(
        AppDbContext context,
        IEnumerable<(string Code, string Name, string Category, string Description)> definitions,
        ILogger? logger)
    {
        var existingCodes = await context.Features
            .Select(x => x.Code)
            .ToListAsync();

        var existing = existingCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = definitions
            .Where(x => !existing.Contains(x.Code))
            .Select(x => new Feature
            {
                Code = x.Code,
                Name = x.Name,
                Category = x.Category,
                Description = x.Description,
                IsEnabled = true
            })
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        context.Features.AddRange(missing);
        await context.SaveChangesAsync();
        logger?.LogInformation("[DbSeeder] Added {Count} missing feature definitions.", missing.Count);
    }

    private static async Task EnsurePlanFeaturesAsync(
        AppDbContext context,
        string planCode,
        IEnumerable<string> featureCodes,
        ILogger? logger)
    {
        var plan = await context.Plans.FirstOrDefaultAsync(x => x.Code == planCode);
        if (plan is null)
        {
            return;
        }

        var featureCodeSet = featureCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var features = (await context.Features.ToListAsync())
            .Where(x => featureCodeSet.Contains(x.Code))
            .ToList();

        var featureIds = features.Select(x => x.Id).ToHashSet();
        var existingFeatureIds = await context.PlanFeatures
            .Where(x => x.PlanId == plan.Id)
            .Select(x => x.FeatureId)
            .ToListAsync();

        var existing = existingFeatureIds
            .Where(featureIds.Contains)
            .ToHashSet();
        var missing = features
            .Where(x => !existing.Contains(x.Id))
            .Select(x => new PlanFeature { PlanId = plan.Id, FeatureId = x.Id })
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        context.PlanFeatures.AddRange(missing);
        await context.SaveChangesAsync();
        logger?.LogInformation("[DbSeeder] Added {Count} missing features to {PlanCode} plan.", missing.Count, planCode);
    }

    private static async Task EnsurePersonaFeaturesAsync(
        AppDbContext context,
        string personaCode,
        string featurePrefix,
        ILogger? logger)
    {
        var features = (await context.Features.ToListAsync())
            .Where(x => x.Code.StartsWith(featurePrefix, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var featureIds = features.Select(x => x.Id).ToHashSet();
        var existingFeatureIds = await context.PersonaFeatures
            .Where(x => x.PersonaCode == personaCode)
            .Select(x => x.FeatureId)
            .ToListAsync();

        var existing = existingFeatureIds
            .Where(featureIds.Contains)
            .ToHashSet();
        var missing = features
            .Where(x => !existing.Contains(x.Id))
            .Select(x => new PersonaFeature { PersonaCode = personaCode, FeatureId = x.Id })
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        context.PersonaFeatures.AddRange(missing);
        await context.SaveChangesAsync();
        logger?.LogInformation("[DbSeeder] Added {Count} missing persona features for {PersonaCode}.", missing.Count, personaCode);
    }

    private static async Task SeedUserExperienceDataAsync(AppDbContext context, ILogger? logger, DateTime now, DateOnly today)
    {
        var users = await context.Users
            .Where(u => u.Username == "admin" || u.Username == "jack" || u.Username == "lisa" || u.Username == "tom")
            .ToDictionaryAsync(u => u.Username);

        foreach (var user in users.Values)
        {
            if (!await context.DailyPlans.AnyAsync(x => x.UserId == user.Id))
            {
                logger?.LogInformation("[DbSeeder] Seeding daily plans for {Username}...", user.Username);
                context.DailyPlans.AddRange(CreateUserDailyPlans(user, now, today));
                await context.SaveChangesAsync();
            }

            if (!await context.Tasks.AnyAsync(x => x.UserId == user.Id))
            {
                logger?.LogInformation("[DbSeeder] Seeding tasks for {Username}...", user.Username);
                context.Tasks.AddRange(CreateUserTasks(user, now, today));
                await context.SaveChangesAsync();
            }

            if (!await context.Habits.AnyAsync(x => x.UserId == user.Id))
            {
                logger?.LogInformation("[DbSeeder] Seeding habits for {Username}...", user.Username);
                var habits = CreateUserHabits(user, now, today);
                context.Habits.AddRange(habits);
                await context.SaveChangesAsync();

                context.HabitCheckIns.AddRange(CreateHabitCheckIns(habits, today));
                await context.SaveChangesAsync();
            }

            if (!await context.GrowthProjects.AnyAsync(x => x.UserId == user.Id))
            {
                logger?.LogInformation("[DbSeeder] Seeding growth projects for {Username}...", user.Username);
                context.GrowthProjects.AddRange(CreateUserGrowthProjects(user, now, today));
                await context.SaveChangesAsync();
            }

            if (!await context.KnowledgeArticles.AnyAsync(x => x.UserId == user.Id))
            {
                logger?.LogInformation("[DbSeeder] Seeding knowledge articles for {Username}...", user.Username);
                context.KnowledgeArticles.AddRange(CreateUserKnowledgeArticles(user, now));
                await context.SaveChangesAsync();
            }
        }

        if (users.TryGetValue("jack", out var jack))
        {
            var project = await context.WorkProjects.OrderBy(x => x.Sort).FirstOrDefaultAsync();
            if (project is not null && !await context.WorkDailyPlans.AnyAsync(x => x.UserId == jack.Id))
            {
                logger?.LogInformation("[DbSeeder] Seeding work daily plans for jack...");
                context.WorkDailyPlans.AddRange(CreateJackWorkDailyPlans(jack, project.Id, now, today));
                await context.SaveChangesAsync();
            }

            if (project is not null && !await context.WorkLogs.AnyAsync(x => x.UserId == jack.Id))
            {
                logger?.LogInformation("[DbSeeder] Seeding work logs for jack...");
                context.WorkLogs.AddRange(CreateJackWorkLogs(jack, project.Id, now, today));
                await context.SaveChangesAsync();
            }
        }
    }

    private static IEnumerable<DailyPlan> CreateUserDailyPlans(AppUser user, DateTime now, DateOnly today)
    {
        return user.Username switch
        {
            "admin" =>
            [
                NewDailyPlan(user.Id, today, "检查平台菜单权限", "确认角色菜单、功能订阅和身份菜单显示一致。", 4, DailyPlanStatus.InProgress, "优先处理普通用户看不到数据的问题。", now.AddHours(-6)),
                NewDailyPlan(user.Id, today, "整理演示账号体验清单", "给每个种子账号补齐首屏可见数据。", 3, DailyPlanStatus.Pending, "避免新账号登录后页面空白。", now.AddHours(-4)),
                NewDailyPlan(user.Id, today.AddDays(1), "复盘订阅功能边界", "核对 Free、Pro、Team 的功能权限。", 2, DailyPlanStatus.Pending, null, now.AddHours(-2)),
                NewDailyPlan(user.Id, today.AddDays(-1), "完成运营指标检查", "检查用户数、菜单数和近期任务数据。", 3, DailyPlanStatus.Completed, "数据正常。", now.AddDays(-1), now.AddDays(-1).AddHours(5)),
            ],
            "jack" =>
            [
                NewDailyPlan(user.Id, today, "完成 DailyPlans 用户隔离", "实体、接口、服务和种子数据都按 UserId 处理。", 4, DailyPlanStatus.InProgress, "重点补迁移和测试。", now.AddHours(-5)),
                NewDailyPlan(user.Id, today, "跑后端测试", "确认 DailyPlanService 和 DbSeeder 编译通过。", 3, DailyPlanStatus.Pending, null, now.AddHours(-3)),
                NewDailyPlan(user.Id, today.AddDays(1), "补一轮前端回归", "检查每日计划列表是否只显示当前用户数据。", 2, DailyPlanStatus.Pending, null, now.AddHours(-1)),
                NewDailyPlan(user.Id, today.AddDays(-1), "完成学生中心种子数据", "Lisa 账号学习中心已经有任务、错题、资料和记录。", 3, DailyPlanStatus.Completed, "已验证。", now.AddDays(-1), now.AddDays(-1).AddHours(4)),
            ],
            "lisa" =>
            [
                NewDailyPlan(user.Id, today, "完成英语阅读精读", "精读 2 篇真题阅读并整理长难句。", 3, DailyPlanStatus.Pending, "和学生中心任务保持一致。", now.AddHours(-4)),
                NewDailyPlan(user.Id, today, "复习数据结构图论错题", "重点看 Dijkstra 和拓扑排序。", 4, DailyPlanStatus.InProgress, "今天必须复盘。", now.AddHours(-3)),
                NewDailyPlan(user.Id, today.AddDays(1), "线代相似对角化练习", "做 10 道判断题并记录错因。", 3, DailyPlanStatus.Pending, null, now.AddHours(-1)),
                NewDailyPlan(user.Id, today.AddDays(-1), "完成操作系统调度小测", "核对平均等待时间和周转时间。", 3, DailyPlanStatus.Completed, "计算公式已掌握。", now.AddDays(-1), now.AddDays(-1).AddHours(3)),
            ],
            "tom" =>
            [
                NewDailyPlan(user.Id, today, "整理今晚待办", "保留三个最重要的小任务。", 2, DailyPlanStatus.Pending, null, now.AddHours(-3)),
                NewDailyPlan(user.Id, today, "完成轻量运动", "散步或拉伸 30 分钟。", 2, DailyPlanStatus.InProgress, "保持节奏。", now.AddHours(-2)),
                NewDailyPlan(user.Id, today.AddDays(1), "周末学习安排", "预留两个学习时间块。", 3, DailyPlanStatus.Pending, null, now.AddHours(-1)),
                NewDailyPlan(user.Id, today.AddDays(-1), "检查个人资料", "确认头像、身份和标签配置。", 1, DailyPlanStatus.Completed, "已确认。", now.AddDays(-1), now.AddDays(-1).AddHours(2)),
            ],
            _ => []
        };
    }

    private static DailyPlan NewDailyPlan(
        Guid userId,
        DateOnly planDate,
        string title,
        string? description,
        int priority,
        DailyPlanStatus status,
        string? remark,
        DateTime createdAt,
        DateTime? completedAt = null)
    {
        return new DailyPlan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanDate = planDate,
            Title = title,
            Description = description,
            Priority = priority,
            Status = status,
            Remark = remark,
            CompletedAt = completedAt,
            CreatedAt = createdAt
        };
    }

    private static IEnumerable<TaskItem> CreateUserTasks(AppUser user, DateTime now, DateOnly today)
    {
        return user.Username switch
        {
            "admin" =>
            [
                NewTask(user.Id, today, "复核本周功能权限配置", "确认专业版菜单、功能点和订阅状态展示一致。", TaskPriority.High, TaskType.Work, TaskSource.Work, TaskItemStatus.InProgress, now.AddHours(-6), "09:30", "11:00", 1.5m),
                NewTask(user.Id, today, "整理平台体验优化清单", "把空状态、种子数据、页面引导整理成下轮迭代项。", TaskPriority.Medium, TaskType.Work, TaskSource.Work, TaskItemStatus.Pending, now.AddHours(-4), "14:00", "15:30", 1.5m),
                NewTask(user.Id, today.AddDays(1), "检查 AI 助手示例提示词", "为规划器和日报生成准备更贴近日常工作的提示词。", TaskPriority.Medium, TaskType.Personal, TaskSource.Growth, TaskItemStatus.Pending, now.AddHours(-2), null, null, 1m),
                NewTask(user.Id, today.AddDays(-1), "完成订阅套餐说明校对", "核对 Free/Pro/Team 的功能边界。", TaskPriority.High, TaskType.Work, TaskSource.Work, TaskItemStatus.Completed, now.AddDays(-1), null, null, 2m, now.AddDays(-1).AddHours(4)),
            ],
            "jack" =>
            [
                NewTask(user.Id, today, "修复工作日志筛选交互", "统一日期范围和项目筛选的默认值。", TaskPriority.Urgent, TaskType.Work, TaskSource.Work, TaskItemStatus.InProgress, now.AddHours(-7), "10:00", "12:00", 2m),
                NewTask(user.Id, today, "补充学生中心接口测试", "覆盖 dashboard、tasks、mistakes、materials 的新路由。", TaskPriority.High, TaskType.Work, TaskSource.Work, TaskItemStatus.Pending, now.AddHours(-5), "15:00", "17:00", 2m),
                NewTask(user.Id, today.AddDays(1), "重构前端 API 命名", "把 postgraduate 旧命名收敛到 student 域。", TaskPriority.Medium, TaskType.Work, TaskSource.Work, TaskItemStatus.Pending, now.AddHours(-2), null, null, 1.5m),
                NewTask(user.Id, today.AddDays(-2), "完成菜单权限联调", "验证 Student persona 菜单按后端配置生成。", TaskPriority.High, TaskType.Work, TaskSource.Work, TaskItemStatus.Completed, now.AddDays(-2), null, null, 2m, now.AddDays(-2).AddHours(5)),
            ],
            "tom" =>
            [
                NewTask(user.Id, today, "整理个人待办清单", "把近期任务拆成今天、明天和本周三类。", TaskPriority.Medium, TaskType.Personal, TaskSource.Growth, TaskItemStatus.Pending, now.AddHours(-3), "20:00", "20:30", 0.5m),
                NewTask(user.Id, today, "阅读产品体验笔记", "关注空状态、引导文案和默认数据。", TaskPriority.Low, TaskType.Personal, TaskSource.Growth, TaskItemStatus.InProgress, now.AddHours(-2), null, null, 1m),
                NewTask(user.Id, today.AddDays(1), "整理周末学习计划", "预留两段连续时间处理技术债。", TaskPriority.Medium, TaskType.Personal, TaskSource.Growth, TaskItemStatus.Pending, now.AddHours(-1), null, null, 1m),
                NewTask(user.Id, today.AddDays(-1), "完成账号资料检查", "确认昵称、头像和身份配置。", TaskPriority.Low, TaskType.Personal, TaskSource.Growth, TaskItemStatus.Completed, now.AddDays(-1), null, null, 0.5m, now.AddDays(-1).AddHours(3)),
            ],
            _ => []
        };
    }

    private static TaskItem NewTask(
        Guid userId,
        DateOnly planDate,
        string title,
        string? description,
        TaskPriority priority,
        TaskType type,
        TaskSource source,
        TaskItemStatus status,
        DateTime createdAt,
        string? startTime,
        string? endTime,
        decimal estimatedHours,
        DateTime? completedAt = null)
    {
        return new TaskItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanDate = planDate,
            Title = title,
            Description = description,
            Remark = description,
            Priority = priority,
            Type = type,
            Source = source,
            Status = status,
            StartTime = startTime,
            EndTime = endTime,
            EstimatedHours = estimatedHours,
            ActualHours = completedAt.HasValue ? estimatedHours : null,
            CompletedAt = completedAt,
            CreatedAt = createdAt
        };
    }

    private static List<Habit> CreateUserHabits(AppUser user, DateTime now, DateOnly today)
    {
        var specs = user.Username switch
        {
            "admin" => new[]
            {
                ("晨间复盘", "工作", "每天用 10 分钟检查平台运营指标。", "每天", 6, 18),
                ("阅读产品案例", "学习", "保持对管理后台和 SaaS 产品体验的输入。", "每周 5 次", 3, 12),
            },
            "jack" => new[]
            {
                ("代码 Review", "工作", "每天至少 Review 一个关键改动。", "每天", 8, 24),
                ("技术笔记", "学习", "记录当天踩坑、调试结论和设计取舍。", "每周 5 次", 4, 16),
            },
            "tom" => new[]
            {
                ("晚间整理", "生活", "睡前整理明天最重要的三件事。", "每天", 2, 9),
                ("轻量运动", "健康", "保持基础活动量。", "每周 4 次", 1, 8),
            },
            _ => []
        };

        return specs.Select((x, index) => new Habit
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Name = x.Item1,
            HabitType = x.Item2,
            Description = x.Item3,
            TargetFrequency = x.Item4,
            Status = 1,
            CurrentStreak = x.Item5,
            LongestStreak = Math.Max(x.Item5, x.Item6),
            TotalCheckIns = x.Item6,
            LastCheckInDate = today.AddDays(index == 0 ? 0 : -1),
            CreatedAt = now.AddDays(-x.Item6)
        }).ToList();
    }

    private static IEnumerable<HabitCheckIn> CreateHabitCheckIns(IEnumerable<Habit> habits, DateOnly today)
    {
        foreach (var habit in habits)
        {
            var days = Math.Min(habit.TotalCheckIns, 10);
            for (var i = 0; i < days; i++)
            {
                yield return new HabitCheckIn
                {
                    Id = Guid.NewGuid(),
                    HabitId = habit.Id,
                    CheckInDate = today.AddDays(-i),
                    Remark = i == 0 ? "今天已完成" : "保持节奏"
                };
            }
        }
    }

    private static IEnumerable<GrowthProject> CreateUserGrowthProjects(AppUser user, DateTime now, DateOnly today)
    {
        return user.Username switch
        {
            "admin" =>
            [
                NewGrowthProject(user.Id, "平台体验治理", "梳理空状态、演示数据、权限体验和关键流程闭环。", today.AddDays(-20), today.AddDays(20), 62, 9, GrowthProjectStatus.InProgress, GrowthProjectType.Work, now.AddDays(-20)),
                NewGrowthProject(user.Id, "管理后台运营手册", "沉淀账号、套餐、菜单和功能点维护流程。", today.AddDays(-10), today.AddDays(30), 35, 6, GrowthProjectStatus.InProgress, GrowthProjectType.Work, now.AddDays(-10)),
            ],
            "jack" =>
            [
                NewGrowthProject(user.Id, "前后端契约治理", "统一 DTO、API 类型和页面提交链路。", today.AddDays(-15), today.AddDays(15), 70, 12, GrowthProjectStatus.InProgress, GrowthProjectType.Work, now.AddDays(-15)),
                NewGrowthProject(user.Id, "TypeScript 质量提升", "补齐核心页面 typecheck 和可复用类型。", today.AddDays(-8), today.AddDays(25), 45, 7, GrowthProjectStatus.InProgress, GrowthProjectType.Study, now.AddDays(-8)),
            ],
            "tom" =>
            [
                NewGrowthProject(user.Id, "个人效率系统搭建", "用任务、习惯和知识库管理日常学习工作。", today.AddDays(-7), today.AddDays(21), 28, 5, GrowthProjectStatus.InProgress, GrowthProjectType.Personal, now.AddDays(-7)),
            ],
            _ => []
        };
    }

    private static GrowthProject NewGrowthProject(Guid userId, string name, string description, DateOnly startDate, DateOnly endDate, int progress, int taskCount, GrowthProjectStatus status, GrowthProjectType type, DateTime createdAt)
    {
        return new GrowthProject
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            Description = description,
            StartDate = startDate,
            EndDate = endDate,
            Progress = progress,
            TaskCount = taskCount,
            Status = status,
            Type = type,
            CreatedAt = createdAt
        };
    }

    private static IEnumerable<KnowledgeArticle> CreateUserKnowledgeArticles(AppUser user, DateTime now)
    {
        return user.Username switch
        {
            "admin" =>
            [
                NewArticle(user.Id, "菜单权限排查清单", "系统管理", "菜单,权限,排查", "1. 确认角色等级\n2. 确认 FeatureCode\n3. 确认用户身份标签\n4. 刷新前端菜单缓存", 18, now.AddDays(-6)),
                NewArticle(user.Id, "演示账号体验标准", "产品体验", "种子数据,空状态,体验", "每个种子账号至少要有可见任务、习惯、知识内容和一条正在进行的主线。", 12, now.AddDays(-3)),
            ],
            "jack" =>
            [
                NewArticle(user.Id, "DatePicker 统一策略", "前端", "Vue,DatePicker,dayjs", "表单内部使用 dayjs 绑定，提交前统一格式化为 yyyy-MM-dd，列表展示只消费字符串。", 24, now.AddDays(-5)),
                NewArticle(user.Id, "学生中心接口迁移记录", "后端", "路由,student,契约", "学生域统一走 /api/student/...，postgraduate 仅作为专项看板语义保留。", 20, now.AddDays(-2)),
            ],
            "tom" =>
            [
                NewArticle(user.Id, "我的任务系统使用方式", "个人成长", "任务,习惯,复盘", "每天只保留 3 个关键任务，晚上用习惯打卡和知识库记录复盘。", 5, now.AddDays(-1)),
            ],
            _ => []
        };
    }

    private static KnowledgeArticle NewArticle(Guid userId, string title, string category, string tags, string content, int viewCount, DateTime createdAt)
    {
        return new KnowledgeArticle
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Category = category,
            Tags = tags,
            Content = content,
            ViewCount = viewCount,
            IsPublished = true,
            CreatedAt = createdAt
        };
    }

    private static IEnumerable<WorkDailyPlan> CreateJackWorkDailyPlans(AppUser user, Guid projectId, DateTime now, DateOnly today)
    {
        return
        [
            new() { Id = Guid.NewGuid(), UserId = user.Id, ProjectId = projectId, PlanDate = today, Title = "学生中心 dashboard 联调", Content = "验证任务、错题、资料、学习记录聚合数据。", Priority = WorkDailyPlanPriority.High, Status = WorkDailyPlanStatus.InProgress, StartTime = "10:00", EndTime = "12:00", EstimatedHours = 2, CreatedAt = now.AddHours(-5) },
            new() { Id = Guid.NewGuid(), UserId = user.Id, ProjectId = projectId, PlanDate = today, Title = "补齐种子账号体验数据", Content = "给不同角色账号补可见样例数据。", Priority = WorkDailyPlanPriority.Medium, Status = WorkDailyPlanStatus.Pending, StartTime = "15:00", EndTime = "16:30", EstimatedHours = 1.5m, CreatedAt = now.AddHours(-4) },
            new() { Id = Guid.NewGuid(), UserId = user.Id, ProjectId = projectId, PlanDate = today.AddDays(-1), Title = "路由规范化回归", Content = "确认学生域接口全部走 /api/student。", Priority = WorkDailyPlanPriority.High, Status = WorkDailyPlanStatus.Completed, EstimatedHours = 2, ActualHours = 2, CreatedAt = now.AddDays(-1), UpdatedAt = now.AddDays(-1).AddHours(4) },
        ];
    }

    private static IEnumerable<WorkLog> CreateJackWorkLogs(AppUser user, Guid projectId, DateTime now, DateOnly today)
    {
        return
        [
            new() { Id = Guid.NewGuid(), UserId = user.Id, ProjectId = projectId, WorkDate = today, WeekDay = today.DayOfWeek.ToString(), Title = "学生中心真实数据联调", OriginalContent = "完成 dashboard、learning、review 三个页面的数据联调，检查空状态和权限过滤。", Summary = "学生中心核心页面已有真实数据支撑。", TotalHours = 3.5m, Status = WorkLogStatus.Normal, SourceType = WorkLogSourceType.Manual, PersonaCode = "Developer", CreatedAt = now.AddHours(-2) },
            new() { Id = Guid.NewGuid(), UserId = user.Id, ProjectId = projectId, WorkDate = today.AddDays(-1), WeekDay = today.AddDays(-1).DayOfWeek.ToString(), Title = "API 路由规范化", OriginalContent = "将学生模块接口收敛到 /api/student，并调整前端 API 调用。", Summary = "完成旧 postgraduate 路由替换和回归验证。", TotalHours = 4m, Status = WorkLogStatus.Normal, SourceType = WorkLogSourceType.Manual, PersonaCode = "Developer", CreatedAt = now.AddDays(-1).AddHours(5) },
            new() { Id = Guid.NewGuid(), UserId = user.Id, ProjectId = projectId, WorkDate = today.AddDays(-2), WeekDay = today.AddDays(-2).DayOfWeek.ToString(), Title = "前端类型检查修复", OriginalContent = "处理学生模块新增页面的 API 类型和枚举映射。", Summary = "typecheck 通过，页面类型更稳定。", TotalHours = 2.5m, Status = WorkLogStatus.Normal, SourceType = WorkLogSourceType.Manual, PersonaCode = "Developer", CreatedAt = now.AddDays(-2).AddHours(6) },
        ];
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
        C(student, "学习总览", "/student/dashboard", "lucide:layout-dashboard", "/views/student/dashboard/index.vue", 0, 1, false, "Student", null, "STUDENT_EXAM");
        C(student, "学习计划", "/student/learning", "lucide:book-open-check", "/views/student/learning/index.vue", 1, 1, false, "Student", null, "STUDENT_LEARNING");
        C(student, "复习日程", "/student/review", "lucide:alarm-clock-check", "/views/student/review/index.vue", 2, 1, false, "Student", null, "STUDENT_REVIEW");
        C(student, "错题本", "/student/mistakes", "lucide:notebook-pen", "/views/student/mistakes/index.vue", 3, 1, false, "Student", null, "STUDENT_MISTAKES");
        C(student, "学习资料", "/student/materials", "lucide:library", "/views/student/materials/index.vue", 4, 1, false, "Student", null, "STUDENT_MATERIALS");
        C(student, "学习记录", "/student/records", "lucide:history", "/views/student/records/index.vue", 5, 1, false, "Student", null, "STUDENT_RECORDS");
        C(student, "科目目标", "/student/subjects", "lucide:target", "/views/student/subjects/index.vue", 6, 1, false, "Student", null, "STUDENT_SUBJECTS");

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
