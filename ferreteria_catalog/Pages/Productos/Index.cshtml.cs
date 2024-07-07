using Microsoft.AspNetCore.Mvc.RazorPages;
using ferreteria_catalog.Services;
using ferreteria_catalog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ferreteria_catalog.Pages.Productos
{
    public class IndexModel : PageModel
    {
        private readonly IProductoService _productoService;

        public IndexModel(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public IEnumerable<Producto>? Productos { get; set; }

        public async Task OnGetAsync()
        {
            Productos = await _productoService.ObtenerTodosProductosAsync();
        }
    }
}
