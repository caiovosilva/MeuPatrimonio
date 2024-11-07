using MeuPatrimonio.User.Models;
using MeuPatrimonio.User.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeuPatrimonio.User.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IAuthService authService) : ControllerBase
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