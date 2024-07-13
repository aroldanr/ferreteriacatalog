namespace ferreteria_catalog.Dtos
{
    public class PaginacionResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
    }
}
