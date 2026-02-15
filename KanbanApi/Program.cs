using KanbanApi.Data;
using KanbanApi.Models;
using KanbanApi.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// DB (EF Core + SQLite)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Auth (Identity + EF storage)
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization();
builder.Services.AddScoped<IBoardService, BoardService>();

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

app.MapGet("/api/users/me", async Task<IResult> (ClaimsPrincipal user, ApplicationDbContext db) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
    if (string.IsNullOrWhiteSpace(userId))
    {
        return Results.Unauthorized();
    }

    var appUser = await db.Users.FindAsync(userId);
    if (appUser is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new { appUser.Id, appUser.UserName, appUser.Email });
}).RequireAuthorization();

app.Run();

public partial class Program { }
