using ecommerce_user.Models;
using ecommerce_user.Repositories;
using ecommerce_user.Services;
using UserEntity = ecommerce_user.Entities.User;
using Microsoft.Extensions.Configuration;
using Moq;
using Bogus;

namespace Ecommerce.User.UnitTests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AuthService _authService;
    private readonly Faker _faker;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        Mock<IConfiguration> configurationMock = new();

        // Set up mock configuration for JWT secret
        configurationMock.Setup(c => c["Jwt:Secret"]).Returns("ThisIsASuperSecureSecretKeyThatIs32Chars!");

        _authService = new AuthService(_userRepositoryMock.Object, configurationMock.Object);
        _faker = new Faker();
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFailure_WhenUsernameAlreadyExists()
    {
        // Arrange
        var username = _faker.Internet.UserName();
        var email = _faker.Internet.Email();
        var existingUser = new UserEntity(username, email, "hashedPassword");
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

        var registerModel = new RegisterModel (username, email, _faker.Internet.Password());

        // Act
        var result = await _authService.RegisterAsync(registerModel);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Username already taken", result.Message);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnSuccess_WhenUserIsRegistered()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((UserEntity)null);

        var registerModel = new RegisterModel
        (
            Username: _faker.Internet.UserName(),
            Email: _faker.Internet.Email(),
            Password: _faker.Internet.Password()
        );

        // Act
        var result = await _authService.RegisterAsync(registerModel);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("User registered successfully", result.Message);
        _userRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<UserEntity>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((UserEntity)null);

        var loginModel = new LoginModel
        (
            Username: _faker.Internet.UserName(),
            Password: _faker.Internet.Password()
        );

        // Act
        var result = await _authService.LoginAsync(loginModel);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
    {
        // Arrange
        var username = _faker.Internet.UserName();
        var email = _faker.Internet.Email();
        var correctPassword = _faker.Internet.Password();
        var existingUser = new UserEntity(username, email, BCrypt.Net.BCrypt.HashPassword(correctPassword));
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

        var loginModel = new LoginModel(username, "wrongpassword");

        // Act
        var result = await _authService.LoginAsync(loginModel);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnJwtToken_WhenCredentialsAreValid()
    {
        // Arrange
        var username = _faker.Internet.UserName();
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var existingUser = new UserEntity(username, email, BCrypt.Net.BCrypt.HashPassword(password));
        _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

        var loginModel = new LoginModel(username, password);

        // Act
        var result = await _authService.LoginAsync(loginModel);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }
}
