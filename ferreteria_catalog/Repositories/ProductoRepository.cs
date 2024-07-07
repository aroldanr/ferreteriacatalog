using ferreteria_catalog.Data;
using ferreteria_catalog.Models;
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

        public async Task<Producto> GetProductoByIdAsync(long id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task<Producto> GetProductoByCodigoAsync(string codigo)
        {
            return await _context.Productos.FirstOrDefaultAsync(p => p.Codigo == codigo);
        }

        public async Task<IEnumerable<Producto>> GetAllProductosAsync()
        {
            return await _context.Productos.ToListAsync();
        }

        public async Task AddStockAsync(int id, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                producto.Stock += cantidad;
                await _context.SaveChangesAsync();
            }
        }

    }
}
