# Personal Growth Management System - 完整项目文档

## 1. 项目概述

### 1.1 项目介绍

**个人成长 + 工作管理 + 多职业平台** 全栈系统，采用 **Vue 3 + Vite + Ant Design Vue** 前端和 **ASP.NET Core 10 + Entity Framework Core + MySQL** 后端。

项目不是传统企业 OA，而是一个**个人主导的平台 + 多职业用户生态**。

### 1.2 技术栈

| 层 | 技术 | 版本 |
|---|---|---|
| **前端框架** | Vue 3 | ^3.5.x |
| **UI 库** | Ant Design Vue | ^4.x |
| **构建工具** | Vite | ^6.x |
| **状态管理** | Pinia | ^3.x |
| **路由** | Vue Router | ^4.x |
| **语言** | TypeScript | ^5.x |
| **前端脚手架** | vue-vben-admin | 5.7.0 |
| **后端框架** | ASP.NET Core | 10.0 |
| **数据库** | MySQL | 10.0.1 |
| **ORM** | Entity Framework Core | 10.0.1 |
| **认证** | JWT Bearer | 10.0.7 |

### 1.3 项目地址

- 前端：`vue-vben-admin/`
- 后端：`WebApplication1/`

---

## 2. 权限体系

### 2.1 角色模型（Role）

| 角色 | 等级 | 说明 |
|---|---|---|
| **Member** | 1 | 基础功能 |
| **Pro** | 2 | 高级功能 |
| **Owner** | 3 | 系统管理 |

### 2.2 职业身份（Persona）

一个用户可拥有**多个 Persona**（多对多关系）：

| Persona | 说明 |
|---|---|
| **Developer** | 开发者 |
| **Designer** | 设计师 |
| **Teacher** | 教师 |
| **Student** | 学生 |
| **Implementation** | 实施工程师 |
| **General** | 通用 |
| **Sales** | 销售 |
| **Freelancer** | 自由职业者 |

### 2.3 功能点系统（Feature）

每个功能点有唯一 Code，按 Category 分组：
- **Work**: WORK_LOG, WORK_PROJECT, WORK_DEVICE, WORK_TASK, WORK_IMPORT, WORK_TEMPLATE, WORK_OKR, WORK_GANTT, WORK_RISK
- **Growth**: TASK_UNIFIED, GROWTH_DAILY_PLAN, GROWTH_HABIT, GROWTH_KNOWLEDGE, GROWTH_SKILL, GROWTH_FITNESS, GROWTH_READING, GROWTH_MOOD
- **AI**: AI_ASSISTANT, AI_PLANNER, AI_REPORT, AI_INSIGHTS, AI_KNOWLEDGE_CHAT, AI_AUTOMATION
- **Assets**: ASSET_DASHBOARD, ASSET_INCOME, ASSET_EXPENSE, ASSET_BUDGET, ASSET_INVEST
- **Analytics**: ANALYTICS_GROWTH, ANALYTICS_WORK, ANALYTICS_FINANCE, ANALYTICS_TIME, ANALYTICS_HABITS, ANALYTICS_CUSTOM, ANALYTICS_AI
- **Persona**: DEV_*, DESIGN_*, TEACHER_*, IMPL_*, STUDENT_*, SALES_*, FREELANCER_*

### 2.4 权限公式

```text
可见菜单 = 当前菜单和所有父级菜单都满足：
         RoleLevel >= MinRoleLevel
         && IsEnabled && IsVisible
         && (PersonaTag == null || User.Personas.Contains(PersonaTag))
         && (FeatureCode == null || User.AvailableFeatures.Contains(FeatureCode))

可用功能 =
  非 Persona 类套餐功能
  ∪ (Persona 类套餐功能 ∩ 用户启用 Persona 功能)
  ∪ Owner 全部启用功能
```

前端页面内部也必须按 `accessCodes` 降级：用户没有对应 FeatureCode 时，不请求该接口，不显示对应统计卡、下拉、快捷入口和操作按钮。

---

## 3. 菜单架构

### 3.1 菜单表结构（RoleMenus）

| 字段 | 类型 | 说明 |
|---|---|---|
| Id | GUID | 主键 |
| ParentId | GUID? | 父菜单ID |
| Name | VARCHAR(100) | 菜单名称 |
| Path | VARCHAR(200) | 路由路径 |
| Icon | VARCHAR(50) | 图标 |
| Component | VARCHAR(200) | 前端组件 |
| Sort | INT | 排序 |
| MinRoleLevel | INT | 最低角色等级 |
| PersonaTag | VARCHAR(50)? | 职业身份标签 |
| FeatureCode | VARCHAR(100)? | 功能点编码 |
| MenuCategory | VARCHAR(50) | 菜单分类 |
| IsBaseMenu | BIT | 是否基础菜单 |
| IsVisible | BIT | 是否显示 |
| IsEnabled | BIT | 是否启用 |

### 3.2 完整菜单树

```
├── 工作台 (/dashboard/workspace) [Base, Dashboard]
│   └── 分析中心 (/dashboard/analytics) [Owner]
│       ├── 访问分析
│       ├── 销售分析
│       ├── 数据报表
│       └── 趋势分析
│
├── 个人成长 (/growth) [Base, Growth]
│   ├── 成长看板 (/growth/dashboard) [Base]
│   ├── 每日计划 (/growth/daily-plans) [Base, GROWTH_DAILY_PLAN]
│   ├── 习惯打卡 (/growth/habits) [Base, GROWTH_HABIT]
│   ├── 知识库 (/growth/knowledge-base) [Pro, GROWTH_KNOWLEDGE]
│   ├── 技能管理 (/growth/skills) [Pro, GROWTH_SKILL]
│   ├── 阅读清单 (/growth/reading-list) [Pro]
│   ├── 心情日记 (/growth/mood-tracker) [Pro]
│   ├── 健身记录 (/growth/fitness) [Pro, GROWTH_FITNESS]
│   ├── 专注计时 (/growth/focus-timer) [Pro]
│   ├── 月度复盘 (/growth/monthly-review) [Pro]
│   ├── 年度规划 (/growth/year-plans) [Pro]
│   ├── 课程学习 (/growth/courses) [Pro]
│   ├── 学习路径 (/growth/learning-path) [Pro]
│   ├── 目标管理 (/growth/goals) [Pro]
│   └── 睡眠追踪 (/growth/sleep) [Pro]
│
├── 工作中心 (/work) [Base, Work]
│   ├── 工作看板 (/work/dashboard) [Base]
│   ├── 工作日志 (/work/log) [Base, WORK_LOG]
│   ├── 实施日志 (/work/impl-log) [Pro, Implementation]
│   ├── 统一任务 (/work/tasks) [Base, WORK_TASK]
│   ├── 工作项目 (/work/project) [Pro, WORK_PROJECT]
│   ├── 周计划 (/work/weekly-plan) [Pro]
│   ├── OKR管理 (/work/okr) [Pro]
│   ├── 甘特图 (/work/gantt) [Pro]
│   ├── 风险管理 (/work/risk-control) [Pro]
│   ├── 设备管理 (/work/device) [Pro, WORK_DEVICE]
│   ├── 软件资产 (/work/software-assets) [Pro]
│   ├── 数据导入 (/work/import) [Pro, WORK_IMPORT]
│   ├── 模板管理 (/work/templates) [Pro]
│   ├── 统计分析 (/work/statistics) [Pro]
│   └── 文件中心 (/work/files) [Pro]
│
├── AI 助手 (/ai) [Pro, AI]
│   ├── AI 助手 (/ai/assistant) [Pro, AI_ASSISTANT]
│   ├── AI 规划器 (/ai/planner) [Pro, AI_PLANNER]
│   ├── AI 报告 (/ai/reports) [Owner, AI_REPORT]
│   ├── 知识库问答 (/ai/knowledge-chat) [Pro]
│   ├── AI 洞察 (/ai/insights) [Owner, AI_INSIGHTS]
│   └── 自动化 (/ai/automation) [Pro]
│
├── 财务资产 (/assets) [Pro, Assets]
│   ├── 资产看板 (/assets/dashboard) [Pro, ASSET_DASHBOARD]
│   ├── 收入记录 (/assets/income) [Pro, ASSET_INCOME]
│   ├── 支出记录 (/assets/expenses) [Pro, ASSET_EXPENSE]
│   ├── 预算管理 (/assets/budget) [Pro, ASSET_BUDGET]
│   ├── 投资管理 (/assets/investments) [Pro, ASSET_INVEST]
│   └── 资源中心 (/assets/resources) [Pro]
│
├── 数据分析 (/analytics) [Owner, Analytics]
│   ├── 成长分析 (/analytics/growth) [Owner, ANALYTICS_GROWTH]
│   ├── 工作分析 (/analytics/work) [Owner, ANALYTICS_WORK]
│   ├── 财务分析 (/analytics/finance) [Owner, ANALYTICS_FINANCE]
│   ├── 时间分析 (/analytics/time) [Owner, ANALYTICS_TIME]
│   ├── 习惯分析 (/analytics/habits) [Owner, ANALYTICS_HABITS]
│   ├── 自定义报表 (/analytics/custom-reports) [Owner, ANALYTICS_CUSTOM]
│   └── AI洞察 (/analytics/ai-insights) [Owner, ANALYTICS_AI]
│
├── 平台管理 (/system) [Pro, System]
│   ├── 用户管理 (/system/user) [Pro]
│   ├── 角色菜单 (/system/role-menu) [Owner]
│   ├── 身份类型 (/system/persona) [Owner]
│   └── 菜单标签 (/system/menu-tag) [Owner]
│
├── 开发者中心 (/dev) [Persona=Developer]
│   ├── 代码仓库 (/dev/code-repository)
│   ├── 问题跟踪 (/dev/issues)
│   └── 流水线 (/dev/pipelines)
│
├── 设计师中心 (/design) [Persona=Designer]
│   ├── 设计资产 (/design/assets)
│   └── 原型管理 (/design/prototypes)
│
├── 教师中心 (/teacher) [Persona=Teacher]
│   ├── 课程管理 (/teacher/courses)
│   └── 学生管理 (/teacher/students)
│
├── 实施中心 (/implementation) [Persona=Implementation]
│   ├── 项目看板 (/implementation/kanban)
│   └── 客户管理 (/implementation/customers)
│
├── 学生中心 (/student) [Persona=Student]
│   ├── 学习总览 (/student/dashboard)
│   ├── 学习计划 (/student/learning)
│   ├── 复习日程 (/student/review)
│   ├── 错题本 (/student/mistakes)
│   ├── 学习资料 (/student/materials)
│   ├── 学习记录 (/student/records)
│   └── 科目目标 (/student/subjects)
│
├── 实验中心 (/labs) [Pro]
│   ├── AI实验室 (/labs/ai-lab)
│   ├── 数据实验室 (/labs/data-lab)
│   ├── 模板市场 (/labs/templates)
│   └── UI组件 (/labs/ui-components)
│
└── 个人中心 (/profile) [Base]
    ├── 基本设置
    ├── 安全设置
    ├── 密码修改
    └── 通知设置
```

---

## 4. 数据库架构

### 4.1 认证相关表

**Users (用户表)**

| 字段 | 类型 | 约束 | 描述 |
|---|---|---|---|
| Id | GUID | PK | 主键 |
| Username | VARCHAR(50) | NOT NULL, UNIQUE | 用户名 |
| PasswordHash | VARCHAR(100) | NOT NULL | 密码哈希 |
| RealName | VARCHAR(100) | NOT NULL | 真实姓名 |
| Avatar | VARCHAR(500) | | 头像URL |
| Email | VARCHAR(100) | | 邮箱 |
| Roles | VARCHAR(100) | Default 'member' | 角色 (member/pro/owner) |
| Status | INT | Default 0 | 状态 |
| CreatedAt | DATETIME | | 创建时间 |

**PersonaTypes (职业身份表)**

| 字段 | 类型 | 约束 | 描述 |
|---|---|---|---|
| Id | GUID | PK | 主键 |
| Code | VARCHAR(50) | NOT NULL, UNIQUE | 编码 |
| Name | VARCHAR(50) | NOT NULL | 名称 |
| Icon | VARCHAR(50) | | 图标 |
| Description | VARCHAR(500) | | 描述 |
| Sort | INT | | 排序 |
| IsActive | BIT | Default TRUE | 是否启用 |

**UserPersonas (用户-身份多对多表)**

| 字段 | 类型 | 约束 | 描述 |
|---|---|---|---|
| Id | GUID | PK | 主键 |
| UserId | GUID | FK | 用户ID |
| PersonaTypeId | GUID | FK | 身份类型ID |
| IsPrimary | BIT | Default FALSE | 是否主身份 |
| CreatedAt | DATETIME | | 创建时间 |

### 4.2 订阅系统表

**Features (功能点表)**

| 字段 | 类型 | 约束 | 描述 |
|---|---|---|---|
| Id | GUID | PK | 主键 |
| Code | VARCHAR(100) | NOT NULL, UNIQUE | 功能编码 |
| Name | VARCHAR(100) | NOT NULL | 功能名称 |
| Category | VARCHAR(50) | NOT NULL | 分类 |
| Description | VARCHAR(500) | | 描述 |
| IsEnabled | BIT | TRUE | 是否启用 |

**Plans (订阅计划表)**

| 字段 | 类型 | 约束 | 描述 |
|---|---|---|---|
| Id | GUID | PK | 主键 |
| Code | VARCHAR(50) | NOT NULL, UNIQUE | 编码 (Free/Pro/Team) |
| Name | VARCHAR(100) | NOT NULL | 名称 |
| Price | DECIMAL(10,2) | | 价格 |
| BillingCycle | INT | | 计费周期 (天) |

**PlanFeatures (计划-功能映射)**

| 字段 | 类型 | 约束 | 描述 |
|---|---|---|---|
| Id | GUID | PK | 主键 |
| PlanId | GUID | FK | 计划ID |
| FeatureId | GUID | FK | 功能ID |

**PersonaFeatures (Persona-功能映射)**

| 字段 | 类型 | 约束 | 描述 |
|---|---|---|---|
| Id | GUID | PK | 主键 |
| PersonaCode | VARCHAR(50) | NOT NULL | Persona编码 |
| FeatureId | GUID | FK | 功能ID |

**UserSubscriptions (用户订阅记录)**

| 字段 | 类型 | 约束 | 描述 |
|---|---|---|---|
| Id | GUID | PK | 主键 |
| UserId | GUID | FK | 用户ID |
| PlanId | GUID | FK | 计划ID |
| StartAt | DATETIME | | 开始时间 |
| ExpireAt | DATETIME? | | 过期时间 |
| IsActive | BIT | TRUE | 是否有效 |

### 4.3 工作模块表

**WorkLogs (工作日志表)**

| 字段 | 类型 | 约束 | 描述 |
|---|---|---|---|
| Id | GUID | PK | 主键 |
| UserId | GUID | | 用户ID |
| WorkDate | DATE | NOT NULL | 工作日期 |
| ProjectId | GUID | FK | 项目ID |
| Title | VARCHAR(200) | NOT NULL | 标题 |
| TotalHours | DECIMAL(10,2) | | 总工时 |
| PersonaCode | VARCHAR(50) | | Persona编码 |
| ExtraData | JSON | | 扩展数据 |
| Status | INT | Default 0 | 状态 |

**WorkLogTemplates (日志模板表)**

| 字段 | 类型 | 约束 | 描述 |
|---|---|---|---|
| Id | GUID | PK | 主键 |
| PersonaCode | VARCHAR(50) | NOT NULL, UNIQUE | Persona编码 |
| Name | VARCHAR(100) | NOT NULL | 模板名称 |
| Description | VARCHAR(500) | | 描述 |
| FieldDefinitions | JSON | NOT NULL | 字段定义 |

### 4.4 成长模块表

- **Habits** / **HabitCheckIns** - 习惯打卡
- **GrowthProjects** - 成长项目
- **KnowledgeArticles** - 知识库
- **PostgraduateTasks** / **ExamMistakes** / **ExamMaterials** - 学生学习任务、错题和资料
- **StudentSubjects** / **StudentStudyRecords** - 学生科目目标和学习记录

### 4.5 其他核心表

- **Tasks** - 通用任务系统（个人+工作，通过 Type/Source 区分）
- **WorkProjects** - 工作项目
- **WorkDevices** - 工作设备
- **WorkTaskTypes** - 任务类型
- **WorkDailyPlans** - 工作日程计划
- **WorkImports** - 数据导入
- **AiPlans** / **AiReports** / **AiChatSessions** - AI模块
- **Income** / **Expense** / **Budget** / **Investment** - 财务资产
- **RoleMenus** - 角色菜单
- **MenuActions** / **RolePermissions** - 按钮级权限

> 注意：DailyPlans 已恢复为按用户隔离的个人每日计划表；WorkDailyPlans 用于工作计划；Tasks 用于通用任务。

---

## 5. API 接口

### 5.1 认证接口 `/api/auth`

| 方法 | 端点 | 描述 |
|---|---|---|
| POST | `/login` | 用户登录 |
| GET | `/codes` | 获取权限码 |
| POST | `/refresh` | 刷新令牌 |
| POST | `/logout` | 用户登出 |

### 5.2 菜单接口 `/api/system/role-menus`

| 方法 | 端点 | 描述 |
|---|---|---|
| GET | `/mine` | 获取当前用户菜单 |
| GET | `/` | 获取所有菜单 |

### 5.3 用户接口 `/api/users` [Pro/Owner]

| 方法 | 端点 | 描述 |
|---|---|---|
| GET | `/` | 用户列表 |
| POST | `/` | 创建用户 |
| PUT | `/{id}` | 更新用户 |
| DELETE | `/{id}` | 删除用户 |

### 5.4 订阅接口 `/api/subscriptions`

| 方法 | 端点 | 描述 |
|---|---|---|
| GET | `/my` | 获取我的订阅 |
| GET | `/my/features` | 获取我的功能列表 |
| POST | `/subscribe` | 订阅计划 |
| GET | `/plans` | 获取所有计划 |

### 5.5 功能管理 `/api/admin/features` [Owner]

| 方法 | 端点 | 描述 |
|---|---|---|
| GET | `/` | 功能列表 |
| GET | `/by-category` | 按分类获取 |
| POST | `/` | 创建功能 |

### 5.6 Persona功能 `/api/admin/persona-features` [Owner]

| 方法 | 端点 | 描述 |
|---|---|---|
| GET | `/{personaCode}` | 获取Persona功能 |
| POST | `/` | 添加功能 |
| POST | `/batch` | 批量添加 |

### 5.7 工作模块 `/api/work/*`

| 前缀 | 描述 |
|---|---|
| `/api/work/logs` | 工作日志 |
| `/api/work/projects` | 工作项目 |
| `/api/work/devices` | 设备管理 |
| `/api/work/task-types` | 任务类型 |
| `/api/work/daily-plans` | 工作日程 |
| `/api/work/statistics` | 统计分析 |
| `/api/work/imports` | 数据导入 |
| `/api/work/templates` | 模板管理 |
| `/api/work/log-templates` | 日志模板 |

### 5.8 成长模块 `/api/growth/*`

| 前缀 | 描述 |
|---|---|
| `/api/growth/habits` | 习惯管理 |
| `/api/growth/projects` | 成长项目 |
| `/api/growth/knowledge-base` | 知识库 |

### 5.9 工作任务 `/api/growth/tasks`

| 方法 | 端点 | 描述 |
|---|---|---|
| GET | `/` | 分页查询（支持 taskType/source 筛选） |
| GET | `/{id}` | 获取详情 |
| POST | `/` | 创建任务 |
| PUT | `/{id}` | 更新任务 |
| DELETE | `/{id}` | 删除任务 |
| POST | `/{id}/complete` | 完成任务 |
| POST | `/convert-to-log` | 转为工作日志 |

### 5.10 学生中心 `/api/student`

| 前缀 | 描述 |
|---|---|
| `/dashboard` | 学习总览聚合 |
| `/tasks` | 学习计划 |
| `/mistakes` | 错题本 |
| `/materials` | 学习资料 |
| `/subjects` | 科目目标 |
| `/records` | 学习记录 |

### 5.11 AI模块 `/api/ai`

| 方法 | 端点 | 描述 |
|---|---|---|
| POST | `/chat` | AI对话 |
| GET | `/plans` | AI计划列表 |
| POST | `/plans` | 创建AI计划 |
| GET | `/reports` | AI报告列表 |
| POST | `/reports` | 创建AI报告 |

### 5.12 个人日程 `/api/daily-plans`

| 方法 | 端点 | 描述 |
|---|---|---|
| GET | `/` | 日程列表 |
| POST | `/` | 创建日程 |
| PUT | `/{id}` | 更新日程 |
| DELETE | `/{id}` | 删除日程 |

### 5.13 Persona管理 `/api/persona-types`

| 方法 | 端点 | 描述 |
|---|---|---|
| GET | `/` | 获取所有身份类型 |
| GET | `/{code}` | 获取特定身份 |

---

## 6. 内置用户

| 用户名 | 密码 | 角色 | Personas |
|---|---|---|---|
| vben | 123456 | owner | Developer(主), Implementation |
| admin | 123456 | pro | General(主) |
| jack | 123456 | member | Developer(主) |
| lisa | 123456 | member | Student(主) |

---

## 7. 前端结构

### 7.1 路由模块 (14个)

| 模块 | 路径 | 页面数 |
|---|---|---|
| dashboard | /dashboard | 6 |
| growth | /growth | 15 |
| work | /work | 16 |
| assets | /assets | 6 |
| ai | /ai | 6 |
| analytics | /analytics | 7 |
| system | /system | 4 |
| dev | /dev | 3 |
| design | /design | 2 |
| teacher | /teacher | 2 |
| implementation | /implementation | 2 |
| student | /student | 7 |
| labs | /labs | 4 |
| _core | /auth, /profile, /fallback | 14 |
| other | /external-links, /about, /demos | 3 |

**总计: 85 个页面**

### 7.2 API 文件 (29个)

```
api/
├── core/        (auth.ts, user.ts, menu.ts, index.ts)
├── system/     (user.ts, persona.ts, menu-tag.ts)
├── work/       (workLog.ts, project.ts, device.ts, taskType.ts, dailyPlan.ts,
│                import.ts, statistics.ts, template.ts, logTemplate.ts, types.ts, index.ts)
├── growth/     (habit.ts, daily-plan.ts, knowledge-base.ts,
│                project.ts, task.ts, index.ts, types.ts)
├── student/    (index.ts)
├── ai/         (index.ts)
├── analytics.ts
└── request.ts
```

### 7.3 组件

| 组件 | 说明 |
|---|---|
| DynamicForm | 动态表单 |
| DynamicFieldForm | 动态字段表单 |
| PersonaSwitch | Persona切换组件 |

### 7.4 Store

| Store | 说明 |
|---|---|
| useAuthStore | 认证状态 |
| usePersonaStore | Persona状态 |

---

## 8. 快速开始

### 8.1 后端启动

```bash
rtk dotnet run --project WebApplication1\WebApplication1\WebApplication1.csproj
```

后端运行在 `http://localhost:5062`

### 8.2 前端启动

```bash
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run dev"
```

前端运行在 `http://localhost:5666`

### 8.3 数据库重置

```bash
rtk dotnet ef database drop --force
rtk dotnet ef database update
```

### 8.4 测试账号

- Owner: `vben` / `123456`
- Pro: `admin` / `123456`
- Member: `jack` / `123456`
- Student: `lisa` / `123456`

---

## 9. 设计原则

1. **模块边界清晰**：个人成长管生活/习惯/学习，工作中心管项目/任务/工作
2. **角色递进**：Member → Pro → Owner 逐级解锁
3. **Persona多对多**：一个用户可拥有多个职业身份
4. **Feature权限**：非 Persona 功能来自套餐；Persona 功能必须套餐和身份同时满足；Owner 拥有全部启用功能
5. **动态日志模板**：不同Persona有不同的日志字段（JSON扩展）
6. **菜单分类**：MenuCategory 字段组织菜单
7. **按钮级权限**：MenuActions + RolePermissions
8. **统一任务系统**：Tasks 表统一管理个人任务和工作任务

---

## 10. 核心服务

### 10.1 RoleMenuService

```csharp
public async Task<List<RoleMenu>> GetMenusForUserAsync(
    Guid userId, string roleCode, List<string> personaCodes,
    HashSet<string> availableFeatureCodes)
{
    // 1. 角色等级检查
    // 2. Persona匹配
    // 3. FeatureCode匹配
    // 4. 构建菜单树
}
```

### 10.2 FeatureService

```csharp
public async Task<bool> HasFeatureAsync(Guid userId, string featureCode)
{
    var features = await GetUserFeaturesAsync(userId);
    return features.Contains(featureCode);
}

public async Task<HashSet<string>> GetUserFeaturesAsync(Guid userId)
{
    // Reuse UserAccessContextService.FeatureCodes
}
```

### 10.3 WorkLogTemplate (动态日志)

```json
{
  "fields": [
    { "key": "location", "label": "项目地", "type": "select", "options": ["惠州", "深圳"] },
    { "key": "equipment", "label": "设备", "type": "multi-select" },
    { "key": "msapItems", "label": "MSAP工作项", "type": "task-list", "fields": ["content", "percent"] }
  ]
}
```

---

## 11. 已知问题

1. ~~两套每日计划~~（已修复 → 统一为 Tasks 表）
2. **AI模块桩代码**：需配置 OPENAI_API_KEY 环境变量启用真实AI
3. ~~前端角色选项~~（已修复）

---

## 12. 环境变量

生产环境需设置以下环境变量：

| 变量 | 说明 | 必需 |
|---|---|---|
| `DB_CONNECTION` | MySQL 连接字符串 | 是 |
| `JWT_SECRET_KEY` | JWT 密钥（最少32字符） | 是 |
| `OPENAI_API_KEY` | OpenAI API Key | 否（不设置则返回模拟响应） |

---

## 13. 数据库迁移

```bash
rtk dotnet ef database drop --force  # 首次或重置
rtk dotnet ef database update
```

---

## 14. 测试

```bash
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore
```

当前测试覆盖：TaskItemService, KnowledgeArticleService, DailyPlanService, WorkProjectService, RoleMenuService

---

## 15. 更新日志

### v2.0 (2025-05)

- 新增 **8种职业身份**：Developer, Designer, Teacher, Student, Implementation, General, Sales, Freelancer
- 新增 **实验中心 (/labs)** 模块：AI实验室、数据实验室、模板市场、UI组件
- 成长模块扩展至 **15个页面**
- AI模块扩展至 **6个页面**
- 数据分析模块扩展至 **7个页面**
- 财务资产模块扩展至 **6个页面**
- 新增工作模块：周计划、OKR、甘特图、风险管理、软件资产、文件中心
- 统一任务系统（Tasks）替代原有的 DailyPlans 表
