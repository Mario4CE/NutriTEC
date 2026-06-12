/*
    Descripción:
        Crea la tabla ADMINISTRADOR para almacenar credenciales de usuarios administradores del sistema.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla ADMINISTRADOR con clave primaria id_admin y restricción UNIQUE sobre email.

    Restricciones:
        - Ejecutar en una base de datos SQL Server seleccionada previamente.
        - No almacenar contraseñas en texto plano; password_hash debe contener un hash seguro.
        - Ejecutar antes de cualquier script que dependa de administradores.
*/
CREATE TABLE ADMINISTRADOR (
    id_admin        INT IDENTITY(1,1) PRIMARY KEY,
    email           VARCHAR(100) NOT NULL UNIQUE,
    password_hash   VARCHAR(255) NOT NULL
);
