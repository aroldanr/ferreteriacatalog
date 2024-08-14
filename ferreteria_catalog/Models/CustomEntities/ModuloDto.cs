namespace ferreteria_catalog.Models.CustomEntities
{
    public class ModuloDto
    {
        public int ModuloId { get; set; }
        public string NombreModulo { get; set; }
        public ICollection<RolModulo> RolModulo { get; set; }
        public string? Ruta { get; set; }
    }
}
