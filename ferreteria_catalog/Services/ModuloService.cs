using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;
using ferreteria_catalog.Repositories;

namespace ferreteria_catalog.Services
{
    public class ModuloService : IModuloService
    {
        private readonly IModulosRepository _modulosRepository;

        public ModuloService(IModulosRepository modulosRepository)
        {
            _modulosRepository = modulosRepository;
        }

        public async Task<IEnumerable<ModuloDto>> GetModulosByRol(string idUsuario)
        {
            return await _modulosRepository.GetModulosByRol(idUsuario);
        }
    }
}
