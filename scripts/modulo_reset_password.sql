/*
    Script para registrar el modulo "Reset contrasena"
    y asignarlo al rol Admin.

    IDs actuales segun la base:
    - Role.Admin = 1
    - Role.Colab = 2
    - ModuloId es autoincremental, por eso el script busca por Ruta
      y usa el ID generado por la base.
*/

DECLARE @Ruta NVARCHAR(200) = '/Login/ResetPassword';
DECLARE @NombreModulo NVARCHAR(100) = 'Reset contrasena';
DECLARE @RolAdminId INT = 1;
DECLARE @ModuloId INT;

IF NOT EXISTS (
    SELECT 1
    FROM Modulos
    WHERE Ruta = @Ruta
)
BEGIN
    INSERT INTO Modulos (NombreModulo, Ruta)
    VALUES (@NombreModulo, @Ruta);
END;

SELECT @ModuloId = ModuloId
FROM Modulos
WHERE Ruta = @Ruta;

IF NOT EXISTS (
    SELECT 1
    FROM RolModulo
    WHERE RolId = @RolAdminId
      AND ModuloId = @ModuloId
)
BEGIN
    INSERT INTO RolModulo (RolId, ModuloId)
    VALUES (@RolAdminId, @ModuloId);
END;
