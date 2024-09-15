using ecommerce_user.Models;

namespace ecommerce_user.Services;

public interface IAuthService
{
    Task<Result> RegisterAsync(RegisterModel model);
    Task<string?> LoginAsync(LoginModel model);
}