# 管理后台 API

## 目录
- [用户管理](#用户管理)
  - [获取用户列表](#获取用户列表)
  - [获取用户详情](#获取用户详情)
  - [创建用户](#创建用户)
  - [更新用户](#更新用户)
  - [删除用户](#删除用户)
- [角色管理](#角色管理)
  - [获取角色列表](#获取角色列表)
  - [创建角色](#创建角色)
  - [更新角色](#更新角色)
  - [删除角色](#删除角色)
- [菜单管理](#菜单管理)
  - [获取菜单列表](#获取菜单列表)
  - [创建菜单](#创建菜单)
  - [更新菜单](#更新菜单)
  - [删除菜单](#删除菜单)
- [功能权限管理](#功能权限管理)
  - [获取功能列表](#获取功能列表)
  - [创建功能](#创建功能)
  - [更新功能](#更新功能)
  - [删除功能](#删除功能)

---

## 用户管理

### 获取用户列表

**GET** `/api/admin/users`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索（用户名、姓名、邮箱） |
| status | int | 否 | 状态（0: 禁用, 1: 启用） |
| roleId | guid | 否 | 角色ID |
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
        "username": "admin",
        "realName": "管理员",
        "email": "admin@example.com",
        "phone": "13800138000",
        "avatar": "https://example.com/avatar.jpg",
        "status": "Active",
        "roles": ["admin", "user"],
        "createdAt": "2026-01-01T00:00:00Z",
        "lastLoginAt": "2026-05-28T10:00:00Z"
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

### 获取用户详情

**GET** `/api/admin/users/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 用户ID |

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "username": "admin",
    "realName": "管理员",
    "email": "admin@example.com",
    "phone": "13800138000",
    "avatar": "https://example.com/avatar.jpg",
    "status": "Active",
    "roles": ["admin", "user"],
    "createdAt": "2026-01-01T00:00:00Z",
    "lastLoginAt": "2026-05-28T10:00:00Z"
  },
  "success": true
}
```

---

### 创建用户

**POST** `/api/admin/users`

#### 请求体

```json
{
  "username": "newuser",
  "password": "password123",
  "realName": "新用户",
  "email": "newuser@example.com",
  "phone": "13900139000",
  "roles": ["user"]
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| username | string | 是 | 用户名（最大50字符） |
| password | string | 是 | 密码（最大100字符） |
| realName | string | 是 | 真实姓名（最大100字符） |
| email | string | 否 | 邮箱（最大100字符） |
| phone | string | 否 | 电话（最大20字符） |
| roles | array | 否 | 角色列表 |

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "username": "newuser",
    "realName": "新用户",
    "email": "newuser@example.com",
    "phone": "13900139000",
    "avatar": null,
    "status": "Active",
    "roles": ["user"],
    "createdAt": "2026-05-28T10:00:00Z",
    "lastLoginAt": null
  },
  "success": true
}
```

---

### 更新用户

**PUT** `/api/admin/users/{id}`

#### 请求体

```json
{
  "realName": "更新后的姓名",
  "email": "updated@example.com",
  "phone": "13700137000",
  "status": 1,
  "roles": ["admin", "user"]
}
```

#### 字段说明

所有字段均为可选，只更新提供的字段。

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| realName | string | 否 | 真实姓名 |
| email | string | 否 | 邮箱 |
| phone | string | 否 | 电话 |
| status | int | 否 | 状态 |
| roles | array | 否 | 角色列表 |

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "username": "newuser",
    "realName": "更新后的姓名",
    "email": "updated@example.com",
    "phone": "13700137000",
    "avatar": null,
    "status": "Active",
    "roles": ["admin", "user"],
    "createdAt": "2026-05-28T10:00:00Z",
    "lastLoginAt": null
  },
  "success": true
}
```

---

### 删除用户

**DELETE** `/api/admin/users/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 用户ID |

#### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

---

## 角色管理

### 获取角色列表

**GET** `/api/admin/roles`

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": [
    {
      "id": "guid",
      "name": "管理员",
      "code": "admin",
      "description": "系统管理员",
      "permissions": ["user:read", "user:write", "role:read"],
      "isSystem": true,
      "createdAt": "2026-01-01T00:00:00Z"
    }
  ],
  "success": true
}
```

---

### 创建角色

**POST** `/api/admin/roles`

#### 请求体

```json
{
  "name": "编辑",
  "code": "editor",
  "description": "内容编辑",
  "permissions": ["article:read", "article:write"]
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| name | string | 是 | 角色名称（最大50字符） |
| code | string | 是 | 角色编码（最大50字符） |
| description | string | 否 | 描述（最大500字符） |
| permissions | array | 否 | 权限列表 |

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "name": "编辑",
    "code": "editor",
    "description": "内容编辑",
    "permissions": ["article:read", "article:write"],
    "isSystem": false,
    "createdAt": "2026-05-28T10:00:00Z"
  },
  "success": true
}
```

---

### 更新角色

**PUT** `/api/admin/roles/{id}`

#### 请求体

```json
{
  "name": "高级编辑",
  "description": "高级内容编辑",
  "permissions": ["article:read", "article:write", "article:delete"]
}
```

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "name": "高级编辑",
    "code": "editor",
    "description": "高级内容编辑",
    "permissions": ["article:read", "article:write", "article:delete"],
    "isSystem": false,
    "createdAt": "2026-05-28T10:00:00Z"
  },
  "success": true
}
```

---

### 删除角色

**DELETE** `/api/admin/roles/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 角色ID |

#### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

#### 说明

系统内置角色（`isSystem: true`）不能删除。

---

## 菜单管理

### 获取菜单列表

**GET** `/api/admin/menus`

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": [
    {
      "id": "guid",
      "name": "仪表盘",
      "path": "/dashboard",
      "icon": "dashboard",
      "component": "Dashboard",
      "parentId": null,
      "sort": 1,
      "isVisible": true,
      "isActive": true,
      "children": []
    }
  ],
  "success": true
}
```

---

### 创建菜单

**POST** `/api/admin/menus`

#### 请求体

```json
{
  "name": "用户管理",
  "path": "/admin/users",
  "icon": "user",
  "component": "UserManagement",
  "parentId": "guid",
  "sort": 1,
  "isVisible": true,
  "isActive": true
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| name | string | 是 | 菜单名称（最大50字符） |
| path | string | 是 | 路由路径（最大200字符） |
| icon | string | 否 | 图标（最大100字符） |
| component | string | 否 | 组件名称（最大200字符） |
| parentId | guid | 否 | 父菜单ID |
| sort | int | 否 | 排序（默认: 0） |
| isVisible | bool | 否 | 是否可见（默认: true） |
| isActive | bool | 否 | 是否启用（默认: true） |

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "name": "用户管理",
    "path": "/admin/users",
    "icon": "user",
    "component": "UserManagement",
    "parentId": "guid",
    "sort": 1,
    "isVisible": true,
    "isActive": true,
    "children": []
  },
  "success": true
}
```

---

### 更新菜单

**PUT** `/api/admin/menus/{id}`

#### 请求体

```json
{
  "name": "用户管理（更新）",
  "path": "/admin/users",
  "icon": "user",
  "sort": 2,
  "isVisible": true,
  "isActive": true
}
```

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "name": "用户管理（更新）",
    "path": "/admin/users",
    "icon": "user",
    "component": "UserManagement",
    "parentId": "guid",
    "sort": 2,
    "isVisible": true,
    "isActive": true,
    "children": []
  },
  "success": true
}
```

---

### 删除菜单

**DELETE** `/api/admin/menus/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 菜单ID |

#### 响应

```json
{
  "code": 200,
  "message": "删除成功",
  "success": true
}
```

#### 说明

删除菜单时会同时删除子菜单。

---

## 功能权限管理

### 获取功能列表

**GET** `/api/admin/features`

#### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": [
    {
      "id": "guid",
      "code": "WORK_LOG",
      "name": "工作日志",
      "category": "Work",
      "description": "记录和管理工作日志",
      "isEnabled": true,
      "createdAt": "2026-01-01T00:00:00Z"
    }
  ],
  "success": true
}
```

---

### 创建功能

**POST** `/api/admin/features`

#### 请求体

```json
{
  "code": "NEW_FEATURE",
  "name": "新功能",
  "category": "Work",
  "description": "新功能描述",
  "isEnabled": true
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| code | string | 是 | 功能编码（最大100字符） |
| name | string | 是 | 功能名称（最大100字符） |
| category | string | 否 | 分类（最大50字符） |
| description | string | 否 | 描述（最大500字符） |
| isEnabled | bool | 否 | 是否启用（默认: true） |

#### 响应

```json
{
  "code": 200,
  "message": "创建成功",
  "data": {
    "id": "guid",
    "code": "NEW_FEATURE",
    "name": "新功能",
    "category": "Work",
    "description": "新功能描述",
    "isEnabled": true,
    "createdAt": "2026-05-28T10:00:00Z"
  },
  "success": true
}
```

---

### 更新功能

**PUT** `/api/admin/features/{id}`

#### 请求体

```json
{
  "name": "更新后的功能",
  "description": "更新后的描述",
  "isEnabled": false
}
```

#### 响应

```json
{
  "code": 200,
  "message": "更新成功",
  "data": {
    "id": "guid",
    "code": "NEW_FEATURE",
    "name": "更新后的功能",
    "category": "Work",
    "description": "更新后的描述",
    "isEnabled": false,
    "createdAt": "2026-05-28T10:00:00Z"
  },
  "success": true
}
```

---

### 删除功能

**DELETE** `/api/admin/features/{id}`

#### 路径参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| id | guid | 是 | 功能ID |

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

### UserDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 用户ID |
| username | string | 用户名 |
| realName | string | 真实姓名 |
| email | string | 邮箱 |
| phone | string | 电话 |
| avatar | string | 头像URL |
| status | string | 状态 |
| roles | array | 角色列表 |
| createdAt | string (datetime) | 创建时间 |
| lastLoginAt | string (datetime) | 最后登录时间 |

### RoleDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 角色ID |
| name | string | 角色名称 |
| code | string | 角色编码 |
| description | string | 描述 |
| permissions | array | 权限列表 |
| isSystem | bool | 是否系统角色 |
| createdAt | string (datetime) | 创建时间 |

### MenuDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 菜单ID |
| name | string | 菜单名称 |
| path | string | 路由路径 |
| icon | string | 图标 |
| component | string | 组件名称 |
| parentId | guid | 父菜单ID |
| sort | int | 排序 |
| isVisible | bool | 是否可见 |
| isActive | bool | 是否启用 |
| children | array | 子菜单 |

### FeatureDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 功能ID |
| code | string | 功能编码 |
| name | string | 功能名称 |
| category | string | 分类 |
| description | string | 描述 |
| isEnabled | bool | 是否启用 |
| createdAt | string (datetime) | 创建时间 |

---

## 权限说明

- 所有管理接口需要管理员权限
- 系统内置角色（`isSystem: true`）不能删除
- 删除菜单会同时删除子菜单
