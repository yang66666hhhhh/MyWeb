# Personal Growth Frontend Structure

## Directory

```text
src/
  api/
    growth/
      daily-plan.ts
      habit.ts
      knowledge-base.ts
      postgraduate.ts
      project.ts
      types.ts
      work-log.ts
  components/
    growth/
      ModuleScaffold.vue
  composables/
    usePagedQuery.ts
  router/
    routes/
      modules/
        growth.ts
  store/
    modules/
      daily-plan.ts
  views/
    growth/
      dashboard/
      daily-plan/
        components/
          DailyPlanForm.vue
        index.vue
      habit/
        components/
          HabitForm.vue
        index.vue
      knowledge-base/
      postgraduate/
      project/
      work-log/
```

## Routes

The route module is `src/router/routes/modules/growth.ts`.

```text
/growth/dashboard
/growth/daily-plans
/growth/habits
/growth/work-logs
/growth/knowledge-base
/growth/postgraduate
/growth/projects
```

## API Contract

The request client already unwraps the backend response with:

```ts
codeField: 'code',
dataField: 'data',
successCode: 0,
```

So module APIs return the actual `data` payload:

```ts
const page = await getDailyPlanPageApi({ page: 1, pageSize: 10 });
```

The backend should return:

```json
{
  "code": 0,
  "message": "success",
  "data": {
    "items": [],
    "total": 0,
    "page": 1,
    "pageSize": 10,
    "totalPages": 0
  }
}
```

## Permissions

Routes use `meta.authority`.

```ts
authority: ['growth:daily-plan:list']
```

The backend `/auth/codes` endpoint should return the permission codes needed by the current user, for example:

```json
[
  "growth:view",
  "growth:dashboard:view",
  "growth:daily-plan:list",
  "growth:habit:list",
  "growth:work-log:list",
  "growth:knowledge-base:list",
  "growth:postgraduate:list",
  "growth:project:list"
]
```

## Backend Proxy

Development proxy is split by responsibility:

```text
/api/auth           -> http://localhost:5320/api
/api/user           -> http://localhost:5320/api
/api/menu           -> http://localhost:5320/api
/api/daily-plans    -> http://localhost:5062/api
/api/habits         -> http://localhost:5062/api
/api/work-logs      -> http://localhost:5062/api
/api/knowledge-base -> http://localhost:5062/api
/api/postgraduate   -> http://localhost:5062/api
/api/projects       -> http://localhost:5062/api
```

Auth, user info and menu still use the Vben mock server until the ASP.NET Core API implements them.
Business modules call the ASP.NET Core API. Frontend code calls `/daily-plans`; Vite proxies `/api/daily-plans` to backend `/api/daily-plans`.
