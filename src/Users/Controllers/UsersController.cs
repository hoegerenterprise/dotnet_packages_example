using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.DTOs;
using Users.Models;

namespace Users.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly DbContext _context;

    public UsersController(DbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _context.Set<User>()
            .Include(u => u.UserUserGroups)
            .ThenInclude(uug => uug.UserGroup)
            .ToListAsync();

        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            LastLoginAt = u.LastLoginAt,
            Groups = u.UserUserGroups.Select(uug => uug.UserGroup.Name).ToList()
        });

        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _context.Set<User>()
            .Include(u => u.UserUserGroups)
            .ThenInclude(uug => uug.UserGroup)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
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
            Groups = user.UserUserGroups.Select(uug => uug.UserGroup.Name).ToList()
        };

        return Ok(userDto);
    }

    [HttpPost]
    [Authorize(Roles = "Administrators")]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        // Check if username exists
        if (await _context.Set<User>().AnyAsync(u => u.Username == createUserDto.Username))
        {
            return BadRequest(new { message = "Username already exists" });
        }

        // Check if email exists
        if (await _context.Set<User>().AnyAsync(u => u.Email == createUserDto.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

        var user = new User
        {
            Username = createUserDto.Username,
            Email = createUserDto.Email,
            PasswordHash = passwordHash,
            FirstName = createUserDto.FirstName,
            LastName = createUserDto.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<User>().Add(user);
        await _context.SaveChangesAsync();

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
            Groups = new List<string>()
        };

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, userDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Administrators,Managers")]
    public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
    {
        var user = await _context.Set<User>().FindAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Update only provided fields
        if (updateUserDto.Email != null)
        {
            // Check if new email is already taken by another user
            if (await _context.Set<User>().AnyAsync(u => u.Email == updateUserDto.Email && u.Id != id))
            {
                return BadRequest(new { message = "Email already exists" });
            }
            user.Email = updateUserDto.Email;
        }

        if (updateUserDto.FirstName != null)
        {
            user.FirstName = updateUserDto.FirstName;
        }

        if (updateUserDto.LastName != null)
        {
            user.LastName = updateUserDto.LastName;
        }

        if (updateUserDto.IsActive.HasValue)
        {
            user.IsActive = updateUserDto.IsActive.Value;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrators")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Set<User>().FindAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        _context.Set<User>().Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
