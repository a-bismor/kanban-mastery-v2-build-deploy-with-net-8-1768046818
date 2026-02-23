using KanbanApi.Data;
using KanbanApi.Models;
using KanbanApi.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

namespace KanbanApi.Tests;

public class UserServiceTests
{
    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(CreateOpenConnection())
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static SqliteConnection CreateOpenConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        return connection;
    }

    [Fact]
    public async Task GetUserProfileAsync_returns_profile_when_user_exists()
    {
        await using var db = CreateDbContext();
        db.Users.Add(new ApplicationUser
        {
            Id = "user-1",
            UserName = "user1@example.com",
            Email = "user1@example.com"
        });
        await db.SaveChangesAsync();

        IUserService service = new UserService(db);

        var profile = await service.GetUserProfileAsync("user-1");

        Assert.NotNull(profile);
        Assert.Equal("user-1", profile.Id);
        Assert.Equal("user1@example.com", profile.UserName);
        Assert.Equal("user1@example.com", profile.Email);
    }

    [Fact]
    public async Task GetUserProfileAsync_returns_null_when_user_does_not_exist()
    {
        await using var db = CreateDbContext();
        IUserService service = new UserService(db);

        var profile = await service.GetUserProfileAsync("missing-user");

        Assert.Null(profile);
    }

    [Fact]
    public async Task GetCurrentUserProfileAsync_returns_unauthorized_when_nameidentifier_claim_missing()
    {
        await using var db = CreateDbContext();
        IUserService service = new UserService(db);
        var user = new ClaimsPrincipal(new ClaimsIdentity());

        var result = await service.GetCurrentUserProfileAsync(user);

        Assert.Equal(UserProfileLookupStatus.Unauthorized, result.Status);
        Assert.Null(result.Profile);
    }

    [Fact]
    public async Task GetCurrentUserProfileAsync_returns_not_found_when_user_missing_from_database()
    {
        await using var db = CreateDbContext();
        IUserService service = new UserService(db);
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "missing-user")
        ]));

        var result = await service.GetCurrentUserProfileAsync(user);

        Assert.Equal(UserProfileLookupStatus.NotFound, result.Status);
        Assert.Null(result.Profile);
    }

    [Fact]
    public async Task GetCurrentUserProfileAsync_returns_ok_with_profile_when_user_exists()
    {
        await using var db = CreateDbContext();
        db.Users.Add(new ApplicationUser
        {
            Id = "user-2",
            UserName = "user2@example.com",
            Email = "user2@example.com"
        });
        await db.SaveChangesAsync();

        IUserService service = new UserService(db);
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, "user-2")
        ]));

        var result = await service.GetCurrentUserProfileAsync(user);

        Assert.Equal(UserProfileLookupStatus.Ok, result.Status);
        Assert.NotNull(result.Profile);
        Assert.Equal("user-2", result.Profile.Id);
        Assert.Equal("user2@example.com", result.Profile.UserName);
        Assert.Equal("user2@example.com", result.Profile.Email);
    }
}
