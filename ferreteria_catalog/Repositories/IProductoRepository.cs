using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;

namespace ferreteria_catalog.Repositories
{
    public interface IProductoRepository
    {
        Task<ProductoDTO> GetProductoByIdAsync(long id);
        Task<Producto> GetProductoByCodigoAsync(string codigo);
        Task<IEnumerable<ProductoDTO>> GetAllProductosAsync();
        Task AddStockAsync(int id, int cantidad);
    }
}
