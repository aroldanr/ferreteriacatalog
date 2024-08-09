﻿using ferreteria_catalog.Data;
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
                            Existencia = e != null ? e.Stock : 0 // Manejo de nulos para Existencia
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
            termino = termino?.ToLower() ?? string.Empty;

            var query = from p in _context.Producto
                        join m in _context.Marca on p.MarcaId equals m.MarcaId into marcaJoin
                        from m in marcaJoin.DefaultIfEmpty()
                        join e in _context.Existencia on p.ProductoId equals e.ProductoId into existenciaJoin
                        from e in existenciaJoin.DefaultIfEmpty()
                        where string.IsNullOrEmpty(termino) ||
                              (p.Descripcion != null && p.Descripcion.ToLower().Contains(termino)) ||
                              (p.Codigo != null && p.Codigo.ToLower().Contains(termino))
                        orderby p.ProductoId
                        select new ProductoDTO
                        {
                            ProductoId = p.ProductoId,
                            Codigo = p.Codigo ?? "N/A",  // Asignar "N/A" si el código es nulo
                            Descripcion = p.Descripcion ?? "Descripción no disponible",  // Valor predeterminado si la descripción es nula
                            UndxBulto = p.UndxBulto ?? 0,  // Si UndxBulto es nulo, asignar 0
                            Marca = m != null ? m.NombreMarca : "Sin Marca",  // Si la marca es nula, asignar "Sin Marca"
                            ImagenURL = p.ImagenURL ?? string.Empty,  // Si ImagenURL es nulo, asignar una cadena vacía
                            Existencia = e != null ? e.Stock : 0   // Si no hay existencia, asignar 0
                        };

            return await query
                .Skip((pagina - 1) * cantidadPorPagina)
                .Take(cantidadPorPagina)
                .ToListAsync();
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
