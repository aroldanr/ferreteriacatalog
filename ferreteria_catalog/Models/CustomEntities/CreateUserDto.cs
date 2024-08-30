namespace ferreteria_catalog.Models.CustomEntities
{
    public class CreateUserDto
    {
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
    }
}
