# 数据分析 API

## 目录
- [仪表盘概览](#仪表盘概览)
- [任务趋势](#任务趋势)
- [任务分布](#任务分布)
- [工作与成长对比](#工作与成长对比)
- [优先级分布](#优先级分布)

---

## 仪表盘概览

**GET** `/api/growth/analytics/dashboard`

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "totalTasks": 150,
    "completedTasks": 100,
    "pendingTasks": 50,
    "completionRate": 66.7,
    "totalWorkLogs": 80,
    "totalWorkHours": 320,
    "todayTasks": 5,
    "todayCompletedTasks": 3,
    "todayWorkHours": 8
  },
  "success": true
}
```

### 字段说明

| 字段名 | 类型 | 说明 |
|--------|------|------|
| totalTasks | int | 总任务数 |
| completedTasks | int | 已完成任务数 |
| pendingTasks | int | 待处理任务数 |
| completionRate | decimal | 完成率（百分比） |
| totalWorkLogs | int | 总工作日志数 |
| totalWorkHours | decimal | 总工作时长 |
| todayTasks | int | 今日任务数 |
| todayCompletedTasks | int | 今日已完成任务数 |
| todayWorkHours | int | 今日工作时长 |

### 缓存说明

- 此接口有5分钟缓存
- 缓存键基于用户ID

---

## 任务趋势

**GET** `/api/growth/analytics/task-trends`

### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| startDate | string (date) | 是 | 开始日期 |
| endDate | string (date) | 是 | 结束日期 |

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": [
    {
      "date": "2026-05-20",
      "created": 5,
      "completed": 3,
      "completionRate": 60.0
    },
    {
      "date": "2026-05-21",
      "created": 8,
      "completed": 6,
      "completionRate": 75.0
    },
    {
      "date": "2026-05-22",
      "created": 3,
      "completed": 3,
      "completionRate": 100.0
    }
  ],
  "success": true
}
```

### 字段说明

| 字段名 | 类型 | 说明 |
|--------|------|------|
| date | string (date) | 日期 |
| created | int | 创建任务数 |
| completed | int | 完成任务数 |
| completionRate | decimal | 完成率（百分比） |

### 缓存说明

- 此接口有10分钟缓存
- 缓存键基于用户ID和日期范围

---

## 任务分布

**GET** `/api/growth/analytics/task-distribution`

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": [
    {
      "type": "Personal",
      "count": 60,
      "percentage": 40.0
    },
    {
      "type": "Work",
      "count": 50,
      "percentage": 33.3
    },
    {
      "type": "Study",
      "count": 40,
      "percentage": 26.7
    }
  ],
  "success": true
}
```

### 字段说明

| 字段名 | 类型 | 说明 |
|--------|------|------|
| type | string | 任务类型 |
| count | int | 任务数量 |
| percentage | decimal | 百分比 |

### 任务类型

| 值 | 说明 |
|----|------|
| Personal | 个人任务 |
| Work | 工作任务 |
| Study | 学习任务 |

### 缓存说明

- 此接口有10分钟缓存

---

## 工作与成长对比

**GET** `/api/growth/analytics/work-vs-growth`

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": [
    {
      "category": "Growth",
      "taskCount": 80
    },
    {
      "category": "Work",
      "taskCount": 70
    }
  ],
  "success": true
}
```

### 字段说明

| 字段名 | 类型 | 说明 |
|--------|------|------|
| category | string | 分类（Growth/Work） |
| taskCount | int | 任务数量 |

### 说明

- Growth: 来源于成长模块的任务
- Work: 来源于工作模块的任务

### 缓存说明

- 此接口有10分钟缓存

---

## 优先级分布

**GET** `/api/growth/analytics/priority-distribution`

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": [
    {
      "priority": "High",
      "count": 30,
      "percentage": 20.0
    },
    {
      "priority": "Medium",
      "count": 80,
      "percentage": 53.3
    },
    {
      "priority": "Low",
      "count": 40,
      "percentage": 26.7
    }
  ],
  "success": true
}
```

### 字段说明

| 字段名 | 类型 | 说明 |
|--------|------|------|
| priority | string | 优先级 |
| count | int | 任务数量 |
| percentage | decimal | 百分比 |

### 优先级说明

| 值 | 说明 |
|----|------|
| Highest | 最高 |
| High | 高 |
| Medium | 中 |
| Low | 低 |

### 缓存说明

- 此接口有10分钟缓存

---

## 数据模型

### DashboardOverviewDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| totalTasks | int | 总任务数 |
| completedTasks | int | 已完成任务数 |
| pendingTasks | int | 待处理任务数 |
| completionRate | decimal | 完成率 |
| totalWorkLogs | int | 总工作日志数 |
| totalWorkHours | decimal | 总工作时长 |
| todayTasks | int | 今日任务数 |
| todayCompletedTasks | int | 今日已完成任务数 |
| todayWorkHours | int | 今日工作时长 |

### TaskTrendDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| date | string (date) | 日期 |
| created | int | 创建任务数 |
| completed | int | 完成任务数 |
| completionRate | decimal | 完成率 |

### TaskDistributionDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| type | string | 任务类型 |
| count | int | 任务数量 |
| percentage | decimal | 百分比 |

### WorkVsGrowthDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| category | string | 分类 |
| taskCount | int | 任务数量 |

### TaskPriorityDistributionDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| priority | string | 优先级 |
| count | int | 任务数量 |
| percentage | decimal | 百分比 |

---

## 权限说明

- 所有分析接口需要用户登录
- 数据基于当前用户的任务和工作日志
- Pro及以上角色可以查看所有用户的数据

---

## 缓存策略

所有分析接口都有缓存，以提高性能：

| 接口 | 缓存时间 | 缓存键 |
|------|----------|--------|
| 仪表盘概览 | 5分钟 | 用户ID |
| 任务趋势 | 10分钟 | 用户ID + 日期范围 |
| 任务分布 | 10分钟 | 用户ID |
| 工作与成长对比 | 10分钟 | 用户ID |
| 优先级分布 | 10分钟 | 用户ID |

当用户创建或更新任务时，相关缓存会自动失效。
