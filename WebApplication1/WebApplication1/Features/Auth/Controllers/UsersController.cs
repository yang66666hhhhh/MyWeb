using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth;
using WebApplication1.Features.Auth.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Authorize]
[Route("api/system/users")]
[Authorize(Roles = "pro,owner")]
[Tags("Users")]
public class UsersController(IUserService userService, ILogger<UsersController> logger) : ControllerBase
{
    private bool IsOwner() =>
        User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Any(c => c.Value.Equals("owner", StringComparison.OrdinalIgnoreCase));

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
        try
        {
            if (input.Roles?.Contains("owner", StringComparison.OrdinalIgnoreCase) == true && !IsOwner())
            {
                return Forbid();
            }
            var result = await userService.CreateAsync(input, cancellationToken);
            logger.LogInformation("创建用户成功: {Id}, {Username}", result.Id, result.Username);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<UserDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建用户失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<UserDto>>> Update(
        Guid id,
        [FromBody] UpdateUserDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await userService.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                return NotFound(ApiResult<UserDto>.Fail("用户不存在", StatusCodes.Status404NotFound));

            if (existing.Roles.Contains("owner", StringComparison.OrdinalIgnoreCase) && !IsOwner())
            {
                return Forbid();
            }

            if (input.Roles?.Contains("owner", StringComparison.OrdinalIgnoreCase) == true && !IsOwner())
            {
                return Forbid();
            }

            var result = await userService.UpdateAsync(id, input, cancellationToken);
            if (result == null)
                return NotFound(ApiResult<UserDto>.Fail("用户不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新用户成功: {Id}", id);
            return Ok(ApiResult<UserDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新用户失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await userService.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                return NotFound(ApiResult.Fail("用户不存在", StatusCodes.Status404NotFound));

            if (existing.Roles.Contains("owner", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(ApiResult.Fail("无法删除平台所有者账号"));
            }

            var deleted = await userService.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound(ApiResult.Fail("用户不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除用户成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除用户失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpPost("{id:guid}/change-password")]
    public async Task<ActionResult<ApiResult>> ChangePassword(
        Guid id,
        [FromBody] ChangePasswordDto input,
        CancellationToken cancellationToken)
    {
        var existing = await userService.GetByIdAsync(id, cancellationToken);
        if (existing != null && existing.Roles.Contains("owner", StringComparison.OrdinalIgnoreCase) && !IsOwner())
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
        if (existing != null && existing.Roles.Contains("owner", StringComparison.OrdinalIgnoreCase) && !IsOwner())
        {
            return Forbid();
        }

        var success = await userService.ResetPasswordAsync(id, input, cancellationToken);
        if (!success)
            return NotFound(ApiResult.Fail("用户不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("密码重置成功"));
    }
}
