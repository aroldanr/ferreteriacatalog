using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace ferreteria_catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductoById(long id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return Ok(producto);
        }

        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetProductoByCodigo(string codigo)
        {
            var producto = await _productoService.ObtenerProductoPorCodigoAsync(codigo);
            if (producto == null)
            {
                return NotFound();
            }
            return Ok(producto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProductos()
        {
            var productos = await _productoService.ObtenerTodosProductosAsync();
            return Ok(productos);
        }

        [HttpPost("{id}/agregar-stock/{cantidad}")]
        public async Task<IActionResult> AgregarStock(int id, int cantidad)
        {
            await _productoService.AgregarStockAsync(id, cantidad);
            return NoContent();
        }
    }
}
