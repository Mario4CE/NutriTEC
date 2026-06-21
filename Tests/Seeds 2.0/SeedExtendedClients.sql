/*
    NutriTEC — seed extendido de 15 clientes.

    Objetivo:
    - Crear 15 clientes de prueba con perfiles más completos.
    - Asociar solo 8 clientes a nutricionistas demo.
    - Dejar 7 clientes sin nutricionista para probar vistas con y sin asociación.
    - Cargar medidas, recetas, registros diarios y planes/asignaciones para los asociados.

    Uso:
    Ejecutar después de crear las tablas. Este archivo también asegura nutricionistas y productos base demo.
    Contraseña de todos los clientes extendidos: Cliente2026!
    Contraseña de nutricionistas extendidos: Nutricion2026!
*/


/* Compatibilidad para bases locales creadas antes de ampliar PRODUCTO. */
IF COL_LENGTH('PRODUCTO', 'porcion_g_ml') IS NULL
BEGIN
    ALTER TABLE PRODUCTO ADD porcion_g_ml DECIMAL(10,2) NOT NULL CONSTRAINT DF_PRODUCTO_EXT_PORCION_G_ML DEFAULT 100;
END;
GO
IF COL_LENGTH('PRODUCTO', 'sodio_mg') IS NULL
BEGIN
    ALTER TABLE PRODUCTO ADD sodio_mg DECIMAL(10,2) NOT NULL CONSTRAINT DF_PRODUCTO_EXT_SODIO_MG DEFAULT 0;
END;
GO
IF COL_LENGTH('PRODUCTO', 'vitaminas') IS NULL
BEGIN
    ALTER TABLE PRODUCTO ADD vitaminas VARCHAR(255) NULL;
END;
GO
IF COL_LENGTH('PRODUCTO', 'calcio_mg') IS NULL
BEGIN
    ALTER TABLE PRODUCTO ADD calcio_mg DECIMAL(10,2) NULL;
END;
GO
IF COL_LENGTH('PRODUCTO', 'hierro_mg') IS NULL
BEGIN
    ALTER TABLE PRODUCTO ADD hierro_mg DECIMAL(10,2) NULL;
END;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

MERGE TIPO_COBRO AS target
USING (VALUES
    ('semanal', 'Semanal', 1),
    ('mensual', 'Mensual', 1),
    ('anual', 'Anual', 1)
) AS source (codigo_tipo_cobro, nombre, activo)
ON target.codigo_tipo_cobro = source.codigo_tipo_cobro
WHEN NOT MATCHED THEN
    INSERT (codigo_tipo_cobro, nombre, activo) VALUES (source.codigo_tipo_cobro, source.nombre, source.activo);

MERGE NUTRICIONISTA AS target
USING (VALUES
    ('EXT-NUT-001', 'Elena', 'Madrigal Salas', 'EXT-COD-001', 36, CONVERT(date, '1990-03-18'), 64.00, 22.10, 'San José, Costa Rica', NULL, '****-****-****-6101', 'mensual', 'ext.nutri1@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNOdXRyaTAwMQ==$O0mzVY+ddrrKzmKyBnJLVvwB2LsshsQyidUbm/hHFtI='),
    ('EXT-NUT-002', 'Rodrigo', 'Paniagua Mora', 'EXT-COD-002', 44, CONVERT(date, '1982-08-07'), 78.00, 25.20, 'Heredia, Costa Rica', NULL, '****-****-****-6102', 'semanal', 'ext.nutri2@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNOdXRyaTAwMQ==$O0mzVY+ddrrKzmKyBnJLVvwB2LsshsQyidUbm/hHFtI='),
    ('EXT-NUT-003', 'Camila', 'Sánchez Acuña', 'EXT-COD-003', 31, CONVERT(date, '1995-05-22'), 60.00, 21.50, 'Alajuela, Costa Rica', NULL, '****-****-****-6103', 'anual', 'ext.nutri3@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNOdXRyaTAwMQ==$O0mzVY+ddrrKzmKyBnJLVvwB2LsshsQyidUbm/hHFtI=')
) AS source (cedula, nombre, apellidos, codigo_nutricionista, edad, fecha_nacimiento, peso, imc, direccion, foto_url, tarjeta_credito, tipo_cobro, email, password_hash)
ON target.cedula = source.cedula
WHEN MATCHED THEN
    UPDATE SET nombre = source.nombre, apellidos = source.apellidos, email = source.email, password_hash = source.password_hash, tipo_cobro = source.tipo_cobro
WHEN NOT MATCHED THEN
    INSERT (cedula, nombre, apellidos, codigo_nutricionista, edad, fecha_nacimiento, peso, imc, direccion, foto_url, tarjeta_credito, tipo_cobro, email, password_hash)
    VALUES (source.cedula, source.nombre, source.apellidos, source.codigo_nutricionista, source.edad, source.fecha_nacimiento, source.peso, source.imc, source.direccion, source.foto_url, source.tarjeta_credito, source.tipo_cobro, source.email, source.password_hash);

MERGE PRODUCTO AS target
USING (VALUES
    (CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000001'), 'Tortilla de maíz extendida', '880000000001', 50.00, 110.00, 2.80, 22.00, 1.40, 12.00, 'B3', 35.00, 0.80, 1, SYSUTCDATETIME()),
    (CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000002'), 'Frijoles negros extendidos', '880000000002', 100.00, 132.00, 8.90, 24.00, 0.50, 2.00, 'B9', 27.00, 2.10, 1, SYSUTCDATETIME()),
    (CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000003'), 'Queso fresco extendido', '880000000003', 30.00, 85.00, 5.20, 1.00, 6.50, 180.00, 'A', 160.00, 0.10, 1, SYSUTCDATETIME()),
    (CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000004'), 'Papaya extendida', '880000000004', 100.00, 43.00, 0.50, 11.00, 0.30, 8.00, 'C', 20.00, 0.30, 1, SYSUTCDATETIME()),
    (CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000005'), 'Atún en agua extendido', '880000000005', 100.00, 116.00, 26.00, 0.00, 1.00, 247.00, 'D', 8.00, 1.20, 1, SYSUTCDATETIME())
) AS source (id_producto, nombre, codigo_barras, porcion_g_ml, calorias, proteinas, carbohidratos, grasas, sodio_mg, vitaminas, calcio_mg, hierro_mg, aprobado, fecha_creacion_utc)
ON target.id_producto = source.id_producto
WHEN NOT MATCHED THEN
    INSERT (id_producto, nombre, codigo_barras, porcion_g_ml, calorias, proteinas, carbohidratos, grasas, sodio_mg, vitaminas, calcio_mg, hierro_mg, aprobado, fecha_creacion_utc)
    VALUES (source.id_producto, source.nombre, source.codigo_barras, source.porcion_g_ml, source.calorias, source.proteinas, source.carbohidratos, source.grasas, source.sodio_mg, source.vitaminas, source.calcio_mg, source.hierro_mg, source.aprobado, source.fecha_creacion_utc);

MERGE USUARIO AS target
USING (VALUES
    ('Andrés', 'Calderón Soto', 28, CONVERT(date, '1998-01-12'), 74.20, 24.10, 'Costa Rica', 84.00, 39.00, 96.00, 41.00, 21.00, 2200.00, 'ext.cliente01@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Gabriela', 'Rojas Umaña', 33, CONVERT(date, '1993-04-08'), 63.50, 22.40, 'Costa Rica', 76.00, 34.00, 93.00, 39.00, 23.00, 1900.00, 'ext.cliente02@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Javier', 'Mora Céspedes', 42, CONVERT(date, '1984-09-19'), 88.00, 27.10, 'Costa Rica', 97.00, 42.00, 104.00, 38.00, 30.00, 2100.00, 'ext.cliente03@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Lucía', 'Vega Alfaro', 26, CONVERT(date, '2000-02-24'), 58.70, 21.30, 'Costa Rica', 70.00, 32.00, 89.00, 40.00, 20.00, 1800.00, 'ext.cliente04@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Diego', 'Fernández Araya', 31, CONVERT(date, '1995-12-03'), 79.40, 25.00, 'Costa Rica', 88.00, 40.00, 100.00, 43.00, 24.00, 2300.00, 'ext.cliente05@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Karla', 'Jiménez León', 37, CONVERT(date, '1989-07-16'), 69.10, 23.60, 'Costa Rica', 82.00, 35.00, 97.00, 38.00, 26.00, 1950.00, 'ext.cliente06@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Mateo', 'Quesada Núñez', 24, CONVERT(date, '2002-05-29'), 72.00, 22.90, 'Costa Rica', 80.00, 38.00, 95.00, 44.00, 18.00, 2400.00, 'ext.cliente07@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Paola', 'Castro Brenes', 45, CONVERT(date, '1981-10-11'), 83.60, 28.20, 'Costa Rica', 96.00, 37.00, 106.00, 35.00, 32.00, 1750.00, 'ext.cliente08@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Roberto', 'Salazar Pineda', 39, CONVERT(date, '1987-03-06'), 91.30, 29.00, 'Costa Rica', 102.00, 43.00, 108.00, 37.00, 33.00, 2000.00, 'ext.cliente09@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Fernanda', 'Campos Solano', 30, CONVERT(date, '1996-08-21'), 61.20, 21.90, 'Costa Rica', 73.00, 33.00, 91.00, 41.00, 19.00, 1850.00, 'ext.cliente10@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Mauricio', 'Blanco Torres', 52, CONVERT(date, '1974-06-14'), 86.50, 26.70, 'Costa Rica', 94.00, 41.00, 103.00, 36.00, 28.00, 2050.00, 'ext.cliente11@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Daniela', 'Navarro Ruiz', 27, CONVERT(date, '1999-11-30'), 64.80, 22.80, 'Costa Rica', 78.00, 34.00, 94.00, 40.00, 22.00, 1900.00, 'ext.cliente12@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Sebastián', 'Arias Chacón', 35, CONVERT(date, '1991-01-05'), 77.70, 24.60, 'Costa Rica', 86.00, 39.00, 98.00, 42.00, 23.00, 2250.00, 'ext.cliente13@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Mónica', 'Pérez Valverde', 48, CONVERT(date, '1978-04-27'), 72.90, 25.40, 'Costa Rica', 90.00, 36.00, 101.00, 34.00, 31.00, 1700.00, 'ext.cliente14@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c='),
    ('Emilio', 'Hernández Madrigal', 29, CONVERT(date, '1997-09-09'), 69.90, 23.00, 'Costa Rica', 79.00, 37.00, 93.00, 45.00, 17.00, 2350.00, 'ext.cliente15@nutritec.test', 'PBKDF2-SHA256$310000$TnV0cmlURUNDbGllbnRlMQ==$tiRxiOvueH5Y9xL4tfnTpsOJJtmyHLfEmDfuUNguq4c=')
) AS source (nombre, apellidos, edad, fecha_nacimiento, peso, imc, pais, cintura, cuello, caderas, pct_musculo, pct_grasa, calorias_diarias_max, email, password_hash)
ON target.email = source.email
WHEN MATCHED THEN
    UPDATE SET nombre = source.nombre, apellidos = source.apellidos, edad = source.edad, fecha_nacimiento = source.fecha_nacimiento, peso = source.peso, imc = source.imc, pais = source.pais, cintura = source.cintura, cuello = source.cuello, caderas = source.caderas, pct_musculo = source.pct_musculo, pct_grasa = source.pct_grasa, calorias_diarias_max = source.calorias_diarias_max, password_hash = source.password_hash
WHEN NOT MATCHED THEN
    INSERT (nombre, apellidos, edad, fecha_nacimiento, peso, imc, pais, cintura, cuello, caderas, pct_musculo, pct_grasa, calorias_diarias_max, email, password_hash)
    VALUES (source.nombre, source.apellidos, source.edad, source.fecha_nacimiento, source.peso, source.imc, source.pais, source.cintura, source.cuello, source.caderas, source.pct_musculo, source.pct_grasa, source.calorias_diarias_max, source.email, source.password_hash);

/* Asociar 8 de 15 clientes. Los clientes 09 al 15 quedan intencionalmente sin nutricionista. */
INSERT INTO PACIENTE_NUTRICIONISTA (cedula_nutricionista, id_usuario)
SELECT v.cedula, u.id_usuario
FROM (VALUES
    ('EXT-NUT-001', 'ext.cliente01@nutritec.test'),
    ('EXT-NUT-001', 'ext.cliente02@nutritec.test'),
    ('EXT-NUT-001', 'ext.cliente03@nutritec.test'),
    ('EXT-NUT-002', 'ext.cliente04@nutritec.test'),
    ('EXT-NUT-002', 'ext.cliente05@nutritec.test'),
    ('EXT-NUT-002', 'ext.cliente06@nutritec.test'),
    ('EXT-NUT-003', 'ext.cliente07@nutritec.test'),
    ('EXT-NUT-003', 'ext.cliente08@nutritec.test')
) AS v(cedula, email)
INNER JOIN USUARIO u ON u.email = v.email
WHERE NOT EXISTS (SELECT 1 FROM PACIENTE_NUTRICIONISTA pn WHERE pn.cedula_nutricionista = v.cedula AND pn.id_usuario = u.id_usuario);

DELETE pn
FROM PACIENTE_NUTRICIONISTA pn
INNER JOIN USUARIO u ON u.id_usuario = pn.id_usuario
WHERE u.email IN ('ext.cliente09@nutritec.test','ext.cliente10@nutritec.test','ext.cliente11@nutritec.test','ext.cliente12@nutritec.test','ext.cliente13@nutritec.test','ext.cliente14@nutritec.test','ext.cliente15@nutritec.test');

/* Medidas para los 15 clientes. */
INSERT INTO MEDIDA_USUARIO (id_usuario, fecha, cintura, cuello, caderas, pct_musculo, pct_grasa)
SELECT u.id_usuario, v.fecha, v.cintura, v.cuello, v.caderas, v.pct_musculo, v.pct_grasa
FROM (VALUES
    ('ext.cliente01@nutritec.test', CONVERT(date, '2026-06-01'), 84.00, 39.00, 96.00, 41.00, 21.00),
    ('ext.cliente02@nutritec.test', CONVERT(date, '2026-06-02'), 76.00, 34.00, 93.00, 39.00, 23.00),
    ('ext.cliente03@nutritec.test', CONVERT(date, '2026-06-03'), 97.00, 42.00, 104.00, 38.00, 30.00),
    ('ext.cliente04@nutritec.test', CONVERT(date, '2026-06-04'), 70.00, 32.00, 89.00, 40.00, 20.00),
    ('ext.cliente05@nutritec.test', CONVERT(date, '2026-06-05'), 88.00, 40.00, 100.00, 43.00, 24.00),
    ('ext.cliente06@nutritec.test', CONVERT(date, '2026-06-06'), 82.00, 35.00, 97.00, 38.00, 26.00),
    ('ext.cliente07@nutritec.test', CONVERT(date, '2026-06-07'), 80.00, 38.00, 95.00, 44.00, 18.00),
    ('ext.cliente08@nutritec.test', CONVERT(date, '2026-06-08'), 96.00, 37.00, 106.00, 35.00, 32.00),
    ('ext.cliente09@nutritec.test', CONVERT(date, '2026-06-09'), 102.00, 43.00, 108.00, 37.00, 33.00),
    ('ext.cliente10@nutritec.test', CONVERT(date, '2026-06-10'), 73.00, 33.00, 91.00, 41.00, 19.00),
    ('ext.cliente11@nutritec.test', CONVERT(date, '2026-06-11'), 94.00, 41.00, 103.00, 36.00, 28.00),
    ('ext.cliente12@nutritec.test', CONVERT(date, '2026-06-12'), 78.00, 34.00, 94.00, 40.00, 22.00),
    ('ext.cliente13@nutritec.test', CONVERT(date, '2026-06-13'), 86.00, 39.00, 98.00, 42.00, 23.00),
    ('ext.cliente14@nutritec.test', CONVERT(date, '2026-06-14'), 90.00, 36.00, 101.00, 34.00, 31.00),
    ('ext.cliente15@nutritec.test', CONVERT(date, '2026-06-15'), 79.00, 37.00, 93.00, 45.00, 17.00)
) AS v(email, fecha, cintura, cuello, caderas, pct_musculo, pct_grasa)
INNER JOIN USUARIO u ON u.email = v.email
WHERE NOT EXISTS (SELECT 1 FROM MEDIDA_USUARIO m WHERE m.id_usuario = u.id_usuario AND m.fecha = v.fecha);

/* Recetas, registros diarios y detalles para todos los clientes extendidos. */
INSERT INTO RECETA (nombre, id_usuario)
SELECT CONCAT('Receta extendida ', RIGHT(u.email, 18)), u.id_usuario
FROM USUARIO u
WHERE u.email LIKE 'ext.cliente%@nutritec.test'
  AND NOT EXISTS (SELECT 1 FROM RECETA r WHERE r.id_usuario = u.id_usuario AND r.nombre = CONCAT('Receta extendida ', RIGHT(u.email, 18)));

INSERT INTO RECETA_PRODUCTO (id_receta, id_producto, cantidad_porciones)
SELECT r.id_receta, CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000002'), 1.00
FROM RECETA r
INNER JOIN USUARIO u ON u.id_usuario = r.id_usuario
WHERE u.email LIKE 'ext.cliente%@nutritec.test'
  AND NOT EXISTS (SELECT 1 FROM RECETA_PRODUCTO rp WHERE rp.id_receta = r.id_receta AND rp.id_producto = CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000002'));

INSERT INTO REGISTRO_DIARIO (id_usuario, fecha, tipo_comida)
SELECT u.id_usuario, CONVERT(date, '2026-06-20'),
    CASE ((ROW_NUMBER() OVER (ORDER BY u.email) - 1) % 5)
        WHEN 0 THEN 'Desayuno'
        WHEN 1 THEN 'Merienda Mañana'
        WHEN 2 THEN 'Almuerzo'
        WHEN 3 THEN 'Merienda Tarde'
        ELSE 'Cena'
    END
FROM USUARIO u
WHERE u.email LIKE 'ext.cliente%@nutritec.test'
  AND NOT EXISTS (SELECT 1 FROM REGISTRO_DIARIO rd WHERE rd.id_usuario = u.id_usuario AND rd.fecha = CONVERT(date, '2026-06-20'));

INSERT INTO REGISTRO_PRODUCTO (id_registro, id_producto, cantidad_porciones)
SELECT rd.id_registro, CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000004'), 1.50
FROM REGISTRO_DIARIO rd
INNER JOIN USUARIO u ON u.id_usuario = rd.id_usuario
WHERE u.email LIKE 'ext.cliente%@nutritec.test'
  AND NOT EXISTS (SELECT 1 FROM REGISTRO_PRODUCTO rp WHERE rp.id_registro = rd.id_registro AND rp.id_producto = CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000004'));

/* Planes y asignaciones solo para clientes asociados. */
INSERT INTO PLAN_ALIMENTACION (nombre, cedula_nutricionista)
SELECT CONCAT('Plan extendido ', v.email), v.cedula
FROM (VALUES
    ('EXT-NUT-001', 'ext.cliente01@nutritec.test'),
    ('EXT-NUT-001', 'ext.cliente02@nutritec.test'),
    ('EXT-NUT-001', 'ext.cliente03@nutritec.test'),
    ('EXT-NUT-002', 'ext.cliente04@nutritec.test'),
    ('EXT-NUT-002', 'ext.cliente05@nutritec.test'),
    ('EXT-NUT-002', 'ext.cliente06@nutritec.test'),
    ('EXT-NUT-003', 'ext.cliente07@nutritec.test'),
    ('EXT-NUT-003', 'ext.cliente08@nutritec.test')
) AS v(cedula, email)
WHERE NOT EXISTS (SELECT 1 FROM PLAN_ALIMENTACION p WHERE p.nombre = CONCAT('Plan extendido ', v.email));

INSERT INTO ASIGNACION_PLAN (id_plan, id_usuario, fecha_inicio, fecha_fin)
SELECT p.id_plan, u.id_usuario, CONVERT(date, '2026-06-20'), CONVERT(date, '2026-07-20')
FROM PLAN_ALIMENTACION p
INNER JOIN USUARIO u ON p.nombre = CONCAT('Plan extendido ', u.email)
WHERE u.email LIKE 'ext.cliente0[1-8]@nutritec.test'
  AND NOT EXISTS (SELECT 1 FROM ASIGNACION_PLAN a WHERE a.id_plan = p.id_plan AND a.id_usuario = u.id_usuario AND a.fecha_inicio = CONVERT(date, '2026-06-20'));

INSERT INTO TIEMPO_COMIDA_PLAN (id_plan, tipo_comida)
SELECT p.id_plan, 'Almuerzo'
FROM PLAN_ALIMENTACION p
WHERE p.nombre LIKE 'Plan extendido ext.cliente%@nutritec.test'
  AND NOT EXISTS (SELECT 1 FROM TIEMPO_COMIDA_PLAN t WHERE t.id_plan = p.id_plan AND t.tipo_comida = 'Almuerzo');

INSERT INTO PLAN_PRODUCTO (id_tiempo, id_producto, cantidad_porciones)
SELECT t.id_tiempo, CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000005'), 1.00
FROM TIEMPO_COMIDA_PLAN t
INNER JOIN PLAN_ALIMENTACION p ON p.id_plan = t.id_plan
WHERE p.nombre LIKE 'Plan extendido ext.cliente%@nutritec.test'
  AND NOT EXISTS (SELECT 1 FROM PLAN_PRODUCTO pp WHERE pp.id_tiempo = t.id_tiempo AND pp.id_producto = CONVERT(uniqueidentifier, '20000000-0000-0000-0000-000000000005'));

COMMIT TRANSACTION;

SELECT 'Clientes extendidos totales' AS verificacion, COUNT(*) AS total FROM USUARIO WHERE email LIKE 'ext.cliente%@nutritec.test'
UNION ALL SELECT 'Clientes extendidos asociados', COUNT(DISTINCT pn.id_usuario) FROM PACIENTE_NUTRICIONISTA pn INNER JOIN USUARIO u ON u.id_usuario = pn.id_usuario WHERE u.email LIKE 'ext.cliente%@nutritec.test'
UNION ALL SELECT 'Clientes extendidos sin nutricionista', COUNT(*) FROM USUARIO u WHERE u.email LIKE 'ext.cliente%@nutritec.test' AND NOT EXISTS (SELECT 1 FROM PACIENTE_NUTRICIONISTA pn WHERE pn.id_usuario = u.id_usuario)
UNION ALL SELECT 'Medidas extendidas', COUNT(*) FROM MEDIDA_USUARIO m INNER JOIN USUARIO u ON u.id_usuario = m.id_usuario WHERE u.email LIKE 'ext.cliente%@nutritec.test'
UNION ALL SELECT 'Recetas extendidas', COUNT(*) FROM RECETA r INNER JOIN USUARIO u ON u.id_usuario = r.id_usuario WHERE u.email LIKE 'ext.cliente%@nutritec.test'
UNION ALL SELECT 'Registros diarios extendidos', COUNT(*) FROM REGISTRO_DIARIO r INNER JOIN USUARIO u ON u.id_usuario = r.id_usuario WHERE u.email LIKE 'ext.cliente%@nutritec.test'
UNION ALL SELECT 'Planes extendidos asignados', COUNT(*) FROM ASIGNACION_PLAN a INNER JOIN USUARIO u ON u.id_usuario = a.id_usuario WHERE u.email LIKE 'ext.cliente%@nutritec.test';
