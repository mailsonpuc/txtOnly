using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using txtOnly.Infrastructure.Identity;
using txtOnly.Infrastructure.Identity.Interfaces;
using txtOnly.Infrastructure.Identity.Models;

namespace txtOnly.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ResponseDTO
            {
                Status = "Error",
                Message = "Username, email and password are required."
            });
        }

        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            return Conflict(new ResponseDTO
            {
                Status = "Error",
                Message = "Email already exists."
            });
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new ResponseDTO
            {
                Status = "Error",
                Message = string.Join("; ", result.Errors.Select(error => error.Description))
            });
        }

        return Ok(new ResponseDTO { Status = "Success", Message = "User created successfully." });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [EnableRateLimiting("loginRateLimit")]
    [ProducesResponseType(typeof(TokenDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDTO request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ResponseDTO
            {
                Status = "Error",
                Message = "Email and password are required."
            });
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new ResponseDTO
            {
                Status = "Error",
                Message = "Invalid email or password."
            });
        }

        return Ok(await _tokenService.CreateTokenAsync(user));
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(TokenDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDTO? tokenModel)
    {
        if (string.IsNullOrWhiteSpace(tokenModel?.RefreshToken))
            return BadRequest("Invalid client request.");

        var token = await _tokenService.RefreshTokenAsync(tokenModel.RefreshToken);
        return token is null ? BadRequest("Invalid refresh token.") : Ok(token);
    }

    [Authorize(Roles = "admin")]
    [HttpPost("roles")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRole([FromQuery] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Role name is required." });

        if (await _roleManager.RoleExistsAsync(roleName))
            return Conflict(new ResponseDTO { Status = "Error", Message = "Role already exists." });

        var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        if (!result.Succeeded)
        {
            _logger.LogWarning("Unable to create role {RoleName}.", roleName);
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Unable to create role." });
        }

        _logger.LogInformation("Role {RoleName} created.", roleName);
        return Ok(new ResponseDTO { Status = "Success", Message = $"Role {roleName} created successfully." });
    }

    [Authorize(Roles = "admin")]
    [HttpPost("roles/users")]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseDTO), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddUserToRole([FromQuery] string email, [FromQuery] string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Unable to find user." });

        if (!await _roleManager.RoleExistsAsync(roleName))
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Role does not exist." });

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Unable to add user {Email} to role {RoleName}.", email, roleName);
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Unable to add user to role." });
        }

        return Ok(new ResponseDTO { Status = "Success", Message = $"User {email} added to role {roleName}." });
    }

    [Authorize(Roles = "admin")]
    [HttpPost("revoke/{username}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
            return NotFound(new ResponseDTO { Status = "Error", Message = "User not found." });

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new ResponseDTO { Status = "Error", Message = "Unable to revoke refresh token." });

        return NoContent();
    }
}