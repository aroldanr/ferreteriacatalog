using System.Text.RegularExpressions;

namespace ferreteria_catalog.Models
{
    public class Producto
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? UndxBulto { get; set; }
        public int? MarcaId { get; set; }
        public Marca Marca { get; set; }
        public string? ImagenURL { get; set; }
        public Existencia Existencia { get; set; }
    }
}
