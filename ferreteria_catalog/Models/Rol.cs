namespace ferreteria_catalog.Models
{
    public class Rol
    {
        public int RolId { get; set; }
        public string NombreRol { get; set; }
        public ICollection<Usuario> Usuario { get; set; }
        public ICollection<RolModulo> RolModulo { get; set; }
    }
}
