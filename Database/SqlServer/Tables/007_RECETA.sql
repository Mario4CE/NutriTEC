/*
    Descripción:
        Crea la tabla RECETA para almacenar recetas creadas por usuarios y sus totales nutricionales agregados.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla RECETA con clave primaria id_receta y llave foránea hacia USUARIO.

    Restricciones:
        - Ejecutar después de crear la tabla USUARIO.
        - id_usuario debe existir previamente en USUARIO.
*/
CREATE TABLE RECETA (
    id_receta           INT IDENTITY(1,1) PRIMARY KEY,
    nombre              VARCHAR(150)    NOT NULL,
    id_usuario          INT             NOT NULL,
    total_calorias      DECIMAL(7,2)    NOT NULL DEFAULT 0,
    total_carbohidratos DECIMAL(7,2)    NOT NULL DEFAULT 0,
    total_proteina      DECIMAL(7,2)    NOT NULL DEFAULT 0,
    total_grasa         DECIMAL(7,2)    NOT NULL DEFAULT 0,
    CONSTRAINT FK_RECETA_USUARIO FOREIGN KEY (id_usuario)
        REFERENCES USUARIO(id_usuario)
);
