/*
    Descripción:
        Crea el catálogo TIPO_COBRO para normalizar los tipos de cobro permitidos por nutricionistas.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla TIPO_COBRO con clave primaria codigo_tipo_cobro.

    Restricciones:
        - Ejecutar antes de NUTRICIONISTA.
        - No almacena datos personales, credenciales, tarjetas ni secretos.
        - Los valores base se insertan desde Database/SqlServer/Seed/001_TIPO_COBRO.sql.
*/
CREATE TABLE TIPO_COBRO (
    codigo_tipo_cobro      VARCHAR(10)     PRIMARY KEY,
    nombre                 VARCHAR(50)     NOT NULL,
    activo                 BIT             NOT NULL DEFAULT 1
);
