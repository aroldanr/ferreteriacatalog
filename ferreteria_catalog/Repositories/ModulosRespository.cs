using ferreteria_catalog.Data;
using ferreteria_catalog.Models;
using ferreteria_catalog.Models.CustomEntities;
using System.Data.Entity;

namespace ferreteria_catalog.Repositories
{
    public class ModulosRepository : IModulosRepository
    {
        private readonly AppDbContext _context;
        public ModulosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ModuloDto>> GetModulosByRol(string idUsuario)
        {
            try
            {
                // Obtener el usuario desde la base de datos
                var user =  _context.Usuario.Where(x => x.NombreUsuario == idUsuario).FirstOrDefault();

                if (user == null)
                {
                    return Enumerable.Empty<ModuloDto>();  // Devolver lista vacía si el usuario no existe
                }

                // Asegurarse de que la consulta se ejecute directamente en la base de datos
                var modulos = from rm in _context.RolModulo
                                     join m in _context.Modulos on rm.ModuloId equals m.ModuloId
                                     where rm.RolId == user.RolId
                                     select new ModuloDto
                                     {
                                         NombreModulo = m.NombreModulo,
                                         Ruta = m.Ruta
                                     };

                return modulos;
            }
            catch (Exception e)
            {
                // Manejo de excepciones
                throw;
            }
        }


    }
}
