# MVP 验收清单

本清单用于验收 Docker Compose 可部署 MVP。默认范围是个人成长 + 工作中心主路径。

## 环境准备

- [ ] 已复制根目录 `.env.example` 为 `.env`。
- [ ] 已替换 `MYSQL_ROOT_PASSWORD`、`DB_CONNECTION`、`JWT_SECRET_KEY`、`SECURITY_ENCRYPTION_KEY`。
- [ ] `DATABASE_AUTO_MIGRATE=true`。
- [ ] `DATABASE_SEED_ON_STARTUP=true`。

## 启动验收

- [ ] 执行 `rtk docker compose --env-file .env up -d --build` 成功。
- [ ] 执行 `rtk docker compose ps` 后 `mysql`、`api`、`web` 均为 healthy。
- [ ] 执行 `rtk pwsh -File scripts\verify-mvp.ps1 -RequireRuntime` 成功。
- [ ] 打开 `http://localhost:5666` 能进入前端。
- [ ] 打开 `http://localhost:5666/api/healthz/live` 返回健康状态。
- [ ] 打开 `http://localhost:5666/api/healthz/ready` 返回 MySQL 就绪状态。
- [ ] 刷新任意前端页面不出现 Nginx 404。

## 登录和权限

- [ ] `vben / 123456` 可登录。
- [ ] 登录后能获取用户信息、权限码和后端菜单。
- [ ] 刷新浏览器后会话保持正常。
- [ ] 菜单中能看到个人成长和工作中心。

## 个人成长主路径

- [ ] 打开成长看板不报错。
- [ ] 每日计划可新建、编辑、完成。
- [ ] 习惯可新建并打卡。
- [ ] 知识库列表可打开，并可创建一条知识记录。

## 工作中心主路径

- [ ] 打开工作看板不报错。
- [ ] 工作项目可新建。
- [ ] 任务可新建，并能关联项目。
- [ ] 工作日志可新建。
- [ ] 工作统计或工作看板能展示数据或空状态，不出现未处理错误。

## 构建验收

- [ ] 后端测试通过：`rtk dotnet test WebApplication1\WebApplication1.Tests\WebApplication1.Tests.csproj --no-restore`。
- [ ] 前端类型检查通过：`rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run typecheck"`。
- [ ] 前端构建通过：`rtk pwsh -Command "Set-Location vue-vben-admin; pnpm -F @vben/web-antd run build"`。
- [ ] 前端构建产物包含 `manifest.webmanifest`、`sw.js` 和 `workbox-*.js`。
- [ ] MVP 验收脚本测试通过：`rtk pwsh -File scripts\verify-mvp.tests.ps1`。
- [ ] MVP 静态验收通过：`rtk pwsh -File scripts\verify-mvp.ps1 -SkipRuntime`。

## 演示账号

| 用户名 | 密码 | 角色 | 说明 |
|---|---|---|---|
| `vben` | `123456` | owner | 管理员，Developer + Implementation |
| `admin` | `123456` | pro | Pro 通用用户 |
| `jack` | `123456` | member | Developer 用户 |
| `lisa` | `123456` | member | Student 用户 |
