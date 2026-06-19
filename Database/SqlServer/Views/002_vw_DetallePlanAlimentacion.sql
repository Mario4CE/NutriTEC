/*
    Descripción:
        Vista de detalle de planes alimenticios con tiempos de comida, productos y subtotal calórico.

    Uso:
        Consultas de lectura y reportes nutricionales.
*/
CREATE OR ALTER VIEW dbo.vw_DetallePlanAlimentacion
AS
SELECT
    pa.id_plan,
    pa.nombre AS nombre_plan,
    pa.cedula_nutricionista,
    CONCAT(n.nombre, ' ', n.apellidos) AS nombre_nutricionista,
    tcp.id_tiempo,
    tcp.tipo_comida,
    p.id_producto,
    p.codigo_barras,
    p.nombre AS nombre_producto,
    pp.cantidad_porciones,
    p.calorias,
    CAST(p.calorias * pp.cantidad_porciones AS DECIMAL(10,2)) AS subtotal_calorias
FROM PLAN_ALIMENTACION pa
INNER JOIN NUTRICIONISTA n ON n.cedula = pa.cedula_nutricionista
INNER JOIN TIEMPO_COMIDA_PLAN tcp ON tcp.id_plan = pa.id_plan
INNER JOIN PLAN_PRODUCTO pp ON pp.id_tiempo = tcp.id_tiempo
INNER JOIN PRODUCTO p ON p.id_producto = pp.id_producto;
GO
