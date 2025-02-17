using ferreteria_catalog.Data;
using ferreteria_catalog.Dtos;
using ferreteria_catalog.Models.CustomEntities;
using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

namespace ferreteria_catalog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ILogger<ProductosController> _logger;
        private readonly AppDbContext _context;

        public ProductosController(IProductoService productoService,
            ILogger<ProductosController> logger,
            AppDbContext context)
        {
            _productoService = productoService;
            _logger = logger;
            _context = context;
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

        [Authorize(Roles = "Admin,Colab")]
        [HttpGet("buscar")]
        public async Task<ActionResult<PaginacionResponse<ProductoDTO>>> BuscarProductos([FromQuery] string termino, int pagina = 1, int cantidadPorPagina = 10)
        {
            if (string.IsNullOrEmpty(termino))
            {
                return BadRequest("Termino de búsqueda no puede estar vacío.");
            }

            var productos = await _productoService.BuscarProductosPorTerminoYPaginacionAsync(termino, pagina, cantidadPorPagina);
            var totalProductos = productos.TotalCount;
            var totalPaginas = (int)Math.Ceiling((double)totalProductos / cantidadPorPagina);

            var response = new PaginacionResponse<ProductoDTO>
            {
                Items = productos.Data,
                PaginaActual = pagina,
                TotalPaginas = totalPaginas
            };

            return Ok(response);
        }

        [Authorize(Roles = "Admin,Colab")]
        [HttpGet("paginacion")]
        public async Task<ActionResult<PaginacionResponse<ProductoDTO>>> GetProductosPaginados([FromQuery] int pagina = 1, int cantidadPorPagina = 10)
        {
            var productos = await _productoService.ObtenerProductosPaginadosAsync(pagina, cantidadPorPagina);
            var totalProductos = productos.TotalCount;
            var totalPaginas = (int)Math.Ceiling((double)totalProductos / cantidadPorPagina);

            var response = new PaginacionResponse<ProductoDTO>
            {
                Items = productos.Data,
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

            //var producto = await _productoService.BuscarProductosPorCodigoAsync(codigo);
            //if (producto == null)
            //{
            //    return NotFound("Producto no encontrado.");
            //}

            var fileName = $"{codigo}{Path.GetExtension(nuevaImagen.FileName)}"; // Guardar solo con el código
            var filePath = Path.Combine("images", fileName);

            // Eliminar la imagen existente si ya existe
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Procesar y comprimir la imagen antes de guardarla
            using (var image = await Image.LoadAsync(nuevaImagen.OpenReadStream()))
            {
                var extension = Path.GetExtension(nuevaImagen.FileName).ToLower();
                if (extension == ".png")
                {
                    // Si es PNG, preservar la transparencia y guardar como PNG
                    var encoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder
                    {
                        CompressionLevel = SixLabors.ImageSharp.Formats.Png.PngCompressionLevel.DefaultCompression
                    };

                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(800, 800)
                    }));

                    await image.SaveAsync(filePath, encoder);
                }
                else
                {
                    var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                    {
                        Quality = 75
                    };

                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(800, 800)
                    }));

                    await image.SaveAsync(filePath, encoder);
                }

            }

            // Actualizar la columna ImagenURL en la tabla Producto
           // producto.FirstOrDefault().ImagenURL = fileName;
            //await _productoService.ActualizarProductoAsync(producto.FirstOrDefault());

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

            // Expresión regular para encontrar el patrón ##-####
            var regex = new Regex(@"\d{2}-\d{4}");

            foreach (var nuevaImagen in imagenes)
            {
                var fileName = Path.GetFileNameWithoutExtension(nuevaImagen.FileName);

                // Buscar el código usando la expresión regular
                var match = regex.Match(fileName);
                var codigo = match.Value;

                if (string.IsNullOrEmpty(codigo))
                {
                    // Si no se encuentra un código válido, pasar a la siguiente imagen
                    continue;
                }

                //var producto = await _productoService.BuscarProductosPorCodigoAsync(codigo);
                //if (producto == null)
                //{
                //    continue;
                //}

                var newFileName = $"{codigo}{Path.GetExtension(nuevaImagen.FileName)}"; // Guardar solo con el código
                var filePath = Path.Combine("images", newFileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                using (var image = await Image.LoadAsync(nuevaImagen.OpenReadStream()))
                {
                    var extension = Path.GetExtension(nuevaImagen.FileName).ToLower();
                    if (extension == ".png")
                    {
                        // Si es PNG, preservar la transparencia y guardar como PNG
                        var encoder = new SixLabors.ImageSharp.Formats.Png.PngEncoder
                        {
                            CompressionLevel = SixLabors.ImageSharp.Formats.Png.PngCompressionLevel.DefaultCompression
                        };

                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(800, 800)
                        }));

                        await image.SaveAsync(filePath, encoder);
                    }
                    else 
                    {
                        var encoder = new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                        {
                            Quality = 75
                        };

                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(800, 800)
                        }));

                        await image.SaveAsync(filePath, encoder);
                    }

                }

                //producto.FirstOrDefault().ImagenURL = newFileName;
                //await _productoService.ActualizarProductoAsync(producto.FirstOrDefault());
            }

            return Ok(new { mensaje = "Imágenes subidas correctamente." });
        }

        [HttpPost("ProcesarImagenes")]
        public async Task<IActionResult> ProcesarImagenes()
        {
            try
            {
                // Ruta donde se almacenan las imágenes en la raíz del proyecto
                string rutaImagenes = Path.Combine(Directory.GetCurrentDirectory(), "images");

                // Obtener todos los archivos en la carpeta de imágenes
                var archivos = Directory.GetFiles(rutaImagenes);
                int totalArchivos = archivos.Length;
                int tamanoLote = 500; // Define el tamaño del lote
                int procesados = 0;

                // Procesar las imágenes en lotes
                while (procesados < totalArchivos)
                {
                    var loteActual = archivos.Skip(procesados).Take(tamanoLote).ToList();

                    foreach (var archivo in loteActual)
                    {
                        var nombreArchivo = Path.GetFileNameWithoutExtension(archivo); // Nombre del archivo sin extensión

                        // Buscar el producto por código
                        var producto = await _context.Producto.FirstOrDefaultAsync(p => p.Codigo == nombreArchivo);

                        if (producto != null)
                        {
                            // Actualizar la URL de la imagen en la base de datos
                            producto.ImagenURL = nombreArchivo;
                        }
                    }

                    // Guardar cambios en la base de datos después de cada lote
                    await _context.SaveChangesAsync();
                    procesados += loteActual.Count;

                    // Log para monitorear progreso en la consola del servidor
                    Console.WriteLine($"Lote procesado: {procesados}/{totalArchivos}");
                }

                return Ok(new { mensaje = "Proceso completado", totalImagenesProcesadas = procesados });
            }
            catch (Exception ex)
            {
                // Manejo de errores
                Console.WriteLine("Error al procesar imágenes: " + ex.Message);
                return StatusCode(500, new { mensaje = "Error durante el procesamiento", detalle = ex.Message });
            }
        }
    }
}
