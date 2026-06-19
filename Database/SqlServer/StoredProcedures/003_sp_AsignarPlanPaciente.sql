/*
    Descripción:
        Asigna un plan alimenticio a un paciente validando fechas y relación previa paciente-nutricionista.

    Entradas:
        @idPlan, @idUsuario, @fechaInicio, @fechaFin.

    Salidas:
        Identificador de la asignación creada.

    Restricciones:
        - La fecha fin no puede ser menor a la fecha inicio.
        - El usuario debe estar asociado al nutricionista propietario del plan.
        - Usa transacción y bloqueo para evitar duplicados activos exactos.
*/
CREATE OR ALTER PROCEDURE dbo.sp_AsignarPlanPaciente
    @idPlan INT,
    @idUsuario INT,
    @fechaInicio DATE,
    @fechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @cedulaNutricionista VARCHAR(20);
    DECLARE @idAsignacion INT;

    IF @fechaInicio IS NULL OR @fechaFin IS NULL OR @fechaFin < @fechaInicio
    BEGIN
        THROW 50020, 'El rango de fechas de la asignación no es válido.', 1;
    END;

    SELECT @cedulaNutricionista = cedula_nutricionista
    FROM PLAN_ALIMENTACION
    WHERE id_plan = @idPlan;

    IF @cedulaNutricionista IS NULL
    BEGIN
        THROW 50021, 'El plan alimenticio no existe.', 1;
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM PACIENTE_NUTRICIONISTA
        WHERE cedula_nutricionista = @cedulaNutricionista
          AND id_usuario = @idUsuario
    )
    BEGIN
        THROW 50022, 'El paciente no está asociado al nutricionista del plan.', 1;
    END;

    BEGIN TRANSACTION;

    IF EXISTS (
        SELECT 1
        FROM ASIGNACION_PLAN WITH (UPDLOCK, HOLDLOCK)
        WHERE id_plan = @idPlan
          AND id_usuario = @idUsuario
          AND fecha_inicio = @fechaInicio
          AND fecha_fin = @fechaFin
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50023, 'Ya existe una asignación igual para ese paciente.', 1;
    END;

    INSERT INTO ASIGNACION_PLAN (id_plan, id_usuario, fecha_inicio, fecha_fin)
    VALUES (@idPlan, @idUsuario, @fechaInicio, @fechaFin);

    SET @idAsignacion = CONVERT(INT, SCOPE_IDENTITY());

    COMMIT TRANSACTION;

    SELECT @idAsignacion AS id_asignacion;
END;
GO
