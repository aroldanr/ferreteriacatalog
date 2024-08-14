using ferreteria_catalog.Dtos;
using ferreteria_catalog.Models.CustomEntities;
using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ferreteria_catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;

        public ProductosController(IProductoService productoService, ILogger<ProductosController> logger)
        {
            _productoService = productoService;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,Colab")]
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

        [Authorize(Roles = "Admin")]
        [HttpPost("SubirImagen")]
        public async Task<IActionResult> SubirImagen([FromForm] string codigo, [FromForm] IFormFile nuevaImagen)
        {
            if (nuevaImagen == null || nuevaImagen.Length == 0)
            {
                return BadRequest("No se ha subido ninguna imagen.");
            }

            var producto = await _productoService.BuscarProductosPorCodigoAsync(codigo);
            if (producto == null)
            {
                return NotFound("Producto no encontrado.");
            }

            var fileName = $"{codigo}_{Path.GetFileNameWithoutExtension(nuevaImagen.FileName)}{Path.GetExtension(nuevaImagen.FileName)}";
            var filePath = Path.Combine("wwwroot/images", fileName);

            // Eliminar la imagen existente si ya existe
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Procesar y comprimir la imagen antes de guardarla
            using (var image = await Image.LoadAsync(nuevaImagen.OpenReadStream()))
            {
                // Ajustar calidad de la imagen (ej. 75%)
                var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                {
                    Quality = 75 // Puedes ajustar la calidad aquí
                };

                // Redimensionar la imagen si es necesario (opcional)
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(800, 800) // Ajusta el tamaño según tus necesidades
                }));

                await image.SaveAsync(filePath, encoder);
            }

            // Actualizar la columna ImagenURL en la tabla Producto
            producto.FirstOrDefault().ImagenURL = fileName;
            await _productoService.ActualizarProductoAsync(producto.FirstOrDefault());

            return Ok(new { imagenURL = fileName });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("SubirImagenesPorLote")]
        public async Task<IActionResult> SubirImagenesPorLote([FromForm] List<IFormFile> imagenes)
        {
            if (imagenes == null || imagenes.Count == 0)
            {
                return BadRequest("No se ha subido ninguna imagen.");
            }

            foreach (var nuevaImagen in imagenes)
            {
                var fileName = nuevaImagen.FileName;
                var filePath = Path.Combine("wwwroot/images", fileName);

                // Eliminar la imagen existente si ya existe
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Procesar y comprimir la imagen antes de guardarla
                using (var image = await Image.LoadAsync(nuevaImagen.OpenReadStream()))
                {
                    // Ajustar calidad de la imagen (ej. 75%)
                    var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                    {
                        Quality = 75 // Puedes ajustar la calidad aquí
                    };

                    // Redimensionar la imagen si es necesario (opcional)
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(800, 800) // Ajusta el tamaño según tus necesidades
                    }));

                    await image.SaveAsync(filePath, encoder);
                }
            }

            return Ok(new { mensaje = "Imágenes subidas correctamente." });
        }


    }
}
