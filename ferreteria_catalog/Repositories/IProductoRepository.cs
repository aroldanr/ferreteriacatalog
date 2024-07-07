using ferreteria_catalog.Models;

namespace ferreteria_catalog.Repositories
{
    public interface IProductoRepository
    {
        Task<Producto> GetProductoByIdAsync(long id);
        Task<Producto> GetProductoByCodigoAsync(string codigo);
        Task<IEnumerable<Producto>> GetAllProductosAsync();
        Task AddStockAsync(int id, int cantidad);
    }
}
