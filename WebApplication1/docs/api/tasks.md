# 任务管理 API

## 目录
- [获取任务列表](#获取任务列表)
- [获取任务详情](#获取任务详情)
- [创建任务](#创建任务)
- [更新任务](#更新任务)
- [删除任务](#删除任务)
- [完成任务](#完成任务)
- [转换为工作日志](#转换为工作日志)

---

## 获取任务列表

**GET** `/api/growth/tasks`

### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| userId | guid | 否 | 用户ID（管理员可用） |
| keyword | string | 否 | 关键词搜索 |
| taskType | string | 否 | 任务类型（Personal/Work/Study） |
| source | string | 否 | 任务来源（Growth/Work/AI） |
| planDate | string (date) | 否 | 计划日期 |
| startDate | string (date) | 否 | 开始日期 |
| endDate | string (date) | 否 | 结束日期 |
| projectId | guid | 否 | 关联项目ID |
| status | int | 否 | 状态（0: 待处理, 1: 进行中, 2: 已完成, 3: 已取消） |
| priority | int | 否 | 优先级（1: 最高, 2: 高, 3: 中, 4: 低） |
| page | int | 否 | 页码（默认: 1） |
| pageSize | int | 否 | 每页数量（默认: 10） |

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "items": [
      {
        "id": "guid",
        "userId": "guid",
        "planDate": "2026-05-28",
        "title": "完成项目文档",
        "description": "编写README和API文档",
        "type": "Personal",
        "source": "Growth",
        "projectId": "guid",
        "projectName": "个人成长系统",
        "priority": "Medium",
        "status": "Pending",
        "startTime": "09:00",
        "endTime": "12:00",
        "estimatedHours": 3,
        "actualHours": null,
        "convertedWorkLogId": null,
        "remark": null,
        "completedAt": null,
        "createdAt": "2026-05-28T10:00:00Z",
        "updatedAt": null
      }
    ],
    "total": 50,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

---

## 获取任务详情

**GET** `/api/growth/tasks/{id}`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 任务ID |

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "planDate": "2026-05-28",
    "title": "完成项目文档",
    "description": "编写README和API文档",
    "type": "Personal",
    "source": "Growth",
    "projectId": "guid",
    "projectName": "个人成长系统",
    "priority": "Medium",
    "status": "Pending",
    "startTime": "09:00",
    "endTime": "12:00",
    "estimatedHours": 3,
    "actualHours": null,
    "convertedWorkLogId": null,
    "remark": null,
    "completedAt": null,
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

### 错误响应

```json
{
  "code": 404,
  "message": "任务不存在",
  "success": false
}
```

---

## 创建任务

**POST** `/api/growth/tasks`

### 请求体

```json
{
  "planDate": "2026-05-28",
  "title": "完成项目文档",
  "description": "编写README和API文档",
  "taskType": "Personal",
  "source": "Growth",
  "projectId": "guid",
  "priority": 2,
  "startTime": "09:00",
  "endTime": "12:00",
  "estimatedHours": 3,
  "remark": null
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| planDate | string (date) | 是 | 计划日期 |
| title | string | 是 | 任务标题（最大200字符） |
| description | string | 否 | 任务描述（最大2000字符） |
| taskType | string | 否 | 任务类型（默认: Personal, 可选: Personal/Work/Study） |
| source | string | 否 | 任务来源（默认: Growth, 可选: Growth/Work/AI） |
| projectId | guid | 否 | 关联项目ID |
| priority | int | 否 | 优先级（默认: 2, 1: 最高, 2: 高, 3: 中, 4: 低） |
| startTime | string | 否 | 开始时间（最大10字符） |
| endTime | string | 否 | 结束时间（最大10字符） |
| estimatedHours | decimal | 否 | 预计工时 |
| remark | string | 否 | 备注（最大1000字符） |

### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "planDate": "2026-05-28",
    "title": "完成项目文档",
    "description": "编写README和API文档",
    "type": "Personal",
    "source": "Growth",
    "projectId": "guid",
    "projectName": "个人成长系统",
    "priority": "Medium",
    "status": "Pending",
    "startTime": "09:00",
    "endTime": "12:00",
    "estimatedHours": 3,
    "actualHours": null,
    "convertedWorkLogId": null,
    "remark": null,
    "completedAt": null,
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

---

## 更新任务

**PUT** `/api/growth/tasks/{id}`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 任务ID |

### 请求体

```json
{
  "planDate": "2026-05-29",
  "title": "完成项目文档（更新）",
  "description": "更新后的描述",
  "taskType": "Personal",
  "source": "Growth",
  "projectId": "guid",
  "priority": 1,
  "status": 1,
  "startTime": "10:00",
  "endTime": "14:00",
  "estimatedHours": 4,
  "actualHours": 2,
  "remark": "进度更新"
}
```

### 字段说明

所有字段均为可选，只更新提供的字段。

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| planDate | string (date) | 否 | 计划日期 |
| title | string | 否 | 任务标题（最大200字符） |
| description | string | 否 | 任务描述（最大2000字符） |
| taskType | string | 否 | 任务类型（Personal/Work/Study） |
| source | string | 否 | 任务来源（Growth/Work/AI） |
| projectId | guid | 否 | 关联项目ID |
| priority | int | 否 | 优先级（1-4） |
| status | int | 否 | 状态（0-3） |
| startTime | string | 否 | 开始时间 |
| endTime | string | 否 | 结束时间 |
| estimatedHours | decimal | 否 | 预计工时 |
| actualHours | decimal | 否 | 实际工时 |
| remark | string | 否 | 备注 |

### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "planDate": "2026-05-29",
    "title": "完成项目文档（更新）",
    "description": "更新后的描述",
    "type": "Personal",
    "source": "Growth",
    "projectId": "guid",
    "projectName": "个人成长系统",
    "priority": "High",
    "status": "InProgress",
    "startTime": "10:00",
    "endTime": "14:00",
    "estimatedHours": 4,
    "actualHours": 2,
    "convertedWorkLogId": null,
    "remark": "进度更新",
    "completedAt": null,
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": "2026-05-28T11:00:00Z"
  },
  "success": true
}
```

---

## 删除任务

**DELETE** `/api/growth/tasks/{id}`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 任务ID |

### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

### 错误响应

```json
{
  "code": 404,
  "message": "任务不存在",
  "success": false
}
```

---

## 完成任务

**POST** `/api/growth/tasks/{id}/complete`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 任务ID |

### 响应

```json
{
  "code": 200,
  "message": "已完成",
  "data": {
    "id": "guid",
    "userId": "guid",
    "planDate": "2026-05-28",
    "title": "完成项目文档",
    "description": "编写README和API文档",
    "type": "Personal",
    "source": "Growth",
    "projectId": "guid",
    "projectName": "个人成长系统",
    "priority": "Medium",
    "status": "Completed",
    "startTime": "09:00",
    "endTime": "12:00",
    "estimatedHours": 3,
    "actualHours": 3,
    "convertedWorkLogId": null,
    "remark": null,
    "completedAt": "2026-05-28T15:00:00Z",
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": "2026-05-28T15:00:00Z"
  },
  "success": true
}
```

### 说明

- 完成任务会自动设置 `completedAt` 为当前时间
- 状态会自动更新为 `Completed`（2）

---

## 转换为工作日志

**POST** `/api/growth/tasks/convert-to-log`

### 请求体

```json
{
  "taskId": "guid",
  "workDate": "2026-05-28",
  "originalContent": "完成项目文档编写",
  "totalHours": 3
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| taskId | guid | 是 | 任务ID |
| workDate | string (date) | 是 | 工作日期 |
| originalContent | string | 否 | 原始内容（最大4000字符） |
| totalHours | decimal | 否 | 总工时 |

### 响应

```json
{
  "code": 200,
  "message": "已转换为工作日志",
  "data": {
    "workLogId": "guid"
  },
  "success": true
}
```

### 说明

- 转换后会在任务中设置 `convertedWorkLogId`
- 可以通过该ID关联到工作日志

---

## 数据模型

### TaskItemDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 任务ID |
| userId | guid | 用户ID |
| planDate | string (date) | 计划日期 |
| title | string | 标题 |
| description | string | 描述 |
| type | string | 任务类型 |
| source | string | 任务来源 |
| projectId | guid | 关联项目ID |
| projectName | string | 项目名称 |
| priority | string | 优先级 |
| status | string | 状态 |
| startTime | string | 开始时间 |
| endTime | string | 结束时间 |
| estimatedHours | decimal | 预计工时 |
| actualHours | decimal | 实际工时 |
| convertedWorkLogId | guid | 转换的工作日志ID |
| remark | string | 备注 |
| completedAt | string (datetime) | 完成时间 |
| createdAt | string (datetime) | 创建时间 |
| updatedAt | string (datetime) | 更新时间 |

### 任务类型枚举

| 值 | 说明 |
|----|------|
| Personal | 个人任务 |
| Work | 工作任务 |
| Study | 学习任务 |

### 任务来源枚举

| 值 | 说明 |
|----|------|
| Growth | 成长模块 |
| Work | 工作模块 |
| AI | AI生成 |

### 优先级枚举

| 值 | 说明 |
|----|------|
| 1 | 最高 |
| 2 | 高 |
| 3 | 中（默认） |
| 4 | 低 |

### 状态枚举

| 值 | 说明 |
|----|------|
| 0 | 待处理 (Pending) |
| 1 | 进行中 (InProgress) |
| 2 | 已完成 (Completed) |
| 3 | 已取消 (Cancelled) |

---

## 权限说明

- 普通用户只能查看和管理自己的任务
- Pro及以上角色可以通过 `userId` 参数查看其他用户的任务
- 需要 `WORK_TASK` 功能权限才能访问
