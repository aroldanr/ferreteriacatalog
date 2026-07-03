using ferreteria_catalog.Models.CustomEntities;
using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ferreteria_catalog.Pages.Productos
{
    [Authorize(Roles = "Admin")]
    public class CatalogoPdfPreviewModel : PageModel
    {
        private readonly IProductoService _productoService;

        public CatalogoPdfPreviewModel(IProductoService productoService)
        {
            _productoService = productoService;
        }

        public IReadOnlyList<ProductoDTO> Productos { get; private set; } = [];
        public string TituloSucursal { get; private set; } = string.Empty;
        public string SucursalId { get; private set; } = string.Empty;
        public string EncabezadoSrc { get; private set; } = string.Empty;
        public string PieSrc { get; private set; } = string.Empty;
        public bool AutoPrint { get; private set; }
        public string Termino { get; private set; } = string.Empty;
        public bool SoloConExistencia { get; private set; } = true;
        public bool EsChulin => SucursalId == "chulin";
        public bool EsCardoza => SucursalId == "cardoza";

        public async Task<IActionResult> OnGetAsync(string? sucursal, string? termino, bool soloConExistencia = true, bool autoPrint = false)
        {
            var configuracion = ObtenerSucursal(sucursal);
            if (configuracion == null)
            {
                return RedirectToPage("/Productos/CatalogoPdf");
            }

            var productos = await _productoService.ObtenerProductosCatalogoAsync(termino, soloConExistencia);

            Productos = productos.ToList();
            SucursalId = configuracion.Id;
            TituloSucursal = configuracion.Nombre;
            EncabezadoSrc = ConvertirImagenBase64(configuracion.EncabezadoArchivo);
            PieSrc = ConvertirImagenBase64(configuracion.PieArchivo);
            AutoPrint = autoPrint;
            Termino = termino?.Trim() ?? string.Empty;
            SoloConExistencia = soloConExistencia;

            return Page();
        }

        private static SucursalCatalogoConfig? ObtenerSucursal(string? sucursal)
        {
            var sucursalId = sucursal?.Trim().ToLowerInvariant();

            return sucursalId switch
            {
                "chulin" => new SucursalCatalogoConfig(
                    "chulin",
                    "Ferreteria Chulin",
                    "encabezado_chulin.png",
                    "pie_pagina_chulin.png"),
                "cardoza" => new SucursalCatalogoConfig(
                    "cardoza",
                    "Ferreteria Ruiz Cardoza",
                    "encabezado_cardoza.png",
                    "pie_cardoza.png"),
                _ => null
            };
        }

        private static string ConvertirImagenBase64(string nombreArchivo)
        {
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "images_pdf_catalog", nombreArchivo);
            var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
            var mimeType = extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };

            var bytes = System.IO.File.ReadAllBytes(ruta);
            return $"data:{mimeType};base64,{Convert.ToBase64String(bytes)}";
        }

        private sealed record SucursalCatalogoConfig(string Id, string Nombre, string EncabezadoArchivo, string PieArchivo);
    }
}
