/*
    Descripción:
        Crea la tabla PLAN_PRODUCTO para relacionar tiempos de comida de un plan con productos y cantidades por porción.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla PLAN_PRODUCTO con clave primaria compuesta y llaves foráneas hacia TIEMPO_COMIDA_PLAN y PRODUCTO.

    Restricciones:
        - Ejecutar después de crear las tablas TIEMPO_COMIDA_PLAN y PRODUCTO.
        - id_tiempo debe existir previamente en TIEMPO_COMIDA_PLAN.
        - codigo_barras debe existir previamente en PRODUCTO.
*/
CREATE TABLE PLAN_PRODUCTO (
    id_tiempo           INT             NOT NULL,
    codigo_barras       VARCHAR(50)     NOT NULL,
    cantidad_porciones  DECIMAL(5,2)    NOT NULL,
    CONSTRAINT PK_PLAN_PRODUCTO PRIMARY KEY (id_tiempo, codigo_barras),
    CONSTRAINT FK_PP_TIEMPO FOREIGN KEY (id_tiempo)
        REFERENCES TIEMPO_COMIDA_PLAN(id_tiempo),
    CONSTRAINT FK_PP_PRODUCTO FOREIGN KEY (codigo_barras)
        REFERENCES PRODUCTO(codigo_barras)
);
