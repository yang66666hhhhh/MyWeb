using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Features.Work.Entities;
using FieldType = WebApplication1.Features.Work.Entities.FieldType;

namespace WebApplication1.Shared.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        Console.WriteLine("[DbSeeder] Starting seed...");

        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(DateTime.Now);

        // Users
        if (!await context.Users.AnyAsync())
        {
            Console.WriteLine("[DbSeeder] Seeding Users...");
            var users = new List<AppUser>
            {
                new()
                {
                    Username = "vben",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RealName = "超级管理员",
                    Avatar = $"https://api.dicebear.com/7.x/avataaars/svg?seed=vben",
                    Email = "vben@example.com",
                    Status = AppUserStatus.Active,
                    Roles = "super",
                    CreatedAt = now
                },
                new()
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    RealName = "系统管理员",
                    Avatar = $"https://api.dicebear.com/7.x/avataaars/svg?seed=admin",
                    Email = "admin@example.com",
                    Status = AppUserStatus.Active,
                    Roles = "admin",
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
                    Roles = "user",
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
                    Roles = "user",
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
                    Roles = "user",
                    CreatedAt = now
                },
            };
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
            Console.WriteLine("[DbSeeder] Users seeded.");
        }

        // Tags
        if (!await context.Tags.AnyAsync())
        {
            Console.WriteLine("[DbSeeder] Seeding Tags...");
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
            Console.WriteLine("[DbSeeder] Tags seeded.");
        }

        // MenuConfigs (下拉框选项)
        if (!await context.MenuConfigs.AnyAsync())
        {
            Console.WriteLine("[DbSeeder] Seeding MenuConfigs...");
            var configs = new List<MenuConfig>
            {
                new() { Id = Guid.NewGuid(), Path = "/dashboard", Name = "仪表盘", Icon = "ant-design:dashboard", Sort = 0, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/work/logs", Name = "工作日志", Icon = "ant-design:file-text", Sort = 10, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/work/projects", Name = "项目列表", Icon = "ant-design:project", Sort = 11, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/work/templates", Name = "模板中心", Icon = "ant-design:layout", Sort = 12, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/work/daily-plan", Name = "每日计划", Icon = "ant-design:calendar", Sort = 13, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/work/import", Name = "数据导入", Icon = "ant-design:upload", Sort = 14, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/growth/habits", Name = "习惯打卡", Icon = "ant-design:check-circle", Sort = 20, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/growth/postgraduate", Name = "考研任务", Icon = "ant-design:book", Sort = 21, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/growth/exam-mistakes", Name = "错题本", Icon = "ant-design:warning", Sort = 22, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/growth/knowledge-base", Name = "知识库", Icon = "ant-design:database", Sort = 23, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/growth/projects", Name = "成长项目", Icon = "ant-design:rocket", Sort = 24, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/system/user", Name = "用户管理", Icon = "lucide:users", Sort = 90, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Path = "/system/menu-tag", Name = "菜单标签", Icon = "lucide:menu", Sort = 91, IsActive = true, CreatedAt = now },
            };
            context.MenuConfigs.AddRange(configs);
            await context.SaveChangesAsync();
            Console.WriteLine("[DbSeeder] MenuConfigs seeded.");
        }

        // MenuItems (用户看到的菜单)
        if (!await context.MenuItems.AnyAsync())
        {
            Console.WriteLine("[DbSeeder] Seeding MenuItems...");
            var tagDict = await context.Tags.ToDictionaryAsync(t => t.Name);
            var menuItems = new List<MenuItem>
            {
                new() { Id = Guid.NewGuid(), Name = "工作日志", Path = "/work/logs", Icon = "ant-design:file-text", Sort = 10, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "项目列表", Path = "/work/projects", Icon = "ant-design:project", Sort = 11, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "模板中心", Path = "/work/templates", Icon = "ant-design:layout", Sort = 12, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "每日计划", Path = "/work/daily-plan", Icon = "ant-design:calendar", Sort = 13, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "数据导入", Path = "/work/import", Icon = "ant-design:upload", Sort = 14, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "习惯打卡", Path = "/growth/habits", Icon = "ant-design:check-circle", Sort = 20, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "考研任务", Path = "/growth/postgraduate", Icon = "ant-design:book", Sort = 21, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "错题本", Path = "/growth/exam-mistakes", Icon = "ant-design:warning", Sort = 22, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "知识库", Path = "/growth/knowledge-base", Icon = "ant-design:database", Sort = 23, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "成长项目", Path = "/growth/projects", Icon = "ant-design:rocket", Sort = 24, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "用户管理", Path = "/system/user", Icon = "lucide:users", Sort = 90, IsActive = true, CreatedAt = now },
                new() { Id = Guid.NewGuid(), Name = "菜单标签", Path = "/system/menu-tag", Icon = "lucide:menu", Sort = 91, IsActive = true, CreatedAt = now },
            };
            context.MenuItems.AddRange(menuItems);
            await context.SaveChangesAsync();

            // MenuTags
            Console.WriteLine("[DbSeeder] Seeding MenuTags...");
            var menuItemDict = menuItems.ToDictionary(m => m.Path);
            var menuTags = new List<MenuTag>
            {
                new() { MenuItemId = menuItemDict["/work/logs"].Id, TagId = tagDict["日志管理"].Id },
                new() { MenuItemId = menuItemDict["/work/projects"].Id, TagId = tagDict["日志管理"].Id },
                new() { MenuItemId = menuItemDict["/work/templates"].Id, TagId = tagDict["日志管理"].Id },
                new() { MenuItemId = menuItemDict["/work/daily-plan"].Id, TagId = tagDict["日志管理"].Id },
                new() { MenuItemId = menuItemDict["/work/import"].Id, TagId = tagDict["日志管理"].Id },
                new() { MenuItemId = menuItemDict["/growth/habits"].Id, TagId = tagDict["习惯养成"].Id },
                new() { MenuItemId = menuItemDict["/growth/postgraduate"].Id, TagId = tagDict["考研学习"].Id },
                new() { MenuItemId = menuItemDict["/growth/exam-mistakes"].Id, TagId = tagDict["考研学习"].Id },
                new() { MenuItemId = menuItemDict["/growth/knowledge-base"].Id, TagId = tagDict["知识库"].Id },
                new() { MenuItemId = menuItemDict["/growth/projects"].Id, TagId = tagDict["考研学习"].Id },
                new() { MenuItemId = menuItemDict["/system/user"].Id, TagId = tagDict["系统配置"].Id },
                new() { MenuItemId = menuItemDict["/system/menu-tag"].Id, TagId = tagDict["系统配置"].Id },
            };
            context.MenuTags.AddRange(menuTags);
            await context.SaveChangesAsync();
            Console.WriteLine("[DbSeeder] MenuItems and MenuTags seeded.");
        }

        // UserTypes
        if (!await context.UserTypes.AnyAsync())
        {
            Console.WriteLine("[DbSeeder] Seeding UserTypes...");
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
            Console.WriteLine("[DbSeeder] UserTypes and UserTypeTags seeded.");
        }

        // UserTags - assign tags to users
        if (!await context.UserTags.AnyAsync())
        {
            Console.WriteLine("[DbSeeder] Seeding UserTags...");
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
            Console.WriteLine("[DbSeeder] UserTags seeded.");
        }

        Console.WriteLine("[DbSeeder] Seed completed successfully!");
    }
}
