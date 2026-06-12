/*
    Descripción:
        Crea la tabla PACIENTE_NUTRICIONISTA para relacionar pacientes con nutricionistas.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla PACIENTE_NUTRICIONISTA con clave primaria compuesta y llaves foráneas hacia NUTRICIONISTA y USUARIO.

    Restricciones:
        - Ejecutar después de crear las tablas NUTRICIONISTA y USUARIO.
        - cedula_nutricionista debe existir previamente en NUTRICIONISTA.
        - id_usuario debe existir previamente en USUARIO.
*/
CREATE TABLE PACIENTE_NUTRICIONISTA (
    cedula_nutricionista    VARCHAR(20) NOT NULL,
    id_usuario              INT         NOT NULL,
    CONSTRAINT PK_PACIENTE_NUTRICIONISTA PRIMARY KEY (cedula_nutricionista, id_usuario),
    CONSTRAINT FK_PN_NUTRICIONISTA FOREIGN KEY (cedula_nutricionista)
        REFERENCES NUTRICIONISTA(cedula),
    CONSTRAINT FK_PN_USUARIO FOREIGN KEY (id_usuario)
        REFERENCES USUARIO(id_usuario)
);
