using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using KanbanApi.Data;
using KanbanApi.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace KanbanApi.Tests;

public class MeEndpointServiceMappingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MeEndpointServiceMappingTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Me_WhenServiceReturnsUnauthorized_ReturnsUnauthorized()
    {
        using var client = CreateClientWithFakeUserService(UserProfileLookupResult.Unauthorized());
        var token = await RegisterAndLoginAsync(client);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WhenServiceReturnsNotFound_ReturnsNotFound()
    {
        using var client = CreateClientWithFakeUserService(UserProfileLookupResult.NotFound());
        var token = await RegisterAndLoginAsync(client);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Me_WhenServiceReturnsProfile_ReturnsOkAndProfilePayload()
    {
        var profile = new UserProfile("fake-id", "fake-user@example.com", "fake-user@example.com");
        using var client = CreateClientWithFakeUserService(UserProfileLookupResult.Ok(profile));
        var token = await RegisterAndLoginAsync(client);

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/users/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("fake-id", payload.RootElement.GetProperty("id").GetString());
        Assert.Equal("fake-user@example.com", payload.RootElement.GetProperty("userName").GetString());
        Assert.Equal("fake-user@example.com", payload.RootElement.GetProperty("email").GetString());
    }

    private HttpClient CreateClientWithFakeUserService(UserProfileLookupResult result)
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbOptionsDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (dbOptionsDescriptor != null)
                {
                    services.Remove(dbOptionsDescriptor);
                }

                var userServiceDescriptors = services
                    .Where(d => d.ServiceType == typeof(IUserService))
                    .ToList();

                foreach (var descriptor in userServiceDescriptors)
                {
                    services.Remove(descriptor);
                }

                var connection = new SqliteConnection("Data Source=:memory:");
                connection.Open();

                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(connection));

                services.AddScoped<IUserService>(_ => new FakeUserService(result));

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();
            });
        }).CreateClient();
    }

    private static async Task<string> RegisterAndLoginAsync(HttpClient client)
    {
        var email = $"user-{Guid.NewGuid():N}@example.com";
        const string password = "Test123!";

        var registerResponse = await client.PostAsJsonAsync("/register", new
        {
            email,
            password
        });
        Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

        var loginResponse = await client.PostAsJsonAsync("/login", new
        {
            email,
            password
        });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        using var loginPayload = JsonDocument.Parse(await loginResponse.Content.ReadAsStringAsync());
        var token = loginPayload.RootElement.GetProperty("accessToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(token));
        return token!;
    }

    private sealed class FakeUserService : IUserService
    {
        private readonly UserProfileLookupResult _result;

        public FakeUserService(UserProfileLookupResult result)
        {
            _result = result;
        }

        public Task<UserProfile?> GetUserProfileAsync(string userId, CancellationToken ct = default)
        {
            return Task.FromResult(_result.Profile);
        }

        public Task<UserProfileLookupResult> GetCurrentUserProfileAsync(System.Security.Claims.ClaimsPrincipal user, CancellationToken ct = default)
        {
            return Task.FromResult(_result);
        }
    }
}
