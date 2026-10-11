using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;

namespace txtOnly.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentsController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("post/{postId:guid}/pagination")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetPaged(Guid postId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var comments = await _commentService.GetCommentsPagedAsync(postId, pageNumber, pageSize);
        PaginationHeader.Add(Response, comments);
        return Ok(new
        {
            items = comments,
            currentPage = comments.CurrentPage,
            pageSize = comments.PageSize,
            totalPages = comments.TotalPages,
            totalCount = comments.TotalCount
        });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDTO>> GetById(Guid id)
    {
        var comment = await _commentService.GetByIdAsync(id);
        return comment is null ? NotFound() : Ok(comment);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDTO>> Create([FromBody] CommentCreateDTO comment)
    {
        var createdComment = await _commentService.CreateAsync(comment);
        return CreatedAtAction(nameof(GetById), new { id = createdComment.CommentId }, createdComment);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CommentDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDTO>> Update(Guid id, [FromBody] CommentDTO comment)
    {
        if (id != comment.CommentId)
            return BadRequest("O ID da rota deve ser igual ao ID do comentário.");
        if (await _commentService.GetByIdAsync(id) is null)
            return NotFound();

        return Ok(await _commentService.UpdateAsync(comment));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var comment = await _commentService.GetByIdAsync(id);
        if (comment is null)
            return NotFound();

        await _commentService.RemoveAsync(comment);
        return NoContent();
    }
}