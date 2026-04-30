using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth;
using WebApplication1.Features.Auth.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
[Authorize(Roles = "admin,super")]
public class UsersController(IUserService userService) : ControllerBase
{
    private bool IsSuperAdmin() =>
        User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Any(c => c.Value.Equals("super", StringComparison.OrdinalIgnoreCase));

    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<UserDto>>>> GetPage(
        [FromQuery] UserQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await userService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<UserDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<UserDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await userService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<UserDto>.Fail("用户不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<UserDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<UserDto>>> Create(
        [FromBody] CreateUserDto input,
        CancellationToken cancellationToken)
    {
        if (input.Roles?.Contains("super", StringComparison.OrdinalIgnoreCase) == true && !IsSuperAdmin())
        {
            return Forbid();
        }
        var result = await userService.CreateAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<UserDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<UserDto>>> Update(
        Guid id,
        [FromBody] UpdateUserDto input,
        CancellationToken cancellationToken)
    {
        var existing = await userService.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            return NotFound(ApiResult<UserDto>.Fail("用户不存在", StatusCodes.Status404NotFound));

        if (existing.Roles.Contains("super", StringComparison.OrdinalIgnoreCase) && !IsSuperAdmin())
        {
            return Forbid();
        }

        if (input.Roles?.Contains("super", StringComparison.OrdinalIgnoreCase) == true && !IsSuperAdmin())
        {
            return Forbid();
        }

        var result = await userService.UpdateAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<UserDto>.Fail("用户不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<UserDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var existing = await userService.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            return NotFound(ApiResult.Fail("用户不存在", StatusCodes.Status404NotFound));

        if (existing.Roles.Contains("super", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(ApiResult.Fail("无法删除超级管理员账号"));
        }

        var deleted = await userService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("用户不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpPost("{id:guid}/change-password")]
    public async Task<ActionResult<ApiResult>> ChangePassword(
        Guid id,
        [FromBody] ChangePasswordDto input,
        CancellationToken cancellationToken)
    {
        var existing = await userService.GetByIdAsync(id, cancellationToken);
        if (existing != null && existing.Roles.Contains("super", StringComparison.OrdinalIgnoreCase) && !IsSuperAdmin())
        {
            return Forbid();
        }

        var success = await userService.ChangePasswordAsync(id, input, cancellationToken);
        if (!success)
            return BadRequest(ApiResult.Fail("原密码错误或用户不存在"));
        return Ok(ApiResult.Success("密码修改成功"));
    }

    [HttpPost("{id:guid}/reset-password")]
    public async Task<ActionResult<ApiResult>> ResetPassword(
        Guid id,
        [FromBody] ResetPasswordDto input,
        CancellationToken cancellationToken)
    {
        var existing = await userService.GetByIdAsync(id, cancellationToken);
        if (existing != null && existing.Roles.Contains("super", StringComparison.OrdinalIgnoreCase) && !IsSuperAdmin())
        {
            return Forbid();
        }

        var success = await userService.ResetPasswordAsync(id, input, cancellationToken);
        if (!success)
            return NotFound(ApiResult.Fail("用户不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("密码重置成功"));
    }
}