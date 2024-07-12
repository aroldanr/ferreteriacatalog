using Microsoft.AspNetCore.Mvc.RazorPages;
using ferreteria_catalog.Services;
using ferreteria_catalog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using ferreteria_catalog.Models.CustomEntities;

namespace ferreteria_catalog.Pages.Productos
{
    public class IndexModel : PageModel
    {
        private readonly IProductoService _productoService;

        public IndexModel(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public IEnumerable<ProductoDTO>? Productos { get; set; }

        public async Task OnGetAsync()
        {
            Productos = await _productoService.ObtenerTodosProductosAsync();
        }
    }
}
