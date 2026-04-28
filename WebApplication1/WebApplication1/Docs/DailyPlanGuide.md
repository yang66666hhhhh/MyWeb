# Personal Growth Management Backend - Phase 1

## Backend Project Structure

```text
WebApplication1/
  Common/
    ApiResult.cs
    PageResult.cs
  Controllers/
    DailyPlansController.cs
  Data/
    AppDbContext.cs
  Dtos/
    DailyPlans/
      CreateDailyPlanDto.cs
      DailyPlanDto.cs
      DailyPlanQueryDto.cs
      UpdateDailyPlanDto.cs
  Entities/
    DailyPlan.cs
  Enums/
    DailyPlanStatus.cs
  Services/
    Interfaces/
      IDailyPlanService.cs
    Implementations/
      DailyPlanService.cs
  Program.cs
  appsettings.json
```

## Unified Response Shape

Vue Vben Admin currently reads `code`, `data`, and `message` in `src/api/request.ts`.

```json
{
  "code": 0,
  "message": "success",
  "data": {}
}
```

Paged APIs return the page object inside `data`.

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

## DailyPlan RESTful APIs

```text
GET    /api/daily-plans
GET    /api/daily-plans/{id}
POST   /api/daily-plans
PUT    /api/daily-plans/{id}
PATCH  /api/daily-plans/{id}/complete
DELETE /api/daily-plans/{id}
```

All DailyPlan endpoints currently require JWT Bearer authentication.

## EF Core Migration Commands

Run these commands in:

```powershell
D:\03_Projects\Personal\MyWebSite\WebApplication1\WebApplication1
```

Install or update the EF CLI if needed:

```powershell
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```

Create the first migration:

```powershell
dotnet ef migrations add InitialDailyPlan
```

Apply the migration to SQL Server LocalDB:

```powershell
dotnet ef database update
```

Default connection string is in `appsettings.json`.

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=PersonalGrowthDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

## Swagger

Start the backend and open:

```text
https://localhost:7015/swagger
```

or:

```text
http://localhost:5062/swagger
```

Use the Swagger `Authorize` button with:

```text
Bearer {your-jwt-token}
```

## Vue Vben Admin API Example

The example file has been added here:

```text
D:\03_Projects\Personal\MyWebSite\vue-vben-admin\apps\web-antd\src\api\daily-plan.ts
```

Example usage:

```ts
import {
  completeDailyPlanApi,
  createDailyPlanApi,
  getDailyPlanPageApi,
} from '#/api';

const page = await getDailyPlanPageApi({
  page: 1,
  pageSize: 10,
  keyword: 'review',
  startDate: '2026-04-25',
  endDate: '2026-04-25',
});

await createDailyPlanApi({
  planDate: '2026-04-25',
  title: 'Finish DailyPlan backend module',
  priority: 2,
  status: 0,
});

await completeDailyPlanApi(page.items[0].id);
```

The Vben dev proxy currently targets `http://localhost:5320/api`. To call this ASP.NET Core backend directly, update `apps/web-antd/vite.config.ts`:

```ts
target: 'http://localhost:5062/api',
```

Then keep calling frontend APIs with paths such as `/daily-plans`; the proxy rewrites `/api/daily-plans` to the backend `/api/daily-plans`.
