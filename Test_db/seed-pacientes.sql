-- =============================================================
-- NutriTEC — seed-pacientes.sql
-- Ejecutar en SSMS DESPUÉS de correr seed-test-data.ps1
-- =============================================================

USE NutriTEC;

DECLARE @idMariana   INT = (SELECT id_usuario FROM USUARIO WHERE email = 'mariana.solis@gmail.com')
DECLARE @idEsteban   INT = (SELECT id_usuario FROM USUARIO WHERE email = 'esteban.vargas@gmail.com')
DECLARE @idSofia     INT = (SELECT id_usuario FROM USUARIO WHERE email = 'sofia.mora@gmail.com')
DECLARE @idCarlos    INT = (SELECT id_usuario FROM USUARIO WHERE email = 'carlos.mendez@gmail.com')

DECLARE @cedPablo    VARCHAR(20) = '101110111'
DECLARE @cedAna      VARCHAR(20) = '202220222'
DECLARE @cedLuis     VARCHAR(20) = '303330333'

-- Asociar pacientes
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

-- Plan de alimentación
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

-- Fechas calculadas antes de pasarlas a los SPs
DECLARE @hoy    DATE = CAST(GETDATE() AS DATE)
DECLARE @hoy30  DATE = CAST(DATEADD(DAY,  30, GETDATE()) AS DATE)
DECLARE @hace30 DATE = CAST(DATEADD(DAY, -30, GETDATE()) AS DATE)
DECLARE @hace15 DATE = CAST(DATEADD(DAY, -15, GETDATE()) AS DATE)

-- Asignar plan a Mariana (sp_AsignarPlanPaciente)
IF @idMariana IS NOT NULL AND @idPlan IS NOT NULL
BEGIN
    EXEC dbo.sp_AsignarPlanPaciente
        @idPlan      = @idPlan,
        @idUsuario   = @idMariana,
        @fechaInicio = @hoy,
        @fechaFin    = @hoy30
END

-- Registrar medidas de Mariana (sp_RegistrarMedidaUsuario)
IF @idMariana IS NOT NULL
BEGIN
    EXEC dbo.sp_RegistrarMedidaUsuario
        @idUsuario = @idMariana, @fecha = @hace30,
        @pesoKg = 70.0, @estaturaCm = 165.0,
        @cintura = 75.0, @cuello = 33.0, @caderas = 97.0,
        @pctMusculo = 37.0, @pctGrasa = 24.0

    EXEC dbo.sp_RegistrarMedidaUsuario
        @idUsuario = @idMariana, @fecha = @hace15,
        @pesoKg = 69.0, @estaturaCm = 165.0,
        @cintura = 73.5, @cuello = 32.5, @caderas = 96.0,
        @pctMusculo = 37.5, @pctGrasa = 23.0

    EXEC dbo.sp_RegistrarMedidaUsuario
        @idUsuario = @idMariana, @fecha = @hoy,
        @pesoKg = 68.0, @estaturaCm = 165.0,
        @cintura = 72.0, @cuello = 32.0, @caderas = 95.0,
        @pctMusculo = 38.0, @pctGrasa = 22.0
END

-- Verificación
SELECT 'Nutricionistas'       AS tabla, COUNT(*) AS total FROM NUTRICIONISTA
UNION ALL
SELECT 'Usuarios',             COUNT(*) FROM USUARIO
UNION ALL
SELECT 'Pacientes asociados',  COUNT(*) FROM PACIENTE_NUTRICIONISTA
UNION ALL
SELECT 'Productos pendientes', COUNT(*) FROM PRODUCTO WHERE aprobado = 0
UNION ALL
SELECT 'Planes',               COUNT(*) FROM PLAN_ALIMENTACION
UNION ALL
SELECT 'Medidas',              COUNT(*) FROM MEDIDA_USUARIO;

PRINT 'Seed completado.'