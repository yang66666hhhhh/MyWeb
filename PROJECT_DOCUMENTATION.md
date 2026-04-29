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
| OpenAPI | Microsoft.AspNetCore.OpenApi | 10.0.5 |
| Excel处理 | ClosedXML | 0.102.2 |

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
    ├── Features/                     # 功能模块
    ├── Shared/                       # 共享模块
    ├── Migrations/                   # 数据库迁移
    └── ...
```

### 2.2 后端目录结构

```
WebApplication1/
├── Features/                          # 功能模块 (Feature-Based)
│   ├── Auth/                        # 认证模块
│   │   ├── AuthController.cs
│   │   ├── AuthService.cs
│   │   └── IAuthService.cs
│   ├── DailyPlans/                  # 个人每日计划模块
│   │   ├── DailyPlan.cs
│   │   ├── DailyPlanDto.cs
│   │   ├── DailyPlanQueryDto.cs
│   │   ├── CreateDailyPlanDto.cs
│   │   ├── UpdateDailyPlanDto.cs
│   │   ├── DailyPlanService.cs
│   │   ├── DailyPlansController.cs
│   │   └── IDailyPlanService.cs
│   ├── Menu/                        # 菜单模块
│   │   └── MenuController.cs
│   ├── User/                        # 用户模块
│   │   └── UserController.cs
│   ├── Timezone/                    # 时区模块
│   └── Work/                        # 工作中心模块
│       ├── Controllers/
│       │   ├── WorkDailyPlansController.cs
│       │   ├── WorkDevicesController.cs
│       │   ├── WorkImportsController.cs
│       │   ├── WorkLogsController.cs
│       │   ├── WorkProjectsController.cs
│       │   ├── WorkStatisticsController.cs
│       │   └── WorkTaskTypesController.cs
│       ├── Dtos/
│       │   ├── WorkDailyPlanDtos.cs
│       │   ├── WorkDeviceDtos.cs
│       │   ├── WorkImportDtos.cs
│       │   ├── WorkLogDtos.cs
│       │   ├── WorkProjectDtos.cs
│       │   ├── WorkStatisticsDtos.cs
│       │   └── WorkTaskTypeDtos.cs
│       ├── Entities/
│       │   ├── WorkDailyPlan.cs
│       │   ├── WorkDevice.cs
│       │   ├── WorkImport.cs
│       │   ├── WorkLog.cs
│       │   ├── WorkProject.cs
│       │   └── WorkTaskType.cs
│       └── Services/
│           ├── IWork/
│           │   └── IServices.cs
│           ├── WorkDailyPlanService.cs
│           ├── WorkDeviceService.cs
│           ├── WorkImportService.cs
│           ├── WorkLogService.cs
│           ├── WorkProjectService.cs
│           ├── WorkStatisticsService.cs
│           └── WorkTaskTypeService.cs
│
├── Shared/                           # 共享模块
│   ├── Common/                      # 公共工具类
│   │   ├── ApiResult.cs             # API 统一响应
│   │   ├── PageQueryDto.cs          # 分页查询基类
│   │   └── PageResult.cs            # 分页结果
│   ├── Data/                        # 数据层
│   │   ├── AppDbContext.cs          # EF Core 上下文
│   │   └── DbSeeder.cs              # 数据库种子数据
│   ├── Enums/                       # 枚举定义
│   │   ├── DailyPlanStatus.cs
│   │   └── WorkEnums.cs
│   └── EntityBase.cs                # 实体基类
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
apps/web-antd/
├── index.html                       # 入口 HTML
├── package.json                     # 应用依赖配置
├── vite.config.ts                   # Vite 构建配置
├── tsconfig.json                   # TypeScript 配置
├── .env                            # 环境变量基础
├── .env.development                # 开发环境变量
├── .env.production                # 生产环境变量
├── public/
│   └── favicon.ico
└── src/
    ├── main.ts                     # 应用入口
    ├── app.vue                     # 根组件
    ├── bootstrap.ts                # 应用初始化
    ├── preferences.ts             # 偏好设置
    │
    ├── types/                     # 全局类型中心
    │   ├── global.d.ts
    │   ├── env.d.ts
    │   └── api.ts
    │
    ├── constants/                  # 常量
    │   ├── app.ts
    │   ├── route.ts
    │   └── permissions.ts
    │
    ├── enums/                    # 枚举
    │   ├── appEnum.ts
    │   ├── growthEnum.ts
    │   ├── workEnum.ts
    │   └── systemEnum.ts
    │
    ├── api/                      # API 请求模块
    │   ├── index.ts
    │   ├── request.ts             # 请求客户端配置
    │   ├── core/                 # 核心 API
    │   │   ├── auth.ts
    │   │   ├── user.ts
    │   │   └── menu.ts
    │   │
    │   ├── growth/               # 成长模块 API
    │   │   ├── index.ts
    │   │   ├── types.ts
    │   │   ├── habit.ts
    │   │   ├── daily-plan.ts
    │   │   ├── work-log.ts
    │   │   ├── knowledge-base.ts
    │   │   ├── project.ts
    │   │   ├── postgraduate.ts
    │   │   └── work/
    │   │       ├── index.ts
    │   │       ├── workLog.ts
    │   │       ├── workDailyPlan.ts
    │   │       ├── workProject.ts
    │   │       ├── workDevice.ts
    │   │       ├── workTaskType.ts
    │   │       ├── workImport.ts
    │   │       └── workStatistics.ts
    │   │
    │   └── system/               # 系统 API
    │       ├── menu.ts
    │       └── auth.ts
    │
    ├── composables/              # 组合式函数
    │   └── usePagedQuery.ts
    │
    ├── router/                   # 路由配置
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
    │           ├── system.ts
    │           ├── demos.ts
    │           ├── analytics.ts
    │           ├── assets.ts
    │           ├── ai.ts
    │           └── vben.ts
    │
    ├── layouts/                  # 布局组件
    │   ├── index.ts
    │   ├── basic.vue
    │   ├── auth.vue
    │   └── components/
    │
    ├── views/                    # 页面视图
    │   ├── _core/               # 核心视图
    │   │   ├── authentication/
    │   │   ├── profile/
    │   │   └── fallback/
    │   │
    │   ├── dashboard/           # 仪表盘
    │   │   ├── workspace/
    │   │   └── analytics/
    │   │
    │   ├── growth/             # 个人成长
    │   │   ├── dashboard/
    │   │   ├── daily-plans/
    │   │   ├── habits/
    │   │   ├── work-log/
    │   │   ├── knowledge-base/
    │   │   ├── postgraduate/
    │   │   │   ├── index.vue
    │   │   │   ├── materials/
    │   │   │   ├── mistakes/
    │   │   │   └── study-plans/
    │   │   └── project/
    │   │
    │   ├── work/              # 工作中心
    │   │   ├── dashboard/
    │   │   ├── daily-plan/
    │   │   ├── task-type/
    │   │   ├── device/
    │   │   ├── project/
    │   │   ├── log/
    │   │   ├── import/
    │   │   └── statistics/
    │   │
    │   ├── analytics/
    │   ├── assets/
    │   ├── ai/
    │   ├── demos/
    │   ├── system/
    │   ├── platform/
    │   ├── network/
    │   ├── content/
    │   ├── labs/
    │   ├── external-links/
    │   └── about/
    │
    ├── locales/                # 国际化
    ├── enums/                  # 枚举定义
    │   ├── appEnum.ts
    │   ├── growthEnum.ts
    │   ├── workEnum.ts
    │   └── systemEnum.ts
    │
    ├── store/                  # Pinia 状态管理
    │   ├── index.ts
    │   ├── auth.ts
    │   └── modules/
    │
    ├── adapter/                # 组件适配器
    ├── plugins/                # 插件系统
    └── workers/                 # Web Worker
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

**路由前缀:** `/api/work/import`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/` | 是 | 获取分页导入批次列表 |
| POST | `/preview` | 是 | 预览 Excel 数据 |
| POST | `/execute` | 是 | 执行导入 |
| GET | `/template` | 是 | 下载导入模板 |

#### 3.1.10 菜单模块 (Features/Menu)

**路由前缀:** `/api/menu`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/all` | 是 | 获取所有菜单 |

#### 3.1.11 用户模块 (Features/User)

**路由前缀:** `/api/user`

| 方法 | 端点 | 需要认证 | 描述 |
|------|------|---------|------|
| GET | `/info` | 是 | 获取当前用户信息 |

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
| `/growth/postgraduate` | `views/growth/postgraduate/index.vue` | 备考中心 |
| `/growth/postgraduate/materials` | `views/growth/postgraduate/materials/index.vue` | 备考资料 |
| `/growth/postgraduate/mistakes` | `views/growth/postgraduate/mistakes/index.vue` | 错题本 |
| `/growth/postgraduate/study-plans` | `views/growth/postgraduate/study-plans/index.vue` | 学习计划 |
| `/growth/projects` | `views/growth/project/index.vue` | 项目管理 |

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

#### 3.2.4 其他页面

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

#### WorkProjects (工作项目表)

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

#### WorkDevices (工作设备表)

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

#### WorkTaskTypes (工作任务类型表)

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

#### WorkLogs (工作日志表)

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
| ImportBatchId | GUID | | 导入批次ID |
| Remark | VARCHAR(1000) | | 备注 |
| CreatedAt | DATETIME | | 创建时间 |
| UpdatedAt | DATETIME | | 更新时间 |

**索引:** WorkDate, Status, ProjectId

#### WorkLogItems (工作日志明细表)

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

#### WorkDailyPlans (工作计划表)

| 字段 | 类型 | 约束 | 描述 |
|------|------|------|------|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| PlanDate | DATE | NOT NULL | 计划日期 |
| Title | VARCHAR(120) | NOT NULL | 标题 |
| Content | VARCHAR(1000) | | 内容 |
| ProjectId | GUID | FK | 项目ID |
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

#### WorkImportBatches (工作导入批次表)

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

#### WorkImportRows (工作导入行表)

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

---

## 5. API 端点汇总

### 5.1 认证 API

| 方法 | 端点 | 描述 |
|------|------|------|
| POST | `/api/auth/login` | 用户登录 |
| GET | `/api/auth/codes` | 获取权限码 |
| POST | `/api/auth/refresh` | 刷新令牌 |
| POST | `/api/auth/logout` | 用户登出 |

### 5.2 用户 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/user/info` | 获取当前用户信息 |

### 5.3 菜单 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/menu/all` | 获取所有菜单 |

### 5.4 个人每日计划 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/daily-plans` | 获取分页列表 |
| GET | `/api/daily-plans/{id}` | 获取单个 |
| POST | `/api/daily-plans` | 创建 |
| PUT | `/api/daily-plans/{id}` | 更新 |
| PATCH | `/api/daily-plans/{id}/complete` | 完成 |
| DELETE | `/api/daily-plans/{id}` | 删除 |

### 5.5 工作项目 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/projects` | 获取分页列表 |
| GET | `/api/work/projects/{id}` | 获取单个 |
| POST | `/api/work/projects` | 创建 |
| PUT | `/api/work/projects/{id}` | 更新 |
| DELETE | `/api/work/projects/{id}` | 删除 |

### 5.6 工作设备 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/devices` | 获取分页列表 |
| GET | `/api/work/devices/{id}` | 获取单个 |
| POST | `/api/work/devices` | 创建 |
| PUT | `/api/work/devices/{id}` | 更新 |
| DELETE | `/api/work/devices/{id}` | 删除 |

### 5.7 工作任务类型 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/task-types` | 获取分页列表 |
| GET | `/api/work/task-types/{id}` | 获取单个 |
| POST | `/api/work/task-types` | 创建 |
| PUT | `/api/work/task-types/{id}` | 更新 |
| DELETE | `/api/work/task-types/{id}` | 删除 |

### 5.8 工作日志 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/logs` | 获取分页列表 |
| GET | `/api/work/logs/{id}` | 获取单个 |
| POST | `/api/work/logs` | 创建 |
| PUT | `/api/work/logs/{id}` | 更新 |
| DELETE | `/api/work/logs/{id}` | 删除 |

### 5.9 工作计划 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/daily-plans` | 获取分页列表 |
| GET | `/api/work/daily-plans/{id}` | 获取单个 |
| POST | `/api/work/daily-plans` | 创建 |
| PUT | `/api/work/daily-plans/{id}` | 更新 |
| DELETE | `/api/work/daily-plans/{id}` | 删除 |
| POST | `/api/work/daily-plans/{id}/complete` | 完成 |
| POST | `/api/work/daily-plans/convert-to-log` | 转换为日志 |

### 5.10 统计分析 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/statistics/overview` | 统计概览 |
| GET | `/api/work/statistics/daily-hours` | 每日工时 |
| GET | `/api/work/statistics/project-hours` | 项目工时 |
| GET | `/api/work/statistics/task-type-distribution` | 任务类型分布 |
| GET | `/api/work/statistics/device-ranking` | 设备排名 |

### 5.11 数据导入 API

| 方法 | 端点 | 描述 |
|------|------|------|
| GET | `/api/work/import` | 获取分页导入批次 |
| POST | `/api/work/import/preview` | 预览 Excel 数据 |
| POST | `/api/work/import/execute` | 执行导入 |
| GET | `/api/work/import/template` | 下载导入模板 |

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
cd D:\03_Projects\Personal\MyWebSite\WebApplication1
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
```

### 9.2 前端枚举 (workEnum.ts)

```typescript
// 工作日志来源类型
enum WorkLogSourceType {
  Manual = 0,           // 手动
  ExcelImport = 1,      // Excel导入
  PlanConversion = 2,    // 计划转换
}

// 工作日志状态
enum WorkLogStatus {
  Normal = 0,                  // 正常
  MissingData = 1,             // 缺失数据
  PendingSupplement = 2,        // 待补充
}

// 工作项目状态
enum WorkProjectStatus {
  Active = 0,     // 进行中
  Completed = 1,  // 已完成
  Suspended = 2,  // 已暂停
  Archived = 3,   // 已归档
}

// 工作设备状态
enum WorkDeviceStatus {
  Active = 0,       // 正常
  Inactive = 1,     // 停用
  Maintenance = 2,  // 维护中
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
  Low = 1,     // 低
  Medium = 2,   // 中
  High = 3,    // 高
  Urgent = 4   // 紧急
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

1. 下载模板: GET `/api/work/import/template`
2. 填写 Excel 文件
3. 上传预览: POST `/api/work/import/preview`
4. 检查预览结果，修正错误
5. 执行导入: POST `/api/work/import/execute`

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

---

## 12. 项目贡献者

- 初始开发: Vben Admin Template
- 后端定制: .NET 10 + EF Core
- 前端定制: Vue 3 + Ant Design Vue

---

*文档最后更新: 2026-04-30*