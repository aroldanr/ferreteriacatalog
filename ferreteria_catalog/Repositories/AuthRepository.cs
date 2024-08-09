using Azure;
using ferreteria_catalog.Data;
using ferreteria_catalog.Models;
using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ferreteria_catalog.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly string _secretKey;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthRepository(AppDbContext context, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _secretKey = configuration.GetValue<string>("JwtSecretKey");
            _httpContextAccessor = httpContextAccessor;
        }

        public string GenerateToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol.NombreRol)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Configura la cookie con el token
            _httpContextAccessor.HttpContext.Response.Cookies.Append("jwtToken", tokenString, new CookieOptions
            {
                HttpOnly = false,
                Secure = false,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return tokenString;
        }
        public async Task<Usuario> Authenticate(string nombreUsuario, string password)
        {
            var usuario = await _context.Usuario
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.PasswordHash == password);

            return usuario;
        }

        public async Task<Usuario> GetUsuarioByUsernameAsync(string nombreUsuario)
        {
            var usuario = await _context.Usuario
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

            return usuario;
        }
    }
}
