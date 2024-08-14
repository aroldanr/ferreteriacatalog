using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;

namespace ferreteria_catalog.Repositories
{
    public interface IModulosRepository
    {
        Task<IEnumerable<ModuloDto>> GetModulosByRol(string idUsuario);
    }
}
