using ferreteria_catalog.Models.CustomEntities;
using ferreteria_catalog.Comunication;

namespace ferreteria_catalog.Services
{
    public interface IUsuarioService
    {
        Task<Response<bool>> CrearUsuarioAsync(CreateUserDto model);
    }
}
