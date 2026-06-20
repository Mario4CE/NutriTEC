/*
    Descripción:
        Calcula el índice de masa corporal a partir del peso en kilogramos y la estatura en centímetros.

    Entradas:
        @pesoKg: peso en kilogramos.
        @estaturaCm: estatura en centímetros.

    Salidas:
        Valor DECIMAL(5,2) con el IMC calculado o NULL si los datos no son válidos.

    Restricciones:
        - No accede a datos sensibles.
        - Puede reutilizarse desde procedimientos almacenados que actualicen medidas de usuarios.
*/
CREATE OR ALTER FUNCTION dbo.fn_CalcularImc
(
    @pesoKg DECIMAL(5,2),
    @estaturaCm DECIMAL(5,2)
)
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @imc DECIMAL(5,2);
    DECLARE @estaturaMetros DECIMAL(8,4);

    IF @pesoKg IS NULL OR @pesoKg <= 0 OR @estaturaCm IS NULL OR @estaturaCm <= 0
    BEGIN
        RETURN NULL;
    END;

    SET @estaturaMetros = @estaturaCm / 100.0;
    SET @imc = ROUND(@pesoKg / NULLIF(@estaturaMetros * @estaturaMetros, 0), 2);

    RETURN @imc;
END;
GO
