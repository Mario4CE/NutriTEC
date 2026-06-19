/*
    Descripción:
        Vista agregada de consumo diario por usuario y fecha.

    Uso:
        Reportes de seguimiento de calorías y macronutrientes consumidos.
*/
CREATE OR ALTER VIEW dbo.vw_ResumenRegistroDiario
AS
SELECT
    rd.id_usuario,
    CONCAT(u.nombre, ' ', u.apellidos) AS nombre_usuario,
    rd.fecha,
    SUM(p.calorias * rp.cantidad_porciones) AS total_calorias,
    SUM(p.proteinas * rp.cantidad_porciones) AS total_proteinas,
    SUM(p.carbohidratos * rp.cantidad_porciones) AS total_carbohidratos,
    SUM(p.grasas * rp.cantidad_porciones) AS total_grasas,
    COUNT_BIG(*) AS cantidad_productos_registrados
FROM REGISTRO_DIARIO rd
INNER JOIN USUARIO u ON u.id_usuario = rd.id_usuario
INNER JOIN REGISTRO_PRODUCTO rp ON rp.id_registro = rd.id_registro
INNER JOIN PRODUCTO p ON p.id_producto = rp.id_producto
GROUP BY rd.id_usuario, u.nombre, u.apellidos, rd.fecha;
GO
