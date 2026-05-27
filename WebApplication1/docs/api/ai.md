# AI助手 API

## 目录
- [生成计划](#生成计划)
- [生成报告](#生成报告)
- [AI对话](#ai对话)
- [获取计划列表](#获取计划列表)
- [获取计划详情](#获取计划详情)
- [删除计划](#删除计划)
- [获取报告列表](#获取报告列表)
- [获取报告详情](#获取报告详情)
- [删除报告](#删除报告)
- [获取对话会话列表](#获取对话会话列表)
- [获取对话消息](#获取对话消息)
- [删除对话会话](#删除对话会话)

---

## 生成计划

**POST** `/api/ai/generate-plan`

### 请求体

```json
{
  "type": "weekly",
  "description": "制定本周学习计划",
  "targetDate": "2026-05-25",
  "relatedProjectId": "guid",
  "includeCategories": ["学习", "工作", "健康"]
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| type | string | 是 | 计划类型（daily/weekly/monthly） |
| description | string | 否 | 描述（最大1000字符） |
| targetDate | string | 否 | 目标日期 |
| relatedProjectId | guid | 否 | 关联项目ID |
| includeCategories | array | 否 | 包含的分类 |

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "本周学习计划",
    "description": "制定本周学习计划",
    "type": "weekly",
    "status": "completed",
    "generatedContent": "# 本周学习计划\n\n## 周一\n- 学习C#高级特性\n- 完成项目文档\n\n## 周二\n- 学习Entity Framework Core\n- 编写单元测试...",
    "remark": null,
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": "2026-05-28T10:01:00Z"
  },
  "success": true
}
```

### 错误响应

```json
{
  "code": 503,
  "message": "AI 服务未配置",
  "success": false
}
```

### 限流

- 10次/分钟

---

## 生成报告

**POST** `/api/ai/generate-report`

### 请求体

```json
{
  "type": "weekly",
  "startDate": "2026-05-22",
  "endDate": "2026-05-28",
  "relatedProjectId": "guid",
  "includeStatistics": true
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| type | string | 是 | 报告类型（daily/weekly/monthly） |
| startDate | string | 否 | 开始日期 |
| endDate | string | 否 | 结束日期 |
| relatedProjectId | guid | 否 | 关联项目ID |
| includeStatistics | bool | 否 | 是否包含统计（默认: true） |

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "本周工作报告",
    "type": "weekly",
    "content": "# 本周工作报告\n\n## 工作概述\n本周共工作40小时，完成了以下主要任务...\n\n## 统计数据\n- 总工时: 40小时\n- 完成任务: 15个\n- 项目进展: 80%",
    "remark": null,
    "relatedProjectId": "guid",
    "relatedProjectName": "个人成长系统",
    "startDate": "2026-05-22",
    "endDate": "2026-05-28",
    "createdAt": "2026-05-28T18:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

---

## AI对话

**POST** `/api/ai/chat`

### 请求体

```json
{
  "message": "帮我分析一下本周的工作情况",
  "sessionId": "guid",
  "history": [
    {
      "role": "user",
      "content": "你好"
    },
    {
      "role": "assistant",
      "content": "你好！有什么可以帮助你的吗？"
    }
  ]
}
```

### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| message | string | 是 | 消息内容（最大5000字符） |
| sessionId | guid | 否 | 会话ID（为空则创建新会话） |
| history | array | 否 | 历史消息 |

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "sessionId": "guid",
    "messageId": "guid",
    "content": "根据本周的工作记录，我为你分析如下：\n\n1. **工作时长**: 本周共工作40小时\n2. **主要任务**: 完成了API文档编写、单元测试等\n3. **建议**: 可以适当休息，保持工作节奏",
    "success": true,
    "errorMessage": null
  },
  "success": true
}
```

---

## 获取计划列表

**GET** `/api/ai/plans`

### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| type | string | 否 | 计划类型 |
| keyword | string | 否 | 关键词搜索 |
| startDate | string | 否 | 开始日期 |
| endDate | string | 否 | 结束日期 |
| relatedProjectId | guid | 否 | 关联项目ID |
| page | int | 否 | 页码 |
| pageSize | int | 否 | 每页数量 |

---

## 获取计划详情

**GET** `/api/ai/plans/{id}`

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "本周学习计划",
    "description": "制定本周学习计划",
    "type": "weekly",
    "status": "completed",
    "generatedContent": "# 本周学习计划...",
    "remark": null,
    "createdAt": "2026-05-28T10:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

---

## 删除计划

**DELETE** `/api/ai/plans/{id}`

### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

---

## 获取报告列表

**GET** `/api/ai/reports`

### 查询参数

同计划列表

---

## 获取报告详情

**GET** `/api/ai/reports/{id}`

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "本周工作报告",
    "type": "weekly",
    "content": "# 本周工作报告...",
    "remark": null,
    "relatedProjectId": "guid",
    "relatedProjectName": "个人成长系统",
    "startDate": "2026-05-22",
    "endDate": "2026-05-28",
    "createdAt": "2026-05-28T18:00:00Z",
    "updatedAt": null
  },
  "success": true
}
```

---

## 删除报告

**DELETE** `/api/ai/reports/{id}`

### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

---

## 获取对话会话列表

**GET** `/api/ai/chat/sessions`

### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索 |
| page | int | 否 | 页码 |
| pageSize | int | 否 | 每页数量 |

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
        "title": "工作分析对话",
        "lastMessage": "根据本周的工作记录...",
        "messageCount": 10,
        "createdAt": "2026-05-28T10:00:00Z"
      }
    ],
    "total": 5,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

---

## 获取对话消息

**GET** `/api/ai/chat/sessions/{sessionId}/messages`

### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| sessionId | guid | 是 | 会话ID |

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": [
    {
      "id": "guid",
      "sessionId": "guid",
      "role": "user",
      "content": "帮我分析一下本周的工作情况",
      "createdAt": "2026-05-28T10:00:00Z"
    },
    {
      "id": "guid",
      "sessionId": "guid",
      "role": "assistant",
      "content": "根据本周的工作记录，我为你分析如下...",
      "createdAt": "2026-05-28T10:01:00Z"
    }
  ],
  "success": true
}
```

---

## 删除对话会话

**DELETE** `/api/ai/chat/sessions/{id}`

### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

---

## 数据模型

### AiPlanDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 计划ID |
| userId | guid | 用户ID |
| title | string | 标题 |
| description | string | 描述 |
| type | string | 类型 |
| status | string | 状态 |
| generatedContent | string | 生成的内容 |
| remark | string | 备注 |
| createdAt | string (datetime) | 创建时间 |
| updatedAt | string (datetime) | 更新时间 |

### AiReportDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 报告ID |
| userId | guid | 用户ID |
| title | string | 标题 |
| type | string | 类型 |
| content | string | 内容 |
| remark | string | 备注 |
| relatedProjectId | guid | 关联项目ID |
| relatedProjectName | string | 关联项目名称 |
| startDate | string (date) | 开始日期 |
| endDate | string (date) | 结束日期 |
| createdAt | string (datetime) | 创建时间 |
| updatedAt | string (datetime) | 更新时间 |

### ChatResponse

| 字段名 | 类型 | 说明 |
|--------|------|------|
| sessionId | guid | 会话ID |
| messageId | guid | 消息ID |
| content | string | 回复内容 |
| success | bool | 是否成功 |
| errorMessage | string | 错误信息 |

### 计划类型

| 值 | 说明 |
|----|------|
| daily | 每日计划 |
| weekly | 每周计划 |
| monthly | 每月计划 |

### 报告类型

| 值 | 说明 |
|----|------|
| daily | 每日报告 |
| weekly | 每周报告 |
| monthly | 每月报告 |
