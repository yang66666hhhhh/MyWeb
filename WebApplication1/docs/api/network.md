# 人脉网络 API

## 目录
- [联系人管理](#联系人管理)
  - [获取联系人列表](#获取联系人列表)
  - [获取联系人详情](#获取联系人详情)
  - [创建联系人](#创建联系人)
  - [更新联系人](#更新联系人)
  - [删除联系人](#删除联系人)
- [互动记录管理](#互动记录管理)
  - [获取互动记录列表](#获取互动记录列表)
  - [获取互动记录详情](#获取互动记录详情)
  - [创建互动记录](#创建互动记录)
  - [更新互动记录](#更新互动记录)
  - [删除互动记录](#删除互动记录)
- [标签管理](#标签管理)
  - [获取联系人标签统计](#获取联系人标签统计)

---

## 联系人管理

### 获取联系人列表

**GET** `/api/network/contacts`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索（姓名、公司、电话、邮箱） |
| tag | string | 否 | 标签筛选 |
| startDate | string | 否 | 开始日期 |
| endDate | string | 否 | 结束日期 |
| page | int | 否 | 页码（默认: 1） |
| pageSize | int | 否 | 每页数量（默认: 10, 最大: 100） |

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
        "name": "张经理",
        "company": "客户A公司",
        "position": "项目经理",
        "phone": "13800138001",
        "email": "zhang@clienta.com",
        "weChat": "zhang_mgr",
        "tags": "客户,重要",
        "remark": "主要对接人",
        "interactionCount": 5,
        "lastInteractionAt": "2026-05-26 10:00:00",
        "createdAt": "2026-04-28 10:00:00"
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

### 获取联系人详情

**GET** `/api/network/contacts/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 联系人ID |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "张经理",
    "company": "客户A公司",
    "position": "项目经理",
    "phone": "13800138001",
    "email": "zhang@clienta.com",
    "weChat": "zhang_mgr",
    "tags": "客户,重要",
    "remark": "主要对接人",
    "interactionCount": 5,
    "lastInteractionAt": "2026-05-26 10:00:00",
    "createdAt": "2026-04-28 10:00:00"
  },
  "success": true
}
```

#### 错误响应

```json
{
  "code": 404,
  "message": "联系人不存在",
  "success": false
}
```

---

### 创建联系人

**POST** `/api/network/contacts`

#### 请求体

```json
{
  "name": "张经理",
  "company": "客户A公司",
  "position": "项目经理",
  "phone": "13800138001",
  "email": "zhang@clienta.com",
  "weChat": "zhang_mgr",
  "tags": "客户,重要",
  "remark": "主要对接人"
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| name | string | 是 | 联系人姓名（最大100字符） |
| company | string | 否 | 公司名称（最大200字符） |
| position | string | 否 | 职位（最大100字符） |
| phone | string | 否 | 电话（最大20字符） |
| email | string | 否 | 邮箱（最大100字符） |
| weChat | string | 否 | 微信号（最大50字符） |
| tags | string | 否 | 标签，逗号分隔（最大500字符） |
| remark | string | 否 | 备注（最大1000字符） |

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "张经理",
    "company": "客户A公司",
    "position": "项目经理",
    "phone": "13800138001",
    "email": "zhang@clienta.com",
    "weChat": "zhang_mgr",
    "tags": "客户,重要",
    "remark": "主要对接人",
    "interactionCount": 0,
    "lastInteractionAt": null,
    "createdAt": "2026-05-28 10:00:00"
  },
  "success": true
}
```

---

### 更新联系人

**PUT** `/api/network/contacts/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 联系人ID |

#### 请求体

```json
{
  "name": "张经理（已更新）",
  "company": "客户A公司",
  "position": "高级项目经理",
  "phone": "13800138001",
  "email": "zhang@clienta.com",
  "weChat": "zhang_mgr",
  "tags": "客户,重要,VIP",
  "remark": "已升级为VIP客户"
}
```

#### 字段说明

所有字段均为可选，只更新提供的字段。

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| name | string | 否 | 联系人姓名（最大100字符） |
| company | string | 否 | 公司名称（最大200字符） |
| position | string | 否 | 职位（最大100字符） |
| phone | string | 否 | 电话（最大20字符） |
| email | string | 否 | 邮箱（最大100字符） |
| weChat | string | 否 | 微信号（最大50字符） |
| tags | string | 否 | 标签，逗号分隔（最大500字符） |
| remark | string | 否 | 备注（最大1000字符） |

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "name": "张经理（已更新）",
    "company": "客户A公司",
    "position": "高级项目经理",
    "phone": "13800138001",
    "email": "zhang@clienta.com",
    "weChat": "zhang_mgr",
    "tags": "客户,重要,VIP",
    "remark": "已升级为VIP客户",
    "interactionCount": 5,
    "lastInteractionAt": "2026-05-26 10:00:00",
    "createdAt": "2026-04-28 10:00:00"
  },
  "success": true
}
```

#### 错误响应

```json
{
  "code": 403,
  "message": "无权限修改此联系人",
  "success": false
}
```

---

### 删除联系人

**DELETE** `/api/network/contacts/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 联系人ID |

#### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

#### 错误响应

```json
{
  "code": 403,
  "message": "无权限删除此联系人",
  "success": false
}
```

#### 说明

删除联系人时，会同时删除该联系人的所有互动记录。

---

## 互动记录管理

### 获取互动记录列表

**GET** `/api/network/contacts/{contactId}/interactions`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| contactId | guid | 是 | 联系人ID |

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索 |
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
        "contactId": "guid",
        "contactName": "张经理",
        "type": "会议",
        "content": "讨论项目进度和下一步计划",
        "interactionDate": "2026-05-26",
        "nextFollowUpDate": "2026-06-01",
        "remark": "会议顺利，达成共识",
        "createdAt": "2026-05-26 10:00:00"
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

### 获取互动记录详情

**GET** `/api/network/interactions/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 互动记录ID |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "contactId": "guid",
    "contactName": "张经理",
    "type": "会议",
    "content": "讨论项目进度和下一步计划",
    "interactionDate": "2026-05-26",
    "nextFollowUpDate": "2026-06-01",
    "remark": "会议顺利，达成共识",
    "createdAt": "2026-05-26 10:00:00"
  },
  "success": true
}
```

---

### 创建互动记录

**POST** `/api/network/interactions`

#### 请求体

```json
{
  "contactId": "guid",
  "type": "会议",
  "content": "讨论项目进度和下一步计划",
  "interactionDate": "2026-05-28",
  "nextFollowUpDate": "2026-06-01",
  "remark": "会议顺利，达成共识"
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| contactId | guid | 是 | 联系人ID |
| type | string | 是 | 互动类型（最大50字符） |
| content | string | 是 | 互动内容（最大4000字符） |
| interactionDate | string | 是 | 互动日期（最大20字符） |
| nextFollowUpDate | string | 否 | 下次跟进日期（最大20字符） |
| remark | string | 否 | 备注（最大1000字符） |

#### 常用互动类型

- 会议
- 电话
- 邮件
- 微信
- 拜访
- 其他

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "contactId": "guid",
    "contactName": "张经理",
    "type": "会议",
    "content": "讨论项目进度和下一步计划",
    "interactionDate": "2026-05-28",
    "nextFollowUpDate": "2026-06-01",
    "remark": "会议顺利，达成共识",
    "createdAt": "2026-05-28 10:00:00"
  },
  "success": true
}
```

#### 说明

创建互动记录时，会自动更新联系人的：
- `interactionCount`（互动次数）+1
- `lastInteractionAt`（最后互动时间）更新为当前时间

---

### 更新互动记录

**PUT** `/api/network/interactions/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 互动记录ID |

#### 请求体

```json
{
  "type": "电话",
  "content": "确认项目交付时间",
  "interactionDate": "2026-05-28",
  "nextFollowUpDate": "2026-06-05",
  "remark": "已确认交付时间"
}
```

#### 字段说明

所有字段均为可选，只更新提供的字段。

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| type | string | 否 | 互动类型（最大50字符） |
| content | string | 否 | 互动内容（最大4000字符） |
| interactionDate | string | 否 | 互动日期（最大20字符） |
| nextFollowUpDate | string | 否 | 下次跟进日期（最大20字符） |
| remark | string | 否 | 备注（最大1000字符） |

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "contactId": "guid",
    "contactName": "张经理",
    "type": "电话",
    "content": "确认项目交付时间",
    "interactionDate": "2026-05-28",
    "nextFollowUpDate": "2026-06-05",
    "remark": "已确认交付时间",
    "createdAt": "2026-05-28 10:00:00"
  },
  "success": true
}
```

---

### 删除互动记录

**DELETE** `/api/network/interactions/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 互动记录ID |

#### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

#### 说明

删除互动记录时，会自动更新联系人的 `interactionCount`（互动次数）-1。

---

## 标签管理

### 获取联系人标签统计

**GET** `/api/network/tags`

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": [
    {
      "id": "客户",
      "name": "客户",
      "color": "#1890ff",
      "usageCount": 15
    },
    {
      "id": "重要",
      "name": "重要",
      "color": "#1890ff",
      "usageCount": 10
    },
    {
      "id": "供应商",
      "name": "供应商",
      "color": "#1890ff",
      "usageCount": 5
    }
  ],
  "success": true
}
```

#### 说明

- 标签统计基于联系人的 `tags` 字段
- 返回按使用次数降序排列
- `usageCount` 表示使用该标签的联系人数量

---

## 数据模型

### ContactDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 联系人ID |
| userId | guid | 用户ID |
| name | string | 联系人姓名 |
| company | string | 公司名称 |
| position | string | 职位 |
| phone | string | 电话 |
| email | string | 邮箱 |
| weChat | string | 微信号 |
| tags | string | 标签（逗号分隔） |
| remark | string | 备注 |
| interactionCount | int | 互动次数 |
| lastInteractionAt | string | 最后互动时间 |
| createdAt | string | 创建时间 |

### InteractionDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 互动记录ID |
| userId | guid | 用户ID |
| contactId | guid | 联系人ID |
| contactName | string | 联系人姓名 |
| type | string | 互动类型 |
| content | string | 互动内容 |
| interactionDate | string | 互动日期 |
| nextFollowUpDate | string | 下次跟进日期 |
| remark | string | 备注 |
| createdAt | string | 创建时间 |

### TagDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | string | 标签名称 |
| name | string | 标签名称 |
| color | string | 标签颜色 |
| usageCount | int | 使用次数 |

---

## 权限说明

- 普通用户只能查看和管理自己创建的联系人和互动记录
- Pro及以上角色可以查看所有用户的联系人和互动记录
- 删除联系人会同时删除关联的互动记录
