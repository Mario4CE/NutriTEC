/*
    Descripción:
        Migra una base existente para completar la ficha nutricional de PRODUCTO.

    Entradas:
        No recibe parámetros. Ejecutar sobre SQL Server cuando PRODUCTO ya existe sin las columnas nuevas.

    Salidas:
        Agrega porción, sodio, vitaminas, calcio y hierro a PRODUCTO de forma idempotente.

    Restricciones:
        - No inserta productos ni datos sensibles.
        - Usa valores por defecto conservadores para no romper bases existentes con filas previas.
*/
IF COL_LENGTH('PRODUCTO', 'porcion_g_ml') IS NULL
BEGIN
    ALTER TABLE PRODUCTO
        ADD porcion_g_ml DECIMAL(10,2) NOT NULL
            CONSTRAINT DF_PRODUCTO_PORCION_G_ML DEFAULT 100;
END;
GO

IF COL_LENGTH('PRODUCTO', 'sodio_mg') IS NULL
BEGIN
    ALTER TABLE PRODUCTO
        ADD sodio_mg DECIMAL(10,2) NOT NULL
            CONSTRAINT DF_PRODUCTO_SODIO_MG DEFAULT 0;
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

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_PRODUCTO_PORCION_POSITIVA'
      AND parent_object_id = OBJECT_ID('PRODUCTO')
)
BEGIN
    ALTER TABLE PRODUCTO
        ADD CONSTRAINT CK_PRODUCTO_PORCION_POSITIVA CHECK (porcion_g_ml > 0);
END;
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_PRODUCTO_NUTRIENTES_NO_NEGATIVOS'
      AND parent_object_id = OBJECT_ID('PRODUCTO')
)
BEGIN
    ALTER TABLE PRODUCTO
        ADD CONSTRAINT CK_PRODUCTO_NUTRIENTES_NO_NEGATIVOS CHECK (
            calorias >= 0
            AND proteinas >= 0
            AND carbohidratos >= 0
            AND grasas >= 0
            AND sodio_mg >= 0
            AND (calcio_mg IS NULL OR calcio_mg >= 0)
            AND (hierro_mg IS NULL OR hierro_mg >= 0)
        );
END;
GO
