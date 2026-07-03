namespace ferreteria_catalog.Models
{
    // Guia rapida para catalogos del sistema.
    // Los roles se manejan con IDs fijos.
    // Los modulos no usan ID fijo aqui porque ModuloId es autoincremental en base de datos.
    public enum RolSistemaId
    {
        Admin = 1,
        Colab = 2
    }

    public static class ModulosSistemaInfo
    {
        public const string CargarImagenNombre = "Cargar Imagen";
        public const string CargarImagenRuta = "/Productos/SubirImagen";

        public const string CargarImagenesLoteNombre = "Cargar ImagenesLote";
        public const string CargarImagenesLoteRuta = "/Productos/CargarImagenesLote";

        public const string ProductosNombre = "Productos";
        public const string ProductosRuta = "/Productos/ProductosAdmin";

        public const string CrearUsuarioNombre = "Crear usuario";
        public const string CrearUsuarioRuta = "/Login/CreateUser";

        public const string ResetPasswordNombre = "Reset contrasena";
        public const string ResetPasswordRuta = "/Login/ResetPassword";
    }
}
