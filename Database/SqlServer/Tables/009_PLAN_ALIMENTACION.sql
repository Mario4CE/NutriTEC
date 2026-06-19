/*
    Descripción:
        Crea la tabla PLAN_ALIMENTACION para almacenar planes alimenticios definidos por nutricionistas.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla PLAN_ALIMENTACION con clave primaria id_plan y llave foránea hacia NUTRICIONISTA.

    Restricciones:
        - Ejecutar después de crear la tabla NUTRICIONISTA.
        - cedula_nutricionista debe existir previamente en NUTRICIONISTA.
*/
CREATE TABLE PLAN_ALIMENTACION (
    id_plan                 INT IDENTITY(1,1) PRIMARY KEY,
    nombre                  VARCHAR(150)    NOT NULL,
    cedula_nutricionista    VARCHAR(20)     NOT NULL,
    total_calorias          DECIMAL(7,2)    NOT NULL DEFAULT 0,
    CONSTRAINT FK_PLAN_NUTRICIONISTA FOREIGN KEY (cedula_nutricionista)
        REFERENCES NUTRICIONISTA(cedula)
);
