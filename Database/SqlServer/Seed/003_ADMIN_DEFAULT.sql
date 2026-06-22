-- =============================================================
-- NutriTEC — 003_ADMIN_DEFAULT.sql
-- Script de población: administrador por defecto
--
-- IMPORTANTE: Este script inserta el hash de la contraseña
-- "Admin2026!" usando PBKDF2-SHA256.
-- El hash fue generado por el sistema al ejecutar el bootstrap.
-- Para regenerar el admin, usar el bootstrap en appsettings:
--   "BootstrapAdmin": { "Enabled": true, "Email": "...", "Password": "..." }
-- =============================================================

USE NutriTEC;

-- Solo insertar si no existe un administrador
IF NOT EXISTS (SELECT 1 FROM ADMINISTRADOR)
BEGIN
    -- El hash se genera automáticamente al arrancar la API con BootstrapAdmin habilitado.
    -- Este INSERT es un placeholder — ejecutar el bootstrap de la API es el método correcto.
    PRINT 'Para crear el administrador, arrancar la SQL API con BootstrapAdmin habilitado en appsettings.Development.json:'
    PRINT '"BootstrapAdmin": { "Enabled": true, "Email": "admin@nutritec.com", "Password": "Admin2026!" }'
END
ELSE
BEGIN
    PRINT 'Ya existe un administrador en la base de datos.'
    SELECT id_admin, email FROM ADMINISTRADOR;
END