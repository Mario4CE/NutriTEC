/*
    Datos de prueba NutriTEC para desarrollo/QA.

    Objetivo:
    - Cargar al menos 5 registros de las entidades operativas principales que se pueden agregar desde las vistas/API:
      administradores, clientes, nutricionistas, productos, pacientes asociados, medidas, recetas, planes,
      asignaciones, tiempos de comida, consumos diarios y detalles con productos.

    Uso:
    1. Crear la base y ejecutar los scripts de Database/SqlServer en el orden documentado.
    2. Ejecutar este archivo sobre la base de pruebas NutriTec con SQL Server Management Studio, Azure Data Studio o sqlcmd.

    Restricciones:
    - Solo usar en ambientes locales/de prueba.
    - No contiene contraseñas reales ni tarjetas reales; los hashes son marcadores no válidos para login real.
    - Es idempotente para los datos identificados por correos, cédulas, códigos y nombres demo.
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

/* Catálogo mínimo requerido por NUTRICIONISTA. */
MERGE TIPO_COBRO AS target
USING (VALUES
    ('semanal', 'Semanal', 1),
    ('mensual', 'Mensual', 1),
    ('anual', 'Anual', 1)
) AS source (codigo_tipo_cobro, nombre, activo)
ON target.codigo_tipo_cobro = source.codigo_tipo_cobro
WHEN NOT MATCHED THEN
    INSERT (codigo_tipo_cobro, nombre, activo)
    VALUES (source.codigo_tipo_cobro, source.nombre, source.activo);

/* 5 administradores demo. */
MERGE ADMINISTRADOR AS target
USING (VALUES
    ('admin.demo1@nutritec.test', 'TEST_HASH_NO_USAR_01'),
    ('admin.demo2@nutritec.test', 'TEST_HASH_NO_USAR_02'),
    ('admin.demo3@nutritec.test', 'TEST_HASH_NO_USAR_03'),
    ('admin.demo4@nutritec.test', 'TEST_HASH_NO_USAR_04'),
    ('admin.demo5@nutritec.test', 'TEST_HASH_NO_USAR_05')
) AS source (email, password_hash)
ON target.email = source.email
WHEN NOT MATCHED THEN
    INSERT (email, password_hash) VALUES (source.email, source.password_hash);

/* 5 clientes demo. */
MERGE USUARIO AS target
USING (VALUES
    ('Cliente', 'Demo Uno', 29, CONVERT(date, '1997-02-10'), 70.50, 23.10, 'Costa Rica', 82.00, 38.00, 96.00, 42.00, 19.00, 2200.00, 'cliente.demo1@nutritec.test', 'TEST_HASH_NO_USAR_01'),
    ('Cliente', 'Demo Dos', 34, CONVERT(date, '1992-06-18'), 81.00, 25.20, 'Costa Rica', 90.00, 40.00, 101.00, 39.00, 24.00, 2100.00, 'cliente.demo2@nutritec.test', 'TEST_HASH_NO_USAR_02'),
    ('Cliente', 'Demo Tres', 25, CONVERT(date, '2001-03-25'), 62.30, 21.40, 'Costa Rica', 74.00, 34.00, 91.00, 44.00, 17.50, 2000.00, 'cliente.demo3@nutritec.test', 'TEST_HASH_NO_USAR_03'),
    ('Cliente', 'Demo Cuatro', 41, CONVERT(date, '1985-11-03'), 88.20, 27.80, 'Costa Rica', 98.00, 42.00, 105.00, 36.00, 29.00, 1900.00, 'cliente.demo4@nutritec.test', 'TEST_HASH_NO_USAR_04'),
    ('Cliente', 'Demo Cinco', 31, CONVERT(date, '1995-09-14'), 68.00, 22.70, 'Costa Rica', 80.00, 36.00, 94.00, 41.50, 20.00, 2150.00, 'cliente.demo5@nutritec.test', 'TEST_HASH_NO_USAR_05')
) AS source (nombre, apellidos, edad, fecha_nacimiento, peso, imc, pais, cintura, cuello, caderas, pct_musculo, pct_grasa, calorias_diarias_max, email, password_hash)
ON target.email = source.email
WHEN NOT MATCHED THEN
    INSERT (nombre, apellidos, edad, fecha_nacimiento, peso, imc, pais, cintura, cuello, caderas, pct_musculo, pct_grasa, calorias_diarias_max, email, password_hash)
    VALUES (source.nombre, source.apellidos, source.edad, source.fecha_nacimiento, source.peso, source.imc, source.pais, source.cintura, source.cuello, source.caderas, source.pct_musculo, source.pct_grasa, source.calorias_diarias_max, source.email, source.password_hash);

/* 5 nutricionistas demo. */
MERGE NUTRICIONISTA AS target
USING (VALUES
    ('NUT-DEMO-001', 'Nutricionista', 'Demo Uno', 'COD-DEMO-001', 35, CONVERT(date, '1991-01-12'), 66.00, 22.30, 'San José', NULL, '****-****-****-1001', 'mensual', 'nutri.demo1@nutritec.test', 'TEST_HASH_NO_USAR_01'),
    ('NUT-DEMO-002', 'Nutricionista', 'Demo Dos', 'COD-DEMO-002', 38, CONVERT(date, '1988-04-04'), 72.00, 24.10, 'Heredia', NULL, '****-****-****-1002', 'semanal', 'nutri.demo2@nutritec.test', 'TEST_HASH_NO_USAR_02'),
    ('NUT-DEMO-003', 'Nutricionista', 'Demo Tres', 'COD-DEMO-003', 32, CONVERT(date, '1994-08-20'), 61.00, 21.80, 'Alajuela', NULL, '****-****-****-1003', 'anual', 'nutri.demo3@nutritec.test', 'TEST_HASH_NO_USAR_03'),
    ('NUT-DEMO-004', 'Nutricionista', 'Demo Cuatro', 'COD-DEMO-004', 45, CONVERT(date, '1981-12-02'), 75.00, 25.00, 'Cartago', NULL, '****-****-****-1004', 'mensual', 'nutri.demo4@nutritec.test', 'TEST_HASH_NO_USAR_04'),
    ('NUT-DEMO-005', 'Nutricionista', 'Demo Cinco', 'COD-DEMO-005', 29, CONVERT(date, '1997-07-07'), 59.00, 20.90, 'Puntarenas', NULL, '****-****-****-1005', 'semanal', 'nutri.demo5@nutritec.test', 'TEST_HASH_NO_USAR_05')
) AS source (cedula, nombre, apellidos, codigo_nutricionista, edad, fecha_nacimiento, peso, imc, direccion, foto_url, tarjeta_credito, tipo_cobro, email, password_hash)
ON target.cedula = source.cedula
WHEN NOT MATCHED THEN
    INSERT (cedula, nombre, apellidos, codigo_nutricionista, edad, fecha_nacimiento, peso, imc, direccion, foto_url, tarjeta_credito, tipo_cobro, email, password_hash)
    VALUES (source.cedula, source.nombre, source.apellidos, source.codigo_nutricionista, source.edad, source.fecha_nacimiento, source.peso, source.imc, source.direccion, source.foto_url, source.tarjeta_credito, source.tipo_cobro, source.email, source.password_hash);

/* 5 productos aprobados demo. */
MERGE PRODUCTO AS target
USING (VALUES
    (CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000001'), 'Avena integral demo', '770000000001', 40.00, 150.00, 5.00, 27.00, 3.00, 2.00, 'B1', 20.00, 1.50, 1, SYSUTCDATETIME()),
    (CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000002'), 'Yogurt natural demo', '770000000002', 125.00, 90.00, 7.00, 10.00, 2.00, 55.00, 'D', 180.00, 0.10, 1, SYSUTCDATETIME()),
    (CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000003'), 'Pechuga de pollo demo', '770000000003', 100.00, 165.00, 31.00, 0.00, 3.60, 74.00, 'B6', 15.00, 1.00, 1, SYSUTCDATETIME()),
    (CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000004'), 'Arroz integral demo', '770000000004', 100.00, 111.00, 2.60, 23.00, 0.90, 5.00, 'B3', 10.00, 0.40, 1, SYSUTCDATETIME()),
    (CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000005'), 'Manzana roja demo', '770000000005', 100.00, 52.00, 0.30, 14.00, 0.20, 1.00, 'C', 6.00, 0.10, 1, SYSUTCDATETIME())
) AS source (id_producto, nombre, codigo_barras, porcion_g_ml, calorias, proteinas, carbohidratos, grasas, sodio_mg, vitaminas, calcio_mg, hierro_mg, aprobado, fecha_creacion_utc)
ON target.id_producto = source.id_producto
WHEN NOT MATCHED THEN
    INSERT (id_producto, nombre, codigo_barras, porcion_g_ml, calorias, proteinas, carbohidratos, grasas, sodio_mg, vitaminas, calcio_mg, hierro_mg, aprobado, fecha_creacion_utc)
    VALUES (source.id_producto, source.nombre, source.codigo_barras, source.porcion_g_ml, source.calorias, source.proteinas, source.carbohidratos, source.grasas, source.sodio_mg, source.vitaminas, source.calcio_mg, source.hierro_mg, source.aprobado, source.fecha_creacion_utc);

/* 5 asociaciones paciente-nutricionista. */
INSERT INTO PACIENTE_NUTRICIONISTA (cedula_nutricionista, id_usuario)
SELECT n.cedula, u.id_usuario
FROM (VALUES
    ('NUT-DEMO-001', 'cliente.demo1@nutritec.test'),
    ('NUT-DEMO-002', 'cliente.demo2@nutritec.test'),
    ('NUT-DEMO-003', 'cliente.demo3@nutritec.test'),
    ('NUT-DEMO-004', 'cliente.demo4@nutritec.test'),
    ('NUT-DEMO-005', 'cliente.demo5@nutritec.test')
) AS pairs(cedula, email)
INNER JOIN NUTRICIONISTA n ON n.cedula = pairs.cedula
INNER JOIN USUARIO u ON u.email = pairs.email
WHERE NOT EXISTS (
    SELECT 1 FROM PACIENTE_NUTRICIONISTA pn
    WHERE pn.cedula_nutricionista = n.cedula AND pn.id_usuario = u.id_usuario
);

/* 5 medidas. */
INSERT INTO MEDIDA_USUARIO (id_usuario, fecha, cintura, cuello, caderas, pct_musculo, pct_grasa)
SELECT u.id_usuario, v.fecha, v.cintura, v.cuello, v.caderas, v.pct_musculo, v.pct_grasa
FROM (VALUES
    ('cliente.demo1@nutritec.test', CONVERT(date, '2026-06-01'), 82.00, 38.00, 96.00, 42.00, 19.00),
    ('cliente.demo2@nutritec.test', CONVERT(date, '2026-06-02'), 90.00, 40.00, 101.00, 39.00, 24.00),
    ('cliente.demo3@nutritec.test', CONVERT(date, '2026-06-03'), 74.00, 34.00, 91.00, 44.00, 17.50),
    ('cliente.demo4@nutritec.test', CONVERT(date, '2026-06-04'), 98.00, 42.00, 105.00, 36.00, 29.00),
    ('cliente.demo5@nutritec.test', CONVERT(date, '2026-06-05'), 80.00, 36.00, 94.00, 41.50, 20.00)
) AS v(email, fecha, cintura, cuello, caderas, pct_musculo, pct_grasa)
INNER JOIN USUARIO u ON u.email = v.email
WHERE NOT EXISTS (SELECT 1 FROM MEDIDA_USUARIO m WHERE m.id_usuario = u.id_usuario AND m.fecha = v.fecha);

/* 5 recetas. */
INSERT INTO RECETA (nombre, id_usuario)
SELECT v.nombre, u.id_usuario
FROM (VALUES
    ('Receta demo 1', 'cliente.demo1@nutritec.test'),
    ('Receta demo 2', 'cliente.demo2@nutritec.test'),
    ('Receta demo 3', 'cliente.demo3@nutritec.test'),
    ('Receta demo 4', 'cliente.demo4@nutritec.test'),
    ('Receta demo 5', 'cliente.demo5@nutritec.test')
) AS v(nombre, email)
INNER JOIN USUARIO u ON u.email = v.email
WHERE NOT EXISTS (SELECT 1 FROM RECETA r WHERE r.nombre = v.nombre AND r.id_usuario = u.id_usuario);

/* Detalle de 5 recetas. */
INSERT INTO RECETA_PRODUCTO (id_receta, id_producto, cantidad_porciones)
SELECT r.id_receta, v.id_producto, v.cantidad_porciones
FROM (VALUES
    ('Receta demo 1', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000001'), 1.00),
    ('Receta demo 2', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000002'), 1.00),
    ('Receta demo 3', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000003'), 1.00),
    ('Receta demo 4', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000004'), 1.50),
    ('Receta demo 5', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000005'), 2.00)
) AS v(nombre_receta, id_producto, cantidad_porciones)
INNER JOIN RECETA r ON r.nombre = v.nombre_receta
WHERE NOT EXISTS (SELECT 1 FROM RECETA_PRODUCTO rp WHERE rp.id_receta = r.id_receta AND rp.id_producto = v.id_producto);

/* 5 planes. */
INSERT INTO PLAN_ALIMENTACION (nombre, cedula_nutricionista)
SELECT v.nombre, v.cedula
FROM (VALUES
    ('Plan demo 1', 'NUT-DEMO-001'),
    ('Plan demo 2', 'NUT-DEMO-002'),
    ('Plan demo 3', 'NUT-DEMO-003'),
    ('Plan demo 4', 'NUT-DEMO-004'),
    ('Plan demo 5', 'NUT-DEMO-005')
) AS v(nombre, cedula)
WHERE NOT EXISTS (SELECT 1 FROM PLAN_ALIMENTACION p WHERE p.nombre = v.nombre AND p.cedula_nutricionista = v.cedula);

/* 5 asignaciones de plan. */
INSERT INTO ASIGNACION_PLAN (id_plan, id_usuario, fecha_inicio, fecha_fin)
SELECT p.id_plan, u.id_usuario, v.fecha_inicio, v.fecha_fin
FROM (VALUES
    ('Plan demo 1', 'cliente.demo1@nutritec.test', CONVERT(date, '2026-06-01'), CONVERT(date, '2026-07-01')),
    ('Plan demo 2', 'cliente.demo2@nutritec.test', CONVERT(date, '2026-06-02'), CONVERT(date, '2026-07-02')),
    ('Plan demo 3', 'cliente.demo3@nutritec.test', CONVERT(date, '2026-06-03'), CONVERT(date, '2026-07-03')),
    ('Plan demo 4', 'cliente.demo4@nutritec.test', CONVERT(date, '2026-06-04'), CONVERT(date, '2026-07-04')),
    ('Plan demo 5', 'cliente.demo5@nutritec.test', CONVERT(date, '2026-06-05'), CONVERT(date, '2026-07-05'))
) AS v(nombre_plan, email, fecha_inicio, fecha_fin)
INNER JOIN PLAN_ALIMENTACION p ON p.nombre = v.nombre_plan
INNER JOIN USUARIO u ON u.email = v.email
WHERE NOT EXISTS (SELECT 1 FROM ASIGNACION_PLAN a WHERE a.id_plan = p.id_plan AND a.id_usuario = u.id_usuario AND a.fecha_inicio = v.fecha_inicio);

/* 5 tiempos de comida. */
INSERT INTO TIEMPO_COMIDA_PLAN (id_plan, tipo_comida)
SELECT p.id_plan, v.tipo_comida
FROM (VALUES
    ('Plan demo 1', 'Desayuno'),
    ('Plan demo 2', 'Merienda Mañana'),
    ('Plan demo 3', 'Almuerzo'),
    ('Plan demo 4', 'Merienda Tarde'),
    ('Plan demo 5', 'Cena')
) AS v(nombre_plan, tipo_comida)
INNER JOIN PLAN_ALIMENTACION p ON p.nombre = v.nombre_plan
WHERE NOT EXISTS (SELECT 1 FROM TIEMPO_COMIDA_PLAN t WHERE t.id_plan = p.id_plan AND t.tipo_comida = v.tipo_comida);

/* Detalle de 5 tiempos de comida. */
INSERT INTO PLAN_PRODUCTO (id_tiempo, id_producto, cantidad_porciones)
SELECT t.id_tiempo, v.id_producto, v.cantidad_porciones
FROM (VALUES
    ('Plan demo 1', 'Desayuno', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000001'), 1.00),
    ('Plan demo 2', 'Merienda Mañana', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000002'), 1.00),
    ('Plan demo 3', 'Almuerzo', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000003'), 1.00),
    ('Plan demo 4', 'Merienda Tarde', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000004'), 1.50),
    ('Plan demo 5', 'Cena', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000005'), 2.00)
) AS v(nombre_plan, tipo_comida, id_producto, cantidad_porciones)
INNER JOIN PLAN_ALIMENTACION p ON p.nombre = v.nombre_plan
INNER JOIN TIEMPO_COMIDA_PLAN t ON t.id_plan = p.id_plan AND t.tipo_comida = v.tipo_comida
WHERE NOT EXISTS (SELECT 1 FROM PLAN_PRODUCTO pp WHERE pp.id_tiempo = t.id_tiempo AND pp.id_producto = v.id_producto);

/* 5 registros diarios. */
INSERT INTO REGISTRO_DIARIO (id_usuario, fecha, tipo_comida)
SELECT u.id_usuario, v.fecha, v.tipo_comida
FROM (VALUES
    ('cliente.demo1@nutritec.test', CONVERT(date, '2026-06-10'), 'Desayuno'),
    ('cliente.demo2@nutritec.test', CONVERT(date, '2026-06-10'), 'Merienda Mañana'),
    ('cliente.demo3@nutritec.test', CONVERT(date, '2026-06-10'), 'Almuerzo'),
    ('cliente.demo4@nutritec.test', CONVERT(date, '2026-06-10'), 'Merienda Tarde'),
    ('cliente.demo5@nutritec.test', CONVERT(date, '2026-06-10'), 'Cena')
) AS v(email, fecha, tipo_comida)
INNER JOIN USUARIO u ON u.email = v.email
WHERE NOT EXISTS (SELECT 1 FROM REGISTRO_DIARIO rd WHERE rd.id_usuario = u.id_usuario AND rd.fecha = v.fecha AND rd.tipo_comida = v.tipo_comida);

/* Detalle de 5 registros diarios. */
INSERT INTO REGISTRO_PRODUCTO (id_registro, id_producto, cantidad_porciones)
SELECT rd.id_registro, v.id_producto, v.cantidad_porciones
FROM (VALUES
    ('cliente.demo1@nutritec.test', CONVERT(date, '2026-06-10'), 'Desayuno', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000001'), 1.00),
    ('cliente.demo2@nutritec.test', CONVERT(date, '2026-06-10'), 'Merienda Mañana', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000002'), 1.00),
    ('cliente.demo3@nutritec.test', CONVERT(date, '2026-06-10'), 'Almuerzo', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000003'), 1.00),
    ('cliente.demo4@nutritec.test', CONVERT(date, '2026-06-10'), 'Merienda Tarde', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000004'), 1.50),
    ('cliente.demo5@nutritec.test', CONVERT(date, '2026-06-10'), 'Cena', CONVERT(uniqueidentifier, '10000000-0000-0000-0000-000000000005'), 2.00)
) AS v(email, fecha, tipo_comida, id_producto, cantidad_porciones)
INNER JOIN USUARIO u ON u.email = v.email
INNER JOIN REGISTRO_DIARIO rd ON rd.id_usuario = u.id_usuario AND rd.fecha = v.fecha AND rd.tipo_comida = v.tipo_comida
WHERE NOT EXISTS (SELECT 1 FROM REGISTRO_PRODUCTO rp WHERE rp.id_registro = rd.id_registro AND rp.id_producto = v.id_producto);

COMMIT TRANSACTION;

/* Resumen rápido de cobertura. */
SELECT 'ADMINISTRADOR demo' AS entidad, COUNT(*) AS cantidad FROM ADMINISTRADOR WHERE email LIKE 'admin.demo%@nutritec.test'
UNION ALL SELECT 'USUARIO demo', COUNT(*) FROM USUARIO WHERE email LIKE 'cliente.demo%@nutritec.test'
UNION ALL SELECT 'NUTRICIONISTA demo', COUNT(*) FROM NUTRICIONISTA WHERE cedula LIKE 'NUT-DEMO-%'
UNION ALL SELECT 'PRODUCTO demo', COUNT(*) FROM PRODUCTO WHERE codigo_barras LIKE '77000000000%'
UNION ALL SELECT 'PACIENTE_NUTRICIONISTA demo', COUNT(*) FROM PACIENTE_NUTRICIONISTA pn INNER JOIN NUTRICIONISTA n ON n.cedula = pn.cedula_nutricionista WHERE n.cedula LIKE 'NUT-DEMO-%'
UNION ALL SELECT 'MEDIDA_USUARIO demo', COUNT(*) FROM MEDIDA_USUARIO m INNER JOIN USUARIO u ON u.id_usuario = m.id_usuario WHERE u.email LIKE 'cliente.demo%@nutritec.test'
UNION ALL SELECT 'RECETA demo', COUNT(*) FROM RECETA WHERE nombre LIKE 'Receta demo %'
UNION ALL SELECT 'PLAN_ALIMENTACION demo', COUNT(*) FROM PLAN_ALIMENTACION WHERE nombre LIKE 'Plan demo %'
UNION ALL SELECT 'ASIGNACION_PLAN demo', COUNT(*) FROM ASIGNACION_PLAN a INNER JOIN USUARIO u ON u.id_usuario = a.id_usuario WHERE u.email LIKE 'cliente.demo%@nutritec.test'
UNION ALL SELECT 'TIEMPO_COMIDA_PLAN demo', COUNT(*) FROM TIEMPO_COMIDA_PLAN t INNER JOIN PLAN_ALIMENTACION p ON p.id_plan = t.id_plan WHERE p.nombre LIKE 'Plan demo %'
UNION ALL SELECT 'REGISTRO_DIARIO demo', COUNT(*) FROM REGISTRO_DIARIO r INNER JOIN USUARIO u ON u.id_usuario = r.id_usuario WHERE u.email LIKE 'cliente.demo%@nutritec.test';
