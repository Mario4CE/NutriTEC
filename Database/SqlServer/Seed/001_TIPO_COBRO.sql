/*
    Descripción:
        Inserta los valores base del catálogo TIPO_COBRO.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Registros idempotentes para los tipos de cobro semanal, mensual y anual.

    Restricciones:
        - Ejecutar después de crear TIPO_COBRO y antes de registrar nutricionistas.
        - No inserta usuarios, nutricionistas, tarjetas, contraseñas ni datos sensibles.
*/
MERGE TIPO_COBRO AS target
USING (VALUES
    ('semanal', 'Semanal', CAST(1 AS BIT)),
    ('mensual', 'Mensual', CAST(1 AS BIT)),
    ('anual', 'Anual', CAST(1 AS BIT))
) AS source (codigo_tipo_cobro, nombre, activo)
ON target.codigo_tipo_cobro = source.codigo_tipo_cobro
WHEN MATCHED THEN
    UPDATE SET
        nombre = source.nombre,
        activo = source.activo
WHEN NOT MATCHED THEN
    INSERT (codigo_tipo_cobro, nombre, activo)
    VALUES (source.codigo_tipo_cobro, source.nombre, source.activo);
