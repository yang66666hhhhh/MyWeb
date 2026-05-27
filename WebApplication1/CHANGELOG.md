# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] - 2026-05-28

### Added
- **核心功能模块**
  - 每日计划管理（CRUD、状态跟踪）
  - 习惯打卡系统（连续打卡、统计分析）
  - 工作日志管理（设备、项目、任务类型）
  - 知识库系统（文章、标签、分类）
  - 考研备选功能（学习任务、错题本、复习材料）
  - AI助手（计划生成、报告分析、对话）
  - 资产管理（收入、支出、预算、投资）
  - 内容管理（文章、媒体、发布日历）

- **安全特性**
  - JWT认证 + Refresh Token机制
  - 请求限流（基于用户/IP，支持反向代理）
  - 防SQL注入检测中间件
  - 防XSS攻击（安全响应头、内容检测）
  - 敏感数据加密存储（AES-256）
  - 审计日志系统（操作追踪）

- **监控特性**
  - 结构化日志支持（Serilog）
  - 请求/响应日志中间件
  - 性能监控中间件（慢API检测）
  - 健康检查系统（内存、磁盘、CPU、数据库）
  - 多端点健康检查（/healthz, /healthz/ready, /healthz/live）

- **技术特性**
  - .NET 10.0 + Entity Framework Core 10.0
  - MySQL 8.0 数据库
  - FluentValidation 数据验证
  - 内存缓存系统
  - API版本控制（v1, v2）
  - Swagger/OpenAPI文档
  - 国际化支持（中文/英文）
  - Docker容器化
  - 响应压缩（Brotli/Gzip）
  - 数据库连接池优化

- **测试**
  - 140个单元测试
  - 覆盖主要业务逻辑
  - 使用xUnit + Moq + EF Core InMemory

### Changed
- 无

### Deprecated
- 无

### Removed
- 无

### Fixed
- 无

### Security
- 初始安全配置
- JWT密钥长度验证（≥32字符）
- CORS配置
- 请求限流配置
