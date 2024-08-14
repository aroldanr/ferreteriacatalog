using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;
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

        public async Task<IEnumerable<ProductoDTO>> BuscarProductosPorNombreAsync(string nombre)
        {
            return await _productoRepository.BuscarProductosPorNombreAsync(nombre);
        }

        public async Task<IEnumerable<ProductoDTO>> BuscarProductosPorCodigoAsync(string codigo)
        {
            return await _productoRepository.BuscarProductosPorCodigoAsync(codigo);
        }

        public async Task<IEnumerable<ProductoDTO>> BuscarProductosPorTerminoAsync(string termino)
        {
            var productosPorNombre = await _productoRepository.BuscarProductosPorNombreAsync(termino);
            if (productosPorNombre.Any())
            {
                return productosPorNombre;
            }

            return await _productoRepository.BuscarProductosPorCodigoAsync(termino);
        }

        public async Task<IEnumerable<ProductoDTO>> ObtenerTodosProductosAsync()
        {
            return await _productoRepository.GetAllProductosAsync();
        }

        public async Task AgregarStockAsync(int id, int cantidad)
        {
            await _productoRepository.AddStockAsync(id, cantidad);
        }

        public async Task<IEnumerable<ProductoDTO>> ObtenerProductosPaginadosAsync(int pagina, int cantidadPorPagina)
        {
            return await _productoRepository.ObtenerProductosPaginadosAsync(pagina, cantidadPorPagina);
        }

        public async Task<int> ObtenerTotalProductosAsync()
        {
            return await _productoRepository.ObtenerTotalProductosAsync();
        }

        public async Task<int> ObtenerTotalProductosPorTerminoAsync(string termino)
        {
            return await _productoRepository.ObtenerTotalProductosPorTerminoAsync(termino);
        }

        public async Task<IEnumerable<ProductoDTO>> BuscarProductosPorTerminoYPaginacionAsync(string termino, int pagina, int cantidadPorPagina)
        {
            return await _productoRepository.BuscarProductosPorTerminoYPaginacionAsync(termino, pagina, cantidadPorPagina);
        }

        public async Task<ProductoDTO> ObtenerProductoPorIdAsync(int id)
        {
            var producto = await _productoRepository.GetProductoByIdAsync(id);
            return producto;
        }

        public async Task ActualizarProductoAsync(ProductoDTO productoDto)
        {
            //var productoDtoDb = await _productoRepository.GetProductoByIdAsync(productoDto.ProductoId);
            if (productoDto != null)
            {
                var producto = new Producto
                {
                    ProductoId = productoDto.ProductoId,
                    Codigo = productoDto.Codigo,
                    Descripcion = productoDto.Descripcion,
                    UndxBulto = productoDto.UndxBulto,
                    ImagenURL = productoDto.ImagenURL,
                    MarcaId = productoDto.MarcaId
                };

                // Pass the Producto entity to the repository
                await _productoRepository.ActualizarProductoAsync(producto);
            }
        }

    }
}
