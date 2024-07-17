using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;

namespace ferreteria_catalog.Repositories
{
    public interface IProductoRepository
    {
        Task<ProductoDTO> GetProductoByIdAsync(long id);
        Task<IEnumerable<ProductoDTO>> BuscarProductosPorNombreAsync(string nombre);
        Task<IEnumerable<ProductoDTO>> BuscarProductosPorCodigoAsync(string codigo);
        Task<IEnumerable<ProductoDTO>> BuscarProductosPorTerminoAsync(string termino);
        Task<IEnumerable<ProductoDTO>> GetAllProductosAsync();
        Task AddStockAsync(int id, int cantidad);
        Task<IEnumerable<ProductoDTO>> ObtenerProductosPaginadosAsync(int pagina, int cantidadPorPagina);
        Task<int> ObtenerTotalProductosAsync();
        Task<int> ObtenerTotalProductosPorTerminoAsync(string termino);
        Task<IEnumerable<ProductoDTO>> BuscarProductosPorTerminoYPaginacionAsync(string termino, int pagina, int cantidadPorPagina);
        Task<ProductoDTO> GetProductoByIdAsync(int id);
        Task ActualizarProductoAsync(Producto producto);
    }
}
