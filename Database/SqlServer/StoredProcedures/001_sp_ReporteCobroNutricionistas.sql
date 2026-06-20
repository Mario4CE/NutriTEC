/*
    Descripción:
        Reporte obligatorio de cobro para administradores. Calcula cuánto debe cobrarse a cada nutricionista según tipo de cobro,
        cantidad de pacientes asociados y descuentos: 5% mensual, 10% anual.

    Entradas:
        @montoBasePorPaciente: monto base que se cobra por paciente.
        @incluirSinPacientes: permite incluir nutricionistas sin pacientes asociados.

    Salidas:
        Una tabla con pacientes, subtotal, descuento y total a cobrar por nutricionista.

    Restricciones:
        - No expone password_hash ni tarjeta_credito.
        - Usa lógica real: variables, tabla temporal, cursor y control de flujo.
*/
CREATE OR ALTER PROCEDURE dbo.sp_ReporteCobroNutricionistas
    @montoBasePorPaciente DECIMAL(10,2),
    @incluirSinPacientes BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @montoBasePorPaciente IS NULL OR @montoBasePorPaciente <= 0
    BEGIN
        THROW 50001, 'El monto base por paciente debe ser mayor que cero.', 1;
    END;

    DECLARE @cedula VARCHAR(20);
    DECLARE @nombreCompleto VARCHAR(210);
    DECLARE @tipoCobro VARCHAR(10);
    DECLARE @cantidadPacientes INT;
    DECLARE @factorPeriodo DECIMAL(10,2);
    DECLARE @porcentajeDescuento DECIMAL(5,2);
    DECLARE @subtotal DECIMAL(10,2);
    DECLARE @descuento DECIMAL(10,2);
    DECLARE @total DECIMAL(10,2);

    CREATE TABLE #ReporteCobro
    (
        cedula_nutricionista VARCHAR(20) NOT NULL,
        nombre_nutricionista VARCHAR(210) NOT NULL,
        tipo_cobro VARCHAR(10) NOT NULL,
        cantidad_pacientes INT NOT NULL,
        monto_base_por_paciente DECIMAL(10,2) NOT NULL,
        subtotal DECIMAL(10,2) NOT NULL,
        porcentaje_descuento DECIMAL(5,2) NOT NULL,
        monto_descuento DECIMAL(10,2) NOT NULL,
        total_cobrar DECIMAL(10,2) NOT NULL
    );

    DECLARE nutricionistas_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT
            reporte.cedula,
            reporte.nombre_completo,
            reporte.tipo_cobro,
            reporte.cantidad_pacientes
        FROM
        (
            SELECT
                n.cedula,
                CONCAT(n.nombre, ' ', n.apellidos) AS nombre_completo,
                n.tipo_cobro,
                COUNT(pn.id_usuario) AS cantidad_pacientes
            FROM NUTRICIONISTA n
            LEFT JOIN PACIENTE_NUTRICIONISTA pn ON pn.cedula_nutricionista = n.cedula
            GROUP BY n.cedula, n.nombre, n.apellidos, n.tipo_cobro
        ) AS reporte
        WHERE @incluirSinPacientes = 1 OR reporte.cantidad_pacientes > 0;

    OPEN nutricionistas_cursor;
    FETCH NEXT FROM nutricionistas_cursor INTO @cedula, @nombreCompleto, @tipoCobro, @cantidadPacientes;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @factorPeriodo = CASE @tipoCobro
            WHEN 'semanal' THEN 1
            WHEN 'mensual' THEN 4
            WHEN 'anual' THEN 52
            ELSE 1
        END;

        SET @porcentajeDescuento = CASE @tipoCobro
            WHEN 'mensual' THEN 0.05
            WHEN 'anual' THEN 0.10
            ELSE 0
        END;

        SET @subtotal = ROUND(@cantidadPacientes * @montoBasePorPaciente * @factorPeriodo, 2);
        SET @descuento = ROUND(@subtotal * @porcentajeDescuento, 2);
        SET @total = @subtotal - @descuento;

        INSERT INTO #ReporteCobro
        VALUES (@cedula, @nombreCompleto, @tipoCobro, @cantidadPacientes, @montoBasePorPaciente, @subtotal, @porcentajeDescuento, @descuento, @total);

        FETCH NEXT FROM nutricionistas_cursor INTO @cedula, @nombreCompleto, @tipoCobro, @cantidadPacientes;
    END;

    CLOSE nutricionistas_cursor;
    DEALLOCATE nutricionistas_cursor;

    SELECT *
    FROM #ReporteCobro
    ORDER BY nombre_nutricionista;
END;
GO
