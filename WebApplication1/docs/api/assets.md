# 资产管理 API

## 目录
- [资产摘要](#资产摘要)
- [收入管理](#收入管理)
- [支出管理](#支出管理)
- [预算管理](#预算管理)
- [投资管理](#投资管理)

---

## 资产摘要

**GET** `/api/assets/summary`

### 响应

```json
{
  "code": 200,
  "message": "success",
  "data": {
    "totalIncome": 50000,
    "totalExpense": 30000,
    "totalInvestment": 100000,
    "netAsset": 120000,
    "incomeCount": 10,
    "expenseCount": 50,
    "investmentCount": 5
  },
  "success": true
}
```

---

## 收入管理

### 获取收入列表

**GET** `/api/assets/incomes`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索 |
| category | string | 否 | 分类 |
| startDate | string | 否 | 开始日期 |
| endDate | string | 否 | 结束日期 |
| page | int | 否 | 页码 |
| pageSize | int | 否 | 每页数量 |

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
        "incomeDate": "2026-05-28",
        "category": "工资",
        "title": "月工资",
        "amount": 10000,
        "description": "5月份工资",
        "remark": null,
        "createdAt": "2026-05-28T10:00:00Z"
      }
    ],
    "total": 10,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

### 获取收入详情

**GET** `/api/assets/incomes/{id}`

### 创建收入

**POST** `/api/assets/incomes`

#### 请求体

```json
{
  "incomeDate": "2026-05-28",
  "category": "工资",
  "title": "月工资",
  "amount": 10000,
  "description": "5月份工资",
  "remark": null
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| incomeDate | string | 是 | 收入日期（最大20字符） |
| category | string | 是 | 分类（最大50字符） |
| title | string | 是 | 标题（最大200字符） |
| amount | decimal | 是 | 金额（大于0） |
| description | string | 否 | 描述（最大1000字符） |
| remark | string | 否 | 备注（最大1000字符） |

### 更新收入

**PUT** `/api/assets/incomes/{id}`

#### 请求体

```json
{
  "incomeDate": "2026-05-28",
  "category": "工资",
  "title": "月工资（已调整）",
  "amount": 12000,
  "description": "调整后的工资",
  "remark": "加薪"
}
```

### 删除收入

**DELETE** `/api/assets/incomes/{id}`

---

## 支出管理

### 获取支出列表

**GET** `/api/assets/expenses`

#### 查询参数

同收入列表

### 获取支出详情

**GET** `/api/assets/expenses/{id}`

### 创建支出

**POST** `/api/assets/expenses`

#### 请求体

```json
{
  "expenseDate": "2026-05-28",
  "category": "餐饮",
  "title": "午餐",
  "amount": 50,
  "description": "工作日午餐",
  "remark": null
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| expenseDate | string | 是 | 支出日期（最大20字符） |
| category | string | 是 | 分类（最大50字符） |
| title | string | 是 | 标题（最大200字符） |
| amount | decimal | 是 | 金额（大于0） |
| description | string | 否 | 描述（最大1000字符） |
| remark | string | 否 | 备注（最大1000字符） |

### 更新支出

**PUT** `/api/assets/expenses/{id}`

### 删除支出

**DELETE** `/api/assets/expenses/{id}`

---

## 预算管理

### 获取预算列表

**GET** `/api/assets/budgets`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索 |
| category | string | 否 | 分类 |
| year | int | 否 | 年份 |
| month | int | 否 | 月份 |
| page | int | 否 | 页码 |
| pageSize | int | 否 | 每页数量 |

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
        "year": 2026,
        "month": 5,
        "category": "餐饮",
        "plannedAmount": 3000,
        "actualAmount": 2500,
        "remainingAmount": 500,
        "remark": null,
        "createdAt": "2026-05-01T00:00:00Z"
      }
    ],
    "total": 12,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

### 获取预算详情

**GET** `/api/assets/budgets/{id}`

### 创建预算

**POST** `/api/assets/budgets`

#### 请求体

```json
{
  "year": 2026,
  "month": 5,
  "category": "餐饮",
  "plannedAmount": 3000,
  "actualAmount": 0,
  "remark": null
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| year | int | 是 | 年份（2000-2100） |
| month | int | 是 | 月份（1-12） |
| category | string | 是 | 分类（最大50字符） |
| plannedAmount | decimal | 是 | 计划金额 |
| actualAmount | decimal | 否 | 实际金额 |
| remark | string | 否 | 备注（最大500字符） |

### 更新预算

**PUT** `/api/assets/budgets/{id}`

### 删除预算

**DELETE** `/api/assets/budgets/{id}`

---

## 投资管理

### 获取投资列表

**GET** `/api/assets/investments`

#### 查询参数

| 参数名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| keyword | string | 否 | 关键词搜索 |
| category | string | 否 | 投资类型 |
| startDate | string | 否 | 开始日期 |
| endDate | string | 否 | 结束日期 |
| page | int | 否 | 页码 |
| pageSize | int | 否 | 每页数量 |

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
        "investmentDate": "2026-01-15",
        "type": "股票",
        "name": "腾讯控股",
        "amount": 50000,
        "currentValue": 55000,
        "returnRate": 10,
        "description": "长期投资",
        "remark": null,
        "createdAt": "2026-01-15T10:00:00Z"
      }
    ],
    "total": 5,
    "page": 1,
    "pageSize": 10
  },
  "success": true
}
```

### 获取投资详情

**GET** `/api/assets/investments/{id}`

### 创建投资

**POST** `/api/assets/investments`

#### 请求体

```json
{
  "investmentDate": "2026-01-15",
  "type": "股票",
  "name": "腾讯控股",
  "amount": 50000,
  "currentValue": 55000,
  "returnRate": 10,
  "description": "长期投资",
  "remark": null
}
```

#### 字段说明

| 字段名 | 类型 | 必需 | 说明 |
|--------|------|------|------|
| investmentDate | string | 是 | 投资日期（最大20字符） |
| type | string | 是 | 投资类型（最大50字符） |
| name | string | 是 | 投资名称（最大200字符） |
| amount | decimal | 是 | 投资金额 |
| currentValue | decimal | 否 | 当前价值 |
| returnRate | decimal | 否 | 回报率（-100到1000） |
| description | string | 否 | 描述（最大1000字符） |
| remark | string | 否 | 备注（最大1000字符） |

### 更新投资

**PUT** `/api/assets/investments/{id}`

### 删除投资

**DELETE** `/api/assets/investments/{id}`

---

## 数据模型

### AssetSummaryDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| totalIncome | decimal | 总收入 |
| totalExpense | decimal | 总支出 |
| totalInvestment | decimal | 总投资 |
| netAsset | decimal | 净资产 |
| incomeCount | int | 收入笔数 |
| expenseCount | int | 支出笔数 |
| investmentCount | int | 投资笔数 |

### IncomeDto / ExpenseDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 记录ID |
| userId | guid | 用户ID |
| incomeDate/expenseDate | string | 日期 |
| category | string | 分类 |
| title | string | 标题 |
| amount | decimal | 金额 |
| description | string | 描述 |
| remark | string | 备注 |
| createdAt | string (datetime) | 创建时间 |

### BudgetDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 预算ID |
| userId | guid | 用户ID |
| year | int | 年份 |
| month | int | 月份 |
| category | string | 分类 |
| plannedAmount | decimal | 计划金额 |
| actualAmount | decimal | 实际金额 |
| remainingAmount | decimal | 剩余金额 |
| remark | string | 备注 |
| createdAt | string (datetime) | 创建时间 |

### InvestmentDto

| 字段名 | 类型 | 说明 |
|--------|------|------|
| id | guid | 投资ID |
| userId | guid | 用户ID |
| investmentDate | string | 投资日期 |
| type | string | 投资类型 |
| name | string | 投资名称 |
| amount | decimal | 投资金额 |
| currentValue | decimal | 当前价值 |
| returnRate | decimal | 回报率 |
| description | string | 描述 |
| remark | string | 备注 |
| createdAt | string (datetime) | 创建时间 |
