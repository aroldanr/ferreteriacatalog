using ferreteria_catalog.Models;
using ferreteria_catalog.Repositories;

namespace ferreteria_catalog.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;

        public ProductoService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<Producto> ObtenerProductoPorIdAsync(long id)
        {
            return await _productoRepository.GetProductoByIdAsync(id);
        }

        public async Task<Producto> ObtenerProductoPorCodigoAsync(string codigo)
        {
            return await _productoRepository.GetProductoByCodigoAsync(codigo);
        }

        public async Task<IEnumerable<Producto>> ObtenerTodosProductosAsync()
        {
            return await _productoRepository.GetAllProductosAsync();
        }

        public async Task AgregarStockAsync(int id, int cantidad)
        {
            await _productoRepository.AddStockAsync(id, cantidad);
        }
    }
}
