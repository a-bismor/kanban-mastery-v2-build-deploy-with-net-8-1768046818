using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using KanbanApi.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace KanbanApi.Tests;

public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                var connection = new SqliteConnection("Data Source=:memory:");
                connection.Open();

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(connection));

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        }).CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsOk()
    {
        var response = await _client.PostAsJsonAsync("/register", new
        {
            email = $"user-{Guid.NewGuid():N}@example.com",
            password = "Test123!"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkAndAuthToken()
    {
        // Use a unique email so each test run creates a distinct user record.
        var email = $"user-{Guid.NewGuid():N}@example.com";
        const string password = "Test123!";

        // Register first: login requires an existing account.
        var registerResponse = await _client.PostAsJsonAsync("/register", new
        {
            email,
            password
        });
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync("/login", new
        {
            email,
            password
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        using var payload = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
        Assert.True(payload.RootElement.TryGetProperty("accessToken", out var tokenElement));
        Assert.False(string.IsNullOrWhiteSpace(tokenElement.GetString()));
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        var email = $"user-{Guid.NewGuid():N}@example.com";
        const string password = "Test123!";

        var firstResponse = await _client.PostAsJsonAsync("/register", new
        {
            email,
            password
        });
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);

        var duplicateResponse = await _client.PostAsJsonAsync("/register", new
        {
            email,
            password
        });

        Assert.Equal(HttpStatusCode.BadRequest, duplicateResponse.StatusCode);
    }
}
