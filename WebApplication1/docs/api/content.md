# 内容管理 API

## 目录
- [文章管理](#文章管理)
  - [获取文章列表](#获取文章列表)
  - [获取文章详情](#获取文章详情)
  - [创建文章](#创建文章)
  - [更新文章](#更新文章)
  - [删除文章](#删除文章)
- [媒体文件管理](#媒体文件管理)
  - [获取媒体文件列表](#获取媒体文件列表)
  - [获取媒体文件详情](#获取媒体文件详情)
  - [创建媒体文件](#创建媒体文件)
  - [更新媒体文件](#更新媒体文件)
  - [删除媒体文件](#删除媒体文件)
- [发布日历管理](#发布日历管理)
  - [获取发布日历列表](#获取发布日历列表)
  - [获取发布日历详情](#获取发布日历详情)
  - [创建发布日历](#创建发布日历)
  - [更新发布日历](#更新发布日历)
  - [删除发布日历](#删除发布日历)

---

## 文章管理

### 获取文章列表

**GET** `/api/content/articles`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| status | string | 否 | 文章状态（draft/published/archived） |
| category | string | 否 | 文章分类 |
| keyword | string | 否 | 关键词搜索（标题、内容） |
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
        "title": "如何提高工作效率",
        "content": "本文介绍了10种提高工作效率的方法...",
        "status": "published",
        "tags": "效率,工作,方法",
        "category": "职场",
        "publishedAt": "2026-05-20 10:00:00",
        "remark": null,
        "createdAt": "2026-05-18 10:00:00"
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

### 获取文章详情

**GET** `/api/content/articles/{id}`

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
    "title": "如何提高工作效率",
    "content": "本文介绍了10种提高工作效率的方法...",
    "status": "published",
    "tags": "效率,工作,方法",
    "category": "职场",
    "publishedAt": "2026-05-20 10:00:00",
    "remark": null,
    "createdAt": "2026-05-18 10:00:00"
  },
  "success": true
}
```

---

### 创建文章

**POST** `/api/content/articles`

#### 请求体

```json
{
  "title": "如何提高工作效率",
  "content": "本文介绍了10种提高工作效率的方法...",
  "status": "draft",
  "tags": "效率,工作,方法",
  "category": "职场",
  "remark": null
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| title | string | 是 | 文章标题（最大200字符） |
| content | string | 否 | 文章内容（最大50000字符） |
| status | string | 否 | 状态（默认: draft, 可选: draft/published/archived） |
| tags | string | 否 | 标签，逗号分隔（最大500字符） |
| category | string | 否 | 分类（最大100字符） |
| remark | string | 否 | 备注（最大1000字符） |

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "如何提高工作效率",
    "content": "本文介绍了10种提高工作效率的方法...",
    "status": "draft",
    "tags": "效率,工作,方法",
    "category": "职场",
    "publishedAt": null,
    "remark": null,
    "createdAt": "2026-05-28 10:00:00"
  },
  "success": true
}
```

---

### 更新文章

**PUT** `/api/content/articles/{id}`

#### 请求体

```json
{
  "title": "如何提高工作效率（更新版）",
  "content": "更新后的内容...",
  "status": "published",
  "tags": "效率,工作,方法,更新",
  "category": "职场",
  "remark": "已更新"
}
```

#### 字段说明

所有字段均为可选，只更新提供的字段。

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| title | string | 否 | 文章标题（最大200字符） |
| content | string | 否 | 文章内容（最大50000字符） |
| status | string | 否 | 状态（draft/published/archived） |
| tags | string | 否 | 标签，逗号分隔（最大500字符） |
| category | string | 否 | 分类（最大100字符） |
| remark | string | 否 | 备注（最大1000字符） |

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "title": "如何提高工作效率（更新版）",
    "content": "更新后的内容...",
    "status": "published",
    "tags": "效率,工作,方法,更新",
    "category": "职场",
    "publishedAt": "2026-05-28 10:00:00",
    "remark": "已更新",
    "createdAt": "2026-05-18 10:00:00"
  },
  "success": true
}
```

#### 特殊逻辑

当文章状态从非 `published` 变为 `published` 时，会自动设置 `publishedAt` 为当前时间。

---

### 删除文章

**DELETE** `/api/content/articles/{id}`

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

## 媒体文件管理

### 获取媒体文件列表

**GET** `/api/content/media`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索（文件名、标签） |
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
        "fileName": "logo.png",
        "fileUrl": "https://example.com/uploads/logo.png",
        "fileType": "image/png",
        "fileSize": 102400,
        "tags": "logo,品牌",
        "remark": "公司Logo",
        "createdAt": "2026-05-20 10:00:00"
      }
    ],
    "total": 100,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

---

### 获取媒体文件详情

**GET** `/api/content/media/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 媒体文件ID |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "fileName": "logo.png",
    "fileUrl": "https://example.com/uploads/logo.png",
    "fileType": "image/png",
    "fileSize": 102400,
    "tags": "logo,品牌",
    "remark": "公司Logo",
    "createdAt": "2026-05-20 10:00:00"
  },
  "success": true
}
```

---

### 创建媒体文件

**POST** `/api/content/media`

#### 请求体

```json
{
  "fileName": "logo.png",
  "fileUrl": "https://example.com/uploads/logo.png",
  "fileType": "image/png",
  "fileSize": 102400,
  "tags": "logo,品牌",
  "remark": "公司Logo"
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| fileName | string | 是 | 文件名（最大200字符） |
| fileUrl | string | 是 | 文件URL（最大500字符） |
| fileType | string | 是 | 文件类型（最大50字符） |
| fileSize | long | 是 | 文件大小（字节，最大100MB） |
| tags | string | 否 | 标签，逗号分隔（最大500字符） |
| remark | string | 否 | 备注（最大500字符） |

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "fileName": "logo.png",
    "fileUrl": "https://example.com/uploads/logo.png",
    "fileType": "image/png",
    "fileSize": 102400,
    "tags": "logo,品牌",
    "remark": "公司Logo",
    "createdAt": "2026-05-28 10:00:00"
  },
  "success": true
}
```

---

### 更新媒体文件

**PUT** `/api/content/media/{id}`

#### 请求体

```json
{
  "fileName": "new-logo.png",
  "tags": "logo,品牌,新",
  "remark": "更新后的Logo"
}
```

#### 字段说明

所有字段均为可选，只更新提供的字段。

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| fileName | string | 否 | 文件名（最大200字符） |
| fileUrl | string | 否 | 文件URL（最大500字符） |
| fileType | string | 否 | 文件类型（最大50字符） |
| fileSize | long | 否 | 文件大小（字节） |
| tags | string | 否 | 标签，逗号分隔（最大500字符） |
| remark | string | 否 | 备注（最大500字符） |

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "fileName": "new-logo.png",
    "fileUrl": "https://example.com/uploads/logo.png",
    "fileType": "image/png",
    "fileSize": 102400,
    "tags": "logo,品牌,新",
    "remark": "更新后的Logo",
    "createdAt": "2026-05-20 10:00:00"
  },
  "success": true
}
```

---

### 删除媒体文件

**DELETE** `/api/content/media/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 媒体文件ID |

#### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

---

## 发布日历管理

### 获取发布日历列表

**GET** `/api/content/calendar`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| status | string | 否 | 状态（pending/published/cancelled） |
| keyword | string | 否 | 关键词搜索（标题、平台） |
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
        "plannedDate": "2026-06-01",
        "platform": "微信公众号",
        "title": "6月工作计划发布",
        "status": "pending",
        "remark": "需要提前准备素材",
        "createdAt": "2026-05-28 10:00:00"
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

### 获取发布日历详情

**GET** `/api/content/calendar/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 发布日历ID |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "userId": "guid",
    "plannedDate": "2026-06-01",
    "platform": "微信公众号",
    "title": "6月工作计划发布",
    "status": "pending",
    "remark": "需要提前准备素材",
    "createdAt": "2026-05-28 10:00:00"
  },
  "success": true
}
```

---

### 创建发布日历

**POST** `/api/content/calendar`

#### 请求体

```json
{
  "plannedDate": "2026-06-01",
  "platform": "微信公众号",
  "title": "6月工作计划发布",
  "status": "pending",
  "remark": "需要提前准备素材"
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| plannedDate | string | 是 | 计划日期（最大20字符） |
| platform | string | 是 | 发布平台（最大50字符） |
| title | string | 是 | 标题（最大200字符） |
| status | string | 否 | 状态（默认: pending, 可选: pending/published/cancelled） |
| remark | string | 否 | 备注（最大500字符） |

#### 常用平台

- 微信公众号
- 微博
- 知乎
- 抖音
- B站
- 小红书
- 其他

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "plannedDate": "2026-06-01",
    "platform": "微信公众号",
    "title": "6月工作计划发布",
    "status": "pending",
    "remark": "需要提前准备素材",
    "createdAt": "2026-05-28 10:00:00"
  },
  "success": true
}
```

---

### 更新发布日历

**PUT** `/api/content/calendar/{id}`

#### 请求体

```json
{
  "plannedDate": "2026-06-05",
  "platform": "微信公众号",
  "title": "6月工作计划发布（延期）",
  "status": "pending",
  "remark": "延期到6月5日"
}
```

#### 字段说明

所有字段均为可选，只更新提供的字段。

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| plannedDate | string | 否 | 计划日期（最大20字符） |
| platform | string | 否 | 发布平台（最大50字符） |
| title | string | 否 | 标题（最大200字符） |
| status | string | 否 | 状态（pending/published/cancelled） |
| remark | string | 否 | 备注（最大500字符） |

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "userId": "guid",
    "plannedDate": "2026-06-05",
    "platform": "微信公众号",
    "title": "6月工作计划发布（延期）",
    "status": "pending",
    "remark": "延期到6月5日",
    "createdAt": "2026-05-28 10:00:00"
  },
  "success": true
}
```

---

### 删除发布日历

**DELETE** `/api/content/calendar/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 发布日历ID |

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

### ArticleDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 文章ID |
| userId | guid | 用户ID |
| title | string | 标题 |
| content | string | 内容 |
| status | string | 状态 |
| tags | string | 标签（逗号分隔） |
| category | string | 分类 |
| publishedAt | string | 发布时间 |
| remark | string | 备注 |
| createdAt | string | 创建时间 |

### MediaItemDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 媒体文件ID |
| userId | guid | 用户ID |
| fileName | string | 文件名 |
| fileUrl | string | 文件URL |
| fileType | string | 文件类型 |
| fileSize | long | 文件大小 |
| tags | string | 标签（逗号分隔） |
| remark | string | 备注 |
| createdAt | string | 创建时间 |

### PublishingCalendarDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 发布日历ID |
| userId | guid | 用户ID |
| plannedDate | string | 计划日期 |
| platform | string | 发布平台 |
| title | string | 标题 |
| status | string | 状态 |
| remark | string | 备注 |
| createdAt | string | 创建时间 |

### 状态枚举

#### 文章状态

| 值 | 说明 |
|----|------|
| draft | 草稿 |
| published | 已发布 |
| archived | 已归档 |

#### 发布日历状态

| 值 | 说明 |
|----|------|
| pending | 待发布 |
| published | 已发布 |
| cancelled | 已取消 |

---

## 权限说明

- 普通用户只能查看和管理自己创建的内容
- Pro及以上角色可以查看所有用户的内容
