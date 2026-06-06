# API Feature 覆盖稳定性设计

## 背景

项目已经形成统一权限主链路：前端动态菜单来自 `/api/system/role-menus/mine`，后端菜单过滤和 API Feature 校验都复用 `IUserAccessContextService` 计算出的 `FeatureCodes`。当前风险集中在部分业务 Controller 只加了 `[Authorize]`，没有绑定业务功能码，用户可能绕过菜单直接访问 API。

本设计聚焦后端稳定性：补齐业务 API 的 `[RequireFeature]` 覆盖，并用测试防止后续新增业务 Action 时再次遗漏。前端 UI 权限降级、菜单管理页收敛、旧菜单模型清理不在本轮范围内。

## 目标

- 业务 API 的可访问性与菜单/模块 FeatureCode 保持一致。
- `RequireFeatureAttribute` 继续通过 `IUserAccessContextService` 获取统一 FeatureCodes，不新增第二套授权逻辑。
- Seeder 能补齐本轮使用到的标准功能码，并让 Pro/Team 默认拥有这些非 Free 功能。
- 新增反射测试，发现业务 Controller 中缺少 Feature 保护的 Action，并输出具体 Controller 和 Action 名称。

## 非目标

- 不做数据库迁移，功能码补齐通过 Seeder 完成。
- 不重构前端路由、菜单管理页或 `system/menu-tag`。
- 不调整登录、当前用户资料、我的菜单、订阅查询、当前用户 Persona 等基础认证接口的 Feature 规则。
- 不引入 ASP.NET Core Policy 重写现有 `RequireFeatureAttribute`。

## 已确认方案

采用“权限矩阵 + 后端特性补齐 + 自动化覆盖检查”的方案。

相比只给明显裸奔 Controller 加特性，本方案同时修正 `PersonaController` 当前复用 `WORK_PROJECT`、`WORK_TASK` 的语义偏差。相比全量前后端命名重构，本方案范围更小，适合本轮交付。

## Feature 映射

| API 范围 | RequiredFeature |
|---|---|
| `api/work/okr/*` | `WORK_OKR` |
| `api/work/risks/*` | `WORK_RISK` |
| `api/work/files/*` | `WORK_TASK` |
| `api/ai/automation/*` | `AI_AUTOMATION` |
| `api/ai/knowledge-chat/*` | `AI_KNOWLEDGE_CHAT` |
| `api/ai/insights/*` | `AI_INSIGHTS` |
| `api/analytics/time/*` | `ANALYTICS_TIME` |
| `api/analytics/habits/*` | `ANALYTICS_HABITS` |
| `api/analytics/finance/*` | `ANALYTICS_FINANCE` |
| `api/analytics/reports/*` | `ANALYTICS_CUSTOM` |
| `api/analytics/ai-insights/*` | `ANALYTICS_AI` |
| `api/persona/dev/repositories/*` | `DEV_CODE_REPO` |
| `api/persona/dev/issues/*` | `DEV_ISSUES` |
| `api/persona/dev/pipelines/*` | `DEV_PIPELINES` |
| `api/persona/design/assets/*` | `DESIGN_ASSETS` |
| `api/persona/design/prototypes/*` | `DESIGN_PROTOTYPE` |
| `api/persona/teacher/courses/*` | `TEACHER_COURSE` |
| `api/persona/teacher/students/*` | `TEACHER_STUDENT` |

`api/work/files/*` 继续使用 `WORK_TASK`，因为当前菜单和前端页面已经按 `WORK_TASK` 控制文件中心。后续如果产品要把文件中心独立收费，再单独引入 `WORK_FILE`。

## 后端改动设计

### Controller

修改范围：

- `WebApplication1/WebApplication1/Features/Work/Controllers/WorkExtendedController.cs`
- `WebApplication1/WebApplication1/Features/Ai/Controllers/AiExtendedController.cs`
- `WebApplication1/WebApplication1/Features/Analytics/Controllers/AnalyticsExtendedController.cs`
- `WebApplication1/WebApplication1/Features/Persona/Controllers/PersonaController.cs`

规则：

- 已有类级 `[Authorize]` 保留。
- 按 Action 所属资源添加方法级 `[RequireFeature]`。
- 一个 Action 只绑定一个业务 FeatureCode。
- 不把用户隔离逻辑移出服务层或 Controller 现有检查；Feature 校验只解决“是否有功能资格”，资源归属检查仍按现有逻辑处理。

### Seeder

修改范围：

- `WebApplication1/WebApplication1/Shared/Data/DbSeeder.cs`

规则：

- 标准功能码定义补齐：`AI_AUTOMATION`、`AI_KNOWLEDGE_CHAT`、`ANALYTICS_TIME`、`ANALYTICS_HABITS`、`ANALYTICS_CUSTOM`、`ANALYTICS_AI`。
- 新库初始化时直接写入完整 Feature 列表。
- 既有库启动 Seeder 时通过已有 `EnsureFeatureDefinitionsAsync` 补齐缺失功能码。
- Pro/Team 计划默认拥有所有启用功能；若既有库缺少新增功能的 PlanFeature，应通过补齐逻辑加入 Pro/Team。
- Free 计划仍只保留现有基础功能，不新增 AI、Analytics、Work Pro 扩展能力。
- Persona 功能仍通过 `EnsurePersonaFeaturesAsync` 和已有前缀规则维护，`DEV_`、`DESIGN_`、`TEACHER_` 功能必须同时满足套餐和用户 Persona。

## 测试设计

### Feature 定义测试

新增或扩展测试覆盖：

- 标准功能码集合包含本轮 Controller 使用的所有 FeatureCode。
- Pro/Team 可获得新增的非 Free 功能码。
- Free 不获得新增的 AI、Analytics、Work Pro 功能码。
- Persona 功能继续满足“套餐包含 + 用户拥有 Persona”双条件，避免因为补齐功能码而扩大 Persona 权限。

### Controller 覆盖测试

新增反射测试文件：

- `WebApplication1/WebApplication1.Tests/Services/ApiFeatureCoverageTests.cs`

扫描规则：

- 扫描 `WebApplication1` 程序集中的 Controller 类型。
- 对业务 Controller 中有 HTTP Method Attribute 的公开 Action，要求方法级或类级存在 `[RequireFeature]`。
- 若 Controller 或 Action 只承担基础认证能力，可进入显式豁免清单。
- 测试失败时输出缺少 Feature 的 `Controller.Action` 列表。

豁免范围：

- `AuthController` 登录、刷新、登出、当前用户信息、权限码。
- `UserController`、`UserProfileController`、`UserPersonaController` 的当前用户自助接口。
- `RoleMenuController.GetMyMenus`。
- 系统管理类接口中已经有 Owner 角色限制的管理 Action。
- `SubscriptionController` 的当前用户订阅查询和计划查询。

豁免清单必须具体到 Controller 或 Action 名称，不能用整个 `Features` 命名空间模糊跳过。

## 错误处理

`RequireFeatureAttribute` 当前无权限时返回 `ForbidResult`，保持 403 语义不变。未登录或无法解析用户 ID 时保持 401。业务资源不存在或资源归属不匹配时沿用现有 Controller 行为，不在本轮改动中统一 404/403。

## 验收

后端验收命令：

```powershell
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore
```

如果实施阶段触及前端功能码命名，追加前端类型检查：

```powershell
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run typecheck"
```

验收标准：

- 后端测试通过。
- 反射测试能覆盖本轮目标 Controller。
- 本轮新增或修正的业务 Action 都有明确 FeatureCode。
- Free 用户不会因新增 Seeder 功能码获得 Pro/AI/Analytics 权限。
- Pro/Team 和 Owner 用户能通过统一 `UserAccessContextService` 获得新增的非 Free 功能码。

## 风险与约束

- 前端存在少量功能码命名不一致，例如 AI 知识库问答和 AI 报告按钮权限码可能与后端标准码不一致。本轮不主动改 UI，但实施阶段如果发现类型检查或基本流程受影响，可以做最小命名修正。
- 反射测试需要维护清晰的豁免清单。豁免应只用于基础认证、自助资料和系统管理角色接口，不能把业务 Controller 整体跳过。
- Seeder 补齐既有库数据时应避免 MySQL Provider 不支持的动态集合查询模式，继续采用“小集合取出后内存过滤”的写法。
