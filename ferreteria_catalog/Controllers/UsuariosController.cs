using ferreteria_catalog.Models.CustomEntities;
using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ferreteria_catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<ProductosController> _logger;
        public UsuariosController(IUsuarioService usuarioService, ILogger<ProductosController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")] // Solo los administradores pueden crear usuarios
        [HttpPost("Crear")]
        public async Task<IActionResult> Crear([FromBody] CreateUserDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _usuarioService.CrearUsuarioAsync(model);

                if (response.IsSuccess)
                {
                    return Ok(new { success = true, message = "Usuario creado exitosamente." });
                }
                else
                {
                    return BadRequest(new { success = false, message = response.Message });
                }
            }

            return BadRequest(new { success = false, message = "Datos inválidos. Por favor, verifique la información proporcionada." });
        }

    }
}
