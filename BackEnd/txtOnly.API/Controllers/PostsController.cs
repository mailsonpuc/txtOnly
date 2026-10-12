using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.API.Pagination;

namespace txtOnly.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PostsController : AuthenticatedControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PostDTO>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PostDTO>>> GetAll()
    {
        var posts = await _postService.GetPostsAsync();
        return Ok(posts);
    }

    [HttpGet("pagination")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetPaged([FromQuery] PageQueryParameters parameters)
    {
        var posts = await _postService.GetPostsPagedAsync(parameters.PageNumber, parameters.PageSize);
        PaginationHeader.Add(Response, posts);
        return Ok(new
        {
            items = posts,
            currentPage = posts.CurrentPage,
            pageSize = posts.PageSize,
            totalPages = posts.TotalPages,
            totalCount = posts.TotalCount
        });
    }

    [HttpGet("user/{userId:guid}/pagination")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetByUserPaged(Guid userId, [FromQuery] PageQueryParameters parameters)
    {
        var posts = await _postService.GetPostsByUserPagedAsync(userId, parameters.PageNumber, parameters.PageSize);
        PaginationHeader.Add(Response, posts);
        return Ok(new
        {
            items = posts,
            currentPage = posts.CurrentPage,
            pageSize = posts.PageSize,
            totalPages = posts.TotalPages,
            totalCount = posts.TotalCount
        });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PostDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDTO>> GetById(Guid id)
    {
        var post = await _postService.GetByIdAsync(id);
        return post is null ? NotFound() : Ok(post);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDTO>> Create([FromBody] PostCreateDTO post)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Unauthorized();

        post.UserId = currentUserId;
        var createdPost = await _postService.CreateAsync(post);
        return CreatedAtAction(nameof(GetById), new { id = createdPost.PostId }, createdPost);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PostDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDTO>> Update(Guid id, [FromBody] PostDTO post)
    {
        if (id != post.PostId)
            return BadRequest("O ID da rota deve ser igual ao ID da publicação.");

        if (!TryGetCurrentUserId(out var currentUserId))
            return Unauthorized();

        var existingPost = await _postService.GetByIdAsync(id);
        if (existingPost is null)
            return NotFound();
        if (existingPost.UserId != currentUserId)
            return Forbid();

        post.UserId = currentUserId;
        return Ok(await _postService.UpdateAsync(post));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Unauthorized();

        var post = await _postService.GetByIdAsync(id);
        if (post is null)
            return NotFound();
        if (post.UserId != currentUserId)
            return Forbid();

        await _postService.RemoveAsync(post);
        return NoContent();
    }

    [HttpPatch("{id:guid}/restore")]
    [ProducesResponseType(typeof(PostDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostDTO>> Restore(Guid id)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Unauthorized();

        var post = await _postService.GetByIdAsync(id);
        if (post is null)
            return NotFound();
        if (post.UserId != currentUserId)
            return Forbid();

        return Ok(await _postService.RestoreAsync(id));
    }
}