/*
    Descripción:
        Crea la tabla REGISTRO_PRODUCTO para relacionar registros diarios con productos consumidos y cantidades por porción.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla REGISTRO_PRODUCTO con clave primaria compuesta y llaves foráneas hacia REGISTRO_DIARIO y PRODUCTO.

    Restricciones:
        - Ejecutar después de crear las tablas REGISTRO_DIARIO y PRODUCTO.
        - id_registro debe existir previamente en REGISTRO_DIARIO.
        - id_producto debe existir previamente en PRODUCTO.
*/
CREATE TABLE REGISTRO_PRODUCTO (
    id_registro         INT             NOT NULL,
    id_producto         UNIQUEIDENTIFIER NOT NULL,
    cantidad_porciones  DECIMAL(5,2)    NOT NULL,
    CONSTRAINT PK_REGISTRO_PRODUCTO PRIMARY KEY (id_registro, id_producto),
    CONSTRAINT FK_REGP_REGISTRO FOREIGN KEY (id_registro)
        REFERENCES REGISTRO_DIARIO(id_registro),
    CONSTRAINT FK_REGP_PRODUCTO FOREIGN KEY (id_producto)
        REFERENCES PRODUCTO(id_producto)
);
