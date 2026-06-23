-- =============================================================
-- NutriTEC — seed-cleanup.sql
-- Borra todos los datos de prueba en el orden correcto
-- respetando las FK. NO borra el administrador.
-- Ejecutar en SSMS cuando quieran limpiar la base.
-- =============================================================

USE NutriTEC;

PRINT 'Limpiando datos de prueba...'

-- Orden inverso a la creación (hijo antes que padre)
DELETE FROM REGISTRO_PRODUCTO
DELETE FROM REGISTRO_DIARIO
DELETE FROM PLAN_PRODUCTO
DELETE FROM TIEMPO_COMIDA_PLAN
DELETE FROM ASIGNACION_PLAN
DELETE FROM PLAN_ALIMENTACION
DELETE FROM RECETA_PRODUCTO
DELETE FROM RECETA
DELETE FROM PACIENTE_NUTRICIONISTA
DELETE FROM MEDIDA_USUARIO
DELETE FROM PRODUCTO
DELETE FROM USUARIO
DELETE FROM NUTRICIONISTA

-- Verificación
SELECT 'Nutricionistas' AS tabla, COUNT(*) AS total FROM NUTRICIONISTA
UNION ALL
SELECT 'Usuarios',       COUNT(*) FROM USUARIO
UNION ALL
SELECT 'Pacientes',      COUNT(*) FROM PACIENTE_NUTRICIONISTA
UNION ALL
SELECT 'Productos',      COUNT(*) FROM PRODUCTO
UNION ALL
SELECT 'Planes',         COUNT(*) FROM PLAN_ALIMENTACION
UNION ALL
SELECT 'Medidas',        COUNT(*) FROM MEDIDA_USUARIO
UNION ALL
SELECT 'Administradores (intactos)', COUNT(*) FROM ADMINISTRADOR;

PRINT 'Limpieza completada. El administrador se mantuvo intacto.'