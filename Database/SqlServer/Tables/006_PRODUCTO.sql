/*
    Descripción:
        Crea la tabla PRODUCTO para almacenar información nutricional de productos aprobados o pendientes de aprobación.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla PRODUCTO con clave primaria codigo_barras y llaves foráneas opcionales hacia USUARIO y NUTRICIONISTA.

    Restricciones:
        - Ejecutar después de crear las tablas USUARIO y NUTRICIONISTA.
        - creado_por_usuario debe existir en USUARIO cuando tenga valor.
        - creado_por_nutricionista debe existir en NUTRICIONISTA cuando tenga valor.
*/
CREATE TABLE PRODUCTO (
    codigo_barras               VARCHAR(50)     PRIMARY KEY,
    descripcion                 VARCHAR(255)    NOT NULL,
    porcion_g_ml                DECIMAL(7,2)    NOT NULL,
    energia_kcal                DECIMAL(7,2)    NOT NULL,
    grasa_g                     DECIMAL(7,2)    NOT NULL,
    sodio_mg                    DECIMAL(7,2)    NOT NULL,
    carbohidratos_g             DECIMAL(7,2)    NOT NULL,
    proteina_g                  DECIMAL(7,2)    NOT NULL,
    vitaminas                   VARCHAR(255),
    calcio_mg                   DECIMAL(7,2),
    hierro_mg                   DECIMAL(7,2),
    aprobado                    BIT             NOT NULL DEFAULT 0,
    creado_por_usuario          INT             NULL,
    creado_por_nutricionista    VARCHAR(20)     NULL,
    CONSTRAINT FK_PROD_USUARIO FOREIGN KEY (creado_por_usuario)
        REFERENCES USUARIO(id_usuario),
    CONSTRAINT FK_PROD_NUTRICIONISTA FOREIGN KEY (creado_por_nutricionista)
        REFERENCES NUTRICIONISTA(cedula)
);
