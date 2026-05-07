using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication1.Features.Admin.Dtos;
using WebApplication1.Features.Auth.Dtos;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth;

public class UserService(AppDbContext context, ILogger<UserService> logger) : IUserService
{
    public async Task<PageResult<UserDto>> GetPageAsync(UserQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            q = q.Where(x =>
                x.Username.Contains(query.Keyword) ||
                x.RealName.Contains(query.Keyword) ||
                (x.Email != null && x.Email.Contains(query.Keyword)));
        }

        if (query.Status.HasValue)
        {
            q = q.Where(x => x.Status == query.Status.Value);
        }

        var total = await q.CountAsync(cancellationToken);
        var users = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var items = users.Select(x => new UserDto
        {
            Id = x.Id,
            Username = x.Username,
            RealName = x.RealName,
            Avatar = x.Avatar,
            Email = x.Email,
            Phone = x.Phone,
            Roles = x.Roles,
            Status = x.Status,
            Personas = context.UserPersonas
                .Where(up => up.UserId == x.Id)
                .Include(up => up.PersonaType)
                .Select(up => new PersonaTypeDto
                {
                    Id = up.PersonaType.Id,
                    Code = up.PersonaType.Code,
                    Name = up.PersonaType.Name,
                    Icon = up.PersonaType.Icon,
                    IsPrimary = up.IsPrimary
                })
                .ToList(),
            LastLoginAt = x.LastLoginAt,
            LastLoginIp = x.LastLoginIp,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        }).ToList();

        return PageResult<UserDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FindAsync([id], cancellationToken);
        if (user == null) return null;

        var personas = await context.UserPersonas
            .Where(x => x.UserId == id)
            .Include(x => x.PersonaType)
            .Select(x => new PersonaTypeDto
            {
                Id = x.PersonaType.Id,
                Code = x.PersonaType.Code,
                Name = x.PersonaType.Name,
                Icon = x.PersonaType.Icon,
                IsPrimary = x.IsPrimary
            })
            .ToListAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Avatar = user.Avatar,
            Email = user.Email,
            Phone = user.Phone,
            Roles = user.Roles,
            Status = user.Status,
            Personas = personas,
            LastLoginAt = user.LastLoginAt,
            LastLoginIp = user.LastLoginIp,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserDto input, CancellationToken cancellationToken = default)
    {
        var user = new AppUser
        {
            Username = input.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password),
            RealName = input.RealName,
            Avatar = input.Avatar,
            Email = input.Email,
            Phone = input.Phone,
            Roles = input.Roles,
            Status = AppUserStatus.Active
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Avatar = user.Avatar,
            Email = user.Email,
            Phone = user.Phone,
            Roles = user.Roles,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserDto input, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FindAsync([id], cancellationToken);
        if (user == null) return null;

        user.RealName = input.RealName;
        user.Avatar = input.Avatar;
        user.Email = input.Email;
        user.Phone = input.Phone;
        if (input.Roles != null)
        {
            user.Roles = input.Roles;
        }
        if (input.Status.HasValue)
        {
            user.Status = input.Status.Value;
        }

        await context.SaveChangesAsync(cancellationToken);

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Avatar = user.Avatar,
            Email = user.Email,
            Phone = user.Phone,
            Roles = user.Roles,
            Status = user.Status,
            LastLoginAt = user.LastLoginAt,
            LastLoginIp = user.LastLoginIp,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FindAsync([id], cancellationToken);
        if (user == null) return false;

        user.Status = AppUserStatus.Deleted;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto input, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FindAsync([id], cancellationToken);
        if (user == null) return false;

        if (!BCrypt.Net.BCrypt.Verify(input.OldPassword, user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.NewPassword);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(Guid id, ResetPasswordDto input, CancellationToken cancellationToken = default)
    {
        var user = await context.Users.FindAsync([id], cancellationToken);
        if (user == null) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.NewPassword);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ValidatePasswordAsync(string username, string password)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);
        if (user == null) return false;
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }
}