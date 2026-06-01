using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Persona.Dtos;
using WebApplication1.Features.Persona.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Persona;

public interface IPersonaFeatureService
{
    Task<PageResult<CodeRepositoryDto>> GetRepositoriesAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default);
    Task<CodeRepositoryDto?> GetRepositoryByIdAsync(Guid id, CancellationToken ct = default);
    Task<CodeRepositoryDto> CreateRepositoryAsync(CreateCodeRepositoryInput input, Guid userId, CancellationToken ct = default);
    Task<CodeRepositoryDto?> UpdateRepositoryAsync(Guid id, CreateCodeRepositoryInput input, CancellationToken ct = default);
    Task<bool> DeleteRepositoryAsync(Guid id, CancellationToken ct = default);

    Task<PageResult<IssueDto>> GetIssuesAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default);
    Task<IssueDto?> GetIssueByIdAsync(Guid id, CancellationToken ct = default);
    Task<IssueDto> CreateIssueAsync(CreateIssueInput input, Guid userId, CancellationToken ct = default);
    Task<IssueDto?> UpdateIssueAsync(Guid id, CreateIssueInput input, CancellationToken ct = default);
    Task<bool> DeleteIssueAsync(Guid id, CancellationToken ct = default);

    Task<PageResult<PipelineDto>> GetPipelinesAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default);
    Task<PipelineDto?> GetPipelineByIdAsync(Guid id, CancellationToken ct = default);
    Task<PipelineDto> CreatePipelineAsync(CreatePipelineInput input, Guid userId, CancellationToken ct = default);
    Task<PipelineDto?> UpdatePipelineAsync(Guid id, CreatePipelineInput input, CancellationToken ct = default);
    Task<bool> DeletePipelineAsync(Guid id, CancellationToken ct = default);

    Task<PageResult<DesignAssetDto>> GetDesignAssetsAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default);
    Task<DesignAssetDto?> GetDesignAssetByIdAsync(Guid id, CancellationToken ct = default);
    Task<DesignAssetDto> CreateDesignAssetAsync(CreateDesignAssetInput input, Guid userId, CancellationToken ct = default);
    Task<DesignAssetDto?> UpdateDesignAssetAsync(Guid id, CreateDesignAssetInput input, CancellationToken ct = default);
    Task<bool> DeleteDesignAssetAsync(Guid id, CancellationToken ct = default);

    Task<PageResult<PrototypeDto>> GetPrototypesAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default);
    Task<PrototypeDto?> GetPrototypeByIdAsync(Guid id, CancellationToken ct = default);
    Task<PrototypeDto> CreatePrototypeAsync(CreatePrototypeInput input, Guid userId, CancellationToken ct = default);
    Task<PrototypeDto?> UpdatePrototypeAsync(Guid id, CreatePrototypeInput input, CancellationToken ct = default);
    Task<bool> DeletePrototypeAsync(Guid id, CancellationToken ct = default);

    Task<PageResult<TeacherCourseDto>> GetTeacherCoursesAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default);
    Task<TeacherCourseDto?> GetTeacherCourseByIdAsync(Guid id, CancellationToken ct = default);
    Task<TeacherCourseDto> CreateTeacherCourseAsync(CreateTeacherCourseInput input, Guid userId, CancellationToken ct = default);
    Task<TeacherCourseDto?> UpdateTeacherCourseAsync(Guid id, CreateTeacherCourseInput input, CancellationToken ct = default);
    Task<bool> DeleteTeacherCourseAsync(Guid id, CancellationToken ct = default);

    Task<PageResult<TeacherStudentDto>> GetTeacherStudentsAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default);
    Task<TeacherStudentDto?> GetTeacherStudentByIdAsync(Guid id, CancellationToken ct = default);
    Task<TeacherStudentDto> CreateTeacherStudentAsync(CreateTeacherStudentInput input, Guid userId, CancellationToken ct = default);
    Task<TeacherStudentDto?> UpdateTeacherStudentAsync(Guid id, CreateTeacherStudentInput input, CancellationToken ct = default);
    Task<bool> DeleteTeacherStudentAsync(Guid id, CancellationToken ct = default);
}

public class PersonaFeatureService(AppDbContext db) : IPersonaFeatureService
{
    // ==================== CodeRepositories ====================

    public async Task<PageResult<CodeRepositoryDto>> GetRepositoriesAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.CodeRepositories.AsNoTracking();

        if (userId.HasValue)
            q = q.Where(x => x.UserId == userId.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.IsPublic == (query.Status.Value == 1));

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            q = q.Where(x => x.Name.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var category = query.Category.Trim();
            q = q.Where(x => x.Language.Contains(category));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return PageResult<CodeRepositoryDto>.Create(items, total, page, pageSize);
    }

    public async Task<CodeRepositoryDto?> GetRepositoryByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.CodeRepositories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<CodeRepositoryDto> CreateRepositoryAsync(CreateCodeRepositoryInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new CodeRepository
        {
            UserId = userId,
            Name = input.Name,
            Description = input.Description,
            Url = input.Url,
            Language = input.Language,
            IsPublic = input.IsPublic,
            Tags = input.Tags
        };

        db.CodeRepositories.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<CodeRepositoryDto?> UpdateRepositoryAsync(Guid id, CreateCodeRepositoryInput input, CancellationToken ct = default)
    {
        var entity = await db.CodeRepositories.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        entity.Name = input.Name;
        entity.Description = input.Description;
        entity.Url = input.Url;
        entity.Language = input.Language;
        entity.IsPublic = input.IsPublic;
        entity.Tags = input.Tags;

        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<bool> DeleteRepositoryAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.CodeRepositories.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        db.CodeRepositories.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static CodeRepositoryDto ToDto(CodeRepository x) => new()
    {
        Id = x.Id.ToString(),
        UserId = x.UserId.ToString(),
        Name = x.Name,
        Description = x.Description,
        Url = x.Url,
        Language = x.Language,
        IsPublic = x.IsPublic,
        Stars = x.Stars,
        Tags = x.Tags,
        CreatedAt = x.CreatedAt.ToString("O")
    };

    // ==================== Issues ====================

    public async Task<PageResult<IssueDto>> GetIssuesAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.Issues.AsNoTracking();

        if (userId.HasValue)
            q = q.Where(x => x.UserId == userId.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            q = q.Where(x => x.Title.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var category = query.Category.Trim();
            q = q.Where(x => x.Repository.Contains(category));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return PageResult<IssueDto>.Create(items, total, page, pageSize);
    }

    public async Task<IssueDto?> GetIssueByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Issues.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<IssueDto> CreateIssueAsync(CreateIssueInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new Issue
        {
            UserId = userId,
            Title = input.Title,
            Description = input.Description,
            Repository = input.Repository,
            Priority = input.Priority,
            Assignee = input.Assignee,
            Labels = input.Labels
        };

        db.Issues.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<IssueDto?> UpdateIssueAsync(Guid id, CreateIssueInput input, CancellationToken ct = default)
    {
        var entity = await db.Issues.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        entity.Title = input.Title;
        entity.Description = input.Description;
        entity.Repository = input.Repository;
        entity.Priority = input.Priority;
        entity.Assignee = input.Assignee;
        entity.Labels = input.Labels;

        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<bool> DeleteIssueAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Issues.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        db.Issues.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static IssueDto ToDto(Issue x) => new()
    {
        Id = x.Id.ToString(),
        UserId = x.UserId.ToString(),
        Title = x.Title,
        Description = x.Description,
        Repository = x.Repository,
        Status = x.Status,
        Priority = x.Priority,
        Assignee = x.Assignee,
        Labels = x.Labels,
        CreatedAt = x.CreatedAt.ToString("O")
    };

    // ==================== Pipelines ====================

    public async Task<PageResult<PipelineDto>> GetPipelinesAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.Pipelines.AsNoTracking();

        if (userId.HasValue)
            q = q.Where(x => x.UserId == userId.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            q = q.Where(x => x.Name.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var category = query.Category.Trim();
            q = q.Where(x => x.Repository.Contains(category));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return PageResult<PipelineDto>.Create(items, total, page, pageSize);
    }

    public async Task<PipelineDto?> GetPipelineByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Pipelines.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<PipelineDto> CreatePipelineAsync(CreatePipelineInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new Pipeline
        {
            UserId = userId,
            Name = input.Name,
            Description = input.Description,
            Repository = input.Repository,
            Branch = input.Branch,
            TriggerType = input.TriggerType,
            Steps = input.Steps
        };

        db.Pipelines.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<PipelineDto?> UpdatePipelineAsync(Guid id, CreatePipelineInput input, CancellationToken ct = default)
    {
        var entity = await db.Pipelines.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        entity.Name = input.Name;
        entity.Description = input.Description;
        entity.Repository = input.Repository;
        entity.Branch = input.Branch;
        entity.TriggerType = input.TriggerType;
        entity.Steps = input.Steps;

        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<bool> DeletePipelineAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Pipelines.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        db.Pipelines.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static PipelineDto ToDto(Pipeline x) => new()
    {
        Id = x.Id.ToString(),
        UserId = x.UserId.ToString(),
        Name = x.Name,
        Description = x.Description,
        Repository = x.Repository,
        Branch = x.Branch,
        Status = x.Status,
        TriggerType = x.TriggerType,
        Steps = x.Steps,
        LastRunAt = x.LastRunAt?.ToString("O"),
        CreatedAt = x.CreatedAt.ToString("O")
    };

    // ==================== DesignAssets ====================

    public async Task<PageResult<DesignAssetDto>> GetDesignAssetsAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.DesignAssets.AsNoTracking();

        if (userId.HasValue)
            q = q.Where(x => x.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            q = q.Where(x => x.Name.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var category = query.Category.Trim();
            q = q.Where(x => x.Category.Contains(category));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return PageResult<DesignAssetDto>.Create(items, total, page, pageSize);
    }

    public async Task<DesignAssetDto?> GetDesignAssetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.DesignAssets.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<DesignAssetDto> CreateDesignAssetAsync(CreateDesignAssetInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new DesignAsset
        {
            UserId = userId,
            Name = input.Name,
            Description = input.Description,
            Category = input.Category,
            FileUrl = input.FileUrl,
            FileType = input.FileType,
            FileSize = input.FileSize,
            Tags = input.Tags
        };

        db.DesignAssets.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<DesignAssetDto?> UpdateDesignAssetAsync(Guid id, CreateDesignAssetInput input, CancellationToken ct = default)
    {
        var entity = await db.DesignAssets.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        entity.Name = input.Name;
        entity.Description = input.Description;
        entity.Category = input.Category;
        entity.FileUrl = input.FileUrl;
        entity.FileType = input.FileType;
        entity.FileSize = input.FileSize;
        entity.Tags = input.Tags;

        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<bool> DeleteDesignAssetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.DesignAssets.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        db.DesignAssets.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static DesignAssetDto ToDto(DesignAsset x) => new()
    {
        Id = x.Id.ToString(),
        UserId = x.UserId.ToString(),
        Name = x.Name,
        Description = x.Description,
        Category = x.Category,
        FileUrl = x.FileUrl,
        FileType = x.FileType,
        FileSize = x.FileSize,
        Tags = x.Tags,
        CreatedAt = x.CreatedAt.ToString("O")
    };

    // ==================== Prototypes ====================

    public async Task<PageResult<PrototypeDto>> GetPrototypesAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.Prototypes.AsNoTracking();

        if (userId.HasValue)
            q = q.Where(x => x.UserId == userId.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            q = q.Where(x => x.Title.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var category = query.Category.Trim();
            q = q.Where(x => x.Project.Contains(category));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return PageResult<PrototypeDto>.Create(items, total, page, pageSize);
    }

    public async Task<PrototypeDto?> GetPrototypeByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Prototypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<PrototypeDto> CreatePrototypeAsync(CreatePrototypeInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new Prototype
        {
            UserId = userId,
            Title = input.Title,
            Description = input.Description,
            Project = input.Project,
            PreviewUrl = input.PreviewUrl,
            Tags = input.Tags
        };

        db.Prototypes.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<PrototypeDto?> UpdatePrototypeAsync(Guid id, CreatePrototypeInput input, CancellationToken ct = default)
    {
        var entity = await db.Prototypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        entity.Title = input.Title;
        entity.Description = input.Description;
        entity.Project = input.Project;
        entity.PreviewUrl = input.PreviewUrl;
        entity.Tags = input.Tags;

        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<bool> DeletePrototypeAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Prototypes.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        db.Prototypes.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static PrototypeDto ToDto(Prototype x) => new()
    {
        Id = x.Id.ToString(),
        UserId = x.UserId.ToString(),
        Title = x.Title,
        Description = x.Description,
        Project = x.Project,
        PreviewUrl = x.PreviewUrl,
        Status = x.Status,
        Tags = x.Tags,
        CreatedAt = x.CreatedAt.ToString("O")
    };

    // ==================== TeacherCourses ====================

    public async Task<PageResult<TeacherCourseDto>> GetTeacherCoursesAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.TeacherCourses.AsNoTracking();

        if (userId.HasValue)
            q = q.Where(x => x.UserId == userId.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            q = q.Where(x => x.Name.Contains(keyword) || x.Code.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return PageResult<TeacherCourseDto>.Create(items, total, page, pageSize);
    }

    public async Task<TeacherCourseDto?> GetTeacherCourseByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.TeacherCourses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<TeacherCourseDto> CreateTeacherCourseAsync(CreateTeacherCourseInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new TeacherCourse
        {
            UserId = userId,
            Name = input.Name,
            Description = input.Description,
            Code = input.Code,
            Semester = input.Semester,
            Year = input.Year,
            Tags = input.Tags
        };

        db.TeacherCourses.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<TeacherCourseDto?> UpdateTeacherCourseAsync(Guid id, CreateTeacherCourseInput input, CancellationToken ct = default)
    {
        var entity = await db.TeacherCourses.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        entity.Name = input.Name;
        entity.Description = input.Description;
        entity.Code = input.Code;
        entity.Semester = input.Semester;
        entity.Year = input.Year;
        entity.Tags = input.Tags;

        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<bool> DeleteTeacherCourseAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.TeacherCourses.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        db.TeacherCourses.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static TeacherCourseDto ToDto(TeacherCourse x) => new()
    {
        Id = x.Id.ToString(),
        UserId = x.UserId.ToString(),
        Name = x.Name,
        Description = x.Description,
        Code = x.Code,
        Semester = x.Semester,
        Year = x.Year,
        StudentCount = x.StudentCount,
        Status = x.Status,
        Tags = x.Tags,
        CreatedAt = x.CreatedAt.ToString("O")
    };

    // ==================== TeacherStudents ====================

    public async Task<PageResult<TeacherStudentDto>> GetTeacherStudentsAsync(PersonaQueryDto query, Guid? userId, CancellationToken ct = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.TeacherStudents.AsNoTracking();

        if (userId.HasValue)
            q = q.Where(x => x.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            q = q.Where(x => x.Name.Contains(keyword) || (x.StudentId != null && x.StudentId.Contains(keyword)) || (x.Email != null && x.Email.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var category = query.Category.Trim();
            q = q.Where(x => x.Course != null && x.Course.Contains(category));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => ToDto(x))
            .ToListAsync(ct);

        return PageResult<TeacherStudentDto>.Create(items, total, page, pageSize);
    }

    public async Task<TeacherStudentDto?> GetTeacherStudentByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.TeacherStudents.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<TeacherStudentDto> CreateTeacherStudentAsync(CreateTeacherStudentInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new TeacherStudent
        {
            UserId = userId,
            Name = input.Name,
            StudentId = input.StudentId,
            Email = input.Email,
            Phone = input.Phone,
            Course = input.Course,
            Tags = input.Tags
        };

        db.TeacherStudents.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<TeacherStudentDto?> UpdateTeacherStudentAsync(Guid id, CreateTeacherStudentInput input, CancellationToken ct = default)
    {
        var entity = await db.TeacherStudents.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return null;

        entity.Name = input.Name;
        entity.StudentId = input.StudentId;
        entity.Email = input.Email;
        entity.Phone = input.Phone;
        entity.Course = input.Course;
        entity.Tags = input.Tags;

        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<bool> DeleteTeacherStudentAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.TeacherStudents.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        db.TeacherStudents.Remove(entity);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static TeacherStudentDto ToDto(TeacherStudent x) => new()
    {
        Id = x.Id.ToString(),
        UserId = x.UserId.ToString(),
        Name = x.Name,
        StudentId = x.StudentId,
        Email = x.Email,
        Phone = x.Phone,
        Course = x.Course,
        Grade = x.Grade,
        Tags = x.Tags,
        CreatedAt = x.CreatedAt.ToString("O")
    };
}
