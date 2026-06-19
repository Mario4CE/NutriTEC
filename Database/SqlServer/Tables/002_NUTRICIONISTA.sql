/*
    Descripción:
        Crea la tabla NUTRICIONISTA para almacenar los datos personales, profesionales y credenciales de nutricionistas.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla NUTRICIONISTA con clave primaria cedula, restricción UNIQUE sobre codigo_nutricionista y email, y FK hacia TIPO_COBRO.

    Restricciones:
        - Ejecutar en una base de datos SQL Server seleccionada previamente.
        - No almacenar contraseñas en texto plano; password_hash debe contener un hash seguro.
        - tarjeta_credito almacena únicamente un valor enmascarado/simulado; no guardar tarjeta completa ni CVV.
        - tipo_cobro debe existir previamente en el catálogo TIPO_COBRO.
        - Ejecutar después de TIPO_COBRO y antes de tablas que referencian nutricionistas.
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
    tarjeta_credito         VARCHAR(20)     NOT NULL, -- Valor enmascarado/simulado, por ejemplo ****-****-****-1111
    tipo_cobro              VARCHAR(10)     NOT NULL,
    email                   VARCHAR(100)    NOT NULL UNIQUE,
    password_hash           VARCHAR(255)    NOT NULL,

    CONSTRAINT FK_NUTRICIONISTA_TIPO_COBRO
        FOREIGN KEY (tipo_cobro)
        REFERENCES TIPO_COBRO(codigo_tipo_cobro)
);
