/*
    Descripción:
        Crea la tabla TIEMPO_COMIDA_PLAN para definir tiempos de comida dentro de un plan alimenticio.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla TIEMPO_COMIDA_PLAN con clave primaria id_tiempo, llave foránea hacia PLAN_ALIMENTACION y restricción de unicidad por plan y tipo de comida.

    Restricciones:
        - Ejecutar después de crear la tabla PLAN_ALIMENTACION.
        - id_plan debe existir previamente en PLAN_ALIMENTACION.
        - tipo_comida solo permite Desayuno, Merienda Mañana, Almuerzo, Merienda Tarde o Cena.
*/
CREATE TABLE TIEMPO_COMIDA_PLAN (
    id_tiempo   INT IDENTITY(1,1) PRIMARY KEY,
    id_plan     INT             NOT NULL,
    tipo_comida VARCHAR(20)     NOT NULL CHECK (tipo_comida IN (
                    'Desayuno',
                    'Merienda Mañana',
                    'Almuerzo',
                    'Merienda Tarde',
                    'Cena'
                )),
    CONSTRAINT FK_TIEMPO_PLAN FOREIGN KEY (id_plan)
        REFERENCES PLAN_ALIMENTACION(id_plan),
    CONSTRAINT UQ_TIEMPO_PLAN UNIQUE (id_plan, tipo_comida)
);
