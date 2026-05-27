# 工作管理 API

## 目录
- [工作日志](#工作日志)
- [工作项目](#工作项目)
- [工作设备](#工作设备)
- [任务类型](#任务类型)
- [工作分类](#工作分类)
- [工作统计](#工作统计)
- [数据导入](#数据导入)
- [周计划](#周计划)
- [软件资产](#软件资产)

---

## 工作日志

### 获取日志列表

**GET** `/api/work/logs`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| userId | guid | 否 | 用户ID（管理员可用） |
| startDate | string (date) | 否 | 开始日期 |
| endDate | string (date) | 否 | 结束日期 |
| projectId | guid | 否 | 项目ID |
| status | int | 否 | 状态 |
| keyword | string | 否 | 关键词搜索 |
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
        "workDate": "2026-05-28",
        "weekDay": "Wednesday",
        "title": "项目开发",
        "summary": "完成API文档编写",
        "totalHours": 8,
        "projectId": "guid",
        "projectName": "个人成长系统",
        "status": 0,
        "createdAt": "2026-05-28T18:00:00Z"
      }
    ],
    "total": 100,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

### 获取日志详情

**GET** `/api/work/logs/{id}`

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "workDate": "2026-05-28",
    "weekDay": "Wednesday",
    "title": "项目开发",
    "originalContent": "原始内容",
    "summary": "完成API文档编写",
    "totalHours": 8,
    "projectId": "guid",
    "projectName": "个人成长系统",
    "templateId": "guid",
    "templateName": "开发日志",
    "status": 0,
    "items": [
      {
        "id": "guid",
        "content": "编写每日计划API文档",
        "hours": 4,
        "taskTypeId": "guid",
        "taskTypeName": "开发",
        "deviceId": "guid",
        "deviceName": "开发机"
      }
    ],
    "createdAt": "2026-05-28T18:00:00Z"
  },
  "success": true
}
```

### 创建日志

**POST** `/api/work/logs`

#### 请求体

```json
{
  "workDate": "2026-05-28",
  "title": "项目开发",
  "summary": "完成API文档编写",
  "projectId": "guid",
  "totalHours": 8,
  "items": [
    {
      "content": "编写每日计划API文档",
      "hours": 4,
      "taskTypeId": "guid",
      "deviceId": "guid"
    },
    {
      "content": "编写习惯打卡API文档",
      "hours": 4,
      "taskTypeId": "guid",
      "deviceId": "guid"
    }
  ]
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| workDate | string (date) | 是 | 工作日期 |
| title | string | 是 | 标题（最大200字符） |
| summary | string | 否 | 摘要（最大1000字符） |
| projectId | guid | 否 | 项目ID |
| totalHours | decimal | 是 | 总工时 |
| items | array | 否 | 工作条目 |
| items[].content | string | 是 | 内容（最大2000字符） |
| items[].hours | decimal | 否 | 工时 |
| items[].taskTypeId | guid | 否 | 任务类型ID |
| items[].deviceId | guid | 否 | 设备ID |

### 更新日志

**PUT** `/api/work/logs/{id}`

#### 请求体

同创建日志

### 删除日志

**DELETE** `/api/work/logs/{id}`

#### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

---

## 工作项目

### 获取项目列表

**GET** `/api/work/projects`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索 |
| status | int | 否 | 状态 |
| page | int | 否 | 页码 |
| pageSize | int | 否 | 每页数量 |

### 创建项目

**POST** `/api/work/projects`

#### 请求体

```json
{
  "projectName": "个人成长系统",
  "projectCode": "PGS-001",
  "projectType": 0,
  "customerName": "内部项目",
  "location": "北京",
  "description": "个人成长与工作管理系统",
  "startDate": "2026-01-01",
  "endDate": "2026-12-31"
}
```

---

## 工作设备

### 获取设备列表

**GET** `/api/work/devices`

### 创建设备

**POST** `/api/work/devices`

#### 请求体

```json
{
  "deviceName": "开发机",
  "deviceCode": "DEV-001",
  "deviceType": 1,
  "projectId": "guid",
  "description": "主力开发设备"
}
```

---

## 任务类型

### 获取任务类型列表

**GET** `/api/work/task-types`

### 创建任务类型

**POST** `/api/work/task-types`

#### 请求体

```json
{
  "typeName": "开发",
  "typeCode": "DEV",
  "description": "开发相关任务",
  "sort": 1
}
```

---

## 工作分类

### 获取分类列表

**GET** `/api/work/categories`

### 创建分类

**POST** `/api/work/categories`

#### 请求体

```json
{
  "name": "前端开发",
  "code": "FE",
  "level": 1,
  "parentId": null,
  "sort": 1
}
```

---

## 工作统计

### 获取统计数据

**GET** `/api/work/statistics`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| startDate | string (date) | 是 | 开始日期 |
| endDate | string (date) | 是 | 结束日期 |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "totalHours": 160,
    "totalDays": 20,
    "averageHoursPerDay": 8,
    "projectStats": [
      {
        "projectId": "guid",
        "projectName": "个人成长系统",
        "hours": 80,
        "percentage": 50
      }
    ],
    "taskTypeStats": [
      {
        "taskTypeId": "guid",
        "taskTypeName": "开发",
        "hours": 120,
        "percentage": 75
      }
    ]
  },
  "success": true
}
```

---

## 数据导入

### 导入工作日志

**POST** `/api/work/imports`

#### 请求

使用 `multipart/form-data` 格式上传文件

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| file | file | 是 | Excel文件 |
| importType | int | 是 | 导入类型 |
| importStrategy | int | 否 | 导入策略（0: 跳过重复, 1: 覆盖, 2: 仅新增） |

### 获取导入状态

**GET** `/api/work/imports/{id}`

---

## 周计划

### 获取周计划列表

**GET** `/api/work/weekly-plans`

### 创建周计划

**POST** `/api/work/weekly-plans`

#### 请求体

```json
{
  "year": 2026,
  "weekNumber": 22,
  "startDate": "2026-05-25",
  "endDate": "2026-05-31",
  "goals": "完成API文档编写",
  "tasks": [
    {
      "title": "编写认证API文档",
      "priority": 1,
      "estimatedHours": 4
    },
    {
      "title": "编写计划API文档",
      "priority": 2,
      "estimatedHours": 8
    }
  ]
}
```

---

## 软件资产

### 获取软件资产列表

**GET** `/api/work/software-assets`

### 创建软件资产

**POST** `/api/work/software-assets`

#### 请求体

```json
{
  "name": "Visual Studio",
  "version": "2022",
  "type": 1,
  "licenseType": 1,
  "vendor": "Microsoft",
  "purchaseDate": "2026-01-01",
  "expireDate": "2027-01-01",
  "cost": 999.99,
  "description": "开发工具"
}
```

---

## 数据模型

### WorkLogDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 日志ID |
| userId | guid | 用户ID |
| workDate | string (date) | 工作日期 |
| weekDay | string | 星期几 |
| title | string | 标题 |
| summary | string | 摘要 |
| totalHours | decimal | 总工时 |
| projectId | guid | 项目ID |
| projectName | string | 项目名称 |
| status | int | 状态 |
| items | array | 工作条目 |
| createdAt | string (datetime) | 创建时间 |

### WorkLogItemDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 条目ID |
| content | string | 内容 |
| hours | decimal | 工时 |
| taskTypeId | guid | 任务类型ID |
| taskTypeName | string | 任务类型名称 |
| deviceId | guid | 设备ID |
| deviceName | string | 设备名称 |
