using System.Security.Claims;

namespace KanbanApi.Services;

public interface IUserService
{
    Task<UserProfile?> GetUserProfileAsync(string userId, CancellationToken ct = default);
    Task<UserProfileLookupResult> GetCurrentUserProfileAsync(ClaimsPrincipal user, CancellationToken ct = default);
}

public sealed record UserProfile(string Id, string? UserName, string? Email);

public enum UserProfileLookupStatus
{
    Ok,
    Unauthorized,
    NotFound
}

public sealed record UserProfileLookupResult(UserProfileLookupStatus Status, UserProfile? Profile)
{
    public static UserProfileLookupResult Ok(UserProfile profile) =>
        new(UserProfileLookupStatus.Ok, profile);

    public static UserProfileLookupResult Unauthorized() =>
        new(UserProfileLookupStatus.Unauthorized, null);

    public static UserProfileLookupResult NotFound() =>
        new(UserProfileLookupStatus.NotFound, null);
}
