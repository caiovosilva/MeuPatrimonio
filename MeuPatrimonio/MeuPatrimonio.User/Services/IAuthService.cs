using MeuPatrimonio.User.Models;

namespace MeuPatrimonio.User.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterModel model);
    Task<string?> LoginAsync(LoginModel model);
}