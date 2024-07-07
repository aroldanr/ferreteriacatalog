namespace ferreteria_catalog.Models
{
    public class Producto
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public string? Codigo { get; set; }
        public int? Stock { get; set; }
        public string? Codigointerno { get; set; }
        public string? Descripcion { get; set; }
        public int? Uncdxbulto { get; set; }
        public string? Marca { get; set; }
        public int? Existencia { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
