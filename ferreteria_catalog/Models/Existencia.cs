using System.ComponentModel.DataAnnotations;

namespace ferreteria_catalog.Models
{
    public class Existencia
    {
        [Key]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public int Stock { get; set; }
    }
}
