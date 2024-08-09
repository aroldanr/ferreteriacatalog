using ferreteria_catalog.Data;
using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;
using Microsoft.EntityFrameworkCore;

namespace ferreteria_catalog.Repositories
{

    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _context;
        public ProductoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductoDTO> GetProductoByIdAsync(long id)
        {
            var producto = await _context.Producto
                        .Include(p => p.Marca)
                        .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (producto == null) return null;

            var existencia = await _context.Existencia
                .FirstOrDefaultAsync(e => e.ProductoId == id);

            return new ProductoDTO
            {
                ProductoId = producto.ProductoId,
                Codigo = producto.Codigo,
                Descripcion = producto.Descripcion,
                UndxBulto = producto.UndxBulto,
                Marca = producto.Marca.NombreMarca,
                ImagenURL = producto.ImagenURL,
                Existencia = existencia?.Stock ?? 0
            };
        }

        public async Task<IEnumerable<ProductoDTO>> BuscarProductosPorNombreAsync(string nombre)
        {
            var productos = await _context.Producto
                .Include(p => p.Marca)
                .Include(p => p.Existencia)
                .Where(p => p.Descripcion.Contains(nombre))
                .ToListAsync();

            return productos.Select(p => new ProductoDTO
            {
                ProductoId = p.ProductoId,
                Codigo = p.Codigo,
                Descripcion = p.Descripcion,
                UndxBulto = p.UndxBulto,
                Marca = p.Marca.NombreMarca,
                ImagenURL = p.ImagenURL,
                Existencia = p.Existencia?.Stock ?? 0
            }).ToList();
        }

        public async Task<IEnumerable<ProductoDTO>> BuscarProductosPorCodigoAsync(string codigo)
        {
            var productos = await _context.Producto.AsNoTracking()
                .Include(p => p.Marca)
                .Include(p => p.Existencia)
                .Where(p => p.Codigo == codigo)
                .ToListAsync();

            return productos.Select(p => new ProductoDTO
            {
                ProductoId = p.ProductoId,
                Codigo = p.Codigo,
                Descripcion = p.Descripcion,
                UndxBulto = p.UndxBulto,
                Marca = p.Marca.NombreMarca,
                ImagenURL = p.ImagenURL,
                Existencia = p.Existencia?.Stock ?? 0,
                MarcaId = p.MarcaId,
            }).ToList();
        }

        public async Task<IEnumerable<ProductoDTO>> BuscarProductosPorTerminoAsync(string termino)
        {
            var productosPorNombre = await BuscarProductosPorNombreAsync(termino);
            if (productosPorNombre.Any())
            {
                return productosPorNombre;
            }

            return await BuscarProductosPorCodigoAsync(termino);
        }


        public async Task<IEnumerable<ProductoDTO>> GetAllProductosAsync()
        {
            var productos = await _context.Producto
            .Include(p => p.Marca)
            .ToListAsync();

            var productoIds = productos.Select(p => p.ProductoId).ToList();
            var existencias = await _context.Existencia
                .Where(e => productoIds.Contains(e.ProductoId))
                .ToDictionaryAsync(e => e.ProductoId, e => e.Stock);

            return productos.Select(p => new ProductoDTO
            {
                ProductoId = p.ProductoId,
                Codigo = p.Codigo,
                Descripcion = p.Descripcion,
                UndxBulto = p.UndxBulto,
                Marca = p.Marca.NombreMarca,
                ImagenURL = p.ImagenURL,
                Existencia = existencias.ContainsKey(p.ProductoId) ? existencias[p.ProductoId] : 0
            }).ToList();
        }

        public async Task AddStockAsync(int id, int cantidad)
        {
            var producto = await _context.Producto.FindAsync(id);
            if (producto != null)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ProductoDTO>> ObtenerProductosPaginadosAsync(int pagina, int cantidadPorPagina)
        {
            var productos = await _context.Producto
                .Include(p => p.Marca)
                .Include(p => p.Existencia)
                .Skip((pagina - 1) * cantidadPorPagina)
                .Take(cantidadPorPagina)
                .ToListAsync();

            return productos.Select(p => new ProductoDTO
            {
                ProductoId = p.ProductoId,
                Codigo = p.Codigo,
                Descripcion = p.Descripcion,
                UndxBulto = p.UndxBulto,
                Marca = p.Marca.NombreMarca,
                ImagenURL = p.ImagenURL,
                Existencia = p.Existencia?.Stock ?? 0
            }).ToList();
        }

        public async Task<int> ObtenerTotalProductosAsync()
        {
            return await _context.Producto.CountAsync();
        }

        public async Task<int> ObtenerTotalProductosPorTerminoAsync(string termino)
        {
            return await _context.Producto
                .Where(p => p.Descripcion.Contains(termino) || p.Codigo.Contains(termino))
                .CountAsync();
        }

        public async Task<IEnumerable<ProductoDTO>> BuscarProductosPorTerminoYPaginacionAsync(string termino, int pagina, int cantidadPorPagina)
        {
            // Normalizar el término de búsqueda a minúsculas para evitar problemas con la búsqueda (opcional)
            termino = termino?.ToLower() ?? string.Empty;

            // Realizar la consulta con paginación y manejo de nulos en la base de datos
            var productos = await _context.Producto
                .Include(p => p.Marca)
                .Include(p => p.Existencia)
                .Where(p => string.IsNullOrEmpty(termino) || p.Descripcion.ToLower().Contains(termino) || p.Codigo.ToLower().Contains(termino))
                .OrderBy(p => p.ProductoId)  // Asegura un orden consistente para la paginación
                .Skip((pagina - 1) * cantidadPorPagina)
                .Take(cantidadPorPagina)
                .ToListAsync();

            // Proyectar los resultados a ProductoDTO, manejando posibles valores nulos
            return productos.Select(p => new ProductoDTO
            {
                ProductoId = p.ProductoId,
                Codigo = p.Codigo ?? "N/A",  // Asignar "N/A" si el código es nulo
                Descripcion = p.Descripcion ?? "Descripción no disponible",  // Valor predeterminado si la descripción es nula
                UndxBulto = p.UndxBulto ?? 0,  // Si UndxBulto es nulo, asignar 0
                Marca = p.Marca?.NombreMarca ?? "Sin Marca",  // Si la marca es nula, asignar "Sin Marca"
                ImagenURL = p.ImagenURL ?? string.Empty,  // Si ImagenURL es nulo, asignar una cadena vacía
                Existencia = p.Existencia?.Stock ?? 0  // Si no hay existencia, asignar 0
            }).ToList();
        }


        public async Task<ProductoDTO> GetProductoByIdAsync(int id)
        {
            var producto = await _context.Producto
                .Include(p => p.Marca)
                .Include(p => p.Existencia)
                .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (producto == null)
            {
                return null;
            }

            return new ProductoDTO
            {
                ProductoId = producto.ProductoId,
                Codigo = producto.Codigo,
                Descripcion = producto.Descripcion,
                UndxBulto = producto.UndxBulto,
                Marca = producto.Marca.NombreMarca,
                ImagenURL = producto.ImagenURL,
                Existencia = producto.Existencia?.Stock ?? 0,
                MarcaId = producto.MarcaId
            };
        }
        public async Task ActualizarProductoAsync(Producto producto)
        {
            try
            {
                _context.Producto.Update(producto);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
    }
}
