using ferreteria_catalog.Comunication;
using ferreteria_catalog.Models.CustomEntities;

namespace ferreteria_catalog.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Response<bool>> CrearUsuarioAsync(CreateUserDto model);
    }
}
