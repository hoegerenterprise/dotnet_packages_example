using Microsoft.EntityFrameworkCore;
using Users.Models;

namespace Users.Data;

public static class UsersDbInitializer
{
    public static void AddUsersData(ModelBuilder modelBuilder)
    {
        // Configure many-to-many relationship
        modelBuilder.Entity<UserUserGroup>()
            .HasKey(uug => new { uug.UserId, uug.UserGroupId });

        modelBuilder.Entity<UserUserGroup>()
            .HasOne(uug => uug.User)
            .WithMany(u => u.UserUserGroups)
            .HasForeignKey(uug => uug.UserId);

        modelBuilder.Entity<UserUserGroup>()
            .HasOne(uug => uug.UserGroup)
            .WithMany(ug => ug.UserUserGroups)
            .HasForeignKey(uug => uug.UserGroupId);

        // Seed UserGroups
        modelBuilder.Entity<UserGroup>().HasData(
            new UserGroup
            {
                Id = 1,
                Name = "Administrators",
                Description = "System administrators with full access",
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new UserGroup
            {
                Id = 2,
                Name = "Managers",
                Description = "Management team members",
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new UserGroup
            {
                Id = 3,
                Name = "Users",
                Description = "Regular users",
                CreatedAt = new DateTime(2024, 1, 1)
            }
        );

        // Seed Users (passwords are "Password123!" hashed with BCrypt)
        // Note: These hashes are pre-computed for consistency
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "$2a$11$x4QY8P3gGZKQb5.3rJkO0ueP8RzRVP8qQX1qQ5QZH3zY9VkK4YQKm", // Password123!
                FirstName = "Admin",
                LastName = "User",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new User
            {
                Id = 2,
                Username = "manager",
                Email = "manager@example.com",
                PasswordHash = "$2a$11$x4QY8P3gGZKQb5.3rJkO0ueP8RzRVP8qQX1qQ5QZH3zY9VkK4YQKm", // Password123!
                FirstName = "Manager",
                LastName = "Smith",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 5)
            },
            new User
            {
                Id = 3,
                Username = "user",
                Email = "user@example.com",
                PasswordHash = "$2a$11$x4QY8P3gGZKQb5.3rJkO0ueP8RzRVP8qQX1qQ5QZH3zY9VkK4YQKm", // Password123!
                FirstName = "Regular",
                LastName = "User",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 10)
            }
        );

        // Seed UserUserGroups (assign users to groups)
        modelBuilder.Entity<UserUserGroup>().HasData(
            new UserUserGroup { UserId = 1, UserGroupId = 1, JoinedAt = new DateTime(2024, 1, 1) }, // admin -> Administrators
            new UserUserGroup { UserId = 2, UserGroupId = 2, JoinedAt = new DateTime(2024, 1, 5) }, // manager -> Managers
            new UserUserGroup { UserId = 2, UserGroupId = 3, JoinedAt = new DateTime(2024, 1, 5) }, // manager -> Users
            new UserUserGroup { UserId = 3, UserGroupId = 3, JoinedAt = new DateTime(2024, 1, 10) }  // user -> Users
        );
    }
}
