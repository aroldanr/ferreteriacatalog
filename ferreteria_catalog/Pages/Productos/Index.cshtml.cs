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
    }
}
