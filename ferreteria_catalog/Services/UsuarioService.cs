using ferreteria_catalog.Comunication;
using ferreteria_catalog.Models.CustomEntities;
using ferreteria_catalog.Repositories;

namespace ferreteria_catalog.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Response<bool>> CrearUsuarioAsync(CreateUserDto model)
        {
            return await _usuarioRepository.CrearUsuarioAsync(model);
        }
    }
}
