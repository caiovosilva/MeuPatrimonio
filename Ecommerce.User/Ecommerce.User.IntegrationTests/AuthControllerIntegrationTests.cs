using Bogus;
using ecommerce_user;
using ecommerce_user.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Testcontainers.PostgreSql;
using Xunit;

namespace ecommerce.User.IntegrationTests;

public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private HttpClient _client;
    private readonly Faker _faker;

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithDatabase("ecommerce_db")
            .WithUsername("postgres")
            .WithPassword("password")
            .Build();

        _faker = new Faker();
    }

    public async Task InitializeAsync()
    {
        // Start the PostgreSQL container
        await _postgreSqlContainer.StartAsync();

        // Create a new HttpClient using the factory
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Use the test PostgreSQL database
                var connectionString = _postgreSqlContainer.GetConnectionString();
                // Replace existing DbContext configuration with test container connection string
            });
        }).CreateClient();
    }

    public async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenUserIsRegistered()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            Username = _faker.Internet.UserName(),
            Email = _faker.Internet.Email(),
            Password = _faker.Internet.Password()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerModel);
        var result = await response.Content.ReadFromJsonAsync<Result>();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnJwtToken_WhenCredentialsAreValid()
    {
        // Arrange
        var registerModel = new RegisterModel
        {
            Username = _faker.Internet.UserName(),
            Email = _faker.Internet.Email(),
            Password = "SecureP@ssw0rd"
        };

        // Register the user first
        await _client.PostAsJsonAsync("/api/auth/register", registerModel);

        var loginModel = new LoginModel
        {
            Username = registerModel.Username,
            Password = registerModel.Password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);
        var token = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        Assert.False(string.IsNullOrEmpty(token));
    }
}
