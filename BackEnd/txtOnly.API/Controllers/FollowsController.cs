using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;

namespace txtOnly.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FollowsController : ControllerBase
{
    private readonly IFollowService _followService;

    public FollowsController(IFollowService followService)
    {
        _followService = followService;
    }

    [HttpGet("user/{userId:guid}/followers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetFollowers(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var follows = await _followService.GetFollowersPagedAsync(userId, pageNumber, pageSize);
        PaginationHeader.Add(Response, follows);
        return Ok(new
        {
            items = follows,
            currentPage = follows.CurrentPage,
            pageSize = follows.PageSize,
            totalPages = follows.TotalPages,
            totalCount = follows.TotalCount
        });
    }

    [HttpGet("user/{userId:guid}/following")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetFollowing(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var follows = await _followService.GetFollowingPagedAsync(userId, pageNumber, pageSize);
        PaginationHeader.Add(Response, follows);
        return Ok(new
        {
            items = follows,
            currentPage = follows.CurrentPage,
            pageSize = follows.PageSize,
            totalPages = follows.TotalPages,
            totalCount = follows.TotalCount
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(FollowDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FollowDTO>> Create([FromBody] FollowCreateDTO follow)
    {
        var createdFollow = await _followService.CreateAsync(follow);
        return StatusCode(StatusCodes.Status201Created, createdFollow);
    }

    [HttpDelete("follower/{followerId:guid}/following/{followingId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid followerId, Guid followingId)
    {
        await _followService.RemoveAsync(new FollowDTO
        {
            FollowerId = followerId,
            FollowingId = followingId
        });
        return NoContent();
    }
}