using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;

namespace txtOnly.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LikesController : AuthenticatedControllerBase
{
    private readonly ILikeService _likeService;

    public LikesController(ILikeService likeService)
    {
        _likeService = likeService;
    }

    [HttpGet("post/{postId:guid}/pagination")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetPaged(Guid postId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var likes = await _likeService.GetLikesPagedAsync(postId, pageNumber, pageSize);
        PaginationHeader.Add(Response, likes);
        return Ok(new
        {
            items = likes,
            currentPage = likes.CurrentPage,
            pageSize = likes.PageSize,
            totalPages = likes.TotalPages,
            totalCount = likes.TotalCount
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(LikeDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LikeDTO>> Create([FromBody] LikeCreateDTO like)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Unauthorized();

        like.UserId = currentUserId;
        var createdLike = await _likeService.CreateAsync(like);
        return StatusCode(StatusCodes.Status201Created, createdLike);
    }

    [HttpDelete("user/{userId:guid}/post/{postId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid userId, Guid postId)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Unauthorized();
        if (userId != currentUserId)
            return Forbid();

        await _likeService.RemoveAsync(new LikeDTO { UserId = userId, PostId = postId });
        return NoContent();
    }
}