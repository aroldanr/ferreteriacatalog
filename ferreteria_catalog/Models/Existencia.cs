using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ferreteria_catalog.Models
{
    public class Existencia
    {
        [Key]
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        [Column("Existencia")]
        public int Stock { get; set; }
    }
}
