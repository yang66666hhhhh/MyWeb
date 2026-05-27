# 每日计划 API

## 目录
- [获取计划列表](#获取计划列表)
- [获取计划详情](#获取计划详情)
- [创建计划](#创建计划)
- [更新计划](#更新计划)
- [完成计划](#完成计划)
- [删除计划](#删除计划)

---

## 获取计划列表

**GET** `/api/growth/daily-plans`

### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| startDate | string (date) | 否 | 开始日期 |
| endDate | string (date) | 否 | 结束日期 |
| status | int | 否 | 状态（0: 待完成, 1: 已完成, 2: 已取消） |
| keyword | string | 否 | 关键词搜索 |
| page | int | 否 | 页码（默认: 1） |
| pageSize | int | 否 | 每页数量（默认: 10, 最大: 100） |

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
        "priority": 3,
        "status": 0,
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

### 状态码

| 状态码 | 说明 |
|--------|------|
| 200 | 成功 |
| 401 | 未授权 |

---

## 获取计划详情

**GET** `/api/growth/daily-plans/{id}`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 计划ID |

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
    "priority": 3,
    "status": 0,
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
  "message": "每日计划不存在",
  "success": false
}
```

### 状态码

| 状态码 | 说明 |
|--------|------|
| 200 | 成功 |
| 401 | 未授权 |
| 404 | 计划不存在 |

---

## 创建计划

**POST** `/api/growth/daily-plans`

### 请求体

```json
{
  "planDate": "2026-05-28",
  "title": "完成项目文档",
  "description": "编写README和API文档",
  "priority": 3,
  "status": 0,
  "remark": null
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| planDate | string (date) | 是 | 计划日期 |
| title | string | 是 | 标题（最大120字符） |
| description | string | 否 | 描述（最大1000字符） |
| priority | int | 否 | 优先级（1-5, 默认3） |
| status | int | 否 | 状态（0: 待完成, 1: 已完成, 2: 已取消） |
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
    "priority": 3,
    "status": 0,
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
  "code": 400,
  "message": "标题不能为空",
  "success": false
}
```

### 状态码

| 状态码 | 说明 |
|--------|------|
| 200 | 创建成功 |
| 400 | 请求参数错误 |
| 401 | 未授权 |

---

## 更新计划

**PUT** `/api/growth/daily-plans/{id}`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 计划ID |

### 请求体

```json
{
  "planDate": "2026-05-28",
  "title": "更新后的标题",
  "description": "更新后的描述",
  "priority": 2,
  "status": 0,
  "remark": "添加备注"
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| planDate | string (date) | 是 | 计划日期 |
| title | string | 是 | 标题（最大120字符） |
| description | string | 否 | 描述（最大1000字符） |
| priority | int | 否 | 优先级（1-5） |
| status | int | 否 | 状态（0: 待完成, 1: 已完成, 2: 已取消） |
| remark | string | 否 | 备注（最大1000字符） |

### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "planDate": "2026-05-28",
    "title": "更新后的标题",
    "description": "更新后的描述",
    "priority": 2,
    "status": 0,
    "remark": "添加备注",
    "completedAt": null,
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": "2026-05-28T11:00:00Z"
  },
  "success": true
}
```

### 错误响应

```json
{
  "code": 404,
  "message": "每日计划不存在",
  "success": false
}
```

### 状态码

| 状态码 | 说明 |
|--------|------|
| 200 | 更新成功 |
| 400 | 请求参数错误 |
| 401 | 未授权 |
| 404 | 计划不存在 |

---

## 完成计划

**PATCH** `/api/growth/daily-plans/{id}/complete`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 计划ID |

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
    "priority": 3,
    "status": 1,
    "remark": null,
    "completedAt": "2026-05-28T15:30:00Z",
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": "2026-05-28T15:30:00Z"
  },
  "success": true
}
```

### 错误响应

```json
{
  "code": 404,
  "message": "每日计划不存在",
  "success": false
}
```

### 状态码

| 状态码 | 说明 |
|--------|------|
| 200 | 完成成功 |
| 401 | 未授权 |
| 404 | 计划不存在 |

---

## 删除计划

**DELETE** `/api/growth/daily-plans/{id}`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 计划ID |

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
  "message": "每日计划不存在",
  "success": false
}
```

### 状态码

| 状态码 | 说明 |
|--------|------|
| 200 | 删除成功 |
| 401 | 未授权 |
| 404 | 计划不存在 |

---

## 数据模型

### DailyPlanDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 计划ID |
| userId | guid | 用户ID |
| planDate | string (date) | 计划日期 |
| title | string | 标题 |
| description | string | 描述 |
| priority | int | 优先级（1-5） |
| status | int | 状态（0: 待完成, 1: 已完成, 2: 已取消） |
| remark | string | 备注 |
| completedAt | string (datetime) | 完成时间 |
| createdAt | string (datetime) | 创建时间 |
| updatedAt | string (datetime) | 更新时间 |

### 状态枚举

| 值 | 说明 |
|----|------|
| 0 | 待完成 (Pending) |
| 1 | 已完成 (Completed) |
| 2 | 已取消 (Cancelled) |

### 优先级说明

| 值 | 说明 |
|----|------|
| 1 | 最高 |
| 2 | 高 |
| 3 | 中（默认） |
| 4 | 低 |
| 5 | 最低 |
