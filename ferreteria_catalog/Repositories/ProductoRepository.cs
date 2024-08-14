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
                Existencia = p != null ? p.UndxBulto : 0
            }).ToList();
        }

        public async Task<IEnumerable<ProductoDTO>> BuscarProductosPorCodigoAsync(string codigo)
        {
            var query = from p in _context.Producto.AsNoTracking()
                        join m in _context.Marca on p.MarcaId equals m.MarcaId into marcaJoin
                        from m in marcaJoin.DefaultIfEmpty()
                        join e in _context.Existencia on p.ProductoId equals e.ProductoId into existenciaJoin
                        from e in existenciaJoin.DefaultIfEmpty()
                        where p.Codigo == codigo
                        select new ProductoDTO
                        {
                            ProductoId = p.ProductoId,
                            Codigo = p.Codigo ?? "N/A",  // Manejo de nulos para Código
                            Descripcion = p.Descripcion ?? "Descripción no disponible",  // Manejo de nulos para Descripción
                            UndxBulto = p.UndxBulto ?? 0,  // Manejo de nulos para UndxBulto
                            Marca = m != null ? m.NombreMarca : "Sin Marca",  // Manejo de nulos para Marca
                            ImagenURL = p.ImagenURL ?? string.Empty,  // Manejo de nulos para ImagenURL
                            Existencia = p != null ? p.UndxBulto : 0,  // Manejo de nulos para Existencia
                            MarcaId = p.MarcaId
                        };

            return await query.ToListAsync();
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
            var query = from p in _context.Producto
                        join m in _context.Marca on p.MarcaId equals m.MarcaId into marcaJoin
                        from m in marcaJoin.DefaultIfEmpty()
                        join e in _context.Existencia on p.ProductoId equals e.ProductoId into existenciaJoin
                        from e in existenciaJoin.DefaultIfEmpty()
                        orderby p.ProductoId
                        select new ProductoDTO
                        {
                            ProductoId = p.ProductoId,
                            Codigo = p.Codigo ?? "N/A",  // Manejo de nulos para Código
                            Descripcion = p.Descripcion ?? "Descripción no disponible",  // Manejo de nulos para Descripción
                            UndxBulto = p.UndxBulto ?? 0,  // Manejo de nulos para UndxBulto
                            Marca = m != null ? m.NombreMarca : "Sin Marca",  // Manejo de nulos para Marca
                            ImagenURL = p.ImagenURL ?? string.Empty,  // Manejo de nulos para ImagenURL
                            Existencia = p != null ? p.UndxBulto : 0 // Manejo de nulos para Existencia
                        };

            return await query
                .Skip((pagina - 1) * cantidadPorPagina)
                .Take(cantidadPorPagina)
                .ToListAsync();
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
            // Normalizar el término de búsqueda a minúsculas
            termino = termino?.ToLower() ?? string.Empty;

            // Dividir el término en palabras individuales
            var palabrasClave = termino.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Construir la consulta
            var query = from p in _context.Producto
                        join m in _context.Marca on p.MarcaId equals m.MarcaId into marcaJoin
                        from m in marcaJoin.DefaultIfEmpty()
                        join e in _context.Existencia on p.ProductoId equals e.ProductoId into existenciaJoin
                        from e in existenciaJoin.DefaultIfEmpty()
                        where palabrasClave.All(palabra =>
                                  (p.Descripcion != null && p.Descripcion.ToLower().Contains(palabra)) ||
                                  (p.Codigo != null && p.Codigo.ToLower().Contains(palabra)) || 
                                  (m.NombreMarca != null && m.NombreMarca.ToLower().Contains(palabra)))
                        orderby p.ProductoId
                        select new ProductoDTO
                        {
                            ProductoId = p.ProductoId,
                            Codigo = p.Codigo ?? "N/A",
                            Descripcion = p.Descripcion ?? "Descripción no disponible",
                            UndxBulto = p.UndxBulto ?? 0,
                            Marca = m != null ? m.NombreMarca : "Sin Marca",
                            ImagenURL = p.ImagenURL ?? string.Empty,
                            Existencia = e != null ? p.UndxBulto : 0
                        };

            // Aplicar paginación
            return await query
                .Skip((pagina - 1) * cantidadPorPagina)
                .Take(cantidadPorPagina)
                .ToListAsync();
        }
        public async Task<ProductoDTO> GetProductoByIdAsync(int id)
        {
            var query = from p in _context.Producto.AsNoTracking()
                        join m in _context.Marca on p.MarcaId equals m.MarcaId into marcaJoin
                        from m in marcaJoin.DefaultIfEmpty()
                        join e in _context.Existencia on p.ProductoId equals e.ProductoId into existenciaJoin
                        from e in existenciaJoin.DefaultIfEmpty()
                        where p.ProductoId == id
                        select new ProductoDTO
                        {
                            ProductoId = p.ProductoId,
                            Codigo = p.Codigo ?? "N/A",  // Manejo de nulos para Código
                            Descripcion = p.Descripcion ?? "Descripción no disponible",  // Manejo de nulos para Descripción
                            UndxBulto = p.UndxBulto ?? 0,  // Manejo de nulos para UndxBulto
                            Marca = m != null ? m.NombreMarca : "Sin Marca",  // Manejo de nulos para Marca
                            ImagenURL = p.ImagenURL ?? string.Empty,  // Manejo de nulos para ImagenURL
                            Existencia = p != null ? p.UndxBulto : 0,  // Manejo de nulos para Existencia
                            MarcaId = p.MarcaId
                        };

            return await query.FirstOrDefaultAsync();
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
