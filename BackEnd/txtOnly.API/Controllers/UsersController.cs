using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;
using txtOnly.API.Pagination;

namespace txtOnly.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : AuthenticatedControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("pagination")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetPaged([FromQuery] PageQueryParameters parameters)
    {
        var users = await _userService.GetUsersPagedAsync(parameters.PageNumber, parameters.PageSize);
        PaginationHeader.Add(Response, users);
        return Ok(new
        {
            items = users,
            currentPage = users.CurrentPage,
            pageSize = users.PageSize,
            totalPages = users.TotalPages,
            totalCount = users.TotalCount
        });
    }

    [HttpGet("filter-by-name")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<object>> FilterByName([FromQuery] UserNameFilterParameters parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters.UserName))
            return BadRequest("O parâmetro userName é obrigatório.");

        var users = await _userService.GetUsersByNamePagedAsync(parameters.UserName, parameters.PageNumber, parameters.PageSize);
        PaginationHeader.Add(Response, users);
        return Ok(new
        {
            items = users,
            currentPage = users.CurrentPage,
            pageSize = users.PageSize,
            totalPages = users.TotalPages,
            totalCount = users.TotalCount
        });
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDTO>> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDTO>> Create([FromBody] UserCreateDTO user)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
            return Unauthorized();

        user.UserId = currentUserId;
        var createdUser = await _userService.CreateAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.UserId }, createdUser);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDTO>> Update(Guid id, [FromBody] UserDTO user)
    {
        if (id != user.UserId)
            return BadRequest("O ID da rota deve ser igual ao ID do usuário.");

        if (!TryGetCurrentUserId(out var currentUserId))
            return Unauthorized();
        if (id != currentUserId)
            return Forbid();

        if (await _userService.GetByIdAsync(id) is null)
            return NotFound();

        return Ok(await _userService.UpdateAsync(user));
    }
}