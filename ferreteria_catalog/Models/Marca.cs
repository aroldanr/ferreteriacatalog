using System.ComponentModel.DataAnnotations.Schema;

namespace ferreteria_catalog.Models
{
    public class Marca
    {
        public int MarcaId { get; set; }
        [Column("Marca")]
        public string NombreMarca { get; set; }
    }
}
