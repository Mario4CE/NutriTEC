/*
    Descripción:
        Crea la tabla MEDIDA_USUARIO para guardar el historial de medidas corporales de cada usuario.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla MEDIDA_USUARIO con clave primaria id_medida y llave foránea hacia USUARIO.

    Restricciones:
        - Ejecutar después de crear la tabla USUARIO.
        - id_usuario debe existir previamente en USUARIO.
*/
CREATE TABLE MEDIDA_USUARIO (
    id_medida       INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario      INT             NOT NULL,
    fecha           DATE            NOT NULL,
    cintura         DECIMAL(5,2),
    cuello          DECIMAL(5,2),
    caderas         DECIMAL(5,2),
    pct_musculo     DECIMAL(5,2),
    pct_grasa       DECIMAL(5,2),
    CONSTRAINT FK_MEDIDA_USUARIO FOREIGN KEY (id_usuario)
        REFERENCES USUARIO(id_usuario)
);
