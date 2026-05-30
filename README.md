# MyWebSite

个人成长与工作管理系统，前端基于 Vben Admin（Vue 3 + Ant Design Vue + pnpm monorepo），后端基于 ASP.NET Core + EF Core + MySQL。

当前工程化目标：主分支随时能通过后端测试、前端类型检查和前端构建。

## 技术栈

- 后端：ASP.NET Core `net10.0`、Entity Framework Core、MySQL
- 前端：Vben Admin `5.7.0`、Vue 3、Vite、Ant Design Vue、pnpm
- 数据库：MySQL，默认库名 `personal_growth`
- 鉴权：JWT Bearer
- AI：OpenAI API，可选配置；未配置时接口会返回明确的不可用状态

## 模块概览

| 模块 | 路径 | 说明 |
|------|------|------|
| 个人成长 | `/growth` | 习惯打卡、每日计划、知识库、技能管理等 |
| 工作中心 | `/work` | 工作日志、项目管理、设备管理、统计分析等 |
| AI 助手 | `/ai` | AI 对话、智能计划、AI 报告 |
| 财务资产 | `/assets` | 收入/支出/预算/投资管理 |
| 内容管理 | `/content` | 文章、媒体文件、发布日历 |
| 人脉网络 | `/network` | 联系人、互动记录 |
| 数据分析 | `/analytics` | 成长/工作/财务/时间/习惯分析 |
| 学生中心 | `/student` | 学习计划、错题本、学习资料、科目目标 |
| 平台管理 | `/system` | 用户、角色菜单、身份类型管理 |

## 本地准备

需要安装：

- .NET SDK 10
- Node.js 20.19+、22.18+ 或 24+
- pnpm 10+
- MySQL 8+

创建数据库：

```sql
CREATE DATABASE IF NOT EXISTS personal_growth
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;
```

后端开发配置位于：

```text
WebApplication1/WebApplication1/appsettings.Development.json
```

可参考：

```text
WebApplication1/WebApplication1/appsettings.Development.example.json
```

最小配置项：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=personal_growth;User=root;Password=123456;CharSet=utf8mb4;"
  },
  "Jwt": {
    "Issuer": "PersonalGrowthManagement",
    "Audience": "VueVbenAdmin",
    "SecretKey": "至少32字符的开发密钥",
    "ExpireMinutes": 30,
    "RefreshTokenExpireDays": 7
  },
  "OpenAI": {
    "ApiKey": "",
    "Model": "gpt-3.5-turbo",
    "MaxTokens": 2000
  }
}
```

生产环境不要提交真实密钥，优先使用环境变量：

```powershell
$env:DB_CONNECTION="Server=...;Port=3306;Database=personal_growth;User=...;Password=...;CharSet=utf8mb4;"
$env:JWT_SECRET_KEY="至少32字符的生产密钥"
$env:OPENAI_API_KEY="sk-..."
```

## 一键启动

目前没有统一的根目录启动脚本，按两个终端启动。

终端 1，启动后端：

```powershell
rtk dotnet run --project WebApplication1\WebApplication1\WebApplication1.csproj
```

默认地址：

```text
http://localhost:5062
```

健康检查：

```text
http://localhost:5062/healthz
```

Swagger（Development 环境）：

```text
http://localhost:5062/swagger
```

终端 2，启动前端：

```powershell
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm install"
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run dev"
```

默认地址：

```text
http://localhost:5666
```

开发环境前端通过 Vite 代理把 `/api` 转发到：

```text
http://localhost:5062/api
```

## 本地验证

后端测试（140 个）：

```powershell
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore
```

前端类型检查：

```powershell
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run typecheck"
```

前端构建：

```powershell
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run build"
```

提交前建议完整跑：

```powershell
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run typecheck"
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run build"
```

## 权限与菜单

运行时菜单以后端 `RoleMenus` 为准，前端使用 backend access mode 从 `/api/system/role-menus/mine` 获取菜单并生成动态路由。

权限判断分三层：

- `Role`：Member / Pro / Owner，决定最低等级门槛。
- `Plan + FeatureCode`：决定具体功能是否可用。
- `Persona`：决定学生、开发、实施等场景是否出现。

有效功能码由后端 `UserAccessContextService` 统一计算：

```text
非 Persona 类功能：来自当前有效套餐
Persona 类功能：套餐包含该功能，并且用户拥有对应 Persona
Owner：拥有全部启用功能
```

前端页面内部也必须按 `accessCodes` 降级。基础页面不能无条件请求 Pro 接口；没有权限的统计卡、下拉、快捷入口和操作按钮应隐藏或降级为普通输入，避免用户看到 403 红色提示。

按钮级权限通过 `AuthService.GetAccessCodesAsync` 返回，包含 FeatureCodes 和 RolePermissions。

学生模块统一使用 `/api/student/...`，不再使用旧的 postgraduate 路由。

## CI

GitHub Actions 配置在：

```text
.github/workflows/ci.yml
```

CI 会执行：

- 后端 restore
- 后端测试
- NuGet 传递依赖漏洞检查
- 前端 pnpm install
- 前端 typecheck
- 前端 build

## 安全依赖检查

此前 NuGet 曾报告：

```text
System.IO.Packaging 6.0.0 High
```

它不是项目直接引用，而是来自传递依赖链。包图显示最可疑链路是：

```text
ClosedXML 0.102.2 -> DocumentFormat.OpenXml 2.16.0 -> System.IO.Packaging 6.0.0
```

本地检查命令：

```powershell
rtk dotnet list WebApplication1\WebApplication1\WebApplication1.csproj package --vulnerable --include-transitive
rtk dotnet list WebApplication1\WebApplication1\WebApplication1.csproj package --outdated
```

项目已将 `ClosedXML` 升级到 `0.105.0`。后续升级依赖或修改 Excel 导入导出后，重新跑：

```powershell
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore
rtk dotnet list WebApplication1\WebApplication1\WebApplication1.csproj package --vulnerable --include-transitive
```

## 部署注意

前端生产配置文件：

```text
vue-vben-admin/apps/web-antd/.env.production
```

当前 `VITE_GLOB_API_URL` 需要在部署前改成真实后端 API 地址，不能继续指向 Vben mock API。

后端生产配置需要提供：

- `DB_CONNECTION`
- `JWT_SECRET_KEY`
- `OPENAI_API_KEY`，如果启用 AI 功能
- `Cors:Origins`，允许的前端域名

## 更新日志

### v2.2 (2025-05-30)

**完善所有页面（消除桩代码）：**

- Growth 模块：11 个页面对接后端 API（skills, goals, year-plans, monthly-review, learning-path, courses, fitness, sleep, mood-tracker, reading-list, focus-timer）
- Work 模块：3 个页面对接后端 API（okr, risk-control, files）
- Analytics 模块：5 个页面对接后端 API（time, habits, finance, custom-reports, ai-insights）
- AI 模块：3 个页面对接后端 API（automation, knowledge-chat, insights）
- Persona 模块：7 个页面对接后端 API（dev/design/teacher 中心）

**测试结果：** 140 个后端测试全部通过，前端类型检查和构建成功

### v2.1 (2025-05-29)

**前端对接后端 API（消除硬编码）：**

- 财务资产模块：5 个页面对接后端 21 个 API 端点
- AI 助手/报告：2 个页面对接真实聊天和报告生成 API
- 工作分析：1 个页面对接统计 API 4 个端点
- 内容管理模块：新建 3 个页面，对接后端 15 个 API 端点
- 人脉网络模块：新建 2 个页面，对接后端 11 个 API 端点

**后端优化：**

- 按钮级权限：修复 `GetAccessCodesAsync` 硬编码，接入 `UserAccessContextService`
- API Feature 覆盖：为 13 个 Controller 补充 `[RequireFeature]` 标记
- 系统管理权限：为 5 个 Controller 补充 `[Authorize(Roles = "owner")]` 角色限制
- RoleMenuController 修复：移除类级别角色限制，确保 pro/member 用户可以获取菜单
- 废弃代码清理：删除 `MenuBindingType` 枚举

**前端按钮级权限应用：**

- 为 Work/Student/Content/Network/Assets/Growth/AI/System 模块的 20 个页面添加了按钮级权限控制
- 使用 `useAccessStore().accessCodes` + `v-if` 控制按钮显示

**测试结果：** 140 个后端测试全部通过，前端类型检查和构建成功
