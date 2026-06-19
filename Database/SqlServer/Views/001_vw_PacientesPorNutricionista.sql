/*
    Descripción:
        Vista de lectura para consultar pacientes asociados a cada nutricionista.

    Uso:
        Reportes administrativos y consultas de seguimiento sin exponer contraseñas ni tarjetas.
*/
CREATE OR ALTER VIEW dbo.vw_PacientesPorNutricionista
AS
SELECT
    n.cedula AS cedula_nutricionista,
    CONCAT(n.nombre, ' ', n.apellidos) AS nombre_nutricionista,
    n.email AS email_nutricionista,
    u.id_usuario,
    CONCAT(u.nombre, ' ', u.apellidos) AS nombre_paciente,
    u.email AS email_paciente,
    u.pais,
    u.calorias_diarias_max
FROM PACIENTE_NUTRICIONISTA pn
INNER JOIN NUTRICIONISTA n ON n.cedula = pn.cedula_nutricionista
INNER JOIN USUARIO u ON u.id_usuario = pn.id_usuario;
GO
