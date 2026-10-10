using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.Domain.Validation;

namespace txtOnly.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LikesController : ControllerBase
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
        try
        {
            var createdLike = await _likeService.CreateAsync(like);
            return StatusCode(StatusCodes.Status201Created, createdLike);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (DomainExceptionValidation exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }

    [HttpDelete("user/{userId:guid}/post/{postId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid userId, Guid postId)
    {
        try
        {
            await _likeService.RemoveAsync(new LikeDTO { UserId = userId, PostId = postId });
            return NoContent();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
    }
}