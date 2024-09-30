using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Bogus;
using ecommerce_user.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace Ecommerce.User.UnitTests;

public class JwtMiddlewareTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly JwtMiddleware _jwtMiddleware;
    private readonly Faker _faker;

    public JwtMiddlewareTests()
    {
        Mock<RequestDelegate> nextMock = new();
        _configurationMock = new Mock<IConfiguration>();

        // Set up mock configuration for JWT secret
        _configurationMock.Setup(c => c["Jwt:Secret"]).Returns("ThisIsASuperSecureSecretKeyThatIs32Chars!");

        _jwtMiddleware = new JwtMiddleware(nextMock.Object, _configurationMock.Object);
        _faker = new Faker();
    }

    [Fact]
    public async Task InvokeAsync_ShouldAttachUserToContext_WhenTokenIsValid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var username = _faker.Internet.UserName();
        var token = GenerateValidJwtToken(username);
        context.Request.Headers["Authorization"] = $"Bearer {token}";

        // Act
        await _jwtMiddleware.InvokeAsync(context);

        // Assert
        Assert.True(context.Items.ContainsKey("Username"));
        Assert.Equal(username, context.Items["Username"]);
    }

    [Fact]
    public async Task InvokeAsync_ShouldNotAttachUserToContext_WhenTokenIsInvalid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer invalidToken";

        // Act
        await _jwtMiddleware.InvokeAsync(context);

        // Assert
        Assert.False(context.Items.ContainsKey("Username"));
    }

    private string GenerateValidJwtToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configurationMock.Object["Jwt:Secret"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}