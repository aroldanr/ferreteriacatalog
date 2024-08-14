using ferreteria_catalog.Models.CustomEntities;

namespace ferreteria_catalog.Services
{
    public interface IModuloService
    {
        Task<IEnumerable<ModuloDto>> GetModulosByRol(string idUsuario);
    }
}
