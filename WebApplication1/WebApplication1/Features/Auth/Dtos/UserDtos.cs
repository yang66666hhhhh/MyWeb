using System.ComponentModel.DataAnnotations;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Auth.Entities;

namespace WebApplication1.Features.Auth.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Roles { get; set; } = "member";
    public AppUserStatus Status { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UserQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public AppUserStatus? Status { get; set; }
}

public class CreateUserDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(50, ErrorMessage = "用户名不能超过50个字符")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [MinLength(6, ErrorMessage = "密码至少6个字符")]
    [MaxLength(100, ErrorMessage = "密码不能超过100个字符")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "真实姓名不能为空")]
    [MaxLength(100, ErrorMessage = "真实姓名不能超过100个字符")]
    public string RealName { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "头像URL不能超过500个字符")]
    public string? Avatar { get; set; }

    [MaxLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    [MaxLength(20, ErrorMessage = "电话不能超过20个字符")]
    [Phone(ErrorMessage = "电话格式不正确")]
    public string? Phone { get; set; }

    [MaxLength(100, ErrorMessage = "角色不能超过100个字符")]
    public string Roles { get; set; } = "member";
}

public class UpdateUserDto
{
    [Required(ErrorMessage = "真实姓名不能为空")]
    [MaxLength(100, ErrorMessage = "真实姓名不能超过100个字符")]
    public string RealName { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "头像URL不能超过500个字符")]
    public string? Avatar { get; set; }

    [MaxLength(100, ErrorMessage = "邮箱不能超过100个字符")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string? Email { get; set; }

    [MaxLength(20, ErrorMessage = "电话不能超过20个字符")]
    [Phone(ErrorMessage = "电话格式不正确")]
    public string? Phone { get; set; }

    [MaxLength(100, ErrorMessage = "角色不能超过100个字符")]
    public string? Roles { get; set; }

    public AppUserStatus? Status { get; set; }
}

public class ChangePasswordDto
{
    [Required(ErrorMessage = "旧密码不能为空")]
    public string OldPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "新密码不能为空")]
    [MinLength(6, ErrorMessage = "新密码至少6个字符")]
    [MaxLength(100, ErrorMessage = "新密码不能超过100个字符")]
    public string NewPassword { get; set; } = string.Empty;
}

public class ResetPasswordDto
{
    [Required(ErrorMessage = "新密码不能为空")]
    [MinLength(6, ErrorMessage = "新密码至少6个字符")]
    [MaxLength(100, ErrorMessage = "新密码不能超过100个字符")]
    public string NewPassword { get; set; } = string.Empty;
}