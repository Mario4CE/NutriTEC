/*
    Descripción:
        Crea la tabla REGISTRO_DIARIO para registrar comidas consumidas por usuarios en una fecha específica.

    Entradas:
        No recibe parámetros. Este script se ejecuta directamente sobre SQL Server.

    Salidas:
        Tabla REGISTRO_DIARIO con clave primaria id_registro y llave foránea hacia USUARIO.

    Restricciones:
        - Ejecutar después de crear la tabla USUARIO.
        - id_usuario debe existir previamente en USUARIO.
        - tipo_comida solo permite Desayuno, Merienda Mañana, Almuerzo, Merienda Tarde o Cena.
*/
CREATE TABLE REGISTRO_DIARIO (
    id_registro INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario  INT             NOT NULL,
    fecha       DATE            NOT NULL,
    tipo_comida VARCHAR(20)     NOT NULL CHECK (tipo_comida IN (
                    'Desayuno',
                    'Merienda Mañana',
                    'Almuerzo',
                    'Merienda Tarde',
                    'Cena'
                )),
    CONSTRAINT FK_REG_USUARIO FOREIGN KEY (id_usuario)
        REFERENCES USUARIO(id_usuario)
);
