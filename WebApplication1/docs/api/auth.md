# 认证 API

## 用户登录

**POST** `/api/auth/login`

### 请求体
```json
{
  "username": "string",
  "password": "string"
}
```

### 响应
```json
{
  "code": 200,
  "message": "success",
  "data": {
    "accessToken": "string",
    "refreshToken": "string",
    "expiresAt": "2026-05-28T12:00:00Z",
    "user": {
      "id": "guid",
      "username": "string",
      "realName": "string",
      "email": "string",
      "roles": ["string"]
    }
  },
  "success": true
}
```

### 错误响应
```json
{
  "code": 401,
  "message": "用户名或密码错误",
  "success": false
}
```

---

## 用户注册

**POST** `/api/auth/register`

### 请求体
```json
{
  "username": "string",
  "password": "string",
  "realName": "string",
  "email": "string",
  "phone": "string"
}
```

### 响应
```json
{
  "code": 200,
  "message": "注册成功",
  "data": {
    "id": "guid",
    "username": "string",
    "realName": "string"
  },
  "success": true
}
```

---

## 刷新Token

**POST** `/api/auth/refresh-token`

### 请求体
```json
{
  "refreshToken": "string"
}
```

### 响应
```json
{
  "code": 200,
  "message": "success",
  "data": {
    "accessToken": "string",
    "refreshToken": "string",
    "expiresAt": "2026-05-28T12:00:00Z"
  },
  "success": true
}
```

---

## 获取当前用户信息

**GET** `/api/auth/me`

### 响应
```json
{
  "code": 200,
  "message": "success",
  "data": {
    "id": "guid",
    "username": "string",
    "realName": "string",
    "email": "string",
    "phone": "string",
    "avatar": "string",
    "roles": ["string"],
    "createdAt": "2026-01-01T00:00:00Z"
  },
  "success": true
}
```

---

## 修改密码

**PUT** `/api/auth/change-password`

### 请求体
```json
{
  "oldPassword": "string",
  "newPassword": "string"
}
```

### 响应
```json
{
  "code": 200,
  "message": "密码修改成功",
  "success": true
}
```

---

## 限流说明

| 端点 | 限制 | 时间窗口 |
|------|------|----------|
| POST /api/auth/login | 5次 | 1分钟 |
| POST /api/auth/register | 3次 | 5分钟 |
| POST /api/auth/refresh-token | 10次 | 1分钟 |
| POST /api/auth/forgot-password | 3次 | 15分钟 |

超出限制返回 429 状态码：
```json
{
  "code": 429,
  "message": "请求过于频繁，请稍后再试",
  "retryAfter": 60,
  "success": false
}
```
