using ferreteria_catalog.Models;

namespace ferreteria_catalog.Services
{
    public interface IProductoService
    {
        Task<Producto> ObtenerProductoPorIdAsync(long id);
        Task<Producto> ObtenerProductoPorCodigoAsync(string codigo);
        Task<IEnumerable<Producto>> ObtenerTodosProductosAsync();
        Task AgregarStockAsync(int id, int cantidad);
    }
}
