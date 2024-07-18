namespace ferreteria_catalog.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public string PasswordHash { get; set; }
        public int RolId { get; set; }
        public Rol Rol { get; set; }
    }
}
