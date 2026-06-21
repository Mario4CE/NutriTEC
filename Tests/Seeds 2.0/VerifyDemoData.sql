/*
    NutriTEC — verificación manual de datos demo y tablas SQL.

    Uso:
    1. Ejecutar primero Database/SqlServer y Tests/SeedDemoData.sql sobre la base NutriTec.
    2. Ejecutar este archivo en SSMS/Azure Data Studio/sqlcmd sobre la misma base.

    Este script NO modifica datos. Solo lista tablas, columnas clave, conteos y muestras
    para confirmar que los cambios recientes funcionan.
*/

SET NOCOUNT ON;

PRINT '============================================================';
PRINT '1) TABLAS DE USUARIO EN LA BASE ACTUAL';
PRINT '============================================================';
SELECT
    TABLE_SCHEMA AS esquema,
    TABLE_NAME AS tabla
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_SCHEMA, TABLE_NAME;

PRINT '============================================================';
PRINT '2) COLUMNAS DE PRODUCTO — debe incluir columnas nutricionales ampliadas';
PRINT '============================================================';
SELECT
    COLUMN_NAME AS columna,
    DATA_TYPE AS tipo,
    IS_NULLABLE AS permite_null,
    CHARACTER_MAXIMUM_LENGTH AS max_texto,
    NUMERIC_PRECISION AS precision_numerica,
    NUMERIC_SCALE AS escala_numerica
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = 'PRODUCTO'
ORDER BY ORDINAL_POSITION;

PRINT '============================================================';
PRINT '3) CONTEO GENERAL POR TABLA PRINCIPAL';
PRINT '============================================================';
SELECT 'ADMINISTRADOR' AS tabla, COUNT(*) AS total FROM dbo.ADMINISTRADOR
UNION ALL SELECT 'TIPO_COBRO', COUNT(*) FROM dbo.TIPO_COBRO
UNION ALL SELECT 'USUARIO', COUNT(*) FROM dbo.USUARIO
UNION ALL SELECT 'NUTRICIONISTA', COUNT(*) FROM dbo.NUTRICIONISTA
UNION ALL SELECT 'PACIENTE_NUTRICIONISTA', COUNT(*) FROM dbo.PACIENTE_NUTRICIONISTA
UNION ALL SELECT 'MEDIDA_USUARIO', COUNT(*) FROM dbo.MEDIDA_USUARIO
UNION ALL SELECT 'PRODUCTO', COUNT(*) FROM dbo.PRODUCTO
UNION ALL SELECT 'RECETA', COUNT(*) FROM dbo.RECETA
UNION ALL SELECT 'RECETA_PRODUCTO', COUNT(*) FROM dbo.RECETA_PRODUCTO
UNION ALL SELECT 'PLAN_ALIMENTACION', COUNT(*) FROM dbo.PLAN_ALIMENTACION
UNION ALL SELECT 'TIEMPO_COMIDA_PLAN', COUNT(*) FROM dbo.TIEMPO_COMIDA_PLAN
UNION ALL SELECT 'PLAN_PRODUCTO', COUNT(*) FROM dbo.PLAN_PRODUCTO
UNION ALL SELECT 'ASIGNACION_PLAN', COUNT(*) FROM dbo.ASIGNACION_PLAN
UNION ALL SELECT 'REGISTRO_DIARIO', COUNT(*) FROM dbo.REGISTRO_DIARIO
UNION ALL SELECT 'REGISTRO_PRODUCTO', COUNT(*) FROM dbo.REGISTRO_PRODUCTO
ORDER BY tabla;

PRINT '============================================================';
PRINT '4) CONTEO DE DATOS DEMO — cada fila esperada debe ser >= 5 salvo catálogo TIPO_COBRO';
PRINT '============================================================';
SELECT 'ADMINISTRADOR demo' AS entidad, COUNT(*) AS total FROM dbo.ADMINISTRADOR WHERE email LIKE 'admin.demo%@nutritec.test'
UNION ALL SELECT 'CLIENTE demo', COUNT(*) FROM dbo.USUARIO WHERE email LIKE 'cliente.demo%@nutritec.test'
UNION ALL SELECT 'NUTRICIONISTA demo', COUNT(*) FROM dbo.NUTRICIONISTA WHERE cedula LIKE 'NUT-DEMO-%'
UNION ALL SELECT 'PRODUCTO demo aprobado', COUNT(*) FROM dbo.PRODUCTO WHERE codigo_barras LIKE '77000000000%' AND aprobado = 1
UNION ALL SELECT 'PACIENTE_NUTRICIONISTA demo', COUNT(*) FROM dbo.PACIENTE_NUTRICIONISTA pn INNER JOIN dbo.NUTRICIONISTA n ON n.cedula = pn.cedula_nutricionista WHERE n.cedula LIKE 'NUT-DEMO-%'
UNION ALL SELECT 'MEDIDA_USUARIO demo', COUNT(*) FROM dbo.MEDIDA_USUARIO m INNER JOIN dbo.USUARIO u ON u.id_usuario = m.id_usuario WHERE u.email LIKE 'cliente.demo%@nutritec.test'
UNION ALL SELECT 'RECETA demo', COUNT(*) FROM dbo.RECETA WHERE nombre LIKE 'Receta demo %'
UNION ALL SELECT 'RECETA_PRODUCTO demo', COUNT(*) FROM dbo.RECETA_PRODUCTO rp INNER JOIN dbo.RECETA r ON r.id_receta = rp.id_receta WHERE r.nombre LIKE 'Receta demo %'
UNION ALL SELECT 'PLAN_ALIMENTACION demo', COUNT(*) FROM dbo.PLAN_ALIMENTACION WHERE nombre LIKE 'Plan demo %'
UNION ALL SELECT 'TIEMPO_COMIDA_PLAN demo', COUNT(*) FROM dbo.TIEMPO_COMIDA_PLAN t INNER JOIN dbo.PLAN_ALIMENTACION p ON p.id_plan = t.id_plan WHERE p.nombre LIKE 'Plan demo %'
UNION ALL SELECT 'PLAN_PRODUCTO demo', COUNT(*) FROM dbo.PLAN_PRODUCTO pp INNER JOIN dbo.TIEMPO_COMIDA_PLAN t ON t.id_tiempo = pp.id_tiempo INNER JOIN dbo.PLAN_ALIMENTACION p ON p.id_plan = t.id_plan WHERE p.nombre LIKE 'Plan demo %'
UNION ALL SELECT 'ASIGNACION_PLAN demo', COUNT(*) FROM dbo.ASIGNACION_PLAN a INNER JOIN dbo.USUARIO u ON u.id_usuario = a.id_usuario WHERE u.email LIKE 'cliente.demo%@nutritec.test'
UNION ALL SELECT 'REGISTRO_DIARIO demo', COUNT(*) FROM dbo.REGISTRO_DIARIO r INNER JOIN dbo.USUARIO u ON u.id_usuario = r.id_usuario WHERE u.email LIKE 'cliente.demo%@nutritec.test'
UNION ALL SELECT 'REGISTRO_PRODUCTO demo', COUNT(*) FROM dbo.REGISTRO_PRODUCTO rp INNER JOIN dbo.REGISTRO_DIARIO r ON r.id_registro = rp.id_registro INNER JOIN dbo.USUARIO u ON u.id_usuario = r.id_usuario WHERE u.email LIKE 'cliente.demo%@nutritec.test';

PRINT '============================================================';
PRINT '5) PRODUCTOS APROBADOS — valida el cambio de filtro público';
PRINT '============================================================';
SELECT
    id_producto,
    nombre,
    codigo_barras,
    porcion_g_ml,
    calorias,
    proteinas,
    carbohidratos,
    grasas,
    sodio_mg,
    vitaminas,
    calcio_mg,
    hierro_mg,
    aprobado
FROM dbo.PRODUCTO
WHERE aprobado = 1
ORDER BY nombre;

PRINT '============================================================';
PRINT '6) CLIENTES DEMO REALISTAS Y CREDENCIALES';
PRINT '============================================================';
SELECT
    id_usuario,
    nombre,
    apellidos,
    email,
    'Cliente2026!' AS contrasena_demo,
    peso,
    imc,
    pais,
    calorias_diarias_max
FROM dbo.USUARIO
WHERE email LIKE 'cliente.demo%@nutritec.test'
ORDER BY email;

PRINT '============================================================';
PRINT '7) NUTRICIONISTAS DEMO REALISTAS Y CREDENCIALES';
PRINT '============================================================';
SELECT
    cedula,
    nombre,
    apellidos,
    codigo_nutricionista,
    email,
    'Nutricion2026!' AS contrasena_demo,
    tipo_cobro,
    direccion
FROM dbo.NUTRICIONISTA
WHERE cedula LIKE 'NUT-DEMO-%'
ORDER BY cedula;

PRINT '============================================================';
PRINT '8) PLANES ASIGNADOS POR CLIENTE — prueba GET /api/planes/usuario/{idUsuario}';
PRINT '============================================================';
SELECT
    u.id_usuario,
    CONCAT(u.nombre, ' ', u.apellidos) AS cliente,
    p.id_plan,
    p.nombre AS plan_nombre,
    p.cedula_nutricionista,
    p.total_calorias,
    a.fecha_inicio,
    a.fecha_fin
FROM dbo.ASIGNACION_PLAN a
INNER JOIN dbo.USUARIO u ON u.id_usuario = a.id_usuario
INNER JOIN dbo.PLAN_ALIMENTACION p ON p.id_plan = a.id_plan
WHERE u.email LIKE 'cliente.demo%@nutritec.test'
ORDER BY u.email, a.fecha_inicio DESC;

PRINT '============================================================';
PRINT '9) PACIENTES POR NUTRICIONISTA — prueba GET /api/pacientes/nutricionista/{cedula}';
PRINT '============================================================';
SELECT
    n.cedula,
    CONCAT(n.nombre, ' ', n.apellidos) AS nutricionista,
    u.id_usuario,
    CONCAT(u.nombre, ' ', u.apellidos) AS paciente,
    u.email
FROM dbo.PACIENTE_NUTRICIONISTA pn
INNER JOIN dbo.NUTRICIONISTA n ON n.cedula = pn.cedula_nutricionista
INNER JOIN dbo.USUARIO u ON u.id_usuario = pn.id_usuario
WHERE n.cedula LIKE 'NUT-DEMO-%'
ORDER BY n.cedula, u.email;

PRINT '============================================================';
PRINT '10) HISTORIAL DE MEDIDAS — prueba GET /api/medidas/usuario/{idUsuario}';
PRINT '============================================================';
SELECT
    u.id_usuario,
    CONCAT(u.nombre, ' ', u.apellidos) AS cliente,
    m.fecha,
    m.cintura,
    m.cuello,
    m.caderas,
    m.pct_musculo,
    m.pct_grasa
FROM dbo.MEDIDA_USUARIO m
INNER JOIN dbo.USUARIO u ON u.id_usuario = m.id_usuario
WHERE u.email LIKE 'cliente.demo%@nutritec.test'
ORDER BY u.email, m.fecha DESC;

PRINT '============================================================';
PRINT '11) RECETAS Y PRODUCTOS — prueba GET /api/recetas/usuario/{idUsuario}';
PRINT '============================================================';
SELECT
    u.id_usuario,
    CONCAT(u.nombre, ' ', u.apellidos) AS cliente,
    r.id_receta,
    r.nombre AS receta,
    r.total_calorias,
    p.nombre AS producto,
    rp.cantidad_porciones
FROM dbo.RECETA r
INNER JOIN dbo.USUARIO u ON u.id_usuario = r.id_usuario
LEFT JOIN dbo.RECETA_PRODUCTO rp ON rp.id_receta = r.id_receta
LEFT JOIN dbo.PRODUCTO p ON p.id_producto = rp.id_producto
WHERE r.nombre LIKE 'Receta demo %'
ORDER BY u.email, r.id_receta;

PRINT '============================================================';
PRINT '12) REGISTROS DIARIOS — prueba GET /api/registros-diarios/usuario/{idUsuario}';
PRINT '============================================================';
SELECT
    u.id_usuario,
    CONCAT(u.nombre, ' ', u.apellidos) AS cliente,
    rd.id_registro,
    rd.fecha,
    rd.tipo_comida,
    p.nombre AS producto,
    rp.cantidad_porciones
FROM dbo.REGISTRO_DIARIO rd
INNER JOIN dbo.USUARIO u ON u.id_usuario = rd.id_usuario
LEFT JOIN dbo.REGISTRO_PRODUCTO rp ON rp.id_registro = rd.id_registro
LEFT JOIN dbo.PRODUCTO p ON p.id_producto = rp.id_producto
WHERE u.email LIKE 'cliente.demo%@nutritec.test'
ORDER BY u.email, rd.fecha DESC, rd.tipo_comida;

PRINT '============================================================';
PRINT '13) OBJETOS PROGRAMABLES IMPORTANTES';
PRINT '============================================================';
SELECT
    o.type_desc AS tipo,
    SCHEMA_NAME(o.schema_id) AS esquema,
    o.name AS objeto
FROM sys.objects o
WHERE o.name IN (
    'fn_CalcularImc',
    'fn_TotalCaloriasPlan',
    'sp_AprobarProducto',
    'sp_AsignarPlanPaciente',
    'sp_RegistrarMedidaUsuario',
    'sp_ReporteCobroNutricionistas',
    'vw_PacientesPorNutricionista',
    'vw_DetallePlanAlimentacion',
    'vw_ResumenRegistroDiario',
    'trg_RecalcularTotalesReceta',
    'trg_RecalcularTotalPlan'
)
ORDER BY o.type_desc, o.name;

PRINT '============================================================';
PRINT '14) CHECK FINAL';
PRINT '============================================================';
SELECT
    CASE
        WHEN (SELECT COUNT(*) FROM dbo.PRODUCTO WHERE codigo_barras LIKE '77000000000%' AND aprobado = 1) >= 5
         AND (SELECT COUNT(*) FROM dbo.USUARIO WHERE email LIKE 'cliente.demo%@nutritec.test') >= 5
         AND (SELECT COUNT(*) FROM dbo.NUTRICIONISTA WHERE cedula LIKE 'NUT-DEMO-%') >= 5
         AND (SELECT COUNT(*) FROM dbo.ASIGNACION_PLAN a INNER JOIN dbo.USUARIO u ON u.id_usuario = a.id_usuario WHERE u.email LIKE 'cliente.demo%@nutritec.test') >= 5
         AND (SELECT COUNT(*) FROM dbo.REGISTRO_DIARIO r INNER JOIN dbo.USUARIO u ON u.id_usuario = r.id_usuario WHERE u.email LIKE 'cliente.demo%@nutritec.test') >= 5
        THEN 'OK: seed y cambios principales verificados'
        ELSE 'REVISAR: faltan datos demo; ejecute Tests/SeedDemoData.sql completo'
    END AS resultado;
