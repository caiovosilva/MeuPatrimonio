using ecommerce_user.Models;
using ecommerce_user.Services;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_user.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var result = await authService.RegisterAsync(model);
        if (result.IsSuccess) return Ok(result.Message);
        return BadRequest(result.Message);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var token = await authService.LoginAsync(model);
        if (token == null) return Unauthorized("Invalid credentials");
        return Ok(new { Token = token });
    }
}