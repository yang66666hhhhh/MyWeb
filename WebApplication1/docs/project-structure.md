# 项目结构

## 目录树

```
WebApplication1/
├── .dockerignore                    # Docker忽略文件
├── .gitignore                       # Git忽略文件
├── docker-compose.yml               # Docker Compose配置
├── Dockerfile                       # Docker构建文件
├── README.md                        # 项目说明
├── CHANGELOG.md                     # 变更日志
├── WebApplication1.slnx             # 解决方案文件
│
├── docs/                            # 文档目录
│   ├── api/                         # API文档
│   │   └── auth.md                  # 认证API文档
│   └── deployment.md                # 部署文档
│
├── WebApplication1/                 # 主项目
│   ├── WebApplication1.csproj       # 项目文件
│   ├── Program.cs                   # 程序入口
│   ├── GlobalUsings.cs              # 全局Using
│   │
│   ├── appsettings.json             # 生产配置
│   ├── appsettings.Development.json # 开发配置
│   ├── appsettings.Development.example.json # 配置模板
│   │
│   ├── Features/                    # 功能模块（按领域划分）
│   │   ├── Admin/                   # 管理后台
│   │   │   ├── Controllers/         # API控制器
│   │   │   ├── Entities/            # 实体类
│   │   │   └── Services/            # 业务服务
│   │   │
│   │   ├── Ai/                      # AI助手
│   │   │   ├── Controllers/
│   │   │   ├── Entities/
│   │   │   └── Services/
│   │   │
│   │   ├── Analytics/               # 数据分析
│   │   │   ├── AnalyticsController.cs
│   │   │   ├── AnalyticsService.cs
│   │   │   └── AnalyticsDtos.cs
│   │   │
│   │   ├── Assets/                  # 资产管理
│   │   │   ├── Controllers/
│   │   │   ├── Entities/
│   │   │   ├── Dtos/
│   │   │   ├── Services/
│   │   │   └── Validators/
│   │   │
│   │   ├── Auth/                    # 认证授权
│   │   │   ├── Controllers/
│   │   │   ├── Entities/
│   │   │   ├── Services/
│   │   │   └── Authorization/
│   │   │
│   │   ├── Content/                 # 内容管理
│   │   │   ├── Controllers/
│   │   │   ├── Entities/
│   │   │   ├── Dtos/
│   │   │   ├── Services/
│   │   │   └── Validators/
│   │   │
│   │   ├── DailyPlans/              # 每日计划
│   │   │   ├── DailyPlansController.cs
│   │   │   ├── DailyPlanService.cs
│   │   │   ├── DailyPlan.cs
│   │   │   └── DailyPlanDtos.cs
│   │   │
│   │   ├── Growth/                  # 个人成长
│   │   │   ├── Controllers/
│   │   │   ├── Entities/
│   │   │   ├── Dtos/
│   │   │   ├── Services/
│   │   │   └── Validators/
│   │   │
│   │   ├── Network/                 # 网络功能
│   │   │
│   │   ├── Tasks/                   # 任务管理
│   │   │   ├── TasksController.cs
│   │   │   ├── TaskItemService.cs
│   │   │   ├── TaskItem.cs
│   │   │   ├── TaskItemDtos.cs
│   │   │   └── Validators/
│   │   │
│   │   ├── User/                    # 用户管理
│   │   │   └── Controllers/
│   │   │
│   │   └── Work/                    # 工作管理
│   │       ├── Controllers/
│   │       ├── Entities/
│   │       ├── Dtos/
│   │       └── Services/
│   │
│   ├── Shared/                      # 共享组件
│   │   ├── Audit/                   # 审计日志
│   │   │   └── AuditService.cs
│   │   │
│   │   ├── Common/                  # 通用类
│   │   │   ├── ApiResult.cs         # API响应格式
│   │   │   ├── BaseApiController.cs # 控制器基类
│   │   │   ├── EntityBase.cs        # 实体基类
│   │   │   └── PageResult.cs        # 分页结果
│   │   │
│   │   ├── Data/                    # 数据访问
│   │   │   ├── AppDbContext.cs      # 数据库上下文
│   │   │   ├── DbSeeder.cs          # 数据库种子
│   │   │   └── Configurations/      # 实体配置
│   │   │
│   │   ├── HealthChecks/            # 健康检查
│   │   │   └── HealthChecks.cs
│   │   │
│   │   ├── Localization/            # 国际化
│   │   │   ├── ResourceNames.cs
│   │   │   ├── SharedResource.cs
│   │   │   └── LocalizationService.cs
│   │   │
│   │   ├── Middleware/               # 中间件
│   │   │   ├── RateLimitingMiddleware.cs
│   │   │   ├── SqlInjectionMiddleware.cs
│   │   │   ├── XssProtectionMiddleware.cs
│   │   │   ├── RequestLoggingMiddleware.cs
│   │   │   └── PerformanceMonitoringMiddleware.cs
│   │   │
│   │   ├── Security/                # 安全服务
│   │   │   └── EncryptionService.cs
│   │   │
│   │   └── Services/                # 通用服务
│   │       ├── CacheService.cs
│   │       └── CacheKeys.cs
│   │
│   ├── Resources/                   # 资源文件
│   │   ├── SharedResource.zh-CN.resx
│   │   └── SharedResource.en-US.resx
│   │
│   ├── Migrations/                  # 数据库迁移
│   │   ├── AppDbContextModelSnapshot.cs
│   │   └── [timestamp]_[name].cs
│   │
│   └── Properties/                  # 属性配置
│       └── launchSettings.json
│
└── WebApplication1.Tests/           # 测试项目
    ├── WebApplication1.Tests.csproj
    └── Services/                    # 服务测试
        ├── AiServiceTests.cs
        ├── AssetServiceTests.cs
        ├── ContentServiceTests.cs
        ├── DailyPlanServiceTests.cs
        ├── HealthCheckTests.cs
        ├── MemoryCacheServiceTests.cs
        ├── TaskItemServiceTests.cs
        ├── ValidatorTests.cs
        └── ...
```

## 命名规范

### 文件命名
- **控制器**: `[Module]Controller.cs` (如 `AuthController.cs`)
- **服务**: `[Module]Service.cs` (如 `AuthService.cs`)
- **接口**: `I[Module]Service.cs` (如 `IAuthService.cs`)
- **实体**: `[Entity].cs` (如 `User.cs`, `TaskItem.cs`)
- **DTO**: `[Entity]Dto.cs` (如 `UserDto.cs`, `CreateUserDto.cs`)
- **验证器**: `[Entity]Validator.cs` (如 `CreateUserDtoValidator.cs`)
- **配置**: `[Entity]Configuration.cs` (如 `UserConfiguration.cs`)

### 命名空间
- `WebApplication1.Features.[Module]` - 功能模块
- `WebApplication1.Features.[Module].Entities` - 实体类
- `WebApplication1.Features.[Module].Services` - 业务服务
- `WebApplication1.Features.[Module].Controllers` - API控制器
- `WebApplication1.Shared` - 共享组件
- `WebApplication1.Shared.Data` - 数据访问
- `WebApplication1.Shared.Middleware` - 中间件

### 数据库表命名
- 使用复数形式: `Users`, `Tasks`, `WorkLogs`
- 关联表使用下划线: `UserRoles`, `MenuTags`

### API路由命名
- 使用复数形式: `/api/users`, `/api/tasks`
- 使用连字符: `/api/daily-plans`, `/api/work-logs`

## 架构原则

### 1. 按功能划分（Feature-based）
每个功能模块包含自己的：
- Controllers - API端点
- Entities - 数据模型
- Services - 业务逻辑
- DTOs - 数据传输对象
- Validators - 数据验证

### 2. 依赖注入
- 使用构造函数注入
- 在 `Program.cs` 中注册服务
- 使用接口解耦

### 3. 数据验证
- 使用 FluentValidation
- 在 DTO 级别验证
- 自动验证（通过中间件）

### 4. 错误处理
- 统一的 `ApiResult` 响应格式
- 全局异常处理中间件
- 详细的错误日志

### 5. 安全性
- JWT认证
- 角色授权
- 输入验证
- SQL注入防护
- XSS防护
- 审计日志
