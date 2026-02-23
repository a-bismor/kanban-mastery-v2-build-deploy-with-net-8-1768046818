using KanbanApi.Data;
using System.Security.Claims;

namespace KanbanApi.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _db;

    public UserService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<UserProfile?> GetUserProfileAsync(string userId, CancellationToken ct = default)
    {
        var user = await _db.Users.FindAsync([userId], ct);
        if (user is null)
        {
            return null;
        }

        return new UserProfile(user.Id, user.UserName, user.Email);
    }

    public async Task<UserProfileLookupResult> GetCurrentUserProfileAsync(ClaimsPrincipal user, CancellationToken ct = default)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return UserProfileLookupResult.Unauthorized();
        }

        var profile = await GetUserProfileAsync(userId, ct);
        if (profile is null)
        {
            return UserProfileLookupResult.NotFound();
        }

        return UserProfileLookupResult.Ok(profile);
    }
}
