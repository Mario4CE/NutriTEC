-- =============================================================
-- NutriTEC — seed-pacientes.sql
-- Ejecutar en SSMS DESPUÉS de correr seed-test-data.ps1
--
-- Este script asume que los usuarios y nutricionistas ya fueron
-- creados por el script de PowerShell. Asocia pacientes con
-- nutricionistas para que el reporte de cobro muestre datos.
-- =============================================================

USE NutriTEC;

-- =============================================================
-- Variables para obtener los IDs reales
-- =============================================================
DECLARE @idMariana   INT = (SELECT id_usuario FROM USUARIO WHERE email = 'mariana.solis@gmail.com')
DECLARE @idEsteban   INT = (SELECT id_usuario FROM USUARIO WHERE email = 'esteban.vargas@gmail.com')
DECLARE @idSofia     INT = (SELECT id_usuario FROM USUARIO WHERE email = 'sofia.mora@gmail.com')
DECLARE @idCarlos    INT = (SELECT id_usuario FROM USUARIO WHERE email = 'carlos.mendez@gmail.com')

DECLARE @cedPablo    VARCHAR(20) = '101110111'  -- semanal
DECLARE @cedAna      VARCHAR(20) = '202220222'  -- mensual
DECLARE @cedLuis     VARCHAR(20) = '303330333'  -- anual

-- =============================================================
-- Asociar pacientes con nutricionistas
-- Pablo (semanal): 3 pacientes  → cobra $3/semana
-- Ana   (mensual): 4 pacientes  → cobra $16 - 5% = $15.20
-- Luis  (anual):   2 pacientes  → cobra $104 - 10% = $93.60
-- =============================================================
INSERT INTO PACIENTE_NUTRICIONISTA (cedula_nutricionista, id_usuario)
SELECT @cedPablo, @idMariana  WHERE @idMariana IS NOT NULL
UNION ALL
SELECT @cedPablo, @idEsteban  WHERE @idEsteban IS NOT NULL
UNION ALL
SELECT @cedPablo, @idSofia    WHERE @idSofia   IS NOT NULL
UNION ALL
SELECT @cedAna,   @idMariana  WHERE @idMariana IS NOT NULL
UNION ALL
SELECT @cedAna,   @idEsteban  WHERE @idEsteban IS NOT NULL
UNION ALL
SELECT @cedAna,   @idSofia    WHERE @idSofia   IS NOT NULL
UNION ALL
SELECT @cedAna,   @idCarlos   WHERE @idCarlos  IS NOT NULL
UNION ALL
SELECT @cedLuis,  @idMariana  WHERE @idMariana IS NOT NULL
UNION ALL
SELECT @cedLuis,  @idSofia    WHERE @idSofia   IS NOT NULL;

-- =============================================================
-- Plan de alimentación de prueba (Pablo → Mariana)
-- =============================================================
DECLARE @idPlan INT

INSERT INTO PLAN_ALIMENTACION (nombre, cedula_nutricionista, total_calorias)
VALUES ('Plan de adelgazamiento saludable', @cedPablo, 0)

SET @idPlan = SCOPE_IDENTITY()

INSERT INTO TIEMPO_COMIDA_PLAN (id_plan, tipo_comida) VALUES
(@idPlan, 'Desayuno'),
(@idPlan, 'Merienda Mañana'),
(@idPlan, 'Almuerzo'),
(@idPlan, 'Merienda Tarde'),
(@idPlan, 'Cena')

-- Asignar el plan a Mariana por 30 días
IF @idMariana IS NOT NULL AND @idPlan IS NOT NULL
BEGIN
    EXEC sp_AsignarPlanPaciente
        @id_plan    = @idPlan,
        @id_usuario = @idMariana,
        @fecha_inicio = CAST(GETDATE() AS DATE),
        @fecha_fin    = CAST(DATEADD(DAY, 30, GETDATE()) AS DATE)
END

-- =============================================================
-- Medidas de usuario de prueba (Mariana)
-- =============================================================
IF @idMariana IS NOT NULL
BEGIN
    EXEC sp_RegistrarMedidaUsuario
        @id_usuario  = @idMariana,
        @fecha       = CAST(DATEADD(DAY, -30, GETDATE()) AS DATE),
        @peso_kg     = 70.0,
        @estatura_cm = 165.0,
        @cintura     = 75.0,
        @cuello      = 33.0,
        @caderas     = 97.0,
        @pct_musculo = 37.0,
        @pct_grasa   = 24.0

    EXEC sp_RegistrarMedidaUsuario
        @id_usuario  = @idMariana,
        @fecha       = CAST(DATEADD(DAY, -15, GETDATE()) AS DATE),
        @peso_kg     = 69.0,
        @estatura_cm = 165.0,
        @cintura     = 73.5,
        @cuello      = 32.5,
        @caderas     = 96.0,
        @pct_musculo = 37.5,
        @pct_grasa   = 23.0

    EXEC sp_RegistrarMedidaUsuario
        @id_usuario  = @idMariana,
        @fecha       = CAST(GETDATE() AS DATE),
        @peso_kg     = 68.0,
        @estatura_cm = 165.0,
        @cintura     = 72.0,
        @cuello      = 32.0,
        @caderas     = 95.0,
        @pct_musculo = 38.0,
        @pct_grasa   = 22.0
END

-- =============================================================
-- Verificación final
-- =============================================================
SELECT 'Nutricionistas' AS tabla, COUNT(*) AS total FROM NUTRICIONISTA
UNION ALL
SELECT 'Usuarios',       COUNT(*) FROM USUARIO
UNION ALL
SELECT 'Pacientes asociados', COUNT(*) FROM PACIENTE_NUTRICIONISTA
UNION ALL
SELECT 'Productos pendientes', COUNT(*) FROM PRODUCTO WHERE aprobado = 0
UNION ALL
SELECT 'Planes',         COUNT(*) FROM PLAN_ALIMENTACION
UNION ALL
SELECT 'Medidas',        COUNT(*) FROM MEDIDA_USUARIO;

PRINT 'Seed completado. Revisá el reporte de cobro en la vista admin.'