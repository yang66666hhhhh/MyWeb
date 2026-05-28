# 测试规范指南

## 概述

本文档定义了项目的测试规范，包括单元测试、集成测试的编写标准和最佳实践。

---

## 测试策略

### 测试金字塔

```
        /\
       /  \        E2E 测试（少量）
      /    \
     /------\      集成测试（适量）
    /        \
   /----------\    单元测试（大量）
  /            \
 /______________\
```

### 测试覆盖率目标

| 测试类型 | 覆盖率目标 | 说明 |
|---------|-----------|------|
| 单元测试 | 80%+ | 核心业务逻辑 |
| 集成测试 | 60%+ | API 端点 |
| E2E 测试 | 关键流程 | 用户核心流程 |

---

## 单元测试

### 测试框架

- **xUnit** - 测试框架
- **Moq** - Mock 框架
- **FluentAssertions** - 断言库（可选）

### 测试项目结构

```
WebApplication1.Tests/
├── Services/
│   ├── UserServiceTests.cs
│   ├── OrderServiceTests.cs
│   └── ...
├── Validators/
│   ├── CreateUserDtoValidatorTests.cs
│   └── ...
└── Helpers/
    ├── TestDbContextFactory.cs
    └── ...
```

### 测试类命名

```csharp
// ✅ 正确
public class UserServiceTests { }
public class OrderServiceTests { }

// ❌ 错误
public class UserServiceTest { }
public class Tests { }
```

### 测试方法命名

```csharp
// ✅ 正确 - 方法名_场景_预期结果
[Fact]
public async Task GetUserAsync_ShouldReturnUser_WhenExists() { }

[Fact]
public async Task GetUserAsync_ShouldReturnNull_WhenNotExists() { }

[Fact]
public async Task CreateUserAsync_ShouldThrowException_WhenUsernameExists() { }

// ❌ 错误
[Fact]
public async Task Test1() { }

[Fact]
public async Task GetUserTest() { }
```

### 测试结构

```csharp
[Fact]
public async Task GetUserAsync_ShouldReturnUser_WhenExists()
{
    // Arrange - 准备测试数据
    var userId = Guid.NewGuid();
    var user = new User
    {
        Id = userId,
        Username = "testuser",
        RealName = "Test User"
    };
    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    // Act - 执行被测试的方法
    var result = await _service.GetUserAsync(userId);

    // Assert - 验证结果
    Assert.NotNull(result);
    Assert.Equal("testuser", result.Username);
    Assert.Equal("Test User", result.RealName);
}
```

### 测试基类

```csharp
public abstract class TestBase : IDisposable
{
    protected readonly AppDbContext _context;
    
    protected TestBase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        _context = new AppDbContext(options);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
```

### 测试服务

```csharp
public class UserServiceTests : TestBase
{
    private readonly UserService _service;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    
    public UserServiceTests()
    {
        _loggerMock = new Mock<ILogger<UserService>>();
        _service = new UserService(_context, _loggerMock.Object);
    }
    
    [Fact]
    public async Task GetUserAsync_ShouldReturnUser_WhenExists()
    {
        // 测试实现
    }
}
```

---

## 测试场景

### 1. 正常流程测试

```csharp
[Fact]
public async Task CreateUserAsync_ShouldCreateUser_WhenInputIsValid()
{
    // Arrange
    var input = new CreateUserDto
    {
        Username = "newuser",
        RealName = "New User",
        Email = "new@example.com"
    };
    
    // Act
    var result = await _service.CreateUserAsync(input);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("newuser", result.Username);
    Assert.Equal("New User", result.RealName);
    
    // 验证数据库
    var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == "newuser");
    Assert.NotNull(user);
}
```

### 2. 边界条件测试

```csharp
[Theory]
[InlineData("")]
[InlineData(null)]
[InlineData("   ")]
public async Task CreateUserAsync_ShouldThrowException_WhenUsernameIsInvalid(string username)
{
    // Arrange
    var input = new CreateUserDto
    {
        Username = username,
        RealName = "Test User"
    };
    
    // Act & Assert
    await Assert.ThrowsAsync<ValidationException>(() => 
        _service.CreateUserAsync(input));
}

[Fact]
public async Task GetUserAsync_ShouldReturnNull_WhenIdIsEmpty()
{
    // Act
    var result = await _service.GetUserAsync(Guid.Empty);
    
    // Assert
    Assert.Null(result);
}
```

### 3. 异常情况测试

```csharp
[Fact]
public async Task CreateUserAsync_ShouldThrowException_WhenUsernameExists()
{
    // Arrange
    var existingUser = new User { Username = "existing" };
    _context.Users.Add(existingUser);
    await _context.SaveChangesAsync();
    
    var input = new CreateUserDto
    {
        Username = "existing",
        RealName = "Test User"
    };
    
    // Act & Assert
    await Assert.ThrowsAsync<ConflictException>(() => 
        _service.CreateUserAsync(input));
}
```

### 4. 权限验证测试

```csharp
[Fact]
public async Task GetUserAsync_ShouldThrowException_WhenNoPermission()
{
    // Arrange
    var userId = Guid.NewGuid();
    var currentUserId = Guid.NewGuid(); // 不同的用户
    
    // Act & Assert
    await Assert.ThrowsAsync<ForbiddenException>(() => 
        _service.GetUserAsync(userId, currentUserId));
}
```

---

## 集成测试

### 测试 API 端点

```csharp
public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    
    public UserControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetUsers_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/users");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
    }
}
```

### 测试数据库操作

```csharp
public class UserServiceIntegrationTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserService _service;
    
    public UserServiceIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySQL("connection-string")
            .Options;
        
        _context = new AppDbContext(options);
        _service = new UserService(_context);
    }
    
    [Fact]
    public async Task CreateUser_ShouldPersistToDatabase()
    {
        // Arrange
        var input = new CreateUserDto
        {
            Username = "testuser",
            RealName = "Test User"
        };
        
        // Act
        var result = await _service.CreateUserAsync(input);
        
        // Assert
        var user = await _context.Users.FindAsync(result.Id);
        Assert.NotNull(user);
        Assert.Equal("testuser", user.Username);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}
```

---

## Mock 使用

### Mock 依赖服务

```csharp
public class UserServiceTests
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _service;
    
    public UserServiceTests()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _loggerMock = new Mock<ILogger<UserService>>();
        
        _service = new UserService(
            _context,
            _emailServiceMock.Object,
            _loggerMock.Object);
    }
    
    [Fact]
    public async Task CreateUserAsync_ShouldSendEmail()
    {
        // Arrange
        var input = new CreateUserDto
        {
            Username = "newuser",
            Email = "new@example.com"
        };
        
        // Act
        await _service.CreateUserAsync(input);
        
        // Assert
        _emailServiceMock.Verify(
            x => x.SendWelcomeEmailAsync("new@example.com"),
            Times.Once);
    }
}
```

### Mock 配置

```csharp
[Fact]
public async Task GetUserAsync_ShouldReturnCachedUser_WhenExists()
{
    // Arrange
    var userId = Guid.NewGuid();
    var cachedUser = new UserDto { Id = userId, Name = "Cached" };
    
    _cacheServiceMock
        .Setup(x => x.GetAsync<UserDto>($"user:{userId}"))
        .ReturnsAsync(cachedUser);
    
    // Act
    var result = await _service.GetUserAsync(userId);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("Cached", result.Name);
    
    // 验证没有查询数据库
    _contextMock.Verify(
        x => x.Users.FindAsync(userId),
        Times.Never);
}
```

---

## 测试数据管理

### 测试数据工厂

```csharp
public static class TestDataFactory
{
    public static User CreateUser(
        Guid? id = null,
        string username = null,
        string realName = null)
    {
        return new User
        {
            Id = id ?? Guid.NewGuid(),
            Username = username ?? $"user_{Guid.NewGuid():N}",
            RealName = realName ?? "Test User",
            Email = $"test_{Guid.NewGuid():N}@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public static Order CreateOrder(
        Guid? userId = null,
        decimal amount = 100)
    {
        return new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            Amount = amount,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }
}
```

### 使用测试数据

```csharp
[Fact]
public async Task GetUserAsync_ShouldReturnUser_WhenExists()
{
    // Arrange
    var user = TestDataFactory.CreateUser(username: "testuser");
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    
    // Act
    var result = await _service.GetUserAsync(user.Id);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal("testuser", result.Username);
}
```

---

## 测试覆盖率

### 覆盖率工具

```xml
<!-- WebApplication1.Tests.csproj -->
<ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
    <PackageReference Include="coverlet.msbuild" Version="6.0.4" />
</ItemGroup>
```

### 运行覆盖率

```bash
# 运行测试并生成覆盖率报告
dotnet test /p:CollectCoverage=true

# 生成 HTML 报告
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```

### 覆盖率目标

| 模块 | 目标覆盖率 | 说明 |
|------|-----------|------|
| Services | 80%+ | 核心业务逻辑 |
| Controllers | 60%+ | API 端点 |
| Validators | 90%+ | 数据验证 |

---

## 测试最佳实践

### 1. 测试独立性

```csharp
// ✅ 正确 - 每个测试独立
[Fact]
public async Task Test1()
{
    // 每个测试有自己的数据
    var user = TestDataFactory.CreateUser();
    // ...
}

[Fact]
public async Task Test2()
{
    // 不依赖 Test1 的数据
    var user = TestDataFactory.CreateUser();
    // ...
}

// ❌ 错误 - 测试依赖
[Fact]
public async Task Test1()
{
    // 创建数据
    var user = TestDataFactory.CreateUser();
    // ...
}

[Fact]
public async Task Test2()
{
    // 依赖 Test1 创建的数据
    var user = await _context.Users.FirstAsync();
    // ...
}
```

### 2. 测试可重复性

```csharp
// ✅ 正确 - 使用唯一数据库
public UserServiceTests()
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    
    _context = new AppDbContext(options);
}

// ❌ 错误 - 使用共享数据库
public UserServiceTests()
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase("SharedDatabase")
        .Options;
    
    _context = new AppDbContext(options);
}
```

### 3. 测试可读性

```csharp
// ✅ 正确 - 清晰的测试结构
[Fact]
public async Task GetUserAsync_ShouldReturnNull_WhenUserNotExists()
{
    // Arrange
    var nonExistentId = Guid.NewGuid();
    
    // Act
    var result = await _service.GetUserAsync(nonExistentId);
    
    // Assert
    Assert.Null(result);
}

// ❌ 错误 - 不清晰的测试
[Fact]
public async Task Test()
{
    var result = await _service.GetUserAsync(Guid.NewGuid());
    Assert.Null(result);
}
```

### 4. 测试速度

```csharp
// ✅ 正确 - 使用内存数据库
public UserServiceTests()
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
        .Options;
    
    _context = new AppDbContext(options);
}

// ❌ 错误 - 使用真实数据库
public UserServiceTests()
{
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseMySQL("connection-string")
        .Options;
    
    _context = new AppDbContext(options);
}
```

---

## 常见测试模式

### 1. 创建-读取-更新-删除（CRUD）

```csharp
[Fact]
public async Task CreateUser_ShouldPersistToDatabase()
{
    // Arrange
    var input = new CreateUserDto { Username = "test" };
    
    // Act
    var result = await _service.CreateUserAsync(input);
    
    // Assert
    var user = await _context.Users.FindAsync(result.Id);
    Assert.NotNull(user);
    Assert.Equal("test", user.Username);
}

[Fact]
public async Task GetUser_ShouldReturnUser_WhenExists()
{
    // Arrange
    var user = TestDataFactory.CreateUser();
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    
    // Act
    var result = await _service.GetUserAsync(user.Id);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(user.Id, result.Id);
}

[Fact]
public async Task UpdateUser_ShouldModifyUser()
{
    // Arrange
    var user = TestDataFactory.CreateUser();
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    
    var input = new UpdateUserDto { RealName = "Updated" };
    
    // Act
    var result = await _service.UpdateUserAsync(user.Id, input);
    
    // Assert
    Assert.Equal("Updated", result.RealName);
}

[Fact]
public async Task DeleteUser_ShouldRemoveUser()
{
    // Arrange
    var user = TestDataFactory.CreateUser();
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    
    // Act
    await _service.DeleteUserAsync(user.Id);
    
    // Assert
    var deletedUser = await _context.Users.FindAsync(user.Id);
    Assert.Null(deletedUser);
}
```

### 2. 分页查询

```csharp
[Fact]
public async Task GetPageAsync_ShouldReturnPaginatedResults()
{
    // Arrange
    for (int i = 0; i < 15; i++)
    {
        _context.Users.Add(TestDataFactory.CreateUser());
    }
    await _context.SaveChangesAsync();
    
    // Act
    var result = await _service.GetPageAsync(1, 10);
    
    // Assert
    Assert.Equal(10, result.Items.Count);
    Assert.Equal(15, result.Total);
}
```

### 3. 过滤查询

```csharp
[Fact]
public async Task GetPageAsync_ShouldFilterByStatus()
{
    // Arrange
    _context.Users.Add(TestDataFactory.CreateUser(status: UserStatus.Active));
    _context.Users.Add(TestDataFactory.CreateUser(status: UserStatus.Inactive));
    await _context.SaveChangesAsync();
    
    // Act
    var result = await _service.GetPageAsync(1, 10, status: UserStatus.Active);
    
    // Assert
    Assert.All(result.Items, x => Assert.Equal(UserStatus.Active, x.Status));
}
```

---

## 测试工具推荐

### 测试框架
- **xUnit** - 推荐的测试框架
- **NUnit** - 替代方案
- **MSTest** - 微软测试框架

### Mock 框架
- **Moq** - 推荐的 Mock 框架
- **NSubstitute** - 替代方案
- **FakeItEasy** - 替代方案

### 断言库
- **FluentAssertions** - 流式断言
- **Shouldly** - 替代方案
- **xUnit Assert** - 内置断言

### 测试工具
- **TestContainers** - 容器化测试
- **Respawn** - 数据库重置
- **Bogus** - 假数据生成

---

## 常见问题

### Q: 如何测试异步方法？
A: 使用 `async Task` 返回类型：
```csharp
[Fact]
public async Task GetUserAsync_ShouldReturnUser()
{
    var result = await _service.GetUserAsync(Guid.NewGuid());
    Assert.NotNull(result);
}
```

### Q: 如何测试异常？
A: 使用 `Assert.ThrowsAsync`：
```csharp
[Fact]
public async Task CreateUserAsync_ShouldThrowException_WhenUsernameExists()
{
    await Assert.ThrowsAsync<ConflictException>(() => 
        _service.CreateUserAsync(input));
}
```

### Q: 如何测试数据库操作？
A: 使用 InMemory 数据库：
```csharp
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

### Q: 如何测试外部服务？
A: 使用 Mock：
```csharp
var mockService = new Mock<IExternalService>();
mockService.Setup(x => x.GetDataAsync()).ReturnsAsync(data);
```
