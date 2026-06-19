/*
    Descripción:
        Totaliza las calorías de un plan alimenticio a partir de sus tiempos de comida y productos asociados.

    Entradas:
        @idPlan: identificador del plan alimenticio.

    Salidas:
        Total de calorías del plan como DECIMAL(10,2).

    Restricciones:
        - Se basa en PRODUCTO.id_producto como llave primaria.
        - No devuelve información personal ni credenciales.
*/
CREATE OR ALTER FUNCTION dbo.fn_TotalCaloriasPlan
(
    @idPlan INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @total DECIMAL(10,2);

    SELECT @total = COALESCE(SUM(p.calorias * pp.cantidad_porciones), 0)
    FROM PLAN_ALIMENTACION pa
    INNER JOIN TIEMPO_COMIDA_PLAN tcp ON tcp.id_plan = pa.id_plan
    INNER JOIN PLAN_PRODUCTO pp ON pp.id_tiempo = tcp.id_tiempo
    INNER JOIN PRODUCTO p ON p.id_producto = pp.id_producto
    WHERE pa.id_plan = @idPlan;

    RETURN COALESCE(@total, 0);
END;
GO
