using System.Reflection;
using System.Text;
using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using WebApplication1.Shared.Audit;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Middleware;
using WebApplication1.Shared.Security;
using WebApplication1.Features.Auth;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.DailyPlans;
using WebApplication1.Features.Tasks;
using WebApplication1.Features.Work.Services;
using WebApplication1.Features.Work.Services.Interfaces;
using WebApplication1.Features.Growth.Services;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Features.Ai.Services;
using WebApplication1.Features.Admin.Services;
using WebApplication1.Features.Auth.Services;
using WebApplication1.Features.Analytics;
using WebApplication1.Features.Assets.Services;
using WebApplication1.Features.Assets.Services.Interfaces;
using WebApplication1.Features.Content.Services;
using WebApplication1.Features.Content.Services.Interfaces;
using WebApplication1.Shared.HealthChecks;
using WebApplication1.Shared.Localization;
using WebApplication1.Shared.Services;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "VueVbenAdmin";

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSingleton<ILocalizationService, LocalizationService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(options =>
{
}).ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors)
            .Select(e => e.ErrorMessage)
            .ToArray();

        var result = new WebApplication1.Shared.Common.ApiResult
        {
            Code = 400,
            Message = string.Join("; ", errors)
        };

        return new Microsoft.AspNetCore.Mvc.ObjectResult(result)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    };
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Personal Growth Management API",
        Version = "v1",
        Description = "全栈个人成长与工作管理系统 API - 支持每日计划、习惯打卡、工作日志、知识库、考研备选、AI助手等功能",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@example.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Title = "Personal Growth Management API",
        Version = "v2",
        Description = "全栈个人成长与工作管理系统 API - V2 版本（实验性）"
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "输入 JWT 令牌，格式: Bearer {token}"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
    });

    options.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return [api.GroupName];
        }

        var controllerName = api.ActionDescriptor.RouteValues["controller"];
        if (!string.IsNullOrEmpty(controllerName))
        {
            return [controllerName];
        }

        return ["default"];
    });

    options.DocInclusionPredicate((docName, api) =>
    {
        if (docName == "v1")
        {
            return true;
        }

        if (docName == "v2")
        {
            return api.RelativePath?.StartsWith("api/v2") ?? false;
        }

        return false;
    });

    options.OrderActionsBy(apiDesc => apiDesc.RelativePath);

    options.EnableAnnotations();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
            ?? ["http://localhost:5666", "http://localhost:5173"];
        policy
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
});

// 注册安全和监控服务
builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection(RateLimitOptions.SectionName));
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IAuditService, AuditService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? Environment.GetEnvironmentVariable("DB_CONNECTION")
        ?? throw new InvalidOperationException("DefaultConnection is not configured.");
    options.UseMySQL(connectionString);
});

var jwtSecretKey = builder.Configuration["Jwt:SecretKey"]
    ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
if (string.IsNullOrWhiteSpace(jwtSecretKey) || jwtSecretKey.Length < 32)
{
    throw new InvalidOperationException("Jwt:SecretKey must be at least 32 characters long.");
}
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDailyPlanService, DailyPlanService>();
builder.Services.AddScoped<ITaskItemService, TaskItemService>();
builder.Services.AddScoped<IWorkProjectService, WorkProjectService>();
builder.Services.AddScoped<IWorkDeviceService, WorkDeviceService>();
builder.Services.AddScoped<IWorkTaskTypeService, WorkTaskTypeService>();
builder.Services.AddScoped<IWorkLogService, WorkLogService>();
builder.Services.AddScoped<IWorkStatisticsService, WorkStatisticsService>();
builder.Services.AddScoped<IWorkDailyPlanService, WorkDailyPlanService>();
builder.Services.AddScoped<IWorkImportService, WorkImportService>();
builder.Services.AddScoped<IImplLogService, ImplLogService>();
builder.Services.AddScoped<IWorkCategoryService, WorkCategoryService>();
builder.Services.AddScoped<IWeeklyPlanService, WeeklyPlanService>();
builder.Services.AddScoped<ISoftwareAssetService, SoftwareAssetService>();
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IGrowthProjectService, GrowthProjectService>();
builder.Services.AddScoped<IKnowledgeArticleService, KnowledgeArticleService>();
builder.Services.AddScoped<IPostgraduateTaskService, PostgraduateTaskService>();
builder.Services.AddScoped<IExamMistakeService, ExamMistakeService>();
builder.Services.AddScoped<IExamMaterialService, ExamMaterialService>();
builder.Services.AddScoped<IStudentSubjectService, StudentSubjectService>();
builder.Services.AddScoped<IStudentStudyRecordService, StudentStudyRecordService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddScoped<IAiService, AiService>();
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<RoleMenuService>();
builder.Services.AddScoped<IUserAccessContextService, UserAccessContextService>();
builder.Services.AddScoped<IFeatureService, FeatureService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IContentService, ContentService>();

builder.Services.AddHealthChecks()
    .AddMySql(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
        name: "mysql",
        timeout: TimeSpan.FromSeconds(3),
        tags: ["db", "mysql"])
    .AddCheck<MemoryHealthCheck>(
        "memory",
        failureStatus: HealthStatus.Degraded,
        tags: ["system", "memory"])
    .AddCheck<DiskSpaceHealthCheck>(
        "disk",
        failureStatus: HealthStatus.Degraded,
        tags: ["system", "disk"])
    .AddCheck<CpuHealthCheck>(
        "cpu",
        failureStatus: HealthStatus.Degraded,
        tags: ["system", "cpu"])
    .AddCheck<ApplicationHealthCheck>(
        "application",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["system", "app"]);

var app = builder.Build();

var supportedCultures = new[] { "zh-CN", "en-US" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("zh-CN")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "swagger/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Personal Growth API v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Personal Growth API v2 (Preview)");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Personal Growth Management API";
        options.DefaultModelsExpandDepth(2);
        options.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Example);
        options.DisplayRequestDuration();
        options.EnableDeepLinking();
        options.EnableFilter();
        options.ShowExtensions();
    });
}

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exception, "Unhandled exception occurred");

        var result = new WebApplication1.Shared.Common.ApiResult
        {
            Code = 500,
            Message = app.Environment.IsDevelopment()
                ? (exception?.Message ?? "Internal server error")
                : "Internal server error"
        };

        await context.Response.WriteAsJsonAsync(result);
    });
});

app.UseHttpsRedirection();

app.UseCors(CorsPolicyName);

// 安全中间件
app.UseMiddleware<XssProtectionMiddleware>();
app.UseMiddleware<SqlInjectionMiddleware>();

// 监控中间件
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<PerformanceMonitoringMiddleware>();

app.UseMiddleware<RateLimitingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                data = e.Value.Data
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.MapHealthChecks("/healthz/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("db"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

app.MapHealthChecks("/healthz/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false,
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsJsonAsync(result);
    }
});

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await DbSeeder.SeedAsync(context, logger);
}

app.Run();
