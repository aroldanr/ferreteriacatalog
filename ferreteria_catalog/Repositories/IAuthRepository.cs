using ferreteria_catalog.Models;

namespace ferreteria_catalog.Repositories
{
    public interface IAuthRepository
    {
        string GenerateToken(Usuario usuario);
        Task<Usuario> Authenticate(string nombreUsuario, string password);
        Task<Usuario> GetUsuarioByUsernameAsync(string nombreUsuario);
    }
}
