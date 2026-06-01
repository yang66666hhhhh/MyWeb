using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Network.Dtos;
using WebApplication1.Features.Network.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Network.Controllers;

[ApiController]
[Authorize]
[RequireFeature("GROWTH_KNOWLEDGE")]
[Route("api/network")]
[Tags("Network")]
public class NetworkController(INetworkService networkService, ILogger<NetworkController> logger) : BaseApiController
{
    #region Contacts

    [HttpGet("contacts")]
    public async Task<ActionResult<ApiResult<PageResult<ContactDto>>>> GetContactPage(
        [FromQuery] NetworkQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await networkService.GetContactPageAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<ContactDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取联系人列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("contacts/{id:guid}")]
    public async Task<ActionResult<ApiResult<ContactDto>>> GetContactById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await networkService.GetContactByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<ContactDto>.Fail("联系人不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
                return NotFound(ApiResult<ContactDto>.Fail("联系人不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<ContactDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取联系人详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost("contacts")]
    public async Task<ActionResult<ApiResult<ContactDto>>> CreateContact(
        [FromBody] CreateContactDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await networkService.CreateContactAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建联系人成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetContactById), new { id = result.Id }, ApiResult<ContactDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建联系人失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("contacts/{id:guid}")]
    public async Task<ActionResult<ApiResult<ContactDto>>> UpdateContact(
        Guid id,
        [FromBody] UpdateContactDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await networkService.GetContactByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult<ContactDto>.Fail("联系人不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此联系人"));

            var result = await networkService.UpdateContactAsync(id, input, cancellationToken);
            if (result is null)
                return NotFound(ApiResult.Fail("联系人不存在"));
            logger.LogInformation("更新联系人成功: {Id}", id);
            return Ok(ApiResult<ContactDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新联系人失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("contacts/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await networkService.GetContactByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("联系人不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此联系人"));

            var deleted = await networkService.DeleteContactAsync(id, cancellationToken);
            logger.LogInformation("删除联系人成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除联系人失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    #endregion

    #region Interactions

    [HttpGet("contacts/{contactId:guid}/interactions")]
    public async Task<ActionResult<ApiResult<PageResult<InteractionDto>>>> GetInteractionPage(
        Guid contactId,
        [FromQuery] NetworkQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await networkService.GetInteractionPageAsync(contactId, query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<InteractionDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取互动记录列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("interactions/{id:guid}")]
    public async Task<ActionResult<ApiResult<InteractionDto>>> GetInteractionById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await networkService.GetInteractionByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<InteractionDto>.Fail("互动记录不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
                return NotFound(ApiResult<InteractionDto>.Fail("互动记录不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<InteractionDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取互动记录详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost("interactions")]
    public async Task<ActionResult<ApiResult<InteractionDto>>> CreateInteraction(
        [FromBody] CreateInteractionDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await networkService.CreateInteractionAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建互动记录成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetInteractionById), new { id = result.Id }, ApiResult<InteractionDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建互动记录失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("interactions/{id:guid}")]
    public async Task<ActionResult<ApiResult<InteractionDto>>> UpdateInteraction(
        Guid id,
        [FromBody] UpdateInteractionDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await networkService.GetInteractionByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult<InteractionDto>.Fail("互动记录不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此互动记录"));

            var result = await networkService.UpdateInteractionAsync(id, input, cancellationToken);
            if (result is null)
                return NotFound(ApiResult.Fail("互动记录不存在"));
            logger.LogInformation("更新互动记录成功: {Id}", id);
            return Ok(ApiResult<InteractionDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新互动记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("interactions/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteInteraction(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await networkService.GetInteractionByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("互动记录不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此互动记录"));

            var deleted = await networkService.DeleteInteractionAsync(id, cancellationToken);
            logger.LogInformation("删除互动记录成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除互动记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    #endregion

    #region Tags

    [HttpGet("tags")]
    public async Task<ActionResult<ApiResult<List<TagDto>>>> GetContactTags(CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await networkService.GetContactTagsAsync(userId, cancellationToken);
        return Ok(ApiResult<List<TagDto>>.Success(result));
    }

    #endregion
}
