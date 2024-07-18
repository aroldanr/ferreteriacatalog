using ferreteria_catalog.Models;
using ferreteria_catalog.Repositories;

namespace ferreteria_catalog.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public string GenerateToken(Usuario usuario)
        {
            return _authRepository.GenerateToken(usuario);
        }
        public async Task<Usuario> Authenticate(string nombreUsuario, string password)
        {
            var usuario = await _authRepository.GetUsuarioByUsernameAsync(nombreUsuario);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            {
                return null;
            }
            return usuario;
        }

        public async Task<Usuario> Register(string username, string password, int rolId)
        {
            var usuario = new Usuario
            {
                NombreUsuario = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                RolId = rolId
            };

            //await _authRepository.CreateUsuarioAsync(usuario);
            return usuario;
        }
    }
}
