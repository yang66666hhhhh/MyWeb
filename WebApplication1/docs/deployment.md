# 部署指南

## 目录
- [本地开发](#本地开发)
- [Docker部署](#docker部署)
- [生产环境部署](#生产环境部署)
- [环境变量配置](#环境变量配置)
- [数据库迁移](#数据库迁移)
- [健康检查](#健康检查)

## 本地开发

### 前置条件
- .NET 10.0 SDK
- MySQL 8.0
- Visual Studio 2022 或 VS Code

### 步骤

1. **克隆项目**
```bash
git clone <repository-url>
cd WebApplication1
```

2. **配置环境变量**
```bash
# 复制配置模板
cp WebApplication1/appsettings.Development.example.json WebApplication1/appsettings.Development.json
```

编辑 `appsettings.Development.json`：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=personal_growth;User=root;Password=your_password"
  },
  "Jwt": {
    "SecretKey": "your-super-secret-key-at-least-32-chars-long"
  },
  "OpenAI": {
    "ApiKey": "sk-your-openai-api-key"
  },
  "Security": {
    "EncryptionKey": "your-encryption-key"
  }
}
```

3. **创建数据库**
```sql
CREATE DATABASE personal_growth CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
```

4. **运行迁移**
```bash
cd WebApplication1
dotnet ef database update
```

5. **启动应用**
```bash
dotnet run
```

6. **访问应用**
- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- 健康检查: http://localhost:5000/healthz

## Docker部署

### 前置条件
- Docker
- Docker Compose

### 步骤

1. **创建环境变量文件**
```bash
cat > .env << EOF
# 数据库配置
DB_CONNECTION=Server=mysql;Port=3306;Database=personal_growth;User=root;Password=your_password
MYSQL_ROOT_PASSWORD=your_root_password
MYSQL_USER=your_user
MYSQL_PASSWORD=your_password

# JWT配置
JWT_SECRET_KEY=your-super-secret-key-at-least-32-chars-long

# OpenAI配置（可选）
OPENAI_API_KEY=sk-your-openai-api-key

# 加密配置
Security__EncryptionKey=your-encryption-key
EOF
```

2. **启动服务**
```bash
docker-compose up -d
```

3. **查看服务状态**
```bash
docker-compose ps
```

4. **查看日志**
```bash
# 查看所有日志
docker-compose logs -f

# 查看Web应用日志
docker-compose logs -f webapp

# 查看MySQL日志
docker-compose logs -f mysql
```

5. **停止服务**
```bash
docker-compose down
```

6. **更新部署**
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

## 生产环境部署

### 使用 systemd 服务

1. **发布应用**
```bash
dotnet publish -c Release -o /var/www/personal-growth
```

2. **创建 systemd 服务文件**
```bash
sudo nano /etc/systemd/system/personal-growth.service
```

```ini
[Unit]
Description=Personal Growth Management API
After=network.target

[Service]
WorkingDirectory=/var/www/personal-growth
ExecStart=/usr/bin/dotnet /var/www/personal-growth/WebApplication1.dll
Restart=always
RestartSec=10
SyslogIdentifier=personal-growth
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

3. **启动服务**
```bash
sudo systemctl daemon-reload
sudo systemctl enable personal-growth
sudo systemctl start personal-growth
```

4. **查看服务状态**
```bash
sudo systemctl status personal-growth
sudo journalctl -u personal-growth -f
```

### 使用 Nginx 反向代理

1. **安装 Nginx**
```bash
sudo apt update
sudo apt install nginx
```

2. **配置 Nginx**
```bash
sudo nano /etc/nginx/sites-available/personal-growth
```

```nginx
server {
    listen 80;
    server_name your-domain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
    }
}
```

3. **启用配置**
```bash
sudo ln -s /etc/nginx/sites-available/personal-growth /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl restart nginx
```

## 环境变量配置

### 必需的环境变量

| 变量名 | 说明 | 示例 |
|--------|------|------|
| `DB_CONNECTION` | 数据库连接字符串 | `Server=localhost;Port=3306;Database=personal_growth;User=root;Password=xxx` |
| `JWT_SECRET_KEY` | JWT密钥（≥32字符） | `your-super-secret-key-at-least-32-chars-long` |
| `Security__EncryptionKey` | 加密密钥 | `your-encryption-key` |

### 可选的环境变量

| 变量名 | 说明 | 默认值 |
|--------|------|--------|
| `ASPNETCORE_ENVIRONMENT` | 运行环境 | `Production` |
| `OPENAI_API_KEY` | OpenAI API Key | 无 |
| `Logging__LogLevel__Default` | 日志级别 | `Information` |

### 设置环境变量

#### Linux/macOS
```bash
export DB_CONNECTION="Server=localhost;Port=3306;Database=personal_growth;User=root;Password=xxx"
export JWT_SECRET_KEY="your-super-secret-key-at-least-32-chars-long"
```

#### Windows
```powershell
$env:DB_CONNECTION="Server=localhost;Port=3306;Database=personal_growth;User=root;Password=xxx"
$env:JWT_SECRET_KEY="your-super-secret-key-at-least-32-chars-long"
```

#### Docker
在 `docker-compose.yml` 或 `.env` 文件中配置。

## 数据库迁移

### 创建迁移
```bash
dotnet ef migrations add MigrationName
```

### 应用迁移
```bash
dotnet ef database update
```

### 回滚迁移
```bash
dotnet ef database update PreviousMigrationName
```

### 删除迁移
```bash
dotnet ef migrations remove
```

## 健康检查

### 端点说明

| 端点 | 说明 | 用途 |
|------|------|------|
| `/healthz` | 完整健康检查 | 监控系统 |
| `/healthz/ready` | 就绪检查 | 负载均衡器 |
| `/healthz/live` | 存活检查 | 容器编排 |

### 响应示例

```json
{
  "status": "Healthy",
  "totalDuration": 123.45,
  "checks": [
    {
      "name": "mysql",
      "status": "Healthy",
      "duration": 50.12,
      "description": "MySQL connection is healthy"
    },
    {
      "name": "memory",
      "status": "Healthy",
      "duration": 1.23,
      "description": "Memory usage: 150MB / 1024MB"
    }
  ]
}
```

### 配置监控

建议使用以下工具监控健康检查：
- Prometheus + Grafana
- Datadog
- New Relic
- AWS CloudWatch
