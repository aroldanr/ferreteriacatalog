using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;
using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ferreteria_catalog.Pages.Productos
{
    public class ProductoDetallesModel : PageModel
    {
        private readonly IProductoService _productoService;

        public ProductoDTO Producto { get; set; }
        public ProductoDetallesModel(IProductoService productoService)
        {
            _productoService = productoService;
        }
    }
}
