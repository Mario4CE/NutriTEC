-- =============================================================
-- NutriTEC — 002_PRODUCTOS_DEFAULT.sql
-- Script de población: productos por defecto (aprobados)
-- Ejecutar en SSMS después de crear las tablas.
-- Requiere que la tabla PRODUCTO exista.
-- =============================================================

USE NutriTEC;

-- Insertar 15 productos nutricionales costarricenses aprobados por defecto
INSERT INTO PRODUCTO (id_producto, nombre, codigo_barras, porcion_g_ml, calorias, proteinas, carbohidratos, grasas, sodio_mg, vitaminas, calcio_mg, hierro_mg, aprobado, fecha_creacion_utc)
VALUES
    (NEWID(), 'Pinto casero',                '7501234560011', 200,  215, 7.5,  38.0, 4.2, 320, 'B1, B6',      45,  2.1, 1, GETUTCDATE()),
    (NEWID(), 'Arroz blanco cocido',         '7501234560028', 180,  234, 4.3,  51.7, 0.4, 1,   NULL,          10,  0.3, 1, GETUTCDATE()),
    (NEWID(), 'Pechuga de pollo a la plancha','7501234560035', 150,  248, 46.5, 0.0,  5.4, 74,  'B3, B6',      15,  1.0, 1, GETUTCDATE()),
    (NEWID(), 'Huevo entero',                '7501234560042', 60,   86,  7.5,  0.6,  5.8, 71,  'A, D, B12',   28,  1.0, 1, GETUTCDATE()),
    (NEWID(), 'Leche semidescremada',        '7501234560059', 250,  115, 8.5,  12.0, 3.5, 120, 'A, D, B12',   295, 0.1, 1, GETUTCDATE()),
    (NEWID(), 'Plátano maduro',              '7501234560066', 120,  134, 1.3,  34.2, 0.5, 4,   'B6, C',       7,   0.4, 1, GETUTCDATE()),
    (NEWID(), 'Frijoles negros cocidos',     '7501234560073', 180,  227, 15.2, 40.8, 0.9, 2,   'B1, B9',      46,  3.6, 1, GETUTCDATE()),
    (NEWID(), 'Atún en agua',               '7501234560080', 85,   109, 25.1, 0.0,  0.5, 287, 'B12, D',      12,  1.3, 1, GETUTCDATE()),
    (NEWID(), 'Avena en hojuelas',           '7501234560097', 40,   152, 5.3,  27.4, 2.5, 2,   'B1, B5',      17,  2.1, 1, GETUTCDATE()),
    (NEWID(), 'Tortilla de maíz',           '7501234560103', 30,   65,  1.7,  13.2, 0.7, 42,  NULL,          44,  0.6, 1, GETUTCDATE()),
    (NEWID(), 'Yogur natural sin azúcar',  '7501234560110', 200,  122, 8.5,  17.0, 2.1, 95,  'B2, B12',     296, 0.1, 1, GETUTCDATE()),
    (NEWID(), 'Ensalada de garbanzos',       '7501234560127', 250,  180, 9.0,  24.0, 5.0, 95,  'C, K',        60,  3.2, 1, GETUTCDATE()),
    (NEWID(), 'Batido proteico de avena',    '7501234560134', 350,  310, 22.0, 40.0, 6.0, 180, NULL,          120, NULL,1, GETUTCDATE()),
    (NEWID(), 'Papa cocinada',               '7501234560141', 150,  130, 2.9,  29.6, 0.2, 8,   'C, B6',       12,  0.8, 1, GETUTCDATE()),
    (NEWID(), 'Pechuga de pavo',             '7501234560158', 150,  180, 40.5, 0.0,  1.5, 77,  'B3, B6, B12', 24,  1.4, 1, GETUTCDATE());

-- Verificación
SELECT COUNT(*) AS total_productos, SUM(CASE WHEN aprobado = 1 THEN 1 ELSE 0 END) AS aprobados
FROM PRODUCTO;

PRINT 'Productos por defecto insertados correctamente.'