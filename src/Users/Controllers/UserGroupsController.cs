using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.DTOs;
using Users.Models;

namespace Users.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Administrators")]
public class UserGroupsController : ControllerBase
{
    private readonly DbContext _context;

    public UserGroupsController(DbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UserGroupDto>>> GetUserGroups()
    {
        var groups = await _context.Set<UserGroup>()
            .Include(g => g.UserUserGroups)
            .ToListAsync();

        var groupDtos = groups.Select(g => new UserGroupDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            CreatedAt = g.CreatedAt,
            MemberCount = g.UserUserGroups.Count
        });

        return Ok(groupDtos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserGroupDto>> GetUserGroup(int id)
    {
        var group = await _context.Set<UserGroup>()
            .Include(g => g.UserUserGroups)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
        {
            return NotFound(new { message = "User group not found" });
        }

        var groupDto = new UserGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            CreatedAt = group.CreatedAt,
            MemberCount = group.UserUserGroups.Count
        };

        return Ok(groupDto);
    }

    [HttpPost]
    public async Task<ActionResult<UserGroupDto>> CreateUserGroup(CreateUserGroupDto createGroupDto)
    {
        // Check if group name exists
        if (await _context.Set<UserGroup>().AnyAsync(g => g.Name == createGroupDto.Name))
        {
            return BadRequest(new { message = "Group name already exists" });
        }

        var group = new UserGroup
        {
            Name = createGroupDto.Name,
            Description = createGroupDto.Description,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<UserGroup>().Add(group);
        await _context.SaveChangesAsync();

        var groupDto = new UserGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            CreatedAt = group.CreatedAt,
            MemberCount = 0
        };

        return CreatedAtAction(nameof(GetUserGroup), new { id = group.Id }, groupDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserGroup(int id, UpdateUserGroupDto updateGroupDto)
    {
        var group = await _context.Set<UserGroup>().FindAsync(id);

        if (group == null)
        {
            return NotFound(new { message = "User group not found" });
        }

        // Update only provided fields
        if (updateGroupDto.Name != null)
        {
            // Check if new name is already taken by another group
            if (await _context.Set<UserGroup>().AnyAsync(g => g.Name == updateGroupDto.Name && g.Id != id))
            {
                return BadRequest(new { message = "Group name already exists" });
            }
            group.Name = updateGroupDto.Name;
        }

        if (updateGroupDto.Description != null)
        {
            group.Description = updateGroupDto.Description;
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserGroup(int id)
    {
        var group = await _context.Set<UserGroup>().FindAsync(id);

        if (group == null)
        {
            return NotFound(new { message = "User group not found" });
        }

        _context.Set<UserGroup>().Remove(group);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{groupId}/users")]
    public async Task<IActionResult> AddUserToGroup(int groupId, AddUserToGroupDto addUserDto)
    {
        var group = await _context.Set<UserGroup>().FindAsync(groupId);
        if (group == null)
        {
            return NotFound(new { message = "User group not found" });
        }

        var user = await _context.Set<User>().FindAsync(addUserDto.UserId);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Check if user is already in the group
        var existingMembership = await _context.Set<UserUserGroup>()
            .FirstOrDefaultAsync(uug => uug.UserId == addUserDto.UserId && uug.UserGroupId == groupId);

        if (existingMembership != null)
        {
            return BadRequest(new { message = "User is already a member of this group" });
        }

        var userUserGroup = new UserUserGroup
        {
            UserId = addUserDto.UserId,
            UserGroupId = groupId,
            JoinedAt = DateTime.UtcNow
        };

        _context.Set<UserUserGroup>().Add(userUserGroup);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{groupId}/users/{userId}")]
    public async Task<IActionResult> RemoveUserFromGroup(int groupId, int userId)
    {
        var userUserGroup = await _context.Set<UserUserGroup>()
            .FirstOrDefaultAsync(uug => uug.UserId == userId && uug.UserGroupId == groupId);

        if (userUserGroup == null)
        {
            return NotFound(new { message = "User is not a member of this group" });
        }

        _context.Set<UserUserGroup>().Remove(userUserGroup);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{groupId}/users")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetGroupMembers(int groupId)
    {
        var group = await _context.Set<UserGroup>()
            .Include(g => g.UserUserGroups)
            .ThenInclude(uug => uug.User)
            .ThenInclude(u => u.UserUserGroups)
            .ThenInclude(uug => uug.UserGroup)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null)
        {
            return NotFound(new { message = "User group not found" });
        }

        var userDtos = group.UserUserGroups.Select(uug => new UserDto
        {
            Id = uug.User.Id,
            Username = uug.User.Username,
            Email = uug.User.Email,
            FirstName = uug.User.FirstName,
            LastName = uug.User.LastName,
            IsActive = uug.User.IsActive,
            CreatedAt = uug.User.CreatedAt,
            LastLoginAt = uug.User.LastLoginAt,
            Groups = uug.User.UserUserGroups.Select(u => u.UserGroup.Name).ToList()
        });

        return Ok(userDtos);
    }
}
