namespace ferreteria_catalog.Models.CustomEntities
{
    public class ProductoDTO
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? UndxBulto { get; set; }
        public string Marca { get; set; }
        public string ImagenURL { get; set; }
        public int Existencia { get; set; }
        public int? MarcaId { get; set; }
    }
}
