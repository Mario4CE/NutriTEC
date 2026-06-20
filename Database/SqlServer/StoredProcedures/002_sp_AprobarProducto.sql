/*
    Descripción:
        Aprueba un producto pendiente después de validar que sus valores nutricionales sean coherentes.

    Entradas:
        @idProducto: identificador del producto.

    Salidas:
        Producto aprobado sin exponer entidades de infraestructura.

    Restricciones:
        - No aprueba productos con datos nutricionales negativos.
        - Usa transacción y validaciones explícitas.
*/
CREATE OR ALTER PROCEDURE dbo.sp_AprobarProducto
    @idProducto UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @idProducto IS NULL
    BEGIN
        THROW 50010, 'El identificador del producto es obligatorio.', 1;
    END;

    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM PRODUCTO WITH (UPDLOCK, HOLDLOCK) WHERE id_producto = @idProducto)
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50011, 'El producto no existe.', 1;
    END;

    IF EXISTS (
        SELECT 1
        FROM PRODUCTO
        WHERE id_producto = @idProducto
          AND (calorias < 0 OR proteinas < 0 OR carbohidratos < 0 OR grasas < 0)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50012, 'El producto tiene valores nutricionales inválidos.', 1;
    END;

    UPDATE PRODUCTO
    SET aprobado = 1
    WHERE id_producto = @idProducto;

    COMMIT TRANSACTION;

    SELECT id_producto, nombre, codigo_barras, calorias, proteinas, carbohidratos, grasas, aprobado, fecha_creacion_utc
    FROM PRODUCTO
    WHERE id_producto = @idProducto;
END;
GO
