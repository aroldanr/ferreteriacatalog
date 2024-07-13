using Microsoft.AspNetCore.Mvc.RazorPages;
using ferreteria_catalog.Services;
using ferreteria_catalog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public IEnumerable<ProductoDTO> Productos { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Termino { get; set; }

        // M�todo para cargar todos los productos al cargar la p�gina
        public async Task OnGetAsync()
        {
            Productos = await _productoService.ObtenerTodosProductosAsync();
        }

        // M�todo para manejar la b�squeda por t�rmino
        public async Task<IActionResult> OnPostBuscarAsync()
        {
            if (!string.IsNullOrEmpty(Termino))
            {
                Productos = await _productoService.BuscarProductosPorTerminoAsync(Termino);
            }
            else
            {
                Productos = await _productoService.ObtenerTodosProductosAsync();
            }
            return Page();
        }
    }
}
