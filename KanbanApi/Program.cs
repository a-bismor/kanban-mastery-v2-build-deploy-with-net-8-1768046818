using KanbanApi.Data;
using KanbanApi.Models;
using KanbanApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB (EF Core + SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Auth (Identity + EF storage)
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("ok"));

app.MapIdentityApi<ApplicationUser>();

app.MapGet("/api/users/me", async Task<IResult> (IUserService userService, HttpContext httpContext, CancellationToken ct) =>
{
    var profileResult = await userService.GetCurrentUserProfileAsync(httpContext.User, ct);

    return profileResult.Status switch
    {
        UserProfileLookupStatus.Unauthorized => Results.Unauthorized(),
        UserProfileLookupStatus.NotFound => Results.NotFound(),
        _ => Results.Ok(profileResult.Profile)
    };
}).RequireAuthorization();

app.Run();

public partial class Program { }
