using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Network.Dtos;
using WebApplication1.Features.Network.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Network.Controllers;

[ApiController]
[Authorize]
[Route("api/network")]
[Tags("Network")]
public class NetworkController(INetworkService networkService) : BaseApiController
{
    #region Contacts

    [HttpGet("contacts")]
    public async Task<ActionResult<ApiResult<PageResult<ContactDto>>>> GetContactPage(
        [FromQuery] NetworkQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await networkService.GetContactPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<ContactDto>>.Success(result));
    }

    [HttpGet("contacts/{id:guid}")]
    public async Task<ActionResult<ApiResult<ContactDto>>> GetContactById(Guid id, CancellationToken cancellationToken)
    {
        var result = await networkService.GetContactByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ContactDto>.Fail("联系人不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<ContactDto>.Fail("联系人不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<ContactDto>.Success(result));
    }

    [HttpPost("contacts")]
    public async Task<ActionResult<ApiResult<ContactDto>>> CreateContact(
        [FromBody] CreateContactDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await networkService.CreateContactAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetContactById), new { id = result.Id }, ApiResult<ContactDto>.Success(result, "创建成功"));
    }

    [HttpPut("contacts/{id:guid}")]
    public async Task<ActionResult<ApiResult<ContactDto>>> UpdateContact(
        Guid id,
        [FromBody] UpdateContactDto input,
        CancellationToken cancellationToken)
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
        return Ok(ApiResult<ContactDto>.Success(result, "更新成功"));
    }

    [HttpDelete("contacts/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteContact(Guid id, CancellationToken cancellationToken)
    {
        var existing = await networkService.GetContactByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult.Fail("联系人不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此联系人"));

        var deleted = await networkService.DeleteContactAsync(id, cancellationToken);
        return Ok(ApiResult.Success("删除成功"));
    }

    #endregion

    #region Interactions

    [HttpGet("contacts/{contactId:guid}/interactions")]
    public async Task<ActionResult<ApiResult<PageResult<InteractionDto>>>> GetInteractionPage(
        Guid contactId,
        [FromQuery] NetworkQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await networkService.GetInteractionPageAsync(contactId, query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<InteractionDto>>.Success(result));
    }

    [HttpGet("interactions/{id:guid}")]
    public async Task<ActionResult<ApiResult<InteractionDto>>> GetInteractionById(Guid id, CancellationToken cancellationToken)
    {
        var result = await networkService.GetInteractionByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<InteractionDto>.Fail("互动记录不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<InteractionDto>.Fail("互动记录不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<InteractionDto>.Success(result));
    }

    [HttpPost("interactions")]
    public async Task<ActionResult<ApiResult<InteractionDto>>> CreateInteraction(
        [FromBody] CreateInteractionDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await networkService.CreateInteractionAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetInteractionById), new { id = result.Id }, ApiResult<InteractionDto>.Success(result, "创建成功"));
    }

    [HttpPut("interactions/{id:guid}")]
    public async Task<ActionResult<ApiResult<InteractionDto>>> UpdateInteraction(
        Guid id,
        [FromBody] UpdateInteractionDto input,
        CancellationToken cancellationToken)
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
        return Ok(ApiResult<InteractionDto>.Success(result, "更新成功"));
    }

    [HttpDelete("interactions/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteInteraction(Guid id, CancellationToken cancellationToken)
    {
        var existing = await networkService.GetInteractionByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult.Fail("互动记录不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此互动记录"));

        var deleted = await networkService.DeleteInteractionAsync(id, cancellationToken);
        return Ok(ApiResult.Success("删除成功"));
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
