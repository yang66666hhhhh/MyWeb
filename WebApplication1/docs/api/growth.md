# 成长模块 API

## 目录
- [知识库管理](#知识库管理)
  - [获取知识文章列表](#获取知识文章列表)
  - [获取知识文章详情](#获取知识文章详情)
  - [创建知识文章](#创建知识文章)
  - [更新知识文章](#更新知识文章)
  - [删除知识文章](#删除知识文章)
- [成长项目管理](#成长项目管理)
  - [获取成长项目列表](#获取成长项目列表)
  - [获取成长项目详情](#获取成长项目详情)
  - [创建成长项目](#创建成长项目)
  - [更新成长项目](#更新成长项目)
  - [删除成长项目](#删除成长项目)
- [考研备选管理](#考研备选管理)
  - [获取考研任务列表](#获取考研任务列表)
  - [获取考研任务详情](#获取考研任务详情)
  - [创建考研任务](#创建考研任务)
  - [更新考研任务](#更新考研任务)
  - [删除考研任务](#删除考研任务)

---

## 知识库管理

### 获取知识文章列表

**GET** `/api/growth/knowledge-base`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索（标题、内容） |
| category | string | 否 | 分类筛选 |
| isPublished | bool | 否 | 是否已发布 |
| startDate | string | 否 | 开始日期 |
| endDate | string | 否 | 结束日期 |
| page | int | 否 | 页码（默认: 1） |
| pageSize | int | 否 | 每页数量（默认: 10） |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "items": [
      {
        "id": "guid",
        "userId": "guid",
        "title": "C# 编程最佳实践",
        "content": "本文介绍了C#编程中的一些最佳实践...",
        "category": "编程",
        "tags": "C#,编程,最佳实践",
        "isPublished": true,
        "viewCount": 150,
        "createdAt": "2026-05-20T10:00:00Z",
        "updatedAt": null
      }
    ],
    "total": 30,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

---

### 获取知识文章详情

**GET** `/api/growth/knowledge-base/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 文章ID |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "C# 编程最佳实践",
    "content": "本文介绍了C#编程中的一些最佳实践...",
    "category": "编程",
    "tags": "C#,编程,最佳实践",
    "isPublished": true,
    "viewCount": 151,
    "createdAt": "2026-05-20T10:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

#### 说明

- 每次获取详情会自动增加 `viewCount`（浏览次数）

---

### 创建知识文章

**POST** `/api/growth/knowledge-base`

#### 请求体

```json
{
  "title": "C# 编程最佳实践",
  "content": "本文介绍了C#编程中的一些最佳实践...",
  "category": "编程",
  "tags": "C#,编程,最佳实践",
  "isPublished": false
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| title | string | 是 | 文章标题（最大200字符） |
| content | string | 否 | 文章内容（最大10000字符） |
| category | string | 否 | 分类（最大50字符） |
| tags | string | 否 | 标签，逗号分隔（最大500字符） |
| isPublished | bool | 否 | 是否发布（默认: false） |

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "C# 编程最佳实践",
    "content": "本文介绍了C#编程中的一些最佳实践...",
    "category": "编程",
    "tags": "C#,编程,最佳实践",
    "isPublished": false,
    "viewCount": 0,
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

---

### 更新知识文章

**PUT** `/api/growth/knowledge-base/{id}`

#### 请求体

```json
{
  "title": "C# 编程最佳实践（更新版）",
  "content": "更新后的内容...",
  "category": "编程",
  "tags": "C#,编程,最佳实践,更新",
  "isPublished": true
}
```

#### 字段说明

所有字段均为可选，只更新提供的字段。

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| title | string | 否 | 文章标题（最大200字符） |
| content | string | 否 | 文章内容（最大10000字符） |
| category | string | 否 | 分类（最大50字符） |
| tags | string | 否 | 标签，逗号分隔（最大500字符） |
| isPublished | bool | 否 | 是否发布 |

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "C# 编程最佳实践（更新版）",
    "content": "更新后的内容...",
    "category": "编程",
    "tags": "C#,编程,最佳实践,更新",
    "isPublished": true,
    "viewCount": 151,
    "createdAt": "2026-05-20T10:00:00Z",
    "updatedAt": "2026-05-28T10:00:00Z"
  },
  "success": true
}
```

---

### 删除知识文章

**DELETE** `/api/growth/knowledge-base/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 文章ID |

#### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

---

## 成长项目管理

### 获取成长项目列表

**GET** `/api/growth/projects`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索 |
| type | int | 否 | 项目类型（0: 学习, 1: 健身, 2: 阅读, 3: 其他） |
| status | int | 否 | 状态（0: 计划中, 1: 进行中, 2: 已完成, 3: 已暂停） |
| startDate | string | 否 | 开始日期 |
| endDate | string | 否 | 结束日期 |
| page | int | 否 | 页码（默认: 1） |
| pageSize | int | 否 | 每页数量（默认: 10） |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "items": [
      {
        "id": "guid",
        "userId": "guid",
        "name": "学习C#高级特性",
        "description": "深入学习C#的高级特性...",
        "type": "Learning",
        "status": "InProgress",
        "progress": 60,
        "taskCount": 10,
        "startDate": "2026-05-01",
        "endDate": "2026-06-30",
        "createdAt": "2026-05-01T10:00:00Z",
        "updatedAt": null
      }
    ],
    "total": 10,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

---

### 获取成长项目详情

**GET** `/api/growth/projects/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 项目ID |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "学习C#高级特性",
    "description": "深入学习C#的高级特性...",
    "type": "Learning",
    "status": "InProgress",
    "progress": 60,
    "taskCount": 10,
    "startDate": "2026-05-01",
    "endDate": "2026-06-30",
    "createdAt": "2026-05-01T10:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

---

### 创建成长项目

**POST** `/api/growth/projects`

#### 请求体

```json
{
  "name": "学习C#高级特性",
  "description": "深入学习C#的高级特性...",
  "type": 0,
  "startDate": "2026-05-01",
  "endDate": "2026-06-30"
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| name | string | 是 | 项目名称（最大100字符） |
| description | string | 否 | 项目描述（最大1000字符） |
| type | int | 否 | 项目类型（默认: 0） |
| startDate | string | 否 | 开始日期 |
| endDate | string | 否 | 结束日期 |

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "学习C#高级特性",
    "description": "深入学习C#的高级特性...",
    "type": "Learning",
    "status": "Planing",
    "progress": 0,
    "taskCount": 0,
    "startDate": "2026-05-01",
    "endDate": "2026-06-30",
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

---

### 更新成长项目

**PUT** `/api/growth/projects/{id}`

#### 请求体

```json
{
  "name": "学习C#高级特性（更新）",
  "description": "更新后的描述...",
  "type": 0,
  "status": 1,
  "progress": 50,
  "startDate": "2026-05-01",
  "endDate": "2026-07-31"
}
```

#### 字段说明

所有字段均为可选，只更新提供的字段。

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| name | string | 否 | 项目名称 |
| description | string | 否 | 项目描述 |
| type | int | 否 | 项目类型 |
| status | int | 否 | 状态 |
| progress | int | 否 | 进度（0-100） |
| startDate | string | 否 | 开始日期 |
| endDate | string | 否 | 结束日期 |

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "学习C#高级特性（更新）",
    "description": "更新后的描述...",
    "type": "Learning",
    "status": "InProgress",
    "progress": 50,
    "taskCount": 10,
    "startDate": "2026-05-01",
    "endDate": "2026-07-31",
    "createdAt": "2026-05-01T10:00:00Z",
    "updatedAt": "2026-05-28T10:00:00Z"
  },
  "success": true
}
```

---

### 删除成长项目

**DELETE** `/api/growth/projects/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 项目ID |

#### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

---

## 考研备选管理

### 获取考研任务列表

**GET** `/api/growth/postgraduate`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索 |
| type | int | 否 | 任务类型（0: 学习, 1: 复习, 2: 练习, 3: 模拟考试） |
| status | int | 否 | 状态（0: 待处理, 1: 进行中, 2: 已完成） |
| priority | int | 否 | 优先级（1: 最高, 2: 高, 3: 中, 4: 低） |
| startDate | string | 否 | 开始日期 |
| endDate | string | 否 | 结束日期 |
| page | int | 否 | 页码（默认: 1） |
| pageSize | int | 否 | 每页数量（默认: 10） |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "items": [
      {
        "id": "guid",
        "userId": "guid",
        "title": "高等数学复习",
        "description": "复习高等数学第一章",
        "type": "Review",
        "status": "InProgress",
        "priority": "High",
        "dueDate": "2026-06-01",
        "createdAt": "2026-05-20T10:00:00Z",
        "updatedAt": null
      }
    ],
    "total": 20,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

---

### 获取考研任务详情

**GET** `/api/growth/postgraduate/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 任务ID |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "高等数学复习",
    "description": "复习高等数学第一章",
    "type": "Review",
    "status": "InProgress",
    "priority": "High",
    "dueDate": "2026-06-01",
    "createdAt": "2026-05-20T10:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

---

### 创建考研任务

**POST** `/api/growth/postgraduate`

#### 请求体

```json
{
  "title": "高等数学复习",
  "description": "复习高等数学第一章",
  "type": 1,
  "priority": 2,
  "dueDate": "2026-06-01"
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| title | string | 是 | 任务标题（最大200字符） |
| description | string | 否 | 任务描述（最大1000字符） |
| type | int | 否 | 任务类型（默认: 0） |
| priority | int | 否 | 优先级（默认: 2） |
| dueDate | string | 否 | 截止日期 |

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "高等数学复习",
    "description": "复习高等数学第一章",
    "type": "Review",
    "status": "Pending",
    "priority": "High",
    "dueDate": "2026-06-01",
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

---

### 更新考研任务

**PUT** `/api/growth/postgraduate/{id}`

#### 请求体

```json
{
  "title": "高等数学复习（更新）",
  "description": "更新后的描述",
  "type": 1,
  "status": 2,
  "priority": 1,
  "dueDate": "2026-06-15"
}
```

#### 字段说明

所有字段均为可选，只更新提供的字段。

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| title | string | 否 | 任务标题 |
| description | string | 否 | 任务描述 |
| type | int | 否 | 任务类型 |
| status | int | 否 | 状态 |
| priority | int | 否 | 优先级 |
| dueDate | string | 否 | 截止日期 |

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "高等数学复习（更新）",
    "description": "更新后的描述",
    "type": "Review",
    "status": "Completed",
    "priority": "Highest",
    "dueDate": "2026-06-15",
    "createdAt": "2026-05-20T10:00:00Z",
    "updatedAt": "2026-05-28T10:00:00Z"
  },
  "success": true
}
```

---

### 删除考研任务

**DELETE** `/api/growth/postgraduate/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 任务ID |

#### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

---

## 数据模型

### KnowledgeArticleDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 文章ID |
| userId | guid | 用户ID |
| title | string | 标题 |
| content | string | 内容 |
| category | string | 分类 |
| tags | string | 标签（逗号分隔） |
| isPublished | bool | 是否已发布 |
| viewCount | int | 浏览次数 |
| createdAt | string (datetime) | 创建时间 |
| updatedAt | string (datetime) | 更新时间 |

### GrowthProjectDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 项目ID |
| userId | guid | 用户ID |
| name | string | 项目名称 |
| description | string | 项目描述 |
| type | string | 项目类型 |
| status | string | 状态 |
| progress | int | 进度（0-100） |
| taskCount | int | 任务数量 |
| startDate | string | 开始日期 |
| endDate | string | 结束日期 |
| createdAt | string (datetime) | 创建时间 |
| updatedAt | string (datetime) | 更新时间 |

### PostgraduateTaskDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 任务ID |
| userId | guid | 用户ID |
| title | string | 标题 |
| description | string | 描述 |
| type | string | 任务类型 |
| status | string | 状态 |
| priority | string | 优先级 |
| dueDate | string | 截止日期 |
| createdAt | string (datetime) | 创建时间 |
| updatedAt | string (datetime) | 更新时间 |

### 枚举类型

#### 成长项目类型

| 值 | 说明 |
|----|------|
| 0 | 学习 (Learning) |
| 1 | 健身 (Fitness) |
| 2 | 阅读 (Reading) |
| 3 | 其他 (Other) |

#### 成长项目状态

| 值 | 说明 |
|----|------|
| 0 | 计划中 (Planing) |
| 1 | 进行中 (InProgress) |
| 2 | 已完成 (Completed) |
| 3 | 已暂停 (Paused) |

#### 考研任务类型

| 值 | 说明 |
|----|------|
| 0 | 学习 (Study) |
| 1 | 复习 (Review) |
| 2 | 练习 (Practice) |
| 3 | 模拟考试 (MockExam) |

#### 考研任务状态

| 值 | 说明 |
|----|------|
| 0 | 待处理 (Pending) |
| 1 | 进行中 (InProgress) |
| 2 | 已完成 (Completed) |

---

## 权限说明

- 普通用户只能查看和管理自己创建的内容
- Pro及以上角色可以查看所有用户的内容
- 获取知识文章详情会自动增加浏览次数
