/*
    Descripción:
        Registra una medición corporal de usuario y actualiza el resumen actual del usuario, incluido IMC calculado.

    Entradas:
        Datos de medida corporal y estatura en centímetros para calcular IMC.

    Salidas:
        Medida creada y resumen actualizado del usuario.

    Restricciones:
        - Usa dbo.fn_CalcularImc.
        - Usa transacción porque inserta historial y actualiza estado actual del usuario.
*/
CREATE OR ALTER PROCEDURE dbo.sp_RegistrarMedidaUsuario
    @idUsuario INT,
    @fecha DATE,
    @pesoKg DECIMAL(5,2),
    @estaturaCm DECIMAL(5,2),
    @cintura DECIMAL(5,2) = NULL,
    @cuello DECIMAL(5,2) = NULL,
    @caderas DECIMAL(5,2) = NULL,
    @pctMusculo DECIMAL(5,2) = NULL,
    @pctGrasa DECIMAL(5,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @imc DECIMAL(5,2) = dbo.fn_CalcularImc(@pesoKg, @estaturaCm);
    DECLARE @idMedida INT;

    IF @fecha IS NULL
    BEGIN
        THROW 50030, 'La fecha de la medida es obligatoria.', 1;
    END;

    IF @imc IS NULL
    BEGIN
        THROW 50031, 'Peso o estatura inválidos para calcular IMC.', 1;
    END;

    IF NOT EXISTS (SELECT 1 FROM USUARIO WHERE id_usuario = @idUsuario)
    BEGIN
        THROW 50032, 'El usuario no existe.', 1;
    END;

    BEGIN TRANSACTION;

    INSERT INTO MEDIDA_USUARIO (id_usuario, fecha, cintura, cuello, caderas, pct_musculo, pct_grasa)
    VALUES (@idUsuario, @fecha, @cintura, @cuello, @caderas, @pctMusculo, @pctGrasa);

    SET @idMedida = CONVERT(INT, SCOPE_IDENTITY());

    UPDATE USUARIO
    SET peso = @pesoKg,
        imc = @imc,
        cintura = @cintura,
        cuello = @cuello,
        caderas = @caderas,
        pct_musculo = @pctMusculo,
        pct_grasa = @pctGrasa
    WHERE id_usuario = @idUsuario;

    COMMIT TRANSACTION;

    SELECT
        @idMedida AS id_medida,
        @idUsuario AS id_usuario,
        @fecha AS fecha,
        @pesoKg AS peso,
        @imc AS imc;
END;
GO
