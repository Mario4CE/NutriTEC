/*
Descripción:
Crea la tabla relacional inicial para registrar productos alimenticios pendientes de aprobación.

Entradas:
No recibe parámetros; debe ejecutarse sobre la base de datos NutriTec.

Salidas:
Crea dbo.Productos y su restricción única de código de barras.

Restricciones:
Ejecutar una sola vez durante la inicialización del esquema SQL Server.
*/


/// <summary>
/// Esta es un tabla de ejmplo de como se va a trabajar, si se tiene que hacer algun cambio se hace ya que aun no esta crada la base de datos. La tabla es de referencia
/// </summary>

CREATE TABLE dbo.Productos
(
    Id UNIQUEIDENTIFIER NOT NULL,
    Nombre NVARCHAR(150) NOT NULL,
    CodigoBarras NVARCHAR(64) NOT NULL,
    Calorias DECIMAL(10, 2) NOT NULL,
    Proteinas DECIMAL(10, 2) NOT NULL,
    Carbohidratos DECIMAL(10, 2) NOT NULL,
    Grasas DECIMAL(10, 2) NOT NULL,
    EstaAprobado BIT NOT NULL CONSTRAINT DF_Productos_EstaAprobado DEFAULT (0),
    FechaCreacionUtc DATETIME2 NOT NULL,
    CONSTRAINT PK_Productos PRIMARY KEY (Id),
    CONSTRAINT UQ_Productos_CodigoBarras UNIQUE (CodigoBarras),
    CONSTRAINT CK_Productos_ValoresNutricionales CHECK
    (
        Calorias >= 0 AND Proteinas >= 0 AND Carbohidratos >= 0 AND Grasas >= 0
    )
);
