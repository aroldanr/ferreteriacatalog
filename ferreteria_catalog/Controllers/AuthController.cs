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
        private readonly string _logDirectoryPath;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
            _logDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
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


        // Endpoint para listar los archivos de logs disponibles
        [HttpGet("listar")]
        public IActionResult ListarLogs()
        {
            if (!Directory.Exists(_logDirectoryPath))
            {
                return NotFound("No se encontraron logs.");
            }

            var archivosLog = Directory.GetFiles(_logDirectoryPath)
                                       .Select(Path.GetFileName)
                                       .ToList();

            return Ok(archivosLog);
        }

        // Endpoint para descargar un archivo de log específico
        [HttpGet("descargar/{nombreArchivo}")]
        public IActionResult DescargarLog(string nombreArchivo)
        {
            var filePath = Path.Combine(_logDirectoryPath, nombreArchivo);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Archivo de log no encontrado.");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", Path.GetFileName(filePath));
        }

    }
}
