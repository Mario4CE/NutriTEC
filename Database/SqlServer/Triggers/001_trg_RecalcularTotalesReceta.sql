/*
    Descripción:
        Recalcula automáticamente los totales nutricionales de una receta cuando cambian sus productos.

    Evento:
        AFTER INSERT, UPDATE, DELETE sobre RECETA_PRODUCTO.

    Justificación:
        Mantiene consistentes los totales agregados de RECETA sin duplicar cálculos manuales en cada operación.
*/
CREATE OR ALTER TRIGGER dbo.trg_RecalcularTotalesReceta
ON dbo.RECETA_PRODUCTO
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH RecetasAfectadas AS
    (
        SELECT id_receta FROM inserted
        UNION
        SELECT id_receta FROM deleted
    ),
    Totales AS
    (
        SELECT
            ra.id_receta,
            COALESCE(SUM(p.calorias * rp.cantidad_porciones), 0) AS total_calorias,
            COALESCE(SUM(p.carbohidratos * rp.cantidad_porciones), 0) AS total_carbohidratos,
            COALESCE(SUM(p.proteinas * rp.cantidad_porciones), 0) AS total_proteina,
            COALESCE(SUM(p.grasas * rp.cantidad_porciones), 0) AS total_grasa
        FROM RecetasAfectadas ra
        LEFT JOIN RECETA_PRODUCTO rp ON rp.id_receta = ra.id_receta
        LEFT JOIN PRODUCTO p ON p.id_producto = rp.id_producto
        GROUP BY ra.id_receta
    )
    UPDATE r
    SET total_calorias = CAST(t.total_calorias AS DECIMAL(7,2)),
        total_carbohidratos = CAST(t.total_carbohidratos AS DECIMAL(7,2)),
        total_proteina = CAST(t.total_proteina AS DECIMAL(7,2)),
        total_grasa = CAST(t.total_grasa AS DECIMAL(7,2))
    FROM RECETA r
    INNER JOIN Totales t ON t.id_receta = r.id_receta;
END;
GO
