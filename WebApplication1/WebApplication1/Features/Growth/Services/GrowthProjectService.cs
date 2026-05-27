using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Entities;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Growth.Services;

public class GrowthProjectService(AppDbContext dbContext) : IGrowthProjectService
{
    public async Task<PageResult<GrowthProjectDto>> GetPageAsync(GrowthProjectQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var projects = dbContext.GrowthProjects.AsNoTracking();

        if (userId.HasValue)
        {
            projects = projects.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            projects = projects.Where(x => x.Name.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));
        }

        if (query.Status.HasValue)
        {
            projects = projects.Where(x => x.Status == (GrowthProjectStatus)query.Status.Value);
        }

        if (query.Type.HasValue)
        {
            projects = projects.Where(x => x.Type == (GrowthProjectType)query.Type.Value);
        }

        var total = await projects.CountAsync(cancellationToken);
        var items = await projects
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<GrowthProjectDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<GrowthProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await dbContext.GrowthProjects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return project is null ? null : ToDto(project);
    }

    public async Task<GrowthProjectDto> CreateAsync(CreateGrowthProjectDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var project = new GrowthProject
        {
            UserId = userId,
            Name = input.Name,
            Description = input.Description,
            StartDate = input.StartDate,
            EndDate = input.EndDate,
            Status = (GrowthProjectStatus)input.Status,
            Type = (GrowthProjectType)input.Type,
            Progress = 0,
            TaskCount = 0
        };

        dbContext.GrowthProjects.Add(project);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(project);
    }

    public async Task<GrowthProjectDto?> UpdateAsync(Guid id, UpdateGrowthProjectDto input, CancellationToken cancellationToken = default)
    {
        var project = await dbContext.GrowthProjects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (project is null) return null;

        if (input.Name is not null) project.Name = input.Name;
        if (input.Description is not null) project.Description = input.Description;
        if (input.StartDate.HasValue) project.StartDate = input.StartDate;
        if (input.EndDate.HasValue) project.EndDate = input.EndDate;
        if (input.Progress.HasValue) project.Progress = input.Progress.Value;
        if (input.TaskCount.HasValue) project.TaskCount = input.TaskCount.Value;
        if (input.Status.HasValue) project.Status = (GrowthProjectStatus)input.Status.Value;
        if (input.Type.HasValue) project.Type = (GrowthProjectType)input.Type.Value;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(project);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await dbContext.GrowthProjects.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (project is null) return false;

        dbContext.GrowthProjects.Remove(project);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static GrowthProjectDto ToDto(GrowthProject project) => new()
    {
        Id = project.Id,
        UserId = project.UserId,
        Name = project.Name,
        Description = project.Description,
        StartDate = project.StartDate,
        EndDate = project.EndDate,
        Progress = project.Progress,
        TaskCount = project.TaskCount,
        Status = (int)project.Status,
        Type = (int)project.Type,
        CreatedAt = project.CreatedAt,
        UpdatedAt = project.UpdatedAt
    };
}