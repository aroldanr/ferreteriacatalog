using ferreteria_catalog.Models;

namespace ferreteria_catalog.Services
{
    public interface IAuthService
    {
        string GenerateToken(Usuario usuario);
        Task<Usuario> Authenticate(string nombreUsuario, string password);
    }
}
