/*
    Descripción:
        Crea la tabla USUARIO para almacenar los datos personales, métricas corporales y credenciales de usuarios finales.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla USUARIO con clave primaria id_usuario y restricción UNIQUE sobre email.

    Restricciones:
        - Ejecutar en una base de datos SQL Server seleccionada previamente.
        - No almacenar contraseñas en texto plano; password_hash debe contener un hash seguro.
        - Ejecutar antes de tablas que referencian usuarios.
*/
CREATE TABLE USUARIO (
    id_usuario              INT IDENTITY(1,1) PRIMARY KEY,
    nombre                  VARCHAR(100)    NOT NULL,
    apellidos               VARCHAR(100)    NOT NULL,
    edad                    INT             NOT NULL,
    fecha_nacimiento        DATE            NOT NULL,
    peso                    DECIMAL(5,2)    NOT NULL,
    imc                     DECIMAL(5,2)    NOT NULL,
    pais                    VARCHAR(100)    NOT NULL,
    cintura                 DECIMAL(5,2),
    cuello                  DECIMAL(5,2),
    caderas                 DECIMAL(5,2),
    pct_musculo             DECIMAL(5,2),
    pct_grasa               DECIMAL(5,2),
    calorias_diarias_max    DECIMAL(7,2)    NOT NULL,
    email                   VARCHAR(100)    NOT NULL UNIQUE,
    password_hash           VARCHAR(255)    NOT NULL
);
