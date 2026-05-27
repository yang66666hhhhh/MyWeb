# Personal Growth Management API

全栈个人成长与工作管理系统 API - 支持每日计划、习惯打卡、工作日志、知识库、考研备选、AI助手等功能

## 功能特性

### 核心模块
- **每日计划** - 创建、管理、完成每日计划
- **习惯打卡** - 习惯追踪、连续打卡、统计分析
- **工作日志** - 工作记录、设备管理、项目跟踪
- **知识库** - 文章管理、标签分类、搜索
- **考研备选** - 学习任务、错题本、复习材料
- **AI助手** - 智能计划生成、报告分析、对话助手
- **资产管理** - 收入、支出、预算、投资管理
- **内容管理** - 文章、媒体、发布日历

### 安全特性
- JWT 认证 + Refresh Token
- 请求限流（基于用户/IP）
- 防SQL注入检测
- 防XSS攻击（安全响应头）
- 敏感数据加密存储
- 审计日志（操作追踪）

### 监控特性
- 结构化日志（Serilog）
- 请求/响应日志中间件
- 性能监控（慢API检测）
- 健康检查（内存、磁盘、CPU、数据库）
- 多端点健康检查（/healthz, /healthz/ready, /healthz/live）

### 技术特性
- .NET 10.0
- Entity Framework Core 10.0
- MySQL 8.0
- FluentValidation 数据验证
- 内存缓存
- API 版本控制（v1, v2）
- Swagger/OpenAPI 文档
- 国际化支持（中文/英文）
- Docker 容器化

## 快速开始

### 前置条件
- .NET 10.0 SDK
- MySQL 8.0
- Docker（可选）

### 本地开发

1. **克隆项目**
```bash
git clone <repository-url>
cd WebApplication1
```

2. **配置环境变量**
```bash
# 复制配置模板
cp WebApplication1/appsettings.Development.example.json WebApplication1/appsettings.Development.json

# 编辑配置文件，填入你的配置
# - 数据库连接字符串
# - JWT密钥（至少32字符）
# - OpenAI API Key
# - 加密密钥
```

3. **运行数据库迁移**
```bash
cd WebApplication1
dotnet ef database update
```

4. **启动应用**
```bash
dotnet run
```

5. **访问API文档**
- Swagger UI: http://localhost:5000/swagger
- 健康检查: http://localhost:5000/healthz

### Docker 部署

1. **配置环境变量**
```bash
# 创建 .env 文件
cat > .env << EOF
DB_CONNECTION=Server=mysql;Port=3306;Database=personal_growth;User=root;Password=your_password
JWT_SECRET_KEY=your-super-secret-key-at-least-32-chars-long
OPENAI_API_KEY=sk-your-openai-api-key
MYSQL_ROOT_PASSWORD=your_root_password
MYSQL_USER=your_user
MYSQL_PASSWORD=your_password
EOF
```

2. **启动服务**
```bash
docker-compose up -d
```

3. **查看日志**
```bash
docker-compose logs -f webapp
```

## API 文档

### 认证
所有 API 请求需要在 Header 中携带 JWT Token：
```
Authorization: Bearer <your-jwt-token>
```

### API 版本
- v1: 稳定版本
- v2: 预览版本（实验性）

通过以下方式指定版本：
- URL路径: `/api/v1/resource`
- Header: `X-Api-Version: 1.0`
- 查询参数: `?api-version=1.0`

### 主要端点

#### 认证
- `POST /api/auth/login` - 用户登录
- `POST /api/auth/register` - 用户注册
- `POST /api/auth/refresh-token` - 刷新Token

#### 每日计划
- `GET /api/daily-plans` - 获取计划列表
- `POST /api/daily-plans` - 创建计划
- `PUT /api/daily-plans/{id}` - 更新计划
- `DELETE /api/daily-plans/{id}` - 删除计划

#### 习惯打卡
- `GET /api/habits` - 获取习惯列表
- `POST /api/habits` - 创建习惯
- `POST /api/habits/{id}/check-in` - 打卡

#### 工作日志
- `GET /api/work/logs` - 获取日志列表
- `POST /api/work/logs` - 创建日志

#### AI助手
- `POST /api/ai/generate-plan` - 生成计划
- `POST /api/ai/generate-report` - 生成报告
- `POST /api/ai/chat` - AI对话

#### 资产管理
- `GET /api/assets/summary` - 资产摘要
- `GET /api/assets/incomes` - 收入列表
- `GET /api/assets/expenses` - 支出列表
- `GET /api/assets/budgets` - 预算列表
- `GET /api/assets/investments` - 投资列表

### 响应格式

#### 成功响应
```json
{
  "code": 200,
  "message": "success",
  "data": { ... },
  "success": true
}
```

#### 错误响应
```json
{
  "code": 400,
  "message": "错误信息",
  "success": false
}
```

#### 分页响应
```json
{
  "code": 200,
  "message": "success",
  "data": {
    "items": [...],
    "total": 100,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

## 限流规则

| 端点 | 限制 | 时间窗口 |
|------|------|----------|
| POST /api/auth/login | 5次 | 1分钟 |
| POST /api/auth/register | 3次 | 5分钟 |
| POST /api/ai/* | 20次 | 1分钟 |
| POST /api/* | 30次 | 1分钟 |
| GET /api/* | 100次 | 1分钟 |

## 健康检查

- `/healthz` - 完整健康检查（所有检查项）
- `/healthz/ready` - 就绪检查（仅数据库）
- `/healthz/live` - 存活检查（轻量级）

## 开发指南

### 项目结构
```
WebApplication1/
├── Features/              # 功能模块
│   ├── Admin/            # 管理后台
│   ├── Ai/               # AI助手
│   ├── Analytics/        # 数据分析
│   ├── Assets/           # 资产管理
│   ├── Auth/             # 认证授权
│   ├── Content/          # 内容管理
│   ├── DailyPlans/       # 每日计划
│   ├── Growth/           # 个人成长
│   ├── Tasks/            # 任务管理
│   ├── User/             # 用户管理
│   └── Work/             # 工作管理
├── Shared/               # 共享组件
│   ├── Audit/            # 审计日志
│   ├── Common/           # 通用类
│   ├── Data/             # 数据访问
│   ├── HealthChecks/     # 健康检查
│   ├── Localization/     # 国际化
│   ├── Middleware/        # 中间件
│   ├── Security/         # 安全服务
│   └── Services/         # 通用服务
├── Migrations/           # 数据库迁移
└── Resources/            # 资源文件
```

### 添加新模块
1. 在 `Features/` 下创建新文件夹
2. 创建 `Entities/` - 实体类
3. 创建 `Dtos/` - 数据传输对象
4. 创建 `Services/` - 业务逻辑
5. 创建 `Controllers/` - API控制器
6. 在 `Program.cs` 中注册服务

### 运行测试
```bash
cd WebApplication1.Tests
dotnet test
```

### 代码规范
- 使用 C# 12 特性
- 遵循 SOLID 原则
- 使用 FluentValidation 验证
- 编写单元测试
- 添加 XML 注释

## 环境变量

| 变量名 | 说明 | 必需 |
|--------|------|------|
| DB_CONNECTION | 数据库连接字符串 | ✅ |
| JWT_SECRET_KEY | JWT密钥（≥32字符） | ✅ |
| OPENAI_API_KEY | OpenAI API Key | ❌ |
| ASPNETCORE_ENVIRONMENT | 运行环境 | ❌ |
| Security__EncryptionKey | 加密密钥 | ✅ |

## 许可证

MIT License
