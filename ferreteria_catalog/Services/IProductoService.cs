using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;

namespace ferreteria_catalog.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDTO>> ObtenerTodosProductosAsync();
        Task AgregarStockAsync(int id, int cantidad);
        Task<IEnumerable<ProductoDTO>> BuscarProductosPorNombreAsync(string nombre);
        Task<IEnumerable<ProductoDTO>> BuscarProductosPorTerminoAsync(string termino);
        Task<IEnumerable<ProductoDTO>> BuscarProductosPorCodigoAsync(string codigo);
        Task<IEnumerable<ProductoDTO>> ObtenerProductosPaginadosAsync(int pagina, int cantidadPorPagina);
        Task<int> ObtenerTotalProductosAsync();
        Task<int> ObtenerTotalProductosPorTerminoAsync(string termino);
        Task<IEnumerable<ProductoDTO>> BuscarProductosPorTerminoYPaginacionAsync(string termino, int pagina, int cantidadPorPagina);
        Task<ProductoDTO> ObtenerProductoPorIdAsync(int id);
        Task ActualizarProductoAsync(ProductoDTO productoDto);

    }
}
