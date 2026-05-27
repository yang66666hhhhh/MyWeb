# 习惯打卡 API

## 目录
- [获取习惯列表](#获取习惯列表)
- [获取习惯详情](#获取习惯详情)
- [创建习惯](#创建习惯)
- [更新习惯](#更新习惯)
- [删除习惯](#删除习惯)
- [打卡](#打卡)
- [更新状态](#更新状态)

---

## 获取习惯列表

**GET** `/api/growth/habits`

### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索 |
| habitType | string | 否 | 习惯类型 |
| status | int | 否 | 状态（0: 进行中, 1: 已暂停, 2: 已归档） |
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
        "name": "早起",
        "habitType": "健康",
        "description": "每天早上6点起床",
        "targetFrequency": "每天",
        "status": 0,
        "currentStreak": 7,
        "longestStreak": 30,
        "totalCheckIns": 50,
        "lastCheckInDate": "2026-05-28",
        "createdAt": "2026-01-01T00:00:00Z",
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

## 获取习惯详情

**GET** `/api/growth/habits/{id}`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 习惯ID |

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "早起",
    "habitType": "健康",
    "description": "每天早上6点起床",
    "targetFrequency": "每天",
    "status": 0,
    "currentStreak": 7,
    "longestStreak": 30,
    "totalCheckIns": 50,
    "lastCheckInDate": "2026-05-28",
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": null,
    "recentCheckIns": [
      {
        "id": "guid",
        "checkInDate": "2026-05-28",
        "remark": "今天6点准时起床"
      },
      {
        "id": "guid",
        "checkInDate": "2026-05-27",
        "remark": null
      }
    ]
  },
  "success": true
}
```

### 错误响应

```json
{
  "code": 404,
  "message": "习惯不存在",
  "success": false
}
```

---

## 创建习惯

**POST** `/api/growth/habits`

### 请求体

```json
{
  "name": "早起",
  "habitType": "健康",
  "description": "每天早上6点起床",
  "targetFrequency": "每天"
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| name | string | 是 | 习惯名称（最大100字符） |
| habitType | string | 是 | 习惯类型（最大50字符） |
| description | string | 否 | 描述（最大500字符） |
| targetFrequency | string | 否 | 目标频率（默认: "每天", 最大20字符） |

### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "早起",
    "habitType": "健康",
    "description": "每天早上6点起床",
    "targetFrequency": "每天",
    "status": 0,
    "currentStreak": 0,
    "longestStreak": 0,
    "totalCheckIns": 0,
    "lastCheckInDate": null,
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
  "message": "习惯名称不能为空",
  "success": false
}
```

---

## 更新习惯

**PUT** `/api/growth/habits/{id}`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 习惯ID |

### 请求体

```json
{
  "name": "早起",
  "habitType": "健康",
  "description": "更新后的描述",
  "targetFrequency": "每天"
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| name | string | 否 | 习惯名称（最大100字符） |
| habitType | string | 否 | 习惯类型（最大50字符） |
| description | string | 否 | 描述（最大500字符） |
| targetFrequency | string | 否 | 目标频率（最大20字符） |
| status | int | 否 | 状态（0: 进行中, 1: 已暂停, 2: 已归档） |

### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "早起",
    "habitType": "健康",
    "description": "更新后的描述",
    "targetFrequency": "每天",
    "status": 0,
    "currentStreak": 7,
    "longestStreak": 30,
    "totalCheckIns": 50,
    "lastCheckInDate": "2026-05-28",
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": "2026-05-28T11:00:00Z"
  },
  "success": true
}
```

### 错误响应

```json
{
  "code": 403,
  "message": "无权限修改此习惯",
  "success": false
}
```

---

## 删除习惯

**DELETE** `/api/growth/habits/{id}`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 习惯ID |

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
  "code": 403,
  "message": "无权限删除此习惯",
  "success": false
}
```

---

## 打卡

**POST** `/api/growth/habits/{id}/check-in`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 习惯ID |

### 请求体

```json
{
  "remark": "今天6点准时起床"
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| remark | string | 否 | 备注 |

### 响应

```json
{
  "code": 200,
  "message": "打卡成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "早起",
    "habitType": "健康",
    "description": "每天早上6点起床",
    "targetFrequency": "每天",
    "status": 0,
    "currentStreak": 8,
    "longestStreak": 30,
    "totalCheckIns": 51,
    "lastCheckInDate": "2026-05-28",
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": "2026-05-28T10:00:00Z"
  },
  "success": true
}
```

### 错误响应

```json
{
  "code": 403,
  "message": "无权限打卡",
  "success": false
}
```

---

## 更新状态

**PUT** `/api/growth/habits/{id}/status`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 习惯ID |

### 请求体

```json
{
  "status": 1
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| status | int | 是 | 状态（0: 进行中, 1: 已暂停, 2: 已归档） |

### 响应

```json
{
  "code": 200,
  "message": "状态更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "早起",
    "habitType": "健康",
    "description": "每天早上6点起床",
    "targetFrequency": "每天",
    "status": 1,
    "currentStreak": 7,
    "longestStreak": 30,
    "totalCheckIns": 50,
    "lastCheckInDate": "2026-05-28",
    "createdAt": "2026-01-01T00:00:00Z",
    "updatedAt": "2026-05-28T11:00:00Z"
  },
  "success": true
}
```

### 错误响应

```json
{
  "code": 400,
  "message": "状态值不能为空",
  "success": false
}
```

---

## 数据模型

### HabitDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 习惯ID |
| userId | guid | 用户ID |
| name | string | 习惯名称 |
| habitType | string | 习惯类型 |
| description | string | 描述 |
| targetFrequency | string | 目标频率 |
| status | int | 状态 |
| currentStreak | int | 当前连续天数 |
| longestStreak | int | 最长连续天数 |
| totalCheckIns | int | 总打卡次数 |
| lastCheckInDate | string (date) | 最后打卡日期 |
| createdAt | string (datetime) | 创建时间 |
| updatedAt | string (datetime) | 更新时间 |

### HabitDetailDto

继承 HabitDto，额外包含：

| 字段名 | 类型 | 说明 |
|--------|------|------|
| recentCheckIns | array | 最近打卡记录 |

### CheckInRecordDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 打卡记录ID |
| checkInDate | string (date) | 打卡日期 |
| remark | string | 备注 |

### 状态枚举

| 值 | 说明 |
|----|------|
| 0 | 进行中 (Active) |
| 1 | 已暂停 (Paused) |
| 2 | 已归档 (Archived) |
