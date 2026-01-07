using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.DTOs;
using Users.Models;
using Users.Services;
using BCrypt.Net;

namespace Users.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DbContext _context;
    private readonly JwtService _jwtService;

    public AuthController(DbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
    {
        var user = await _context.Set<User>()
            .Include(u => u.UserUserGroups)
            .ThenInclude(uug => uug.UserGroup)
            .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { message = "Account is inactive" });
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Get user groups
        var groups = user.UserUserGroups.Select(uug => uug.UserGroup.Name).ToList();

        // Generate JWT token
        var token = _jwtService.GenerateToken(user, groups);

        var response = new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Groups = groups,
            ExpiresAt = _jwtService.GetTokenExpiration()
        };

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        // Check if username exists
        if (await _context.Set<User>().AnyAsync(u => u.Username == registerDto.Username))
        {
            return BadRequest(new { message = "Username already exists" });
        }

        // Check if email exists
        if (await _context.Set<User>().AnyAsync(u => u.Email == registerDto.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<User>().Add(user);
        await _context.SaveChangesAsync();

        // Automatically add to "Users" group (ID 3)
        var usersGroup = await _context.Set<UserGroup>().FindAsync(3);
        if (usersGroup != null)
        {
            var userUserGroup = new UserUserGroup
            {
                UserId = user.Id,
                UserGroupId = usersGroup.Id,
                JoinedAt = DateTime.UtcNow
            };
            _context.Set<UserUserGroup>().Add(userUserGroup);
            await _context.SaveChangesAsync();
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Groups = usersGroup != null ? new List<string> { usersGroup.Name } : new List<string>()
        };

        return CreatedAtAction(nameof(Login), new { username = user.Username }, userDto);
    }
}
