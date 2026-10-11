using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using txtOnly.Application.DTOs;
using txtOnly.Application.Interfaces;

namespace txtOnly.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("pagination")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var users = await _userService.GetUsersPagedAsync(pageNumber, pageSize);
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

        if (await _userService.GetByIdAsync(id) is null)
            return NotFound();

        return Ok(await _userService.UpdateAsync(user));
    }
}