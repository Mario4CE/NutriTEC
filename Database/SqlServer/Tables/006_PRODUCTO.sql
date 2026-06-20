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
        - Incluye porción, macronutrientes, sodio y micronutrientes opcionales para que la ficha nutricional esté completa.
        - Los productos nuevos quedan pendientes de aprobación por defecto.
*/
CREATE TABLE PRODUCTO (
    id_producto         UNIQUEIDENTIFIER    PRIMARY KEY,
    nombre              VARCHAR(150)        NOT NULL,
    codigo_barras       VARCHAR(64)         NOT NULL UNIQUE,
    porcion_g_ml        DECIMAL(10,2)       NOT NULL,
    calorias            DECIMAL(10,2)       NOT NULL,
    proteinas           DECIMAL(10,2)       NOT NULL,
    carbohidratos       DECIMAL(10,2)       NOT NULL,
    grasas              DECIMAL(10,2)       NOT NULL,
    sodio_mg            DECIMAL(10,2)       NOT NULL,
    vitaminas           VARCHAR(255)        NULL,
    calcio_mg           DECIMAL(10,2)       NULL,
    hierro_mg           DECIMAL(10,2)       NULL,
    aprobado            BIT                 NOT NULL DEFAULT 0,
    fecha_creacion_utc  DATETIME2           NOT NULL,
    CONSTRAINT CK_PRODUCTO_PORCION_POSITIVA CHECK (porcion_g_ml > 0),
    CONSTRAINT CK_PRODUCTO_NUTRIENTES_NO_NEGATIVOS CHECK (
        calorias >= 0
        AND proteinas >= 0
        AND carbohidratos >= 0
        AND grasas >= 0
        AND sodio_mg >= 0
        AND (calcio_mg IS NULL OR calcio_mg >= 0)
        AND (hierro_mg IS NULL OR hierro_mg >= 0)
    )
);
