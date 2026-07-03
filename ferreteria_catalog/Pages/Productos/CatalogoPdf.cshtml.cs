using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ferreteria_catalog.Pages.Productos
{
    [Authorize(Roles = "Admin")]
    public class CatalogoPdfModel : PageModel
    {
        public IReadOnlyList<SucursalCatalogoOption> Sucursales { get; } =
        [
            new("chulin", "Ferreteria Chulin"),
            new("cardoza", "Ferreteria Ruiz Cardoza")
        ];
    }

    public record SucursalCatalogoOption(string Id, string Nombre);
}
