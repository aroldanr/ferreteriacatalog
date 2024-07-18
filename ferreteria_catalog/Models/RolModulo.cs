namespace ferreteria_catalog.Models
{
    public class RolModulo
    {
        public int RolId { get; set; }
        public Rol Rol { get; set; }
        public int ModuloId { get; set; }
        public Modulo Modulo { get; set; }
    }
}
