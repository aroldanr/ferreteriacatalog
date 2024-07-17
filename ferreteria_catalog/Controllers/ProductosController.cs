using ferreteria_catalog.Dtos;
using ferreteria_catalog.Models.CustomEntities;
using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Mvc;

namespace ferreteria_catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetProductoById(long id)
        //{
        //    var producto = await _productoService.ObtenerProductoPorIdAsync(id);
        //    if (producto == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(producto);
        //}

        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetProductoByCodigo(string codigo)
        {
            var producto = await _productoService.BuscarProductosPorCodigoAsync(codigo);
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

        [HttpGet("buscar")]
        public async Task<ActionResult<PaginacionResponse<ProductoDTO>>> BuscarProductos([FromQuery] string termino, int pagina = 1, int cantidadPorPagina = 10)
        {
            if (string.IsNullOrEmpty(termino))
            {
                return BadRequest("Termino de búsqueda no puede estar vacío.");
            }

            var productos = await _productoService.BuscarProductosPorTerminoYPaginacionAsync(termino, pagina, cantidadPorPagina);
            var totalProductos = await _productoService.ObtenerTotalProductosPorTerminoAsync(termino);
            var totalPaginas = (int)Math.Ceiling((double)totalProductos / cantidadPorPagina);

            var response = new PaginacionResponse<ProductoDTO>
            {
                Items = productos,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas
            };

            return Ok(response);
        }

        [HttpGet("paginacion")]
        public async Task<ActionResult<PaginacionResponse<ProductoDTO>>> GetProductosPaginados([FromQuery] int pagina = 1, int cantidadPorPagina = 10)
        {
            var productos = await _productoService.ObtenerProductosPaginadosAsync(pagina, cantidadPorPagina);
            var totalProductos = await _productoService.ObtenerTotalProductosAsync();
            var totalPaginas = (int)Math.Ceiling((double)totalProductos / cantidadPorPagina);

            var response = new PaginacionResponse<ProductoDTO>
            {
                Items = productos,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductoById(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return Ok(producto);
        }
    }
}
