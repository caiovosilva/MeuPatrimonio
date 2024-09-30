using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ecommerce_user.Models;
using ecommerce_user.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace ecommerce_user.Services;

public class AuthService(IUserRepository userRepository, IConfiguration configuration)
    : IAuthService
{
    public async Task<Result> RegisterAsync(RegisterModel model)
    {
        var existingUser = await userRepository.GetByUsernameAsync(model.Username);
        if (existingUser != null) return Result.Failure("Username already taken");

        var user = new Entities.User(model.Username, model.Email, BCrypt.Net.BCrypt.HashPassword(model.Password));
        await userRepository.AddAsync(user);
        return Result.Success("User registered successfully");
    }

    public async Task<string?> LoginAsync(LoginModel model)
    {
        var user = await userRepository.GetByUsernameAsync(model.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash)) return null;

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(Entities.User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"] ?? string.Empty);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
