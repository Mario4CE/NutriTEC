/*
    Descripción:
        Crea la tabla NUTRICIONISTA para almacenar los datos personales, profesionales y credenciales de nutricionistas.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla NUTRICIONISTA con clave primaria cedula y restricción UNIQUE sobre codigo_nutricionista y email.

    Restricciones:
        - Ejecutar en una base de datos SQL Server seleccionada previamente.
        - No almacenar contraseñas en texto plano; password_hash debe contener un hash seguro.
        - tipo_cobro solo permite los valores semanal, mensual o anual.
        - Ejecutar antes de tablas que referencian nutricionistas.
*/
CREATE TABLE NUTRICIONISTA (
    cedula                  VARCHAR(20)     PRIMARY KEY,
    nombre                  VARCHAR(100)    NOT NULL,
    apellidos               VARCHAR(100)    NOT NULL,
    codigo_nutricionista    VARCHAR(50)     NOT NULL UNIQUE,
    edad                    INT             NOT NULL,
    fecha_nacimiento        DATE            NOT NULL,
    peso                    DECIMAL(5,2)    NOT NULL,
    imc                     DECIMAL(5,2)    NOT NULL,
    direccion               VARCHAR(255)    NOT NULL,
    foto_url                VARCHAR(500),
    tarjeta_credito         VARCHAR(20)     NOT NULL,
    tipo_cobro              VARCHAR(10)     NOT NULL CHECK (tipo_cobro IN ('semanal', 'mensual', 'anual')),
    email                   VARCHAR(100)    NOT NULL UNIQUE,
    password_hash           VARCHAR(255)    NOT NULL
);
