using Azure.Identity;
using ferreteria_catalog.Comunication;
using ferreteria_catalog.Data;
using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;
using System.Data.Entity;

namespace ferreteria_catalog.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Response<bool>> CrearUsuarioAsync(CreateUserDto model)
        {
            var response = new Response<bool>();
            try
            {
                if (VerificarUsuarioExistente(model.NombreUsuario))
                {
                    response.IsSuccess = false;
                    response.Message = "El usuario ya existe en el sistema.";
                    response.Data = false;
                    return response;
                }

                var usuario = new Usuario
                {
                    NombreUsuario = model.NombreUsuario,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    RolId = model.RolId
                };

                _context.Usuario.Add(usuario);
                await _context.SaveChangesAsync();
                response.IsSuccess = true;
                response.Message = "Usuario creado exitosamente.";
                response.Data = true;
            }
            catch (Exception e)
            {
                response.IsSuccess = false;
                response.Message = $"Ocurrió un error al crear el usuario: {e.Message}";
                response.Data = false;
            }           

            return response;
        }

        public bool VerificarUsuarioExistente(string userName)
        {
            return _context.Usuario.Any(x => x.NombreUsuario == userName);
        }

    }
}
