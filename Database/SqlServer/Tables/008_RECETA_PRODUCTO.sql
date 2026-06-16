/*
    Descripción:
        Crea la tabla RECETA_PRODUCTO para relacionar recetas con productos y cantidades por porción.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla RECETA_PRODUCTO con clave primaria compuesta y llaves foráneas hacia RECETA y PRODUCTO.

    Restricciones:
        - Ejecutar después de crear las tablas RECETA y PRODUCTO.
        - id_receta debe existir previamente en RECETA.
        - id_producto debe existir previamente en PRODUCTO.
*/
CREATE TABLE RECETA_PRODUCTO (
    id_receta           INT             NOT NULL,
    id_producto         UNIQUEIDENTIFIER NOT NULL,
    cantidad_porciones  DECIMAL(5,2)    NOT NULL,
    CONSTRAINT PK_RECETA_PRODUCTO PRIMARY KEY (id_receta, id_producto),
    CONSTRAINT FK_RP_RECETA FOREIGN KEY (id_receta)
        REFERENCES RECETA(id_receta),
    CONSTRAINT FK_RP_PRODUCTO FOREIGN KEY (id_producto)
        REFERENCES PRODUCTO(id_producto)
);
