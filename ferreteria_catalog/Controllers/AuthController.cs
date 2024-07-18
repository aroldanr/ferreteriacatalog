using ferreteria_catalog.Pages.Login;
using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace ferreteria_catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string username, [FromForm] string password)
        {
            var usuario = await _authService.Authenticate(username, password);

            if (usuario == null)
            {
                return Unauthorized();
            }

            var token = _authService.GenerateToken(usuario);

            // Configura la cookie con el token
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };
            Response.Cookies.Append("jwtToken", token, cookieOptions);

            return Ok(new { token });
        }

    }
}
