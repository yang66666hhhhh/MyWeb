# 部署指南

## 本地开发

前置条件：

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

复制并编辑后端开发配置：

```powershell
rtk pwsh -Command "Copy-Item WebApplication1\WebApplication1\appsettings.Development.example.json WebApplication1\WebApplication1\appsettings.Development.json"
```

启动后端：

```powershell
rtk dotnet run --project WebApplication1\WebApplication1\WebApplication1.csproj
```

默认地址：

- API: `http://localhost:5062`
- Swagger: `http://localhost:5062/swagger`
- 健康检查: `http://localhost:5062/healthz`

启动前端：

```powershell
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm install"
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run dev"
```

前端地址：`http://localhost:5666`。开发环境通过 Vite 将 `/api` 代理到 `http://localhost:5062/api`。

## Docker Compose 部署

根目录 `docker-compose.yml` 提供 MVP 默认部署方式，包含：

- `mysql`：MySQL 8
- `api`：ASP.NET Core API
- `web`：Nginx 托管前端静态资源，并把 `/api` 代理到 API 容器

1. 复制环境变量模板：

```powershell
rtk pwsh -Command "Copy-Item .env.example .env"
```

2. 编辑根目录 `.env`，替换密码和密钥：

```text
MYSQL_ROOT_PASSWORD=...
DB_CONNECTION=Server=mysql;Port=3306;Database=personal_growth;User=root;Password=...;CharSet=utf8mb4;
JWT_SECRET_KEY=至少32字符的随机密钥
SECURITY_ENCRYPTION_KEY=随机加密密钥
OPENAI_API_KEY=
DATABASE_AUTO_MIGRATE=true
DATABASE_SEED_ON_STARTUP=true
```

3. 构建并启动：

```powershell
rtk docker compose --env-file .env up -d --build
```

4. 查看状态和日志：

```powershell
rtk docker compose ps
rtk docker compose logs -f api
rtk docker compose logs -f web
rtk docker compose logs -f mysql
```

5. 访问服务：

- 前端：`http://localhost:5666`
- API 存活检查：`http://localhost:5666/api/healthz/live`
- API 就绪检查：`http://localhost:5666/api/healthz/ready`
- Web 健康检查：`http://localhost:5666/health`

停止服务：

```powershell
rtk docker compose down
```

如需清空数据库卷：

```powershell
rtk docker compose down -v
```

## 生产环境变量

后端必需：

| 变量名 | 说明 |
|---|---|
| `DB_CONNECTION` | MySQL 连接字符串 |
| `Jwt__SecretKey` 或 `JWT_SECRET_KEY` | JWT 密钥，至少 32 字符 |
| `Jwt__Issuer` | JWT Issuer，Docker 默认 `PersonalGrowthManagement` |
| `Jwt__Audience` | JWT Audience，Docker 默认 `VueVbenAdmin` |
| `Security__EncryptionKey` | 敏感数据加密密钥 |

后端可选：

| 变量名 | 说明 |
|---|---|
| `OpenAI__ApiKey` 或 `OPENAI_API_KEY` | 启用真实 AI 能力 |
| `Database__AutoMigrate` | 启动时自动应用 EF Core 迁移 |
| `Database__SeedOnStartup` | 启动时写入基础菜单、功能码和演示数据 |
| `Cors__Origins__0` | 分域部署时允许的前端域名 |

前端生产配置位于 `vue-vben-admin/apps/web-antd/.env.production`。Docker Compose 默认使用：

```text
VITE_GLOB_API_URL=/api
```

如果前后端分开部署，改为真实 API 地址，例如：

```text
VITE_GLOB_API_URL=https://api.example.com/api
```

## 数据库初始化策略

开发环境会自动执行 Seeder，方便本地获得演示账号和菜单。

生产环境默认不应隐式初始化数据库。Docker Compose MVP 通过 `.env` 显式开启：

```text
DATABASE_AUTO_MIGRATE=true
DATABASE_SEED_ON_STARTUP=true
```

传统服务器或云平台部署时，建议先手动执行迁移，确认数据库备份和变更窗口后再开启自动迁移。

## 健康检查

| 端点 | 说明 | 用途 |
|---|---|---|
| `/healthz` | 完整健康检查 | 监控系统 |
| `/healthz/ready` | MySQL 就绪检查 | 容器依赖和负载均衡 |
| `/healthz/live` | 应用存活检查 | 容器存活探针 |
| `/health` | 前端 Nginx 健康检查 | Web 容器健康探针 |

## 验证命令

后端测试：

```powershell
rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore
```

前端类型检查：

```powershell
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run typecheck"
```

前端生产构建：

```powershell
rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run build"
```
