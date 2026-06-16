/*
    Descripción:
        Crea la tabla PRODUCTO para almacenar información nutricional de productos aprobados o pendientes de aprobación.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla PRODUCTO con clave primaria id_producto e índice único sobre codigo_barras.

    Restricciones:
        - id_producto lo genera la API como UNIQUEIDENTIFIER.
        - codigo_barras se mantiene como valor único para búsqueda, pero no como clave primaria.
        - Los productos nuevos quedan pendientes de aprobación por defecto.
*/
CREATE TABLE PRODUCTO (
    id_producto         UNIQUEIDENTIFIER    PRIMARY KEY,
    nombre              VARCHAR(150)        NOT NULL,
    codigo_barras       VARCHAR(64)         NOT NULL UNIQUE,
    calorias            DECIMAL(10,2)       NOT NULL,
    proteinas           DECIMAL(10,2)       NOT NULL,
    carbohidratos       DECIMAL(10,2)       NOT NULL,
    grasas              DECIMAL(10,2)       NOT NULL,
    aprobado            BIT                 NOT NULL DEFAULT 0,
    fecha_creacion_utc  DATETIME2           NOT NULL
);
