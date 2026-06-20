/*
    Descripción:
        Recalcula automáticamente el total de calorías de un plan cuando cambian los productos de sus tiempos de comida.

    Evento:
        AFTER INSERT, UPDATE, DELETE sobre PLAN_PRODUCTO.

    Justificación:
        Mantiene consistente PLAN_ALIMENTACION.total_calorias con el detalle de PLAN_PRODUCTO.
*/
CREATE OR ALTER TRIGGER dbo.trg_RecalcularTotalPlan
ON dbo.PLAN_PRODUCTO
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH TiemposAfectados AS
    (
        SELECT id_tiempo FROM inserted
        UNION
        SELECT id_tiempo FROM deleted
    ),
    PlanesAfectados AS
    (
        SELECT DISTINCT tcp.id_plan
        FROM TiemposAfectados ta
        INNER JOIN TIEMPO_COMIDA_PLAN tcp ON tcp.id_tiempo = ta.id_tiempo
    )
    UPDATE pa
    SET total_calorias = CAST(dbo.fn_TotalCaloriasPlan(pa.id_plan) AS DECIMAL(7,2))
    FROM PLAN_ALIMENTACION pa
    INNER JOIN PlanesAfectados afectados ON afectados.id_plan = pa.id_plan;
END;
GO
