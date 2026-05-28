# C# 编码规范

## 概述

本文档定义了项目的 C# 编码规范，确保代码风格一致、可读性和可维护性。

---

## 命名规范

### 类和接口

```csharp
// ✅ 正确
public class UserService { }
public interface IUserService { }
public class CreateOrderDto { }
public abstract class BaseEntity { }

// ❌ 错误
public class userService { }
public interface iUserService { }
public class createOrderDTO { }
```

**规则：**
- 类名使用 PascalCase
- 接口以 `I` 开头
- 抽象类以 `Abstract` 开头或使用形容词
- DTO 类以 `Dto` 结尾
- 验证器类以 `Validator` 结尾

### 方法

```csharp
// ✅ 正确
public async Task<UserDto> GetUserByIdAsync(Guid id) { }
public void ValidateInput(CreateUserDto input) { }

// ❌ 错误
public async Task<UserDto> get_user_by_id(Guid id) { }
public void validateInput(CreateUserDto input) { }
```

**规则：**
- 方法名使用 PascalCase
- 异步方法以 `Async` 结尾
- 布尔方法以 `Is`、`Has`、`Can` 开头
- 获取数据的方法以 `Get` 开头
- 创建数据的方法以 `Create` 开头
- 更新数据的方法以 `Update` 开头
- 删除数据的方法以 `Delete` 开头

### 变量和参数

```csharp
// ✅ 正确
var userName = "John";
var itemCount = 10;
public void ProcessOrder(Order order, bool isUrgent) { }

// ❌ 错误
var UserName = "John";
var item_count = 10;
public void ProcessOrder(Order ORDER, bool IsUrgent) { }
```

**规则：**
- 局部变量使用 camelCase
- 参数使用 camelCase
- 私有字段使用 camelCase（可选 `_` 前缀）
- 公共属性使用 PascalCase

### 常量

```csharp
// ✅ 正确
public const string DefaultRole = "user";
public const int MaxRetryCount = 3;
private const string ConnectionStringKey = "DefaultConnection";

// ❌ 错误
public const string DEFAULT_ROLE = "user";
public const string defaultRole = "user";
```

**规则：**
- 常量使用 PascalCase
- 私有常量使用 PascalCase

### 枚举

```csharp
// ✅ 正确
public enum TaskStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

// ❌ 错误
public enum task_status
{
    pending,
    in_progress,
    completed,
    cancelled
}
```

**规则：**
- 枚举名使用 PascalCase
- 枚举值使用 PascalCase

---

## 代码结构

### 文件组织

```csharp
// ✅ 正确的文件顺序
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Users.Dtos;
using WebApplication1.Features.Users.Services;

namespace WebApplication1.Features.Users.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    // 1. 私有字段
    private readonly IUserService _userService;
    
    // 2. 构造函数
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    // 3. 公共方法
    [HttpGet]
    public async Task<ActionResult<PageResult<UserDto>>> GetPage() { }
    
    // 4. 私有方法
    private void ValidateInput() { }
}
```

**规则：**
- Using 语句按字母顺序排列
- 命名空间使用文件范围声明
- 成员按访问级别分组
- 公共方法在前，私有方法在后

### 类的长度

**规则：**
- 单个类不超过 300 行
- 如果超过，考虑拆分为多个类
- 使用部分类（partial class）组织大型类

### 方法的长度

**规则：**
- 单个方法不超过 30 行
- 如果超过，考虑提取子方法
- 保持方法职责单一

---

## 代码风格

### 缩进和空格

```csharp
// ✅ 正确
public class UserService
{
    public async Task<UserDto> GetUserAsync(Guid id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (user is null)
        {
            return null;
        }
        
        return ToDto(user);
    }
}

// ❌ 错误
public class UserService
{
public async Task<UserDto> GetUserAsync(Guid id){
var user=await _context.Users.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id);
if(user is null){return null;}
return ToDto(user);
}
}
```

**规则：**
- 使用 4 个空格缩进
- 大括号独占一行
- 运算符前后加空格
- 逗号后加空格
- 方法之间空一行

### 空行

```csharp
// ✅ 正确
public class UserService
{
    private readonly AppDbContext _context;
    
    public UserService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<UserDto> GetUserAsync(Guid id)
    {
        // 方法实现
    }
}
```

**规则：**
- 成员之间空一行
- 逻辑块之间空一行
- 不要有多余的空行

---

## 异步编程

### 使用 async/await

```csharp
// ✅ 正确
public async Task<UserDto> GetUserAsync(Guid id)
{
    var user = await _context.Users.FindAsync(id);
    return ToDto(user);
}

// ❌ 错误
public Task<UserDto> GetUserAsync(Guid id)
{
    var user = _context.Users.FindAsync(id);
    return Task.FromResult(ToDto(user.Result));
}
```

**规则：**
- 异步方法使用 `async` 和 `await`
- 不要使用 `.Result` 或 `.Wait()`
- 异步方法以 `Async` 结尾

### 取消令牌

```csharp
// ✅ 正确
public async Task<UserDto> GetUserAsync(
    Guid id, 
    CancellationToken cancellationToken = default)
{
    var user = await _context.Users
        .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    return ToDto(user);
}

// ❌ 错误
public async Task<UserDto> GetUserAsync(Guid id)
{
    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    return ToDto(user);
}
```

**规则：**
- 异步方法接受 `CancellationToken` 参数
- 默认值为 `default`
- 传递给所有异步操作

---

## 错误处理

### 异常处理

```csharp
// ✅ 正确
public async Task<UserDto> GetUserAsync(Guid id)
{
    var user = await _context.Users.FindAsync(id);
    if (user is null)
    {
        throw new NotFoundException($"User with ID {id} not found");
    }
    return ToDto(user);
}

// ❌ 错误
public async Task<UserDto> GetUserAsync(Guid id)
{
    try
    {
        var user = await _context.Users.FindAsync(id);
        return ToDto(user);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting user");
        return null;
    }
}
```

**规则：**
- 使用具体的异常类型
- 不要捕获通用异常
- 记录异常日志
- 向上层抛出有意义的异常

### 验证

```csharp
// ✅ 正确
public async Task<UserDto> CreateUserAsync(CreateUserDto input)
{
    if (string.IsNullOrWhiteSpace(input.Username))
    {
        throw new ValidationException("Username is required");
    }
    
    if (await _context.Users.AnyAsync(x => x.Username == input.Username))
    {
        throw new ConflictException("Username already exists");
    }
    
    // 创建用户
}

// ❌ 错误
public async Task<UserDto> CreateUserAsync(CreateUserDto input)
{
    // 直接创建，不验证
    var user = new User { Username = input.Username };
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    return ToDto(user);
}
```

**规则：**
- 在方法开始处验证输入
- 使用 FluentValidation 或手动验证
- 抛出有意义的异常

---

## 注释规范

### XML 注释

```csharp
/// <summary>
/// 获取用户详情
/// </summary>
/// <param name="id">用户ID</param>
/// <param name="cancellationToken">取消令牌</param>
/// <returns>用户详情，如果不存在返回null</returns>
/// <exception cref="UnauthorizedAccessException">无权限访问时抛出</exception>
public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
{
    // 实现
}
```

**规则：**
- 公共 API 必须有 XML 注释
- 描述方法的作用
- 说明参数和返回值
- 列出可能的异常

### 行内注释

```csharp
// ✅ 正确
// 检查用户是否有权限访问此资源
if (!await HasAccessAsync(userId, resourceId))
{
    return Forbid();
}

// ❌ 错误
// 检查权限
if (!await HasAccessAsync(userId, resourceId))
{
    return Forbid();
}
```

**规则：**
- 解释为什么这样做，而不是做了什么
- 保持注释简洁
- 不要注释显而易见的代码

---

## LINQ 使用

### 查询语法

```csharp
// ✅ 正确 - 使用方法语法
var users = await _context.Users
    .AsNoTracking()
    .Where(x => x.IsActive)
    .OrderBy(x => x.Name)
    .Select(x => new UserDto
    {
        Id = x.Id,
        Name = x.Name
    })
    .ToListAsync();

// ✅ 也正确 - 使用查询语法（复杂查询时更清晰）
var users = await (from u in _context.Users
                   where u.IsActive
                   orderby u.Name
                   select new UserDto
                   {
                       Id = u.Id,
                       Name = u.Name
                   }).ToListAsync();
```

**规则：**
- 简单查询使用方法语法
- 复杂查询（多表连接）可使用查询语法
- 保持可读性

### 性能优化

```csharp
// ✅ 正确 - 使用 AsNoTracking
var users = await _context.Users
    .AsNoTracking()
    .Where(x => x.IsActive)
    .ToListAsync();

// ❌ 错误 - 不必要的跟踪
var users = await _context.Users
    .Where(x => x.IsActive)
    .ToListAsync();
```

**规则：**
- 只读查询使用 `AsNoTracking()`
- 需要更新时才使用跟踪
- 避免 N+1 查询

---

## 依赖注入

### 构造函数注入

```csharp
// ✅ 正确
public class UserService
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserService> _logger;
    
    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }
}

// ❌ 错误 - 使用属性注入
public class UserService
{
    [Inject]
    public AppDbContext Context { get; set; }
}
```

**规则：**
- 使用构造函数注入
- 依赖应该是私有只读字段
- 不要使用属性注入

### 服务生命周期

```csharp
// ✅ 正确
builder.Services.AddScoped<IUserService, UserService>();      // 请求作用域
builder.Services.AddSingleton<ICacheService, CacheService>(); // 单例
builder.Services.AddTransient<IEmailService, EmailService>(); // 瞬态

// ❌ 错误 - 作用域服务注入到单例
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddScoped<IUserService, UserService>();
// CacheService 不能依赖 UserService
```

**规则：**
- DbContext 使用 Scoped
- 无状态服务使用 Singleton
- 有状态服务使用 Scoped 或 Transient
- 注意生命周期兼容性

---

## 安全编码

### 输入验证

```csharp
// ✅ 正确
public async Task<UserDto> CreateUserAsync(CreateUserDto input)
{
    // 使用 FluentValidation 验证
    var validationResult = await _validator.ValidateAsync(input);
    if (!validationResult.IsValid)
    {
        throw new ValidationException(validationResult.Errors);
    }
    
    // 手动验证关键字段
    if (await _context.Users.AnyAsync(x => x.Username == input.Username))
    {
        throw new ConflictException("Username already exists");
    }
    
    // 创建用户
}

// ❌ 错误 - 直接使用用户输入
public async Task<UserDto> CreateUserAsync(CreateUserDto input)
{
    var user = new User
    {
        Username = input.Username,  // 可能包含恶意内容
        Email = input.Email
    };
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
}
```

**规则：**
- 始终验证用户输入
- 使用参数化查询
- 防止 SQL 注入
- 防止 XSS 攻击

### 敏感数据

```csharp
// ✅ 正确 - 使用加密服务
public async Task StoreApiKeyAsync(string apiKey)
{
    var encrypted = _encryptionService.Encrypt(apiKey);
    // 存储加密后的数据
}

// ❌ 错误 - 明文存储
public async Task StoreApiKeyAsync(string apiKey)
{
    // 直接存储明文
    _context.Settings.Add(new Setting
    {
        Key = "ApiKey",
        Value = apiKey  // 明文存储
    });
}
```

**规则：**
- 敏感数据必须加密存储
- 不要在日志中记录敏感信息
- 使用安全的随机数生成器

---

## 性能优化

### 数据库查询

```csharp
// ✅ 正确 - 分页查询
public async Task<PageResult<UserDto>> GetPageAsync(int page, int pageSize)
{
    var query = _context.Users.AsNoTracking();
    
    var total = await query.CountAsync();
    var items = await query
        .OrderBy(x => x.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => ToDto(x))
        .ToListAsync();
    
    return PageResult<UserDto>.Create(items, total, page, pageSize);
}

// ❌ 错误 - 加载所有数据
public async Task<List<UserDto>> GetAllAsync()
{
    var users = await _context.Users.ToListAsync();
    return users.Select(ToDto).ToList();
}
```

**规则：**
- 使用分页查询
- 避免加载不必要的数据
- 使用 `Select` 投影需要的字段

### 缓存

```csharp
// ✅ 正确 - 使用缓存
public async Task<UserDto> GetUserAsync(Guid id)
{
    var cacheKey = $"user:{id}";
    var cached = await _cacheService.GetAsync<UserDto>(cacheKey);
    if (cached != null)
    {
        return cached;
    }
    
    var user = await _context.Users.FindAsync(id);
    if (user == null) return null;
    
    var dto = ToDto(user);
    await _cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(30));
    return dto;
}

// ❌ 错误 - 不使用缓存
public async Task<UserDto> GetUserAsync(Guid id)
{
    var user = await _context.Users.FindAsync(id);
    return user == null ? null : ToDto(user);
}
```

**规则：**
- 频繁读取的数据使用缓存
- 设置合理的过期时间
- 数据更新时清除缓存

---

## 工具和配置

### EditorConfig

项目根目录的 `.editorconfig` 文件：

```ini
root = true

[*]
indent_style = space
indent_size = 4
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true

[*.cs]
csharp_new_line_before_open_brace = all
csharp_indent_case_contents = true
csharp_indent_switch_labels = true
csharp_space_after_cast = false
csharp_space_after_keywords_in_control_flow_statements = true
csharp_space_between_method_declaration_parameter_list_parentheses = false
csharp_space_between_method_call_parameter_list_parentheses = false
```

### 代码分析

项目使用以下代码分析工具：
- Roslyn 分析器
- StyleCop
- SonarAnalyzer

---

## 总结

遵循这些规范可以确保：
- 代码风格一致
- 提高可读性
- 减少 Bug
- 便于维护
- 团队协作顺畅
