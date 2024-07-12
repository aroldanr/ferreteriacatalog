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
                Marca = producto.Marca.Nombre,
                ImagenURL = producto.ImagenURL,
                Existencia = existencia?.Stock ?? 0
            };
        }

        public async Task<Producto> GetProductoByCodigoAsync(string codigo)
        {
            return await _context.Producto.FirstOrDefaultAsync(p => p.Codigo == codigo);
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
                Marca = p.Marca.Nombre,
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

    }
}
