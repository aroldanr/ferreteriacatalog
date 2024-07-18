namespace ferreteria_catalog.Models
{
    public class Modulo
    {
        public int ModuloId { get; set; }
        public string NombreModulo { get; set; }
        public ICollection<RolModulo> RolModulo { get; set; }
    }
}
