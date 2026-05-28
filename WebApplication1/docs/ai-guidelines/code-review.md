# 代码审查清单

## 概述

本文档定义了代码审查的标准清单，确保代码质量、安全性和可维护性。

---

## 审查流程

### 1. 准备阶段
- [ ] 阅读相关需求文档
- [ ] 了解变更的背景和目的
- [ ] 准备审查环境

### 2. 审查阶段
- [ ] 按照清单逐项检查
- [ ] 记录发现的问题
- [ ] 提出改进建议

### 3. 反馈阶段
- [ ] 与开发者沟通问题
- [ ] 讨论解决方案
- [ ] 确认修改计划

---

## 代码质量检查

### 命名规范

- [ ] 类名使用 PascalCase
- [ ] 接口以 `I` 开头
- [ ] 方法名使用 PascalCase
- [ ] 异步方法以 `Async` 结尾
- [ ] 变量和参数使用 camelCase
- [ ] 常量使用 PascalCase
- [ ] 命名具有描述性，避免缩写

**检查示例：**
```csharp
// ✅ 正确
public class UserService { }
public interface IUserService { }
public async Task<UserDto> GetUserAsync(Guid id) { }

// ❌ 错误
public class userService { }
public interface iUserService { }
public async Task<UserDto> get_user(Guid id) { }
```

### 代码结构

- [ ] 类不超过 300 行
- [ ] 方法不超过 30 行
- [ ] 单一职责原则
- [ ] 适当的代码组织
- [ ] 正确的访问修饰符

**检查示例：**
```csharp
// ✅ 正确 - 单一职责
public class UserService
{
    public async Task<UserDto> GetUserAsync(Guid id) { }
    public async Task<UserDto> CreateUserAsync(CreateUserDto input) { }
    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto input) { }
    public async Task<bool> DeleteUserAsync(Guid id) { }
}

// ❌ 错误 - 职责过多
public class UserService
{
    public async Task<UserDto> GetUserAsync(Guid id) { }
    public async Task SendEmailAsync(string to, string subject, string body) { }
    public async Task GenerateReportAsync() { }
    public async Task ProcessPaymentAsync() { }
}
```

### 代码风格

- [ ] 适当的缩进（4个空格）
- [ ] 大括号独占一行
- [ ] 运算符前后有空格
- [ ] 逗号后有空格
- [ ] 方法之间空一行
- [ ] 没有多余的空行

**检查示例：**
```csharp
// ✅ 正确
public class UserService
{
    public async Task<UserDto> GetUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        
        if (user is null)
        {
            return null;
        }
        
        return ToDto(user);
    }
}

// ❌ 错误
public class UserService{
public async Task<UserDto> GetUserAsync(Guid id){
var user=await _context.Users.FindAsync(id);
if(user is null){return null;}
return ToDto(user);
}}
```

---

## 安全性检查

### 输入验证

- [ ] 所有用户输入都已验证
- [ ] 使用 FluentValidation 或手动验证
- [ ] 防止 SQL 注入
- [ ] 防止 XSS 攻击
- [ ] 防止路径遍历攻击

**检查示例：**
```csharp
// ✅ 正确 - 使用 FluentValidation
public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MaximumLength(50).WithMessage("用户名长度不能超过50");
    }
}

// ❌ 错误 - 无验证
public async Task<UserDto> CreateUserAsync(CreateUserDto input)
{
    var user = new User { Username = input.Username };
    // 直接使用，无验证
}
```

### 身份验证和授权

- [ ] API 端点需要身份验证
- [ ] 敏感操作需要授权检查
- [ ] 使用 `[Authorize]` 属性
- [ ] 检查用户权限

**检查示例：**
```csharp
// ✅ 正确
[Authorize]
[HttpGet("{id:guid}")]
public async Task<ActionResult<ApiResult<UserDto>>> GetById(Guid id)
{
    var result = await _userService.GetByIdAsync(id);
    if (result is null)
        return NotFound();
        
    var currentUserId = GetCurrentUserId();
    if (!IsProOrAbove() && result.UserId != currentUserId)
        return Forbid();
        
    return Ok(ApiResult<UserDto>.Success(result));
}

// ❌ 错误 - 无授权检查
[HttpGet("{id:guid}")]
public async Task<ActionResult<ApiResult<UserDto>>> GetById(Guid id)
{
    var result = await _userService.GetByIdAsync(id);
    return Ok(ApiResult<UserDto>.Success(result));
}
```

### 敏感数据处理

- [ ] 密码使用 BCrypt 加密
- [ ] 敏感数据加密存储
- [ ] 不在日志中记录敏感信息
- [ ] 使用安全的随机数生成器

**检查示例：**
```csharp
// ✅ 正确
public async Task<UserDto> CreateUserAsync(CreateUserDto input)
{
    var user = new User
    {
        Username = input.Username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password)
    };
}

// ❌ 错误
public async Task<UserDto> CreateUserAsync(CreateUserDto input)
{
    var user = new User
    {
        Username = input.Username,
        Password = input.Password  // 明文存储
    };
}
```

---

## 性能检查

### 数据库查询

- [ ] 使用 `AsNoTracking()` 进行只读查询
- [ ] 避免 N+1 查询
- [ ] 使用分页查询
- [ ] 使用 `Select` 投影需要的字段
- [ ] 合理使用索引

**检查示例：**
```csharp
// ✅ 正确
public async Task<PageResult<UserDto>> GetPageAsync(int page, int pageSize)
{
    var query = _context.Users.AsNoTracking();
    
    var total = await query.CountAsync();
    var items = await query
        .OrderBy(x => x.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => new UserDto
        {
            Id = x.Id,
            Name = x.Name
        })
        .ToListAsync();
    
    return PageResult<UserDto>.Create(items, total, page, pageSize);
}

// ❌ 错误
public async Task<List<UserDto>> GetAllAsync()
{
    var users = await _context.Users.ToListAsync();
    return users.Select(ToDto).ToList();
}
```

### 缓存使用

- [ ] 频繁读取的数据使用缓存
- [ ] 设置合理的过期时间
- [ ] 数据更新时清除缓存

**检查示例：**
```csharp
// ✅ 正确
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

---

## 可维护性检查

### 代码重复

- [ ] 没有重复的代码块
- [ ] 共用逻辑提取为公共方法
- [ ] 使用基类或扩展方法

**检查示例：**
```csharp
// ✅ 正确 - 使用基类方法
public abstract class BaseApiController : ControllerBase
{
    protected Guid? GetCurrentUserId() { }
    protected bool IsProOrAbove() { }
    protected Guid? GetUserIdForQuery() { }
}

// ❌ 错误 - 重复代码
public class UserController : ControllerBase
{
    private Guid? GetCurrentUserId() { /* 实现 */ }
}

public class OrderController : ControllerBase
{
    private Guid? GetCurrentUserId() { /* 相同实现 */ }
}
```

### 依赖注入

- [ ] 使用构造函数注入
- [ ] 依赖应该是私有只读字段
- [ ] 注意服务生命周期

**检查示例：**
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

// ❌ 错误
public class UserService
{
    public AppDbContext Context { get; set; }  // 属性注入
}
```

### 错误处理

- [ ] 使用具体的异常类型
- [ ] 不捕获通用异常
- [ ] 记录异常日志
- [ ] 向上层抛出有意义的异常

**检查示例：**
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
        _logger.LogError(ex, "Error");
        return null;
    }
}
```

---

## 测试检查

### 单元测试

- [ ] 测试覆盖所有公共方法
- [ ] 测试正常流程
- [ ] 测试边界条件
- [ ] 测试异常情况
- [ ] 使用 Mock 隔离依赖

**检查示例：**
```csharp
// ✅ 正确
[Fact]
public async Task GetUserAsync_ShouldReturnUser_WhenExists()
{
    // Arrange
    var userId = Guid.NewGuid();
    var user = new User { Id = userId, Name = "Test" };
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    
    // Act
    var result = await _service.GetUserAsync(userId);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Test", result.Name);
}

// ❌ 错误 - 无断言
[Fact]
public async Task GetUserAsync_ShouldWork()
{
    var result = await _service.GetUserAsync(Guid.NewGuid());
}
```

### 测试命名

- [ ] 使用 `方法名_场景_预期结果` 格式
- [ ] 命名清晰描述测试意图

**检查示例：**
```csharp
// ✅ 正确
[Fact]
public async Task GetUserAsync_ShouldReturnNull_WhenUserNotExists() { }

[Fact]
public async Task CreateUserAsync_ShouldThrowException_WhenUsernameExists() { }

// ❌ 错误
[Fact]
public async Task Test1() { }

[Fact]
public async Task GetUserTest() { }
```

---

## 文档检查

### XML 注释

- [ ] 公共 API 有 XML 注释
- [ ] 描述方法的作用
- [ ] 说明参数和返回值
- [ ] 列出可能的异常

**检查示例：**
```csharp
// ✅ 正确
/// <summary>
/// 获取用户详情
/// </summary>
/// <param name="id">用户ID</param>
/// <param name="cancellationToken">取消令牌</param>
/// <returns>用户详情，如果不存在返回null</returns>
public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) { }

// ❌ 错误
public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) { }
```

### 行内注释

- [ ] 解释为什么这样做
- [ ] 不注释显而易见的代码
- [ ] 保持注释简洁

**检查示例：**
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

---

## 架构检查

### 依赖方向

- [ ] Controller 依赖 Service
- [ ] Service 依赖 Repository/DbContext
- [ ] 没有循环依赖
- [ ] 遵循依赖倒置原则

**检查示例：**
```csharp
// ✅ 正确
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }
}

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    
    public UserService(AppDbContext context)
    {
        _context = context;
    }
}

// ❌ 错误 - Controller 直接依赖 DbContext
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public UserController(AppDbContext context)
    {
        _context = context;
    }
}
```

### 模块边界

- [ ] 模块之间通过接口通信
- [ ] 没有跨模块直接引用
- [ ] 遵循单一职责原则

---

## 审查记录模板

### 问题记录

```markdown
## 问题 #1
- **文件**: [文件路径]
- **行号**: [行号]
- **类型**: [安全/性能/代码质量]
- **严重程度**: [高/中/低]
- **描述**: [问题描述]
- **建议**: [改进建议]
```

### 审查总结

```markdown
## 审查总结

### 发现问题
- 高严重度: [数量]
- 中严重度: [数量]
- 低严重度: [数量]

### 改进建议
1. [建议1]
2. [建议2]
3. [建议3]

### 结论
- [ ] 通过
- [ ] 需要修改后重新审查
- [ ] 需要重大修改
```

---

## 自动化工具

### 静态代码分析
- Roslyn 分析器
- StyleCop
- SonarAnalyzer

### 安全扫描
- OWASP ZAP
- Snyk
- WhiteSource

### 性能分析
- BenchmarkDotNet
- dotTrace
- ANTS Performance Profiler

---

## 最佳实践

1. **及时审查**
   - 代码提交后24小时内审查
   - 避免代码积压

2. **建设性反馈**
   - 提供具体的改进建议
   - 避免人身攻击
   - 肯定好的代码

3. **持续改进**
   - 定期更新审查清单
   - 分享审查经验
   - 学习最佳实践

4. **自动化辅助**
   - 使用静态代码分析工具
   - 集成到 CI/CD 流程
   - 自动化常见检查
