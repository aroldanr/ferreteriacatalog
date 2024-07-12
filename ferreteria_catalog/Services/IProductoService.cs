using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;

namespace ferreteria_catalog.Services
{
    public interface IProductoService
    {
        Task<ProductoDTO> ObtenerProductoPorIdAsync(long id);
        Task<Producto> ObtenerProductoPorCodigoAsync(string codigo);
        Task<IEnumerable<ProductoDTO>> ObtenerTodosProductosAsync();
        Task AgregarStockAsync(int id, int cantidad);
    }
}
