/*
    Descripción:
        Crea la tabla ASIGNACION_PLAN para asignar planes alimenticios a usuarios durante un rango de fechas.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla ASIGNACION_PLAN con clave primaria id_asignacion, llaves foráneas hacia PLAN_ALIMENTACION y USUARIO, y validación de fechas.

    Restricciones:
        - Ejecutar después de crear las tablas PLAN_ALIMENTACION y USUARIO.
        - id_plan debe existir previamente en PLAN_ALIMENTACION.
        - id_usuario debe existir previamente en USUARIO.
        - fecha_fin debe ser mayor o igual que fecha_inicio.
*/
CREATE TABLE ASIGNACION_PLAN (
    id_asignacion   INT IDENTITY(1,1) PRIMARY KEY,
    id_plan         INT             NOT NULL,
    id_usuario      INT             NOT NULL,
    fecha_inicio    DATE            NOT NULL,
    fecha_fin       DATE            NOT NULL,
    CONSTRAINT FK_ASIG_PLAN FOREIGN KEY (id_plan)
        REFERENCES PLAN_ALIMENTACION(id_plan),
    CONSTRAINT FK_ASIG_USUARIO FOREIGN KEY (id_usuario)
        REFERENCES USUARIO(id_usuario),
    CONSTRAINT CHK_FECHAS CHECK (fecha_fin >= fecha_inicio)
);
