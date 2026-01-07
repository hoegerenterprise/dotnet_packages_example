namespace Users.DTOs;

public class UserGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int MemberCount { get; set; }
}

public class CreateUserGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdateUserGroupDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class AddUserToGroupDto
{
    public int UserId { get; set; }
}
