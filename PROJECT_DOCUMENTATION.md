# Personal Growth Management System - 项目文档

## 1. 项目概述

### 1.1 项目介绍

这是一个基于 **Vue 3 + Vite + Ant Design Vue** 的前端管理和 **ASP.NET Core 10 + Entity Framework Core + MySQL** 的后端 API 组成的全栈项目管理系统。

项目采用前后端分离架构，前端负责页面展示和用户交互，后端提供 RESTful API 进行数据处理。

### 1.2 技术栈

#### 前端技术栈

| 组件 | 技术 | 版本 |
|------|------|------|
| 框架 | Vue 3 | ^3.5.x |
| UI 库 | Ant Design Vue | ^4.x |
| 构建工具 | Vite | ^6.x |
| 状态管理 | Pinia | ^3.x |
| 路由 | Vue Router | ^4.x |
| 语言 | TypeScript | ^5.x |
| Mock 服务 | Nitro | ^3.x |
| 包管理 | pnpm | ^9.x |

#### 后端技术栈

| 组件 | 技术 | 版本 |
|------|------|------|
| 框架 | ASP.NET Core | 10.0 |
| 数据库 | MySQL (MySql.EntityFrameworkCore) | 10.0.1 |
| ORM | Entity Framework Core | 10.0.7 |
| 认证 | JWT Bearer | 10.0.7 |
| API 文档 | Swashbuckle (Swagger) | 10.1.7 |
| Excel处理 | ClosedXML | 0.102.2 |
| 密码哈希 | BCrypt.Net-Next | 4.0.3 |

### 1.3 项目地址

- 前端：`D:\03_Projects\Personal\MyWebSite\vue-vben-admin`
- 后端：`D:\03_Projects\Personal\MyWebSite\WebApplication1`

---

## 2. 项目结构

### 2.1 整体目录结构

```
MyWebSite/
├── vue-vben-admin/                    # 前端项目
│   ├── apps/
│   │   ├── web-antd/                 # 前端 Ant Design Vue 应用
│   │   └── backend-mock/             # Mock 后端服务 (Nitro)
│   ├── packages/                      # 共享工具包
│   ├── internal/                      # Vite 配置共享模块
│   └── ...
│
└── WebApplication1/                   # 后端项目
    ├── Features/                     # 功能模块 (Feature-Based)
    ├── Shared/                       # 共享模块
    ├── Migrations/                   # 数据库迁移
    └── ...
```

### 2.2 后端目录结构

```
WebApplication1/
├── Features/                          # 功能模块 (Feature-Based)
│   ├── Auth/                         # 认证授权模块
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── MenuController.cs
│   │   │   ├── MenuAdminController.cs
│   │   │   ├── TagsController.cs
│   │   │   ├── UserTagController.cs
│   │   │   ├── UserTypeController.cs
│   │   │   └── UsersController.cs
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   └── UserService.cs
│   │   ├── Entities/
│   │   │   ├── AppUser.cs
│   │   │   ├── MenuConfig.cs
│   │   │   ├── MenuTag.cs
│   │   │   ├── Permission.cs
│   │   │   ├── Tenant.cs
│   │   │   └── UserType.cs
│   │   └── Dtos/
│   │       └── UserDtos.cs
│   │
│   ├── DailyPlans/                   # 个人每日计划模块
│   │   ├── DailyPlan.cs
│   │   ├── DailyPlanDto.cs
│   │   ├── DailyPlanQueryDto.cs
│   │   ├── CreateDailyPlanDto.cs
│   │   ├── UpdateDailyPlanDto.cs
│   │   ├── DailyPlanService.cs
│   │   ├── DailyPlansController.cs
│   │   └── IDailyPlanService.cs
│   │
│   ├── Work/                         # 工作中心模块
│   │   ├── Controllers/
│   │   │   ├── WorkDailyPlansController.cs
│   │   │   ├── WorkDevicesController.cs
│   │   │   ├── WorkImportsController.cs
│   │   │   ├── WorkLogsController.cs
│   │   │   ├── WorkProjectsController.cs
│   │   │   ├── WorkStatisticsController.cs
│   │   │   ├── WorkTaskTypesController.cs
│   │   │   └── TemplatesController.cs
│   │   ├── Dtos/
│   │   │   ├── WorkDailyPlanDtos.cs
│   │   │   ├── WorkDeviceDtos.cs
│   │   │   ├── WorkImportDtos.cs
│   │   │   ├── WorkLogDtos.cs
│   │   │   ├── WorkProjectDtos.cs
│   │   │   ├── WorkStatisticsDtos.cs
│   │   │   └── WorkTaskTypeDtos.cs
│   │   ├── Entities/
│   │   │   ├── WorkDailyPlan.cs
│   │   │   ├── WorkDevice.cs
│   │   │   ├── WorkImport.cs
│   │   │   ├── WorkLog.cs
│   │   │   ├── WorkProject.cs
│   │   │   ├── WorkTaskType.cs
│   │   │   ├── WorkCategory.cs
│   │   │   └── DynamicFields.cs
│   │   └── Services/
│   │       ├── IWork/IServices.cs
│   │       ├── Interfaces/ITemplateService.cs
│   │       ├── WorkDailyPlanService.cs
│   │       ├── WorkDeviceService.cs
│   │       ├── WorkImportService.cs
│   │       ├── WorkLogService.cs
│   │       ├── WorkProjectService.cs
│   │       ├── WorkStatisticsService.cs
│   │       ├── WorkTaskTypeService.cs
│   │       └── TemplateService.cs
│   │
│   ├── Growth/                       # 个人成长模块
│   │   ├── Controllers/
│   │   │   ├── GrowthProjectsController.cs
│   │   │   ├── HabitsController.cs
│   │   │   ├── KnowledgeBaseController.cs
│   │   │   └── PostgraduateController.cs
│   │   ├── Dtos/
│   │   │   ├── GrowthProjectDtos.cs
│   │   │   ├── HabitDtos.cs
│   │   │   └── KnowledgePostgraduateDtos.cs
│   │   ├── Entities/
│   │   │   ├── GrowthProject.cs
│   │   │   ├── Habit.cs
│   │   │   └── KnowledgePostgraduate.cs
│   │   └── Services/
│   │       ├── GrowthProjectService.cs
│   │       ├── HabitService.cs
│   │       ├── KnowledgeArticleService.cs
│   │       ├── PostgraduateServices.cs
│   │       └── Interfaces/
│   │           ├── IGrowthProjectService.cs
│   │           ├── IHabitService.cs
│   │           ├── IKnowledgeArticleService.cs
│   │           └── IPostgraduateServices.cs
│   │
│   ├── User/                         # 用户模块
│   │   ├── UserController.cs
│   │   └── UserProfileController.cs
│   │
│   ├── Menu/                         # 菜单模块
│   │   └── MenuController.cs
│   │
│   ├── Ai/                           # AI模块 (占位 Stub)
│   │   ├── Controllers/AiController.cs
│   │   └── Dtos/AiDtos.cs
│   │
│   ├── Analytics/                    # 数据分析模块 (占位 Stub)
│   │   ├── Controllers/AnalyticsController.cs
│   │   └── Dtos/AnalyticsDtos.cs
│   │
│   ├── Assets/                       # 资产管理模块 (占位 Stub)
│   │   ├── Controllers/AssetsController.cs
│   │   ├── Dtos/AssetDtos.cs
│   │   └── Entities/
│   │       ├── Budget.cs
│   │       ├── Expense.cs
│   │       ├── Income.cs
│   │       └── Investment.cs
│   │
│   ├── Content/                      # 内容管理模块 (占位 Stub)
│   │   ├── Controllers/ContentController.cs
│   │   ├── Dtos/ContentDtos.cs
│   │   └── Entities/ContentEntities.cs
│   │
│   ├── Labs/                         # 实验室模块 (占位 Stub)
│   │   └── Controllers/LabsController.cs
│   │
│   ├── Network/                      # 社交网络模块 (占位 Stub)
│   │   ├── Controllers/NetworkController.cs
│   │   └── Dtos/NetworkDtos.cs
│   │
│   └── Platform/                     # 平台集成模块 (占位 Stub)
│       └── Controllers/PlatformController.cs
│
├── Shared/                           # 共享模块
│   ├── Common/                       # 公共工具类
│   │   ├── ApiResult.cs             # API 统一响应
│   │   ├── EntityBase.cs            # 实体基类
│   │   ├── PageQueryDto.cs          # 分页查询基类
│   │   └── PageResult.cs            # 分页结果
│   ├── Data/                        # 数据层
│   │   ├── AppDbContext.cs          # EF Core 上下文
│   │   ├── DbSeeder.cs              # 数据库种子数据
│   │   └── DesignTimeDbContextFactory.cs
│   └── Enums/                       # 枚举定义
│       ├── DailyPlanStatus.cs
│       └── WorkEnums.cs
│
├── Migrations/                       # 数据库迁移
├── Properties/
│   └── launchSettings.json
├── appsettings.Development.json
├── appsettings.json
├── Program.cs
└── WebApplication1.csproj
```

### 2.3 前端目录结构

```
apps/web-antd/src/
├── main.ts                          # 应用入口
├── app.vue                          # 根组件
├── bootstrap.ts                     # 应用初始化
├── preferences.ts                   # 偏好设置
│
├── types/                           # 全局类型中心
│   ├── global.d.ts
│   ├── env.d.ts
│   └── api.ts
│
├── constants/                       # 常量
│   ├── app.ts
│   ├── route.ts
│   └── permissions.ts
│
├── enums/                           # 枚举
│   ├── appEnum.ts
│   ├── growthEnum.ts
│   ├── workEnum.ts
│   └── systemEnum.ts
│
├── api/                             # API 请求模块
│   ├── index.ts
│   ├── request.ts                   # 请求客户端配置
│   ├── core/                        # 核心 API
│   │   ├── auth.ts
│   │   ├── user.ts
│   │   └── menu.ts
│   ├── system/                      # 系统 API
│   │   ├── user.ts
│   │   ├── profile.ts
│   │   ├── settings.ts
│   │   └── menu-tag.ts
│   ├── dashboard/                   # 仪表盘 API
│   │   ├── workspace.ts
│   │   └── statistics.ts
│   ├── growth/                      # 成长模块 API
│   │   ├── index.ts
│   │   ├── types.ts
│   │   ├── habit.ts
│   │   ├── daily-plan.ts
│   │   ├── work-log.ts
│   │   ├── knowledge-base.ts
│   │   ├── postgraduate.ts
│   │   ├── project.ts
│   │   └── work/
│   │       ├── index.ts
│   │       ├── workLog.ts
│   │       ├── workDailyPlan.ts
│   │       ├── workProject.ts
│   │       ├── workDevice.ts
│   │       ├── workTaskType.ts
│   │       ├── workImport.ts
│   │       ├── workStatistics.ts
│   │       └── template.ts
│   ├── work/                        # 工作模块 API
│   │   ├── project.ts
│   │   ├── device.ts
│   │   ├── taskType.ts
│   │   ├── workLog.ts
│   │   ├── dailyPlan.ts
│   │   ├── statistics.ts
│   │   ├── import.ts
│   │   └── template.ts
│   ├── assets/                      # 资产 API
│   │   ├── income.ts
│   │   ├── expense.ts
│   │   ├── budget.ts
│   │   └── investment.ts
│   └── analytics/                  # 分析 API
│       └── growthAnalytics.ts
│
├── composables/                     # 组合式函数
│   └── usePagedQuery.ts
│
├── router/                          # 路由配置
│   ├── index.ts
│   ├── guard.ts
│   ├── access.ts
│   └── routes/
│       ├── index.ts
│       ├── core.ts
│       └── modules/
│           ├── dashboard.ts
│           ├── growth.ts
│           ├── work.ts
│           ├── ai.ts
│           ├── analytics.ts
│           ├── assets.ts
│           ├── system.ts
│           ├── profile.ts
│           ├── vben.ts
│           └── demos.ts
│
├── layouts/                         # 布局组件
│   ├── index.ts
│   ├── basic.vue
│   ├── auth.vue
│   └── components/
│
├── views/                           # 页面视图
│   ├── _core/                       # 核心视图
│   │   ├── authentication/
│   │   │   ├── login.vue
│   │   │   ├── code-login.vue
│   │   │   ├── qrcode-login.vue
│   │   │   ├── forget-password.vue
│   │   │   └── register.vue
│   │   ├── profile/
│   │   │   ├── index.vue
│   │   │   ├── base-setting.vue
│   │   │   ├── security-setting.vue
│   │   │   ├── password-setting.vue
│   │   │   └── notification-setting.vue
│   │   └── fallback/
│   │       ├── not-found.vue
│   │       ├── forbidden.vue
│   │       ├── offline.vue
│   │       ├── internal-error.vue
│   │       └── coming-soon.vue
│   │
│   ├── dashboard/                    # 仪表盘
│   │   ├── workspace/
│   │   │   └── index.vue
│   │   └── analytics/
│   │       ├── index.vue
│   │       ├── analytics-visits.vue
│   │       ├── analytics-visits-source.vue
│   │       ├── analytics-visits-sales.vue
│   │       ├── analytics-visits-data.vue
│   │       └── analytics-trends.vue
│   │
│   ├── growth/                       # 个人成长
│   │   ├── dashboard/
│   │   │   └── index.vue
│   │   ├── daily-plans/
│   │   │   └── index.vue
│   │   ├── habits/
│   │   │   └── index.vue
│   │   ├── work-log/
│   │   │   └── index.vue
│   │   ├── knowledge-base/
│   │   │   └── index.vue
│   │   ├── postgraduate/
│   │   │   ├── index.vue
│   │   │   ├── materials/
│   │   │   ├── mistakes/
│   │   │   └── study-plans/
│   │   ├── project/
│   │   │   └── index.vue
│   │   ├── goals/
│   │   │   └── index.vue
│   │   ├── year-plans/
│   │   │   └── index.vue
│   │   ├── monthly-review/
│   │   │   └── index.vue
│   │   ├── focus-timer/
│   │   │   └── index.vue
│   │   ├── sleep/
│   │   │   └── index.vue
│   │   ├── courses/
│   │   │   └── index.vue
│   │   ├── skills/
│   │   │   └── index.vue
│   │   ├── reading-list/
│   │   │   └── index.vue
│   │   ├── fitness/
│   │   │   └── index.vue
│   │   ├── learning-path/
│   │   │   └── index.vue
│   │   └── mood-tracker/
│   │       └── index.vue
│   │
│   ├── work/                         # 工作中心
│   │   ├── dashboard/
│   │   │   └── index.vue
│   │   ├── daily-plan/
│   │   │   └── index.vue
│   │   ├── task-type/
│   │   │   └── index.vue
│   │   ├── device/
│   │   │   └── index.vue
│   │   ├── project/
│   │   │   └── index.vue
│   │   ├── log/
│   │   │   └── index.vue
│   │   ├── import/
│   │   │   ├── index.vue
│   │   │   ├── ImportResult.vue
│   │   │   └── ImportPreviewTable.vue
│   │   ├── statistics/
│   │   │   └── index.vue
│   │   ├── tasks/
│   │   │   └── index.vue
│   │   ├── gantt/
│   │   │   └── index.vue
│   │   ├── weekly-plan/
│   │   │   └── index.vue
│   │   ├── okr/
│   │   │   └── index.vue
│   │   ├── files/
│   │   │   └── index.vue
│   │   ├── risk-control/
│   │   │   └── index.vue
│   │   ├── software-assets/
│   │   │   └── index.vue
│   │   └── templates/
│   │       └── index.vue
│   │
│   ├── ai/                           # AI 模块
│   │   ├── assistant/
│   │   │   └── index.vue
│   │   ├── planner/
│   │   │   └── index.vue
│   │   ├── reports/
│   │   │   └── index.vue
│   │   ├── knowledge-chat/
│   │   │   └── index.vue
│   │   ├── insights/
│   │   │   └── index.vue
│   │   └── automation/
│   │       └── index.vue
│   │
│   ├── analytics/                    # 数据分析
│   │   ├── growth/
│   │   │   └── index.vue
│   │   ├── work/
│   │   │   └── index.vue
│   │   ├── finance/
│   │   │   └── index.vue
│   │   ├── time/
│   │   │   └── index.vue
│   │   ├── habits/
│   │   │   └── index.vue
│   │   ├── custom-reports/
│   │   │   └── index.vue
│   │   └── ai-insights/
│   │       └── index.vue
│   │
│   ├── assets/                       # 资产管理
│   │   ├── dashboard/
│   │   │   └── index.vue
│   │   ├── income/
│   │   │   └── index.vue
│   │   ├── expenses/
│   │   │   └── index.vue
│   │   ├── budget/
│   │   │   └── index.vue
│   │   ├── investments/
│   │   │   └── index.vue
│   │   └── resources/
│   │       └── index.vue
│   │
│   ├── system/                       # 系统管理
│   │   ├── user/
│   │   │   └── index.vue
│   │   └── menu-tag/
│   │       └── index.vue
│   │
│   ├── network/                      # 社交网络 (视图存在,未配置路由)
│   │   ├── contacts/
│   │   ├── interactions/
│   │   ├── opportunities/
│   │   └── tags/
│   │
│   ├── content/                      # 内容管理 (视图存在,未配置路由)
│   │   ├── dashboard/
│   │   ├── articles/
│   │   ├── drafts/
│   │   ├── ideas/
│   │   ├── media-library/
│   │   ├── publishing-calendar/
│   │   └── seo/
│   │
│   ├── platform/                     # 平台集成 (视图存在,未配置路由)
│   │   ├── api-docs/
│   │   ├── integrations/
│   │   ├── scripts/
│   │   └── webhooks/
│   │
│   ├── labs/                         # 实验室 (视图存在,未配置路由)
│   │   ├── ai-lab/
│   │   ├── data-lab/
│   │   ├── templates/
│   │   └── ui-components/
│   │
│   ├── demos/
│   │   └── antd/
│   │       └── index.vue
│   │
│   ├── about/
│   │   └── index.vue
│   │
│   └── external-links/
│       └── index.vue
│
├── components/                       # 组件库
│   ├── adapter/                      # 组件适配器
│   ├── business/                     # 业务组件
│   ├── common/                       # 通用组件
│   └── DynamicForm/                  # 动态表单
│       └── index.vue
│
├── locales/                          # 国际化
├── store/                            # Pinia 状态管理
│   ├── index.ts
│   ├── auth.ts
│   └── modules/
│
├── services/                         # 业务服务层
├── utils/                            # 工具函数
├── mocks/                            # Mock 数据
├── docs/                             # 文档
└── workers/                          # Web Worker
```

---

## 3. 功能模块

### 3.1 后端功能模块

#### 3.1.1 认证模块 (Features/Auth)

**路由前缀:** `/api/auth`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| POST | `/login` | 否 | 用户登录，返回 JWT 令牌 |
| GET | `/codes` | 是 | 获取用户权限码 |
| POST | `/refresh` | 是 | 刷新 JWT 令牌 |
| POST | `/logout` | 否 | 用户登出 |

**用户管理路由前缀:** `/api/users`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取用户分页列表 |
| GET | `/{id:guid}` | 是 | 获取单个用户 |
| POST | `/` | 是 | 创建用户 |
| PUT | `/{id:guid}` | 是 | 更新用户 |
| DELETE | `/{id:guid}` | 是 | 删除用户 |
| POST | `/{id:guid}/change-password` | 是 | 修改密码 |
| POST | `/{id:guid}/reset-password` | 是 | 重置密码 |

**菜单管理路由前缀:** `/api/menu`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/all` | 是 | 获取所有菜单 |

**菜单管理后台路由前缀:** `/api/menu-admin`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取菜单配置列表 |
| GET | `/paths` | 是 | 获取菜单路径 |
| POST | `/` | 是 | 创建菜单配置 |
| PUT | `/{id:guid}` | 是 | 更新菜单配置 |
| DELETE | `/{id:guid}` | 是 | 删除菜单配置 |

**标签管理路由前缀:** `/api/tags`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取标签列表 |
| POST | `/` | 是 | 创建标签 |
| PUT | `/{id:guid}` | 是 | 更新标签 |
| DELETE | `/{id:guid}` | 是 | 删除标签 |

**用户标签路由前缀:** `/api/user-tags`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/{userId:guid}` | 是 | 获取用户标签 |
| PUT | `/{userId:guid}` | 是 | 更新用户标签 |
| GET | `/users` | 是 | 获取使用某标签的用户 |

**用户类型路由前缀:** `/api/user-types`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取用户类型列表 |
| POST | `/` | 是 | 创建用户类型 |
| PUT | `/{id:guid}` | 是 | 更新用户类型 |
| DELETE | `/{id:guid}` | 是 | 删除用户类型 |
| PUT | `/{userId:guid}/assign` | 是 | 分配用户类型 |

**内置测试用户:**

| 用户名 | 密码 | 角色 | 权限码 |
|--------|------|------|--------|
| vben | 123456 | super | dashboard, growth |
| admin | 123456 | admin | dashboard, growth |
| jack | 123456 | user | dashboard, growth |

#### 3.1.2 个人每日计划模块 (Features/DailyPlans)

**路由前缀:** `/api/daily-plans`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页每日计划列表 |
| GET | `/{id:guid}` | 是 | 获取单个每日计划 |
| POST | `/` | 是 | 创建每日计划 |
| PUT | `/{id:guid}` | 是 | 更新每日计划 |
| PATCH | `/{id:guid}/complete` | 是 | 标记完成 |
| DELETE | `/{id:guid}` | 是 | 删除每日计划 |

#### 3.1.3 工作项目模块 (Features/Work)

**路由前缀:** `/api/work/projects`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页项目列表 |
| GET | `/{id:guid}` | 是 | 获取单个项目 |
| POST | `/` | 是 | 创建项目 |
| PUT | `/{id:guid}` | 是 | 更新项目 |
| DELETE | `/{id:guid}` | 是 | 删除项目 |

#### 3.1.4 工作设备模块 (Features/Work)

**路由前缀:** `/api/work/devices`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页设备列表 |
| GET | `/{id:guid}` | 是 | 获取单个设备 |
| POST | `/` | 是 | 创建设备 |
| PUT | `/{id:guid}` | 是 | 更新设备 |
| DELETE | `/{id:guid}` | 是 | 删除设备 |

#### 3.1.5 工作任务类型模块 (Features/Work)

**路由前缀:** `/api/work/task-types`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页任务类型列表 |
| GET | `/{id:guid}` | 是 | 获取单个任务类型 |
| POST | `/` | 是 | 创建任务类型 |
| PUT | `/{id:guid}` | 是 | 更新任务类型 |
| DELETE | `/{id:guid}` | 是 | 删除任务类型 |

#### 3.1.6 工作日志模块 (Features/Work)

**路由前缀:** `/api/work/logs`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页工作日志列表 |
| GET | `/{id:guid}` | 是 | 获取单个工作日志 |
| POST | `/` | 是 | 创建工作日志 |
| PUT | `/{id:guid}` | 是 | 更新工作日志 |
| DELETE | `/{id:guid}` | 是 | 删除工作日志 |

#### 3.1.7 工作计划模块 (Features/Work)

**路由前缀:** `/api/work/daily-plans`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页工作计划列表 |
| GET | `/{id:guid}` | 是 | 获取单个工作计划 |
| POST | `/` | 是 | 创建工作计划 |
| PUT | `/{id:guid}` | 是 | 更新工作计划 |
| DELETE | `/{id:guid}` | 是 | 删除工作计划 |
| POST | `/{id:guid}/complete` | 是 | 标记完成 |
| POST | `/convert-to-log` | 是 | 转换为工作日志 |

#### 3.1.8 统计分析模块 (Features/Work)

**路由前缀:** `/api/work/statistics`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/overview` | 是 | 获取统计概览 |
| GET | `/daily-hours` | 是 | 获取每日工时统计 |
| GET | `/project-hours` | 是 | 获取项目工时分布 |
| GET | `/task-type-distribution` | 是 | 获取任务类型分布 |
| GET | `/device-ranking` | 是 | 获取设备工时排名 |

#### 3.1.9 数据导入模块 (Features/Work)

**路由前缀:** `/api/work/imports`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页导入批次列表 |
| POST | `/worklog/preview` | 是 | 预览工作日志 Excel 数据 |
| POST | `/worklog/execute` | 是 | 执行工作日志导入 |
| GET | `/worklog/template` | 是 | 下载工作日志导入模板 |
| POST | `/project/preview` | 是 | 预览项目 Excel 数据 |
| POST | `/project/execute` | 是 | 执行项目导入 |
| GET | `/project/template` | 是 | 下载项目导入模板 |
| POST | `/device/preview` | 是 | 预览设备 Excel 数据 |
| POST | `/device/execute` | 是 | 执行设备导入 |
| GET | `/device/template` | 是 | 下载设备导入模板 |
| POST | `/tasktype/preview` | 是 | 预览任务类型 Excel 数据 |
| POST | `/tasktype/execute` | 是 | 执行任务类型导入 |
| GET | `/tasktype/template` | 是 | 下载任务类型导入模板 |

#### 3.1.10 模板模块 (Features/Work)

**路由前缀:** `/api/work/templates`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页模板列表 |
| GET | `/{id:guid}` | 是 | 获取单个模板 |
| GET | `/{id:guid}/fields` | 是 | 获取模板字段 |
| POST | `/` | 是 | 创建模板 |
| PUT | `/{id:guid}` | 是 | 更新模板 |
| DELETE | `/{id:guid}` | 是 | 删除模板 |
| POST | `/{id:guid}/set-default` | 是 | 设置默认模板 |

#### 3.1.11 个人成长模块 (Features/Growth)

**习惯管理路由前缀:** `/api/growth/habits`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页习惯列表 |
| GET | `/{id:guid}` | 是 | 获取单个习惯 |
| POST | `/` | 是 | 创建习惯 |
| PUT | `/{id:guid}` | 是 | 更新习惯 |
| DELETE | `/{id:guid}` | 是 | 删除习惯 |
| POST | `/{id:guid}/check-in` | 是 | 习惯打卡 |
| PUT | `/{id:guid}/status` | 是 | 更新习惯状态 |

**成长项目管理路由前缀:** `/api/growth/projects`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页项目列表 |
| GET | `/{id:guid}` | 是 | 获取单个项目 |
| POST | `/` | 是 | 创建项目 |
| PUT | `/{id:guid}` | 是 | 更新项目 |
| DELETE | `/{id:guid}` | 是 | 删除项目 |

**知识库路由前缀:** `/api/growth/knowledge`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页文章列表 |
| GET | `/{id:guid}` | 是 | 获取单个文章 |
| POST | `/` | 是 | 创建文章 |
| PUT | `/{id:guid}` | 是 | 更新文章 |
| DELETE | `/{id:guid}` | 是 | 删除文章 |

**考研备烤路由前缀:** `/api/growth/postgraduate`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/tasks` | 是 | 获取任务列表 |
| GET | `/tasks/{id:guid}` | 是 | 获取单个任务 |
| GET | `/dashboard` | 是 | 获取考研仪表盘 |
| POST | `/tasks` | 是 | 创建任务 |
| PUT | `/tasks/{id:guid}` | 是 | 更新任务 |
| DELETE | `/tasks/{id:guid}` | 是 | 删除任务 |
| GET | `/mistakes` | 是 | 获取错题列表 |
| GET | `/mistakes/{id:guid}` | 是 | 获取单条错题 |
| POST | `/mistakes` | 是 | 创建错题 |
| PUT | `/mistakes/{id:guid}` | 是 | 更新错题 |
| DELETE | `/mistakes/{id:guid}` | 是 | 删除错题 |
| GET | `/materials` | 是 | 获取资料列表 |
| GET | `/materials/{id:guid}` | 是 | 获取单条资料 |
| POST | `/materials` | 是 | 创建资料 |
| PUT | `/materials/{id:guid}` | 是 | 更新资料 |
| DELETE | `/materials/{id:guid}` | 是 | 删除资料 |

#### 3.1.12 用户模块 (Features/User)

**路由前缀:** `/api/user`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/info` | 是 | 获取当前用户信息 |

**用户资料路由前缀:** `/api/user/profile`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取当前用户资料 |
| PUT | `/` | 是 | 更新用户资料 |
| POST | `/change-password` | 是 | 修改密码 |

#### 3.1.13 占位模块 (Stub/Placeholder)

以下模块存在但功能为占位实现,返回空数据或模拟数据:

**AI 模块路由前缀:** `/api/ai`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| POST | `/generate-plan` | 是 | 生成计划 (模拟) |
| POST | `/generate-report` | 是 | 生成报告 (模拟) |
| POST | `/chat` | 是 | AI 聊天 (模拟) |
| GET | `/plans` | 是 | 获取计划列表 (模拟) |
| GET | `/reports` | 是 | 获取报告列表 (模拟) |

**分析模块路由前缀:** `/api/analytics`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/growth` | 是 | 成长分析 (占位) |
| GET | `/work` | 是 | 工作分析 (占位) |
| GET | `/time` | 是 | 时间分析 (占位) |
| GET | `/finance` | 是 | 财务分析 (占位) |
| GET | `/dashboard` | 是 | 分析仪表盘 (占位) |

**网络模块路由前缀:** `/api/network`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/contacts` | 是 | 联系人列表 (空) |
| GET | `/interactions` | 是 | 互动记录 (空) |
| GET | `/tags` | 是 | 标签列表 (空) |

**内容模块路由前缀:** `/api/content`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/articles` | 是 | 文章列表 (空) |
| GET | `/media` | 是 | 媒体列表 (空) |
| GET | `/calendar` | 是 | 日历数据 (空) |

**资产模块路由前缀:** `/api/assets`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/summary` | 是 | 资产摘要 (零值) |
| GET | `/incomes` | 是 | 收入列表 (空) |
| GET | `/expenses` | 是 | 支出列表 (空) |
| GET | `/budgets` | 是 | 预算列表 (空) |
| GET | `/investments` | 是 | 投资列表 (空) |

**实验室模块路由前缀:** `/api/labs`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/experiments` | 是 | 实验列表 (空) |
| GET | `/templates` | 是 | 模板列表 (空) |
| GET | `/ui-components` | 是 | UI组件列表 (空) |

**平台模块路由前缀:** `/api/platform`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/api-docs` | 是 | API文档 (返回信息) |
| GET | `/scripts` | 是 | 脚本列表 (空) |
| GET | `/integrations` | 是 | 集成列表 (空) |
| GET | `/webhooks` | 是 | Webhook列表 (空) |

---

### 3.2 前端页面路由

#### 3.2.1 仪表盘 (Dashboard)

| 路由 | 视图文件 | 描述 |
|------|---------|------|
| `/dashboard/workspace` | `views/dashboard/workspace/index.vue` | 工作台 |
| `/dashboard/analytics` | `views/dashboard/analytics/index.vue` | 数据分析 |

#### 3.2.2 个人成长 (Growth)

| 路由 | 视图文件 | 描述 |
|------|---------|------|
| `/growth/dashboard` | `views/growth/dashboard/index.vue` | 成长看板 |
| `/growth/daily-plans` | `views/growth/daily-plans/index.vue` | 每日计划 |
| `/growth/habits` | `views/growth/habits/index.vue` | 习惯打卡 |
| `/growth/work-log` | `views/growth/work-log/index.vue` | 工作日志 |
| `/growth/knowledge-base` | `views/growth/knowledge-base/index.vue` | 知识库 |
| `/growth/postgraduate` | `views/growth/postgraduate/index.vue` | 考研备烤 |
| `/growth/postgraduate/materials` | `views/growth/postgraduate/materials/index.vue` | 备考资料 |
| `/growth/postgraduate/mistakes` | `views/growth/postgraduate/mistakes/index.vue` | 错题本 |
| `/growth/postgraduate/study-plans` | `views/growth/postgraduate/study-plans/index.vue` | 学习计划 |
| `/growth/projects` | `views/growth/project/index.vue` | 项目管理 |
| `/growth/goals` | `views/growth/goals/index.vue` | 目标设定 |
| `/growth/year-plans` | `views/growth/year-plans/index.vue` | 年度计划 |
| `/growth/monthly-review` | `views/growth/monthly-review/index.vue` | 月度回顾 |
| `/growth/focus-timer` | `views/growth/focus-timer/index.vue` | 专注计时器 |
| `/growth/sleep` | `views/growth/sleep/index.vue` | 睡眠追踪 |
| `/growth/courses` | `views/growth/courses/index.vue` | 课程学习 |
| `/growth/skills` | `views/growth/skills/index.vue` | 技能管理 |
| `/growth/reading-list` | `views/growth/reading-list/index.vue` | 阅读清单 |
| `/growth/fitness` | `views/growth/fitness/index.vue` | 健身记录 |
| `/growth/learning-path` | `views/growth/learning-path/index.vue` | 学习路径 |
| `/growth/mood-tracker` | `views/growth/mood-tracker/index.vue` | 心情追踪 |

#### 3.2.3 工作中心 (Work)

| 路由 | 视图文件 | 描述 |
|------|---------|------|
| `/work/dashboard` | `views/work/dashboard/index.vue` | 工作看板 |
| `/work/daily-plan` | `views/work/daily-plan/index.vue` | 每日计划 |
| `/work/task-type` | `views/work/task-type/index.vue` | 任务类型 |
| `/work/device` | `views/work/device/index.vue` | 设备管理 |
| `/work/project` | `views/work/project/index.vue` | 工作项目 |
| `/work/log` | `views/work/log/index.vue` | 工作日志 |
| `/work/import` | `views/work/import/index.vue` | 数据导入 |
| `/work/statistics` | `views/work/statistics/index.vue` | 统计分析 |
| `/work/tasks` | `views/work/tasks/index.vue` | 任务管理 |
| `/work/gantt` | `views/work/gantt/index.vue` | 甘特图 |
| `/work/weekly-plan` | `views/work/weekly-plan/index.vue` | 周计划 |
| `/work/okr` | `views/work/okr/index.vue` | OKR管理 |
| `/work/files` | `views/work/files/index.vue` | 文件管理 |
| `/work/risk-control` | `views/work/risk-control/index.vue` | 风险控制 |
| `/work/software-assets` | `views/work/software-assets/index.vue` | 软件资产 |
| `/work/templates` | `views/work/templates/index.vue` | 模板管理 |

#### 3.2.4 AI 模块 (AI)

| 路由 | 视图文件 | 描述 |
|------|---------|------|
| `/ai/assistant` | `views/ai/assistant/index.vue` | AI 助手 |
| `/ai/planner` | `views/ai/planner/index.vue` | AI 规划器 |
| `/ai/reports` | `views/ai/reports/index.vue` | AI 报告 |
| `/ai/knowledge-chat` | `views/ai/knowledge-chat/index.vue` | 知识库问答 |
| `/ai/insights` | `views/ai/insights/index.vue` | AI 洞察 |
| `/ai/automation` | `views/ai/automation/index.vue` | 自动化 |

#### 3.2.5 数据分析 (Analytics)

| 路由 | 视图文件 | 描述 |
|------|---------|------|
| `/analytics/growth` | `views/analytics/growth/index.vue` | 成长分析 |
| `/analytics/work` | `views/analytics/work/index.vue` | 工作分析 |
| `/analytics/finance` | `views/analytics/finance/index.vue` | 财务分析 |
| `/analytics/time` | `views/analytics/time/index.vue` | 时间分析 |
| `/analytics/habits` | `views/analytics/habits/index.vue` | 习惯分析 |
| `/analytics/custom-reports` | `views/analytics/custom-reports/index.vue` | 自定义报告 |
| `/analytics/ai-insights` | `views/analytics/ai-insights/index.vue` | AI 洞察 |

#### 3.2.6 资产管理 (Assets)

| 路由 | 视图文件 | 描述 |
|------|---------|------|
| `/assets/dashboard` | `views/assets/dashboard/index.vue` | 资产仪表盘 |
| `/assets/income` | `views/assets/income/index.vue` | 收入管理 |
| `/assets/expenses` | `views/assets/expenses/index.vue` | 支出管理 |
| `/assets/budget` | `views/assets/budget/index.vue` | 预算管理 |
| `/assets/investments` | `views/assets/investments/index.vue` | 投资管理 |
| `/assets/resources` | `views/assets/resources/index.vue` | 资源管理 |

#### 3.2.7 系统管理 (System)

| 路由 | 视图文件 | 描述 |
|------|---------|------|
| `/system/user` | `views/system/user/index.vue` | 用户管理 |
| `/system/menu-tag` | `views/system/menu-tag/index.vue` | 菜单标签 |

#### 3.2.8 其他页面

| 路由 | 视图文件 | 描述 |
|------|---------|------|
| `/system` | `views/system/index.vue` | 系统管理 |
| `/external-links` | `views/external-links/index.vue` | 外部链接 |
| `/about` | `views/about/index.vue` | 关于 |
| `/demos/antd` | `views/demos/antd/index.vue` | Ant Design 示例 |
| `/auth/login` | `views/_core/authentication/login.vue` | 登录 |
| `/profile` | `views/_core/profile/index.vue` | 个人设置 |

---

## 4. 数据库架构

### 4.1 数据库表

#### 4.1.1 认证相关表

**Tenants (租户表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Name | VARCHAR(100) | NOT NULL | 租户名称 |
| Code | VARCHAR(50) | NOT NULL, UNIQUE | 租户编码 |
| Description | VARCHAR(500) | | 描述 |
| CreatedAt | DATETIME | | 创建时间 |

**Users (用户表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| TenantId | GUID | FK | 租户ID |
| Username | VARCHAR(50) | NOT NULL, UNIQUE | 用户名 |
| PasswordHash | VARCHAR(100) | NOT NULL | 密码哈希 |
| RealName | VARCHAR(100) | NOT NULL | 真实姓名 |
| Avatar | VARCHAR(500) | | 头像URL |
| Email | VARCHAR(100) | | 邮箱 |
| Phone | VARCHAR(20) | | 电话 |
| Roles | VARCHAR(100) | Default 'user' | 角色 |
| Status | INT | Default 0 | 状态 |
| LastLoginIp | VARCHAR(50) | | 最后登录IP |
| CreatedAt | DATETIME | | 创建时间 |

**Roles (角色表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Name | VARCHAR(50) | NOT NULL | 角色名 |
| Code | VARCHAR(50) | NOT NULL, UNIQUE | 角色编码 |
| Description | VARCHAR(500) | | 描述 |
| Permissions | VARCHAR(2000) | | 权限列表 |
| CreatedAt | DATETIME | | 创建时间 |

**UserRoles (用户角色表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| UserId | GUID | PK, FK | 用户ID |
| RoleId | GUID | PK, FK | 角色ID |

**Tags (标签表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Name | VARCHAR(50) | NOT NULL, UNIQUE | 标签名 |
| Description | VARCHAR(500) | | 描述 |
| Color | VARCHAR(20) | | 颜色 |
| CreatedAt | DATETIME | | 创建时间 |

**MenuItems (菜单项表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| ParentId | GUID | FK | 父菜单ID |
| Name | VARCHAR(50) | NOT NULL | 菜单名称 |
| Path | VARCHAR(200) | NOT NULL, UNIQUE | 菜单路径 |
| Icon | VARCHAR(100) | | 图标 |
| RequiredPermissions | VARCHAR(500) | | 所需权限 |
| CreatedAt | DATETIME | | 创建时间 |

**MenuTags (菜单标签关联表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| MenuItemId | GUID | PK, FK | 菜单ID |
| TagId | GUID | PK, FK | 标签ID |

**UserTags (用户标签关联表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| UserId | GUID | PK, FK | 用户ID |
| TagId | GUID | PK, FK | 标签ID |

**MenuConfigs (菜单配置表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Path | VARCHAR(200) | NOT NULL, UNIQUE | 路径 |
| Name | VARCHAR(50) | NOT NULL | 名称 |
| Icon | VARCHAR(100) | | 图标 |
| Description | VARCHAR(500) | | 描述 |
| CreatedAt | DATETIME | | 创建时间 |

**UserTypes (用户类型表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Name | VARCHAR(50) | NOT NULL | 类型名称 |
| Code | VARCHAR(50) | NOT NULL, UNIQUE | 类型编码 |
| Description | VARCHAR(500) | | 描述 |
| Color | VARCHAR(20) | | 颜色 |
| CreatedAt | DATETIME | | 创建时间 |

**UserTypeTags (用户类型标签关联表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| UserTypeId | GUID | PK, FK | 用户类型ID |
| TagId | GUID | PK, FK | 标签ID |

#### 4.1.2 个人计划表

**DailyPlans (每日计划表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| PlanDate | DATE | NOT NULL | 计划日期 |
| Title | VARCHAR(120) | NOT NULL | 标题 |
| Description | VARCHAR(1000) | | 描述 |
| Priority | INT | Default 3 | 优先级 |
| Status | INT | Default 0 | 状态 |
| Remark | VARCHAR(1000) | | 备注 |
| CreatedAt | DATETIME | | 创建时间 |
| UpdatedAt | DATETIME | | 更新时间 |

**索引:** PlanDate, Status

#### 4.1.3 工作相关表

**WorkProjects (工作项目表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| ProjectName | VARCHAR(100) | NOT NULL | 项目名称 |
| ProjectCode | VARCHAR(50) | | 项目编码 |
| ProjectType | INT | Default 0 | 项目类型 |
| CustomerName | VARCHAR(100) | | 客户名称 |
| Description | VARCHAR(1000) | | 描述 |
| StartDate | DATE | | 开始日期 |
| EndDate | DATE | | 结束日期 |
| Status | INT | Default 0 | 状态 |
| Sort | INT | | 排序 |
| CreatedAt | DATETIME | | 创建时间 |
| UpdatedAt | DATETIME | | 更新时间 |

**索引:** ProjectName, Status

**WorkDevices (工作设备表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| ProjectId | GUID | FK | 项目ID |
| DeviceName | VARCHAR(100) | NOT NULL | 设备名称 |
| DeviceCode | VARCHAR(50) | | 设备编码 |
| DeviceType | INT | Default 1 | 设备类型 |
| Description | VARCHAR(1000) | | 描述 |
| Status | INT | Default 0 | 状态 |
| CreatedAt | DATETIME | | 创建时间 |
| UpdatedAt | DATETIME | | 更新时间 |

**索引:** DeviceName, Status, ProjectId

**WorkTaskTypes (工作任务类型表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| TypeName | VARCHAR(50) | NOT NULL, UNIQUE | 类型名称 |
| TypeCode | VARCHAR(50) | | 类型编码 |
| Description | VARCHAR(500) | | 描述 |
| Sort | INT | | 排序 |
| Enabled | BOOL | Default TRUE | 是否启用 |
| CreatedAt | DATETIME | | 创建时间 |
| UpdatedAt | DATETIME | | 更新时间 |

**索引:** TypeName (唯一)

**WorkLogs (工作日志表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| WorkDate | DATE | NOT NULL | 工作日期 |
| WeekDay | VARCHAR(10) | | 星期几 |
| ProjectId | GUID | FK, NOT NULL | 项目ID |
| Title | VARCHAR(200) | NOT NULL | 标题 |
| OriginalContent | VARCHAR(4000) | | 原始内容 |
| Summary | VARCHAR(1000) | | 摘要 |
| TotalHours | DECIMAL(10,2) | | 总工时 |
| Status | INT | Default 0 | 状态 |
| SourceType | INT | Default 0 | 来源类型 |
| TemplateId | GUID | FK | 模板ID |
| ImportBatchId | GUID | | 导入批次ID |
| Remark | VARCHAR(1000) | | 备注 |
| CreatedAt | DATETIME | | 创建时间 |
| UpdatedAt | DATETIME | | 更新时间 |

**索引:** WorkDate, Status, ProjectId, UserId

**WorkLogItems (工作日志明细表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| WorkLogId | GUID | FK, NOT NULL | 工作日志ID |
| Content | VARCHAR(2000) | NOT NULL | 内容 |
| TaskTypeId | GUID | FK | 任务类型ID |
| DeviceId | GUID | FK | 设备ID |
| ProgressPercent | INT | | 进度百分比 |
| Hours | DECIMAL(10,2) | | 工时 |
| Sort | INT | | 排序 |
| Remark | VARCHAR(500) | | 备注 |
| CreatedAt | DATETIME | | 创建时间 |
| UpdatedAt | DATETIME | | 更新时间 |

**索引:** WorkLogId, TaskTypeId, DeviceId

**WorkDailyPlans (工作计划表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| ProjectId | GUID | FK | 项目ID |
| PlanDate | DATE | NOT NULL | 计划日期 |
| Title | VARCHAR(120) | NOT NULL | 标题 |
| Content | VARCHAR(1000) | | 内容 |
| Priority | INT | Default 2 | 优先级 |
| Status | INT | Default 0 | 状态 |
| StartTime | VARCHAR(10) | | 开始时间 |
| EndTime | VARCHAR(10) | | 结束时间 |
| EstimatedHours | DECIMAL(10,2) | | 预计工时 |
| ActualHours | DECIMAL(10,2) | | 实际工时 |
| Remark | VARCHAR(1000) | | 备注 |
| CreatedAt | DATETIME | | 创建时间 |
| UpdatedAt | DATETIME | | 更新时间 |

**索引:** PlanDate, Status, ProjectId

**WorkImportBatches (工作导入批次表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| FileName | VARCHAR(200) | NOT NULL | 文件名 |
| FileSize | BIGINT | | 文件大小 |
| ImportType | INT | | 导入类型 |
| TotalRows | INT | | 总行数 |
| SuccessRows | INT | | 成功行数 |
| FailedRows | INT | | 失败行数 |
| SkippedRows | INT | | 跳过行数 |
| DuplicateRows | INT | | 重复行数 |
| Status | INT | Default 0 | 状态 |
| ImportStrategy | INT | Default 0 | 导入策略 |
| StartedAt | DATETIME | | 开始时间 |
| FinishedAt | DATETIME | | 结束时间 |
| ErrorMessage | VARCHAR(2000) | | 错误信息 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** Status

**WorkImportRows (工作导入行表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| BatchId | GUID | FK, NOT NULL | 批次ID |
| RowNumber | INT | | 行号 |
| RawDate | VARCHAR(50) | | 原始日期 |
| RawWeekDay | VARCHAR(10) | | 原始星期 |
| RawProject | VARCHAR(100) | | 原始项目 |
| RawDevice | VARCHAR(200) | | 原始设备 |
| RawTaskType | VARCHAR(200) | | 原始任务类型 |
| RawContent | VARCHAR(2000) | | 原始内容 |
| RawHours | VARCHAR(20) | | 原始工时 |
| RawRemark | VARCHAR(500) | | 原始备注 |
| ParsedDate | DATE | | 解析后日期 |
| ParsedHours | DECIMAL(10,2) | | 解析后工时 |
| ValidationStatus | INT | Default 0 | 验证状态 |
| DuplicateStatus | INT | Default 0 | 重复状态 |
| ErrorMessage | VARCHAR(500) | | 错误信息 |
| ImportedWorkLogId | GUID | | 导入的工作日志ID |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** BatchId

**WorkCategories (工作分类表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| ParentId | GUID | FK | 父分类ID |
| Name | VARCHAR(50) | NOT NULL | 分类名称 |
| Code | VARCHAR(50) | NOT NULL, UNIQUE | 分类编码 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** Code, ParentId

**IndustryTemplates (行业模板表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| Name | VARCHAR(100) | NOT NULL, UNIQUE | 模板名称 |
| Description | VARCHAR(500) | | 描述 |
| Industry | VARCHAR(50) | NOT NULL | 行业 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** Name (唯一)

**TemplateFields (模板字段表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| TemplateId | GUID | FK, NOT NULL | 模板ID |
| FieldName | VARCHAR(50) | NOT NULL | 字段名称 |
| FieldLabel | VARCHAR(100) | NOT NULL | 字段标签 |
| FieldType | INT | Default 0 | 字段类型 |
| Options | VARCHAR(1000) | | 选项列表 |
| DefaultValue | VARCHAR(500) | | 默认值 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** (TemplateId, FieldName) 唯一

**WorkLogDynamicValues (工作日志动态值表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| WorkLogId | GUID | FK, NOT NULL | 工作日志ID |
| FieldName | VARCHAR(50) | NOT NULL | 字段名称 |
| StringValue | VARCHAR(1000) | | 字符串值 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** (WorkLogId, FieldName) 唯一

#### 4.1.4 成长相关表

**Habits (习惯表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| Name | VARCHAR(100) | NOT NULL | 习惯名称 |
| HabitType | VARCHAR(50) | NOT NULL | 习惯类型 |
| Description | VARCHAR(500) | | 描述 |
| TargetFrequency | VARCHAR(20) | | 目标频率 |
| Status | INT | Default 1 | 状态 |
| CurrentStreak | INT | Default 0 | 当前连续 |
| LongestStreak | INT | Default 0 | 最长连续 |
| TotalCheckIns | INT | Default 0 | 总打卡次数 |
| LastCheckInDate | DATE | | 最后打卡日期 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** Name, Status, UserId

**HabitCheckIns (习惯打卡表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| HabitId | GUID | FK, NOT NULL | 习惯ID |
| CheckInDate | DATE | NOT NULL | 打卡日期 |
| Remark | VARCHAR(500) | | 备注 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** CheckInDate, (HabitId, CheckInDate) 唯一

**GrowthProjects (成长项目表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| Name | VARCHAR(100) | NOT NULL | 项目名称 |
| Description | VARCHAR(1000) | | 描述 |
| Type | INT | Default 0 | 项目类型 |
| StartDate | DATE | | 开始日期 |
| EndDate | DATE | | 结束日期 |
| Status | INT | Default 0 | 状态 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** Name, Status, UserId

**KnowledgeArticles (知识文章表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| Title | VARCHAR(200) | NOT NULL | 标题 |
| Content | VARCHAR(10000) | | 内容 |
| Category | VARCHAR(50) | NOT NULL | 分类 |
| Tags | VARCHAR(500) | | 标签 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** Title, Category, UserId

**PostgraduateTasks (考研任务表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| Title | VARCHAR(200) | NOT NULL | 标题 |
| Description | VARCHAR(1000) | | 描述 |
| Type | INT | Default 0 | 任务类型 |
| DueDate | DATE | | 截止日期 |
| Status | INT | Default 0 | 状态 |
| Priority | INT | Default 2 | 优先级 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** Status, DueDate, UserId

**ExamMistakes (错题表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| Question | VARCHAR(1000) | NOT NULL | 题目 |
| Answer | VARCHAR(2000) | | 答案 |
| Explanation | VARCHAR(2000) | | 解析 |
| Subject | VARCHAR(50) | NOT NULL | 科目 |
| Tags | VARCHAR(500) | | 标签 |
| Status | INT | Default 0 | 状态 |
| LastReviewDate | DATE | | 最后复习日期 |
| NextReviewDate | DATE | | 下次复习日期 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** Subject, UserId, Status

**ExamMaterials (考试资料表)**

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| Title | VARCHAR(200) | NOT NULL | 标题 |
| Content | VARCHAR(10000) | | 内容 |
| Subject | VARCHAR(50) | NOT NULL | 科目 |
| Tags | VARCHAR(500) | | 标签 |
| Type | INT | Default 0 | 资料类型 |
| CreatedAt | DATETIME | | 创建时间 |

**索引:** Subject, Type, UserId

---

## 5. API 端点汇总

### 5.1 认证 API

| 方法 | 端点 | 描述 |
|------|------|------|
| POST | `/api/auth/login` | 用户登录 |
| GET | `/api/auth/codes` | 获取权限码 |
| POST | `/api/auth/refresh` | 刷新令牌 |
| POST | `/api/auth/logout` | 用户登出 |

### 5.2 用户管理 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/users` | 获取用户分页列表 |
| GET | `/api/users/{id}` | 获取单个用户 |
| POST | `/api/users` | 创建用户 |
| PUT | `/api/users/{id}` | 更新用户 |
| DELETE | `/api/users/{id}` | 删除用户 |
| POST | `/api/users/{id}/change-password` | 修改密码 |
| POST | `/api/users/{id}/reset-password` | 重置密码 |

### 5.3 菜单 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/menu/all` | 获取所有菜单 |
| GET | `/api/menu-admin` | 获取菜单配置列表 |
| GET | `/api/menu-admin/paths` | 获取菜单路径 |
| POST | `/api/menu-admin` | 创建菜单配置 |
| PUT | `/api/menu-admin/{id}` | 更新菜单配置 |
| DELETE | `/api/menu-admin/{id}` | 删除菜单配置 |

### 5.4 标签 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/tags` | 获取标签列表 |
| POST | `/api/tags` | 创建标签 |
| PUT | `/api/tags/{id}` | 更新标签 |
| DELETE | `/api/tags/{id}` | 删除标签 |
| GET | `/api/user-tags/{userId}` | 获取用户标签 |
| PUT | `/api/user-tags/{userId}` | 更新用户标签 |
| GET | `/api/user-tags/users` | 获取使用某标签的用户 |
| GET | `/api/user-types` | 获取用户类型列表 |
| POST | `/api/user-types` | 创建用户类型 |
| PUT | `/api/user-types/{id}` | 更新用户类型 |
| DELETE | `/api/user-types/{id}` | 删除用户类型 |
| PUT | `/api/user-types/{userId}/assign` | 分配用户类型 |

### 5.5 用户 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/user/info` | 获取当前用户信息 |
| GET | `/api/user/profile` | 获取用户资料 |
| PUT | `/api/user/profile` | 更新用户资料 |
| POST | `/api/user/profile/change-password` | 修改密码 |

### 5.6 个人每日计划 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/daily-plans` | 获取分页列表 |
| GET | `/api/daily-plans/{id}` | 获取单个 |
| POST | `/api/daily-plans` | 创建 |
| PUT | `/api/daily-plans/{id}` | 更新 |
| PATCH | `/api/daily-plans/{id}/complete` | 完成 |
| DELETE | `/api/daily-plans/{id}` | 删除 |

### 5.7 工作项目 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/projects` | 获取分页列表 |
| GET | `/api/work/projects/{id}` | 获取单个 |
| POST | `/api/work/projects` | 创建 |
| PUT | `/api/work/projects/{id}` | 更新 |
| DELETE | `/api/work/projects/{id}` | 删除 |

### 5.8 工作设备 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/devices` | 获取分页列表 |
| GET | `/api/work/devices/{id}` | 获取单个 |
| POST | `/api/work/devices` | 创建 |
| PUT | `/api/work/devices/{id}` | 更新 |
| DELETE | `/api/work/devices/{id}` | 删除 |

### 5.9 工作任务类型 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/task-types` | 获取分页列表 |
| GET | `/api/work/task-types/{id}` | 获取单个 |
| POST | `/api/work/task-types` | 创建 |
| PUT | `/api/work/task-types/{id}` | 更新 |
| DELETE | `/api/work/task-types/{id}` | 删除 |

### 5.10 工作日志 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/logs` | 获取分页列表 |
| GET | `/api/work/logs/{id}` | 获取单个 |
| POST | `/api/work/logs` | 创建 |
| PUT | `/api/work/logs/{id}` | 更新 |
| DELETE | `/api/work/logs/{id}` | 删除 |

### 5.11 工作计划 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/daily-plans` | 获取分页列表 |
| GET | `/api/work/daily-plans/{id}` | 获取单个 |
| POST | `/api/work/daily-plans` | 创建 |
| PUT | `/api/work/daily-plans/{id}` | 更新 |
| DELETE | `/api/work/daily-plans/{id}` | 删除 |
| POST | `/api/work/daily-plans/{id}/complete` | 完成 |
| POST | `/api/work/daily-plans/convert-to-log` | 转换为日志 |

### 5.12 统计分析 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/statistics/overview` | 统计概览 |
| GET | `/api/work/statistics/daily-hours` | 每日工时 |
| GET | `/api/work/statistics/project-hours` | 项目工时 |
| GET | `/api/work/statistics/task-type-distribution` | 任务类型分布 |
| GET | `/api/work/statistics/device-ranking` | 设备排名 |

### 5.13 数据导入 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/imports` | 获取分页导入批次 |
| POST | `/api/work/imports/worklog/preview` | 预览工作日志 |
| POST | `/api/work/imports/worklog/execute` | 执行工作日志导入 |
| GET | `/api/work/imports/worklog/template` | 下载工作日志模板 |
| POST | `/api/work/imports/project/preview` | 预览项目 |
| POST | `/api/work/imports/project/execute` | 执行项目导入 |
| GET | `/api/work/imports/project/template` | 下载项目模板 |
| POST | `/api/work/imports/device/preview` | 预览设备 |
| POST | `/api/work/imports/device/execute` | 执行设备导入 |
| GET | `/api/work/imports/device/template` | 下载设备模板 |
| POST | `/api/work/imports/tasktype/preview` | 预览任务类型 |
| POST | `/api/work/imports/tasktype/execute` | 执行任务类型导入 |
| GET | `/api/work/imports/tasktype/template` | 下载任务类型模板 |

### 5.14 模板 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/templates` | 获取分页模板列表 |
| GET | `/api/work/templates/{id}` | 获取单个模板 |
| GET | `/api/work/templates/{id}/fields` | 获取模板字段 |
| POST | `/api/work/templates` | 创建模板 |
| PUT | `/api/work/templates/{id}` | 更新模板 |
| DELETE | `/api/work/templates/{id}` | 删除模板 |
| POST | `/api/work/templates/{id}/set-default` | 设置默认模板 |

### 5.15 成长习惯 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/growth/habits` | 获取分页列表 |
| GET | `/api/growth/habits/{id}` | 获取单个 |
| POST | `/api/growth/habits` | 创建 |
| PUT | `/api/growth/habits/{id}` | 更新 |
| DELETE | `/api/growth/habits/{id}` | 删除 |
| POST | `/api/growth/habits/{id}/check-in` | 打卡 |
| PUT | `/api/growth/habits/{id}/status` | 更新状态 |

### 5.16 成长项目 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/growth/projects` | 获取分页列表 |
| GET | `/api/growth/projects/{id}` | 获取单个 |
| POST | `/api/growth/projects` | 创建 |
| PUT | `/api/growth/projects/{id}` | 更新 |
| DELETE | `/api/growth/projects/{id}` | 删除 |

### 5.17 知识库 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/growth/knowledge` | 获取分页列表 |
| GET | `/api/growth/knowledge/{id}` | 获取单个 |
| POST | `/api/growth/knowledge` | 创建 |
| PUT | `/api/growth/knowledge/{id}` | 更新 |
| DELETE | `/api/growth/knowledge/{id}` | 删除 |

### 5.18 考研备烤 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/growth/postgraduate/tasks` | 获取任务列表 |
| GET | `/api/growth/postgraduate/tasks/{id}` | 获取单个任务 |
| GET | `/api/growth/postgraduate/dashboard` | 获取仪表盘 |
| POST | `/api/growth/postgraduate/tasks` | 创建任务 |
| PUT | `/api/growth/postgraduate/tasks/{id}` | 更新任务 |
| DELETE | `/api/growth/postgraduate/tasks/{id}` | 删除任务 |
| GET | `/api/growth/postgraduate/mistakes` | 获取错题列表 |
| GET | `/api/growth/postgraduate/mistakes/{id}` | 获取单条错题 |
| POST | `/api/growth/postgraduate/mistakes` | 创建错题 |
| PUT | `/api/growth/postgraduate/mistakes/{id}` | 更新错题 |
| DELETE | `/api/growth/postgraduate/mistakes/{id}` | 删除错题 |
| GET | `/api/growth/postgraduate/materials` | 获取资料列表 |
| GET | `/api/growth/postgraduate/materials/{id}` | 获取单条资料 |
| POST | `/api/growth/postgraduate/materials` | 创建资料 |
| PUT | `/api/growth/postgraduate/materials/{id}` | 更新资料 |
| DELETE | `/api/growth/postgraduate/materials/{id}` | 删除资料 |

### 5.19 占位模块 API

| 方法 | 端点 | 描述 |
|------|------|------|
| POST | `/api/ai/generate-plan` | AI生成计划 (模拟) |
| POST | `/api/ai/generate-report` | AI生成报告 (模拟) |
| POST | `/api/ai/chat` | AI聊天 (模拟) |
| GET | `/api/ai/plans` | AI计划列表 (模拟) |
| GET | `/api/ai/reports` | AI报告列表 (模拟) |
| GET | `/api/analytics/growth` | 成长分析 (占位) |
| GET | `/api/analytics/work` | 工作分析 (占位) |
| GET | `/api/analytics/time` | 时间分析 (占位) |
| GET | `/api/analytics/finance` | 财务分析 (占位) |
| GET | `/api/analytics/dashboard` | 分析仪表盘 (占位) |
| GET | `/api/network/contacts` | 联系人列表 (空) |
| GET | `/api/network/interactions` | 互动记录 (空) |
| GET | `/api/network/tags` | 标签列表 (空) |
| GET | `/api/content/articles` | 文章列表 (空) |
| GET | `/api/content/media` | 媒体列表 (空) |
| GET | `/api/content/calendar` | 日历数据 (空) |
| GET | `/api/assets/summary` | 资产摘要 (零值) |
| GET | `/api/assets/incomes` | 收入列表 (空) |
| GET | `/api/assets/expenses` | 支出列表 (空) |
| GET | `/api/assets/budgets` | 预算列表 (空) |
| GET | `/api/assets/investments` | 投资列表 (空) |
| GET | `/api/labs/experiments` | 实验列表 (空) |
| GET | `/api/labs/templates` | 模板列表 (空) |
| GET | `/api/labs/ui-components` | UI组件列表 (空) |
| GET | `/api/platform/api-docs` | API文档信息 |
| GET | `/api/platform/scripts` | 脚本列表 (空) |
| GET | `/api/platform/integrations` | 集成列表 (空) |
| GET | `/api/platform/webhooks` | Webhook列表 (空) |

---

## 6. 配置

### 6.1 后端配置 (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=personal_growth;User=root;Password=123456;CharSet=utf8mb4;"
  },
  "Jwt": {
    "Issuer": "PersonalGrowthManagement",
    "Audience": "VueVbenAdmin",
    "SecretKey": "PersonalGrowthManagement.Dev.SecretKey.ChangeMe.2026",
    "ExpireMinutes": 120
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### 6.2 前端环境变量 (.env.development)

```
VITE_APP_TITLE=个人成长管理系统
VITE_API_BASE_URL=/api
VITE_USE_MOCK=false
```

### 6.3 前端代理配置 (vite.config.ts)

```typescript
// API 代理配置
proxy: {
  '/api': {
    changeOrigin: true,
    rewrite: (path) => path.replace(/^\/api/, ''),
    target: 'http://localhost:5000/api',
    ws: true,
  },
}
```

---

## 7. 认证与授权

### 7.1 JWT 配置

```csharp
// Token 验证参数
- ValidateIssuer = true        // 验证发行者
- ValidIssuer = "PersonalGrowthManagement"
- ValidateAudience = true       // 验证受众
- ValidAudience = "VueVbenAdmin"
- ValidateIssuerSigningKey = true
- IssuerSigningKey = <配置中的密钥>
- ValidateLifetime = true       // 验证生命周期
- ClockSkew = TimeSpan.Zero
```

### 7.2 认证流程

1. **登录:** POST `/api/auth/login` with username and password
2. **获取 Token:** Server validates and returns JWT token
3. **API 访问:** Client includes token in header: `Authorization: Bearer {token}`
4. **刷新:** Call `/api/auth/refresh` to get new token

### 7.3 CORS 配置

允许的源:
- `http://localhost:5666` (前端开发服务器)
- `http://localhost:5173` (备用前端端口)

---

## 8. 开发说明

### 8.1 启动项目

#### 后端启动
```bash
cd D:\03_Projects\Personal\MyWebSite\WebApplication1\WebApplication1
dotnet run --project WebApplication1 --environment Development
# 运行在 http://localhost:5000
```

#### 前端启动
```bash
cd D:\03_Projects\Personal\MyWebSite\vue-vben-admin
pnpm dev
# 运行在 http://localhost:5666
```

### 8.2 数据库迁移

```bash
cd D:\03_Projects\Personal\MyWebSite\WebApplication1\WebApplication1

# 创建迁移
dotnet ef migrations add <MigrationName>

# 应用迁移到数据库
dotnet ef database update

# 删除数据库后重新创建
dotnet ef database drop --force
dotnet ef database update

# 查看迁移状态
dotnet ef migrations list

# 移除最后一个迁移（如果还没应用）
dotnet ef migrations remove
```

### 8.3 Swagger UI

开发模式下访问: http://localhost:5000/swagger

### 8.4 数据库种子数据

开发模式下，应用程序启动时会自动填充以下示例数据:

**用户 (3个):**
- vben / 123456 (super)
- admin / 123456 (admin)
- jack / 123456 (user)

**项目 (6个):**
- 生产线升级项目
- 质量改进项目
- 设备维护项目
- 新产品导入项目
- 能耗优化项目
- 安全生产项目

**任务类型 (9个):**
- 设备调试、问题处理、日常维护、生产作业、质量检测、数据记录、培训学习、会议讨论、其他

**设备 (13个):**
- A/B/C/D/E线体、测试设备、机器人手臂等

**工作日志 (~90条):**
- 过去14天每天3-5条记录

**每日计划 (20+条):**
- 今日计划5条（含已完成、进行中、待执行）
- 明日计划4条
- 过去3天已完成计划

**导入批次:**
- 1条已完成导入记录

---

## 9. 枚举定义

### 9.1 后端枚举 (WorkEnums.cs)

```csharp
// 工作项目类型
enum WorkProjectType {
    Internal = 0,   // 内部项目
    External = 1,    // 外部项目
    RAndD = 2,      // 研发项目
    Support = 3,     // 支持项目
    Other = 4        // 其他
}

// 工作项目状态
enum WorkProjectStatus {
    Active = 0,      // 进行中
    Completed = 1,   // 已完成
    Suspended = 2,   // 已暂停
    Archived = 3      // 已归档
}

// 工作设备类型
enum WorkDeviceType {
    ProductionLine = 0,  // 生产线
    Equipment = 1,       // 设备
    TestingDevice = 2,  // 测试设备
    Other = 3            // 其他
}

// 工作设备状态
enum WorkDeviceStatus {
    Active = 0,        // 正常
    Inactive = 1,     // 停用
    Maintenance = 2    // 维护中
}

// 工作日志来源类型
enum WorkLogSourceType {
    Manual = 0,           // 手动
    ExcelImport = 1,      // Excel导入
    PlanConversion = 2    // 计划转换
}

// 工作日志状态
enum WorkLogStatus {
    Normal = 0,           // 正常
    MissingData = 1,       // 缺失数据
    PendingSupplement = 2  // 待补充
}

// 工作计划状态
enum WorkDailyPlanStatus {
    Pending = 0,      // 待执行
    InProgress = 1,  // 进行中
    Completed = 2,   // 已完成
    Cancelled = 3    // 已取消
}

// 工作计划优先级
enum WorkDailyPlanPriority {
    Low = 1,      // 低
    Medium = 2,    // 中
    High = 3,     // 高
    Urgent = 4    // 紧急
}

// 导入策略
enum WorkImportStrategy {
    SkipDuplicate = 0,     // 跳过重复
    OverwriteDuplicate = 1, // 覆盖重复
    Merge = 2              // 合并导入
}

// 导入状态
enum WorkImportStatus {
    Pending = 0,     // 等待处理
    Processing = 1, // 处理中
    Completed = 2,  // 已完成
    Failed = 3      // 失败
}

// 导入验证状态
enum WorkImportValidationStatus {
    Valid = 0,    // 有效
    Warning = 1,  // 警告
    Error = 2     // 错误
}

// 模板字段类型
enum FieldType {
    Text = 0,
    Number = 1,
    Date = 2,
    Select = 3,
    MultiSelect = 4,
    Textarea = 5,
    File = 6
}

// 成长项目状态
enum GrowthProjectStatus {
    InProgress = 0,
    Completed = 1,
    OnHold = 2
}

// 成长项目类型
enum GrowthProjectType {
    Personal = 0,
    Professional = 1,
    Academic = 2
}

// 考研任务状态
enum PostgraduateTaskStatus {
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

// 考研任务优先级
enum PostgraduateTaskPriority {
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

// 考研任务类型
enum PostgraduateTaskType {
    Study = 0,
    Review = 1,
    Practice = 2,
    Test = 3
}

// 错题状态
enum ExamMistakeStatus {
    Pending = 0,
    Reviewed = 1,
    Mastered = 2
}

// 资料类型
enum ExamMaterialType {
    Note = 0,
    Summary = 1,
    Formula = 2,
    Diagram = 3
}

// 用户状态
enum AppUserStatus {
    Active = 0,
    Inactive = 1,
    Banned = 2
}
```

### 9.2 前端枚举 (workEnum.ts)

```typescript
// 工作日志来源类型
enum WorkLogSourceType {
  Manual = 0,
  ExcelImport = 1,
  PlanConversion = 2,
}

// 工作日志状态
enum WorkLogStatus {
  Normal = 0,
  MissingData = 1,
  PendingSupplement = 2,
}

// 工作项目状态
enum WorkProjectStatus {
  Active = 0,
  Completed = 1,
  Suspended = 2,
  Archived = 3,
}

// 工作设备状态
enum WorkDeviceStatus {
  Active = 0,
  Inactive = 1,
  Maintenance = 2,
}

// 工作计划状态
enum WorkDailyPlanStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3,
}

// 工作计划优先级
enum WorkDailyPlanPriority {
  Low = 1,
  Medium = 2,
  High = 3,
  Urgent = 4,
}

// 导入策略
enum WorkImportStrategy {
  SkipDuplicate = 0,
  OverwriteDuplicate = 1,
  Merge = 2,
}

// 导入状态
enum WorkImportStatus {
  Pending = 0,
  Processing = 1,
  Completed = 2,
  Failed = 3,
}
```

---

## 10. Excel 导入功能

### 10.1 导入模板格式

Excel 文件应包含以下列：

| 列名 | 说明 | 示例 |
|------|------|------|
| 日期 | 工作日期，格式 YYYY-MM-DD | 2024-05-01 |
| 项目名称 | 关联的项目名称 | 生产线升级项目 |
| 设备名称 | 关联的设备名称 | A线体 |
| 任务类型 | 任务类型名称 | 设备调试 |
| 工作内容 | 具体工作描述 | 完成设备调试和参数优化 |
| 工时(小时) | 工作时长（小时） | 4 |
| 备注 | 额外说明 | - |

### 10.2 导入流程

1. 下载模板: GET `/api/work/imports/worklog/template`
2. 填写 Excel 文件
3. 上传预览: POST `/api/work/imports/worklog/preview`
4. 检查预览结果，修正错误
5. 执行导入: POST `/api/work/imports/worklog/execute`

### 10.3 导入策略

- **跳过重复**: 如果数据已存在，则跳过
- **覆盖重复**: 如果数据已存在，则覆盖
- **合并导入**: 保留旧数据，添加新数据

---

## 11. 常见问题

### 11.1 DateOnly 类型转换问题

MySQL 的 `DATE` 类型无法直接映射到 C# 的 `DateOnly`。解决方案是在 EF Core 配置中使用 `ValueConverter`:

```csharp
var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
    v => v.ToDateTime(TimeOnly.MinValue),
    v => DateOnly.FromDateTime(v));

entity.Property(x => x.PlanDate).HasConversion(dateOnlyConverter);
```

### 11.2 Guid 类型前端兼容性

前端传过来的 `projectId` 可能是空字符串或字符串格式，需要在 Service 层进行解析:

```csharp
Guid? projectId = null;
if (!string.IsNullOrWhiteSpace(input.ProjectId) && Guid.TryParse(input.ProjectId, out var parsed))
{
    projectId = parsed;
}
```

### 11.3 后端文件锁定

如果 `dotnet build` 报 "文件被锁定" 错误，需要先停止正在运行的后端进程。

### 11.4 迁移问题

如果数据库迁移失败，检查以下几点:
1. 确保 MySQL 服务正在运行
2. 确保数据库用户有足够的权限
3. 如果表已存在但结构不匹配，可以删除数据库重新创建
4. 使用 `dotnet ef database drop --force` 删除数据库，然后用 `dotnet ef database update` 重新创建

### 11.5 BCrypt 密码问题

密码使用 BCrypt.Net-Next 进行哈希存储。登录时，服务端自动进行密码验证。

---

## 12. 项目特点

### 12.1 Feature-Based 架构

后端采用 Feature-Based (功能模块化) 架构，每个功能模块包含:
- Controllers (控制器)
- Services (服务)
- Entities (实体)
- DTOs (数据传输对象)

### 12.2 前后端分离

- 前端: Vue 3 + Vite + Ant Design Vue + Pinia + Vue Router
- 后端: ASP.NET Core 10 + Entity Framework Core + MySQL
- 通信: RESTful API + JWT Authentication

### 12.3 模块化设计

项目分为多个功能模块:
- **个人成长**: 习惯、知识库、考研备烤、成长项目等
- **工作中心**: 项目、设备、任务类型、工作日志、统计等
- **系统管理**: 用户、角色、菜单、标签等
- **占位模块**: AI、分析、资产、内容、实验室、平台等 (功能待实现)

### 12.4 本地存储

部分前端数据使用 localStorage 持久化，如日程计划的时间安排。

---

## 13. 项目贡献者

- 初始开发: Vben Admin Template
- 后端定制: .NET 10 + EF Core
- 前端定制: Vue 3 + Ant Design Vue

---

*文档最后更新: 2026-05-01*
