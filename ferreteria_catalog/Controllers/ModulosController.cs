using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ferreteria_catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModulosController : Controller
    {
        private readonly IModuloService _moduloService;
        private readonly ILogger<ProductosController> _logger;

        public ModulosController(IModuloService moduloService, ILogger<ProductosController> logger)
        {
            _moduloService = moduloService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Colab")]
        [HttpGet("GetModulosByRol")]
        public async Task<IActionResult> GetModulosByRol([FromQuery] string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("El nombre de usuario es requerido.");
            }

            var modulos = await _moduloService.GetModulosByRol(username);

            return Ok(modulos);
        }

    }
}
