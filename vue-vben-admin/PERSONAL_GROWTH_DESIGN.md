# Personal Growth Website Design

## 目标定位

这是一个给在职备考人员使用的个人管理网站。核心目标不是做一个泛泛的博客或后台模板，而是把工作、408 备考、暨南大学非全日制人工智能目标和生活状态放到同一个系统里，帮助你每天做取舍、每周做复盘、长期沉淀作品集和知识资产。

技术栈：

- Frontend: Vue Vben Admin, Vue 3, Ant Design Vue
- Backend: ASP.NET Core Web API
- Database: MySQL
- Local database: `Server=localhost;Port=3306;Database=personal_growth;User=root;Password=123456;CharSet=utf8mb4;`

## 产品结构

### 1. 成长看板

入口：`/growth/dashboard`

用途：

- 汇总今日计划、完成率、本周主线
- 展示工作、408、暨南大学 AI、生活四条主线
- 作为每天打开系统后的第一屏

关键指标：

- 今日计划数
- 今日完成数
- 今日计划完成率
- 本周主攻科目
- 连续学习天数
- 本周有效学习时长

### 2. 每日计划

入口：`/growth/daily-plans`

用途：

- 把一天拆成工作、学习、生活三类任务
- 支持计划、进行中、完成、取消
- 作为所有模块的执行层

推荐字段：

- `planDate`
- `title`
- `description`
- `category`: Work, Study, Life
- `priority`: 1 high, 2 medium, 3 normal
- `status`: Pending, InProgress, Completed, Cancelled
- `estimatedMinutes`
- `actualMinutes`
- `linkedSubject`
- `linkedProjectId`
- `remark`

### 3. 习惯打卡

入口：`/growth/habits`

用途：

- 跟踪长期续航能力，而不是只盯学习进度
- 建议先放：早起、运动、英语、错题复盘、睡眠、阅读

关键指标：

- 今日是否打卡
- 连续天数
- 本周完成率
- 最近一次打卡时间

### 4. 工作日志

入口：`/growth/work-logs`

用途：

- 记录在职工作产出、问题、技术决策和复盘
- 未来可直接转成简历素材、项目答辩素材、复试项目介绍

推荐字段：

- `workDate`
- `title`
- `summary`
- `problem`
- `solution`
- `result`
- `tags`
- `relatedProjectId`
- `review`

### 5. 知识库

入口：`/growth/knowledge-base`

用途：

- 沉淀 408 笔记、错题、工作技术、AI 项目资料
- 建议用统一文章模型，靠 `category` 和 `tags` 分类

推荐分类：

- `408/DataStructure`
- `408/ComputerOrganization`
- `408/OperatingSystem`
- `408/ComputerNetwork`
- `AI/MachineLearning`
- `AI/DeepLearning`
- `Work/Engineering`
- `Interview/Postgraduate`

### 6. 备考中心

入口：`/growth/postgraduate`

用途：

- 专门管理 408 和暨南大学非全日制人工智能备考
- 管初试，也管复试和作品集

模块拆分：

- 408 科目进度
- 章节任务
- 真题训练
- 错题复盘
- 阶段计划
- 复试材料
- 项目作品集

推荐 408 科目表：

```text
PostgraduateSubjects
- Id
- Name
- Code
- TotalChapters
- CompletedChapters
- Progress
- CurrentFocus
- SortOrder
```

推荐备考任务表：

```text
PostgraduateTasks
- Id
- SubjectId
- Title
- TaskType
- DueDate
- EstimatedMinutes
- ActualMinutes
- Progress
- Status
- Note
- CreatedAt
- UpdatedAt
```

### 7. 项目管理

入口：`/growth/projects`

用途：

- 管工作项目、个人网站、AI 作品集
- 为复试和职业成长服务

推荐项目：

- Personal Growth Website
- 408 Knowledge Graph
- AI Learning Portfolio
- Work Automation Toolkit

## 数据库设计建议

第一阶段先做这些表：

```text
Users
DailyPlans
Habits
HabitCheckIns
WorkLogs
KnowledgeArticles
PostgraduateSubjects
PostgraduateTasks
Projects
ProjectMilestones
```

通用字段：

```text
Id char(36) primary key
CreatedAt datetime
UpdatedAt datetime null
DeletedAt datetime null
```

建议所有业务表都加：

```text
UserId char(36)
```

即使现在只有你一个用户，也给后续扩展留好边界。

## API 设计

统一响应结构已经和前端适配：

```json
{
  "code": 0,
  "message": "success",
  "data": {}
}
```

分页结构：

```json
{
  "items": [],
  "page": 1,
  "pageSize": 10,
  "total": 0,
  "totalPages": 0
}
```

第一阶段接口：

```text
GET    /api/dashboard/summary
GET    /api/daily-plans
POST   /api/daily-plans
PUT    /api/daily-plans/{id}
PATCH  /api/daily-plans/{id}/complete
DELETE /api/daily-plans/{id}

GET    /api/habits
POST   /api/habits
POST   /api/habits/{id}/check-ins

GET    /api/work-logs
POST   /api/work-logs

GET    /api/knowledge-base
POST   /api/knowledge-base

GET    /api/postgraduate/subjects
GET    /api/postgraduate/tasks
POST   /api/postgraduate/tasks

GET    /api/projects
POST   /api/projects
```

## 前端落地顺序

1. 保留 Vben 登录、布局、权限、菜单能力。
2. 用 `growth` 作为主业务域。
3. 每个模块统一采用：查询表单 + 表格 + 弹窗表单 + 详情/复盘。
4. 看板只做聚合，不承载复杂编辑。
5. 备考中心优先做 408 科目进度和每日任务，不急着做复杂图表。

## 后端落地顺序

1. 把 EF Core Provider 切到 MySQL。
2. 先跑通 `DailyPlans` 的 MySQL migration。
3. 补齐 Vben 所需认证接口，最终替换前端 mock：
   - `POST /api/auth/login`
   - `GET /api/auth/codes`
   - `GET /api/user/info`
4. 依次实现 Habits、WorkLogs、KnowledgeArticles、PostgraduateTasks、Projects。
5. 最后做 Dashboard 聚合接口。

## 每日使用流程

```text
早上：
打开成长看板 -> 确认今日计划 -> 选定 1 个工作重点和 1 个 408 重点

中午：
补一条工作日志或错题记录

晚上：
完成 408 学习块 -> 更新备考任务 -> 记录每日计划完成情况

睡前：
习惯打卡 -> 1 分钟复盘 -> 明日计划草稿
```

## 当前代码状态

已存在：

- 前端 `growth` 路由和页面结构
- `DailyPlan` 前端页面、表单、Store、API
- `Habit` 前端页面、表单、API
- 后端 `DailyPlansController`
- 后端 `DailyPlanService`

需要继续补：

- MySQL migration
- Auth/User/Codes 后端接口
- Habits 后端接口
- PostgraduateTasks 后端接口
- Dashboard 聚合接口
