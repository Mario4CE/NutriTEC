/* =========================================================
   NutriTEC - Script SQL Server corregido

   Correcciones para los errores mostrados:
   1) Se separan lotes con GO antes de FUNCTION, VIEW,
      PROCEDURE y TRIGGER.
   2) Se elimina CREATE OR ALTER y se usa DROP + CREATE.
   3) Se reemplaza THROW por RAISERROR, útil si Visual Studio
      o el proyecto SQL está configurado para una versión antigua.
   4) Se usa el esquema dbo de forma explícita.

   IMPORTANTE:
   - Ejecutar sobre una base de datos de prueba o una base vacía.
   - El bloque DROP elimina los objetos existentes de este modelo.
   ========================================================= */

/* =========================================================
   0. LIMPIEZA DE OBJETOS EXISTENTES
   ========================================================= */

IF OBJECT_ID('dbo.trg_RecalcularTotalPlan', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_RecalcularTotalPlan;
GO

IF OBJECT_ID('dbo.trg_RecalcularTotalesReceta', 'TR') IS NOT NULL
    DROP TRIGGER dbo.trg_RecalcularTotalesReceta;
GO

IF OBJECT_ID('dbo.sp_RegistrarMedidaUsuario', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_RegistrarMedidaUsuario;
GO

IF OBJECT_ID('dbo.sp_AsignarPlanPaciente', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_AsignarPlanPaciente;
GO

IF OBJECT_ID('dbo.sp_AprobarProducto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_AprobarProducto;
GO

IF OBJECT_ID('dbo.sp_ReporteCobroNutricionistas', 'P') IS NOT NULL
    DROP PROCEDURE dbo.sp_ReporteCobroNutricionistas;
GO

IF OBJECT_ID('dbo.vw_ResumenRegistroDiario', 'V') IS NOT NULL
    DROP VIEW dbo.vw_ResumenRegistroDiario;
GO

IF OBJECT_ID('dbo.vw_DetallePlanAlimentacion', 'V') IS NOT NULL
    DROP VIEW dbo.vw_DetallePlanAlimentacion;
GO

IF OBJECT_ID('dbo.vw_PacientesPorNutricionista', 'V') IS NOT NULL
    DROP VIEW dbo.vw_PacientesPorNutricionista;
GO

IF OBJECT_ID('dbo.fn_TotalCaloriasPlan', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_TotalCaloriasPlan;
GO

IF OBJECT_ID('dbo.fn_CalcularImc', 'FN') IS NOT NULL
    DROP FUNCTION dbo.fn_CalcularImc;
GO

IF OBJECT_ID('dbo.REGISTRO_PRODUCTO', 'U') IS NOT NULL
    DROP TABLE dbo.REGISTRO_PRODUCTO;
GO

IF OBJECT_ID('dbo.REGISTRO_DIARIO', 'U') IS NOT NULL
    DROP TABLE dbo.REGISTRO_DIARIO;
GO

IF OBJECT_ID('dbo.ASIGNACION_PLAN', 'U') IS NOT NULL
    DROP TABLE dbo.ASIGNACION_PLAN;
GO

IF OBJECT_ID('dbo.PLAN_PRODUCTO', 'U') IS NOT NULL
    DROP TABLE dbo.PLAN_PRODUCTO;
GO

IF OBJECT_ID('dbo.TIEMPO_COMIDA_PLAN', 'U') IS NOT NULL
    DROP TABLE dbo.TIEMPO_COMIDA_PLAN;
GO

IF OBJECT_ID('dbo.PLAN_ALIMENTACION', 'U') IS NOT NULL
    DROP TABLE dbo.PLAN_ALIMENTACION;
GO

IF OBJECT_ID('dbo.RECETA_PRODUCTO', 'U') IS NOT NULL
    DROP TABLE dbo.RECETA_PRODUCTO;
GO

IF OBJECT_ID('dbo.RECETA', 'U') IS NOT NULL
    DROP TABLE dbo.RECETA;
GO

IF OBJECT_ID('dbo.PACIENTE_NUTRICIONISTA', 'U') IS NOT NULL
    DROP TABLE dbo.PACIENTE_NUTRICIONISTA;
GO

IF OBJECT_ID('dbo.MEDIDA_USUARIO', 'U') IS NOT NULL
    DROP TABLE dbo.MEDIDA_USUARIO;
GO

IF OBJECT_ID('dbo.PRODUCTO', 'U') IS NOT NULL
    DROP TABLE dbo.PRODUCTO;
GO

IF OBJECT_ID('dbo.NUTRICIONISTA', 'U') IS NOT NULL
    DROP TABLE dbo.NUTRICIONISTA;
GO

IF OBJECT_ID('dbo.USUARIO', 'U') IS NOT NULL
    DROP TABLE dbo.USUARIO;
GO

IF OBJECT_ID('dbo.ADMINISTRADOR', 'U') IS NOT NULL
    DROP TABLE dbo.ADMINISTRADOR;
GO

IF OBJECT_ID('dbo.TIPO_COBRO', 'U') IS NOT NULL
    DROP TABLE dbo.TIPO_COBRO;
GO

/* =========================================================
   1. TABLAS
   ========================================================= */

CREATE TABLE dbo.TIPO_COBRO (
    codigo_tipo_cobro      VARCHAR(10)     PRIMARY KEY,
    nombre                 VARCHAR(50)     NOT NULL,
    activo                 BIT             NOT NULL DEFAULT 1
);
GO

MERGE dbo.TIPO_COBRO AS target
USING (VALUES
    ('semanal', 'Semanal', CAST(1 AS BIT)),
    ('mensual', 'Mensual', CAST(1 AS BIT)),
    ('anual', 'Anual', CAST(1 AS BIT))
) AS source (codigo_tipo_cobro, nombre, activo)
ON target.codigo_tipo_cobro = source.codigo_tipo_cobro
WHEN MATCHED THEN
    UPDATE SET
        nombre = source.nombre,
        activo = source.activo
WHEN NOT MATCHED THEN
    INSERT (codigo_tipo_cobro, nombre, activo)
    VALUES (source.codigo_tipo_cobro, source.nombre, source.activo);
GO

CREATE TABLE dbo.ADMINISTRADOR (
    id_admin        INT IDENTITY(1,1) PRIMARY KEY,
    email           VARCHAR(100) NOT NULL UNIQUE,
    password_hash   VARCHAR(255) NOT NULL
);
GO

CREATE TABLE dbo.NUTRICIONISTA (
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
    tipo_cobro              VARCHAR(10)     NOT NULL,
    email                   VARCHAR(100)    NOT NULL UNIQUE,
    password_hash           VARCHAR(255)    NOT NULL,

    CONSTRAINT FK_NUTRICIONISTA_TIPO_COBRO
        FOREIGN KEY (tipo_cobro)
        REFERENCES dbo.TIPO_COBRO(codigo_tipo_cobro)
);
GO

CREATE TABLE dbo.USUARIO (
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
GO

CREATE TABLE dbo.MEDIDA_USUARIO (
    id_medida       INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario      INT             NOT NULL,
    fecha           DATE            NOT NULL,
    cintura         DECIMAL(5,2),
    cuello          DECIMAL(5,2),
    caderas         DECIMAL(5,2),
    pct_musculo     DECIMAL(5,2),
    pct_grasa       DECIMAL(5,2),
    CONSTRAINT FK_MEDIDA_USUARIO
        FOREIGN KEY (id_usuario)
        REFERENCES dbo.USUARIO(id_usuario)
);
GO

CREATE TABLE dbo.PACIENTE_NUTRICIONISTA (
    cedula_nutricionista    VARCHAR(20) NOT NULL,
    id_usuario              INT         NOT NULL,
    CONSTRAINT PK_PACIENTE_NUTRICIONISTA
        PRIMARY KEY (cedula_nutricionista, id_usuario),
    CONSTRAINT FK_PN_NUTRICIONISTA
        FOREIGN KEY (cedula_nutricionista)
        REFERENCES dbo.NUTRICIONISTA(cedula),
    CONSTRAINT FK_PN_USUARIO
        FOREIGN KEY (id_usuario)
        REFERENCES dbo.USUARIO(id_usuario)
);
GO

CREATE TABLE dbo.PRODUCTO (
    id_producto         UNIQUEIDENTIFIER    PRIMARY KEY,
    nombre              VARCHAR(150)        NOT NULL,
    codigo_barras       VARCHAR(64)         NOT NULL UNIQUE,
    calorias            DECIMAL(10,2)       NOT NULL,
    proteinas           DECIMAL(10,2)       NOT NULL,
    carbohidratos       DECIMAL(10,2)       NOT NULL,
    grasas              DECIMAL(10,2)       NOT NULL,
    aprobado            BIT                 NOT NULL DEFAULT 0,
    fecha_creacion_utc  DATETIME2           NOT NULL
);
GO

CREATE TABLE dbo.RECETA (
    id_receta           INT IDENTITY(1,1) PRIMARY KEY,
    nombre              VARCHAR(150)    NOT NULL,
    id_usuario          INT             NOT NULL,
    total_calorias      DECIMAL(7,2)    NOT NULL DEFAULT 0,
    total_carbohidratos DECIMAL(7,2)    NOT NULL DEFAULT 0,
    total_proteina      DECIMAL(7,2)    NOT NULL DEFAULT 0,
    total_grasa         DECIMAL(7,2)    NOT NULL DEFAULT 0,
    CONSTRAINT FK_RECETA_USUARIO
        FOREIGN KEY (id_usuario)
        REFERENCES dbo.USUARIO(id_usuario)
);
GO

CREATE TABLE dbo.RECETA_PRODUCTO (
    id_receta           INT              NOT NULL,
    id_producto         UNIQUEIDENTIFIER NOT NULL,
    cantidad_porciones  DECIMAL(5,2)     NOT NULL,
    CONSTRAINT PK_RECETA_PRODUCTO
        PRIMARY KEY (id_receta, id_producto),
    CONSTRAINT FK_RP_RECETA
        FOREIGN KEY (id_receta)
        REFERENCES dbo.RECETA(id_receta),
    CONSTRAINT FK_RP_PRODUCTO
        FOREIGN KEY (id_producto)
        REFERENCES dbo.PRODUCTO(id_producto)
);
GO

CREATE TABLE dbo.PLAN_ALIMENTACION (
    id_plan                 INT IDENTITY(1,1) PRIMARY KEY,
    nombre                  VARCHAR(150)    NOT NULL,
    cedula_nutricionista    VARCHAR(20)     NOT NULL,
    total_calorias          DECIMAL(7,2)    NOT NULL DEFAULT 0,
    CONSTRAINT FK_PLAN_NUTRICIONISTA
        FOREIGN KEY (cedula_nutricionista)
        REFERENCES dbo.NUTRICIONISTA(cedula)
);
GO

CREATE TABLE dbo.TIEMPO_COMIDA_PLAN (
    id_tiempo   INT IDENTITY(1,1) PRIMARY KEY,
    id_plan     INT             NOT NULL,
    tipo_comida VARCHAR(20)     NOT NULL CHECK (tipo_comida IN (
                    'Desayuno',
                    'Merienda Mañana',
                    'Almuerzo',
                    'Merienda Tarde',
                    'Cena'
                )),
    CONSTRAINT FK_TIEMPO_PLAN
        FOREIGN KEY (id_plan)
        REFERENCES dbo.PLAN_ALIMENTACION(id_plan),
    CONSTRAINT UQ_TIEMPO_PLAN
        UNIQUE (id_plan, tipo_comida)
);
GO

CREATE TABLE dbo.PLAN_PRODUCTO (
    id_tiempo           INT              NOT NULL,
    id_producto         UNIQUEIDENTIFIER NOT NULL,
    cantidad_porciones  DECIMAL(5,2)     NOT NULL,
    CONSTRAINT PK_PLAN_PRODUCTO
        PRIMARY KEY (id_tiempo, id_producto),
    CONSTRAINT FK_PP_TIEMPO
        FOREIGN KEY (id_tiempo)
        REFERENCES dbo.TIEMPO_COMIDA_PLAN(id_tiempo),
    CONSTRAINT FK_PP_PRODUCTO
        FOREIGN KEY (id_producto)
        REFERENCES dbo.PRODUCTO(id_producto)
);
GO

CREATE TABLE dbo.ASIGNACION_PLAN (
    id_asignacion   INT IDENTITY(1,1) PRIMARY KEY,
    id_plan         INT             NOT NULL,
    id_usuario      INT             NOT NULL,
    fecha_inicio    DATE            NOT NULL,
    fecha_fin       DATE            NOT NULL,
    CONSTRAINT FK_ASIG_PLAN
        FOREIGN KEY (id_plan)
        REFERENCES dbo.PLAN_ALIMENTACION(id_plan),
    CONSTRAINT FK_ASIG_USUARIO
        FOREIGN KEY (id_usuario)
        REFERENCES dbo.USUARIO(id_usuario),
    CONSTRAINT CHK_FECHAS
        CHECK (fecha_fin >= fecha_inicio)
);
GO

CREATE TABLE dbo.REGISTRO_DIARIO (
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
    CONSTRAINT FK_REG_USUARIO
        FOREIGN KEY (id_usuario)
        REFERENCES dbo.USUARIO(id_usuario)
);
GO

CREATE TABLE dbo.REGISTRO_PRODUCTO (
    id_registro         INT              NOT NULL,
    id_producto         UNIQUEIDENTIFIER NOT NULL,
    cantidad_porciones  DECIMAL(5,2)     NOT NULL,
    CONSTRAINT PK_REGISTRO_PRODUCTO
        PRIMARY KEY (id_registro, id_producto),
    CONSTRAINT FK_REGP_REGISTRO
        FOREIGN KEY (id_registro)
        REFERENCES dbo.REGISTRO_DIARIO(id_registro),
    CONSTRAINT FK_REGP_PRODUCTO
        FOREIGN KEY (id_producto)
        REFERENCES dbo.PRODUCTO(id_producto)
);
GO

/* =========================================================
   2. FUNCIONES
   ========================================================= */

CREATE FUNCTION dbo.fn_CalcularImc
(
    @pesoKg DECIMAL(5,2),
    @estaturaCm DECIMAL(5,2)
)
RETURNS DECIMAL(5,2)
AS
BEGIN
    DECLARE @imc DECIMAL(5,2);
    DECLARE @estaturaMetros DECIMAL(8,4);

    IF @pesoKg IS NULL OR @pesoKg <= 0 OR @estaturaCm IS NULL OR @estaturaCm <= 0
    BEGIN
        RETURN NULL;
    END;

    SET @estaturaMetros = @estaturaCm / 100.0;
    SET @imc = ROUND(@pesoKg / NULLIF(@estaturaMetros * @estaturaMetros, 0), 2);

    RETURN @imc;
END;
GO

CREATE FUNCTION dbo.fn_TotalCaloriasPlan
(
    @idPlan INT
)
RETURNS DECIMAL(10,2)
AS
BEGIN
    DECLARE @total DECIMAL(10,2);

    SELECT @total = COALESCE(SUM(p.calorias * pp.cantidad_porciones), 0)
    FROM dbo.PLAN_ALIMENTACION pa
    INNER JOIN dbo.TIEMPO_COMIDA_PLAN tcp ON tcp.id_plan = pa.id_plan
    INNER JOIN dbo.PLAN_PRODUCTO pp ON pp.id_tiempo = tcp.id_tiempo
    INNER JOIN dbo.PRODUCTO p ON p.id_producto = pp.id_producto
    WHERE pa.id_plan = @idPlan;

    RETURN COALESCE(@total, 0);
END;
GO

/* =========================================================
   3. VISTAS
   ========================================================= */

CREATE VIEW dbo.vw_PacientesPorNutricionista
AS
SELECT
    n.cedula AS cedula_nutricionista,
    CONCAT(n.nombre, ' ', n.apellidos) AS nombre_nutricionista,
    n.email AS email_nutricionista,
    u.id_usuario,
    CONCAT(u.nombre, ' ', u.apellidos) AS nombre_paciente,
    u.email AS email_paciente,
    u.pais,
    u.calorias_diarias_max
FROM dbo.PACIENTE_NUTRICIONISTA pn
INNER JOIN dbo.NUTRICIONISTA n ON n.cedula = pn.cedula_nutricionista
INNER JOIN dbo.USUARIO u ON u.id_usuario = pn.id_usuario;
GO

CREATE VIEW dbo.vw_DetallePlanAlimentacion
AS
SELECT
    pa.id_plan,
    pa.nombre AS nombre_plan,
    pa.cedula_nutricionista,
    CONCAT(n.nombre, ' ', n.apellidos) AS nombre_nutricionista,
    tcp.id_tiempo,
    tcp.tipo_comida,
    p.id_producto,
    p.codigo_barras,
    p.nombre AS nombre_producto,
    pp.cantidad_porciones,
    p.calorias,
    CAST(p.calorias * pp.cantidad_porciones AS DECIMAL(10,2)) AS subtotal_calorias
FROM dbo.PLAN_ALIMENTACION pa
INNER JOIN dbo.NUTRICIONISTA n ON n.cedula = pa.cedula_nutricionista
INNER JOIN dbo.TIEMPO_COMIDA_PLAN tcp ON tcp.id_plan = pa.id_plan
INNER JOIN dbo.PLAN_PRODUCTO pp ON pp.id_tiempo = tcp.id_tiempo
INNER JOIN dbo.PRODUCTO p ON p.id_producto = pp.id_producto;
GO

CREATE VIEW dbo.vw_ResumenRegistroDiario
AS
SELECT
    rd.id_usuario,
    CONCAT(u.nombre, ' ', u.apellidos) AS nombre_usuario,
    rd.fecha,
    SUM(p.calorias * rp.cantidad_porciones) AS total_calorias,
    SUM(p.proteinas * rp.cantidad_porciones) AS total_proteinas,
    SUM(p.carbohidratos * rp.cantidad_porciones) AS total_carbohidratos,
    SUM(p.grasas * rp.cantidad_porciones) AS total_grasas,
    COUNT_BIG(*) AS cantidad_productos_registrados
FROM dbo.REGISTRO_DIARIO rd
INNER JOIN dbo.USUARIO u ON u.id_usuario = rd.id_usuario
INNER JOIN dbo.REGISTRO_PRODUCTO rp ON rp.id_registro = rd.id_registro
INNER JOIN dbo.PRODUCTO p ON p.id_producto = rp.id_producto
GROUP BY rd.id_usuario, u.nombre, u.apellidos, rd.fecha;
GO

/* =========================================================
   4. PROCEDIMIENTOS ALMACENADOS
   ========================================================= */

CREATE PROCEDURE dbo.sp_ReporteCobroNutricionistas
    @montoBasePorPaciente DECIMAL(10,2),
    @incluirSinPacientes BIT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @montoBasePorPaciente IS NULL OR @montoBasePorPaciente <= 0
    BEGIN
        RAISERROR('El monto base por paciente debe ser mayor que cero.', 16, 1);
        RETURN;
    END;

    DECLARE @cedula VARCHAR(20);
    DECLARE @nombreCompleto VARCHAR(210);
    DECLARE @tipoCobro VARCHAR(10);
    DECLARE @cantidadPacientes INT;
    DECLARE @factorPeriodo DECIMAL(10,2);
    DECLARE @porcentajeDescuento DECIMAL(5,2);
    DECLARE @subtotal DECIMAL(10,2);
    DECLARE @descuento DECIMAL(10,2);
    DECLARE @total DECIMAL(10,2);

    CREATE TABLE #ReporteCobro
    (
        cedula_nutricionista VARCHAR(20) NOT NULL,
        nombre_nutricionista VARCHAR(210) NOT NULL,
        tipo_cobro VARCHAR(10) NOT NULL,
        cantidad_pacientes INT NOT NULL,
        monto_base_por_paciente DECIMAL(10,2) NOT NULL,
        subtotal DECIMAL(10,2) NOT NULL,
        porcentaje_descuento DECIMAL(5,2) NOT NULL,
        monto_descuento DECIMAL(10,2) NOT NULL,
        total_cobrar DECIMAL(10,2) NOT NULL
    );

    DECLARE nutricionistas_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT
            n.cedula,
            CONCAT(n.nombre, ' ', n.apellidos),
            n.tipo_cobro,
            COUNT(pn.id_usuario) AS cantidad_pacientes
        FROM dbo.NUTRICIONISTA n
        LEFT JOIN dbo.PACIENTE_NUTRICIONISTA pn ON pn.cedula_nutricionista = n.cedula
        GROUP BY n.cedula, n.nombre, n.apellidos, n.tipo_cobro
        HAVING @incluirSinPacientes = 1 OR COUNT(pn.id_usuario) > 0;

    OPEN nutricionistas_cursor;
    FETCH NEXT FROM nutricionistas_cursor INTO @cedula, @nombreCompleto, @tipoCobro, @cantidadPacientes;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @factorPeriodo = CASE @tipoCobro
            WHEN 'semanal' THEN 1
            WHEN 'mensual' THEN 4
            WHEN 'anual' THEN 52
            ELSE 1
        END;

        SET @porcentajeDescuento = CASE @tipoCobro
            WHEN 'mensual' THEN 0.05
            WHEN 'anual' THEN 0.10
            ELSE 0
        END;

        SET @subtotal = ROUND(@cantidadPacientes * @montoBasePorPaciente * @factorPeriodo, 2);
        SET @descuento = ROUND(@subtotal * @porcentajeDescuento, 2);
        SET @total = @subtotal - @descuento;

        INSERT INTO #ReporteCobro
        VALUES (@cedula, @nombreCompleto, @tipoCobro, @cantidadPacientes, @montoBasePorPaciente, @subtotal, @porcentajeDescuento, @descuento, @total);

        FETCH NEXT FROM nutricionistas_cursor INTO @cedula, @nombreCompleto, @tipoCobro, @cantidadPacientes;
    END;

    CLOSE nutricionistas_cursor;
    DEALLOCATE nutricionistas_cursor;

    SELECT *
    FROM #ReporteCobro
    ORDER BY nombre_nutricionista;
END;
GO

CREATE PROCEDURE dbo.sp_AprobarProducto
    @idProducto UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    IF @idProducto IS NULL
    BEGIN
        RAISERROR('El identificador del producto es obligatorio.', 16, 1);
        RETURN;
    END;

    BEGIN TRANSACTION;

    IF NOT EXISTS (SELECT 1 FROM dbo.PRODUCTO WITH (UPDLOCK, HOLDLOCK) WHERE id_producto = @idProducto)
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('El producto no existe.', 16, 1);
        RETURN;
    END;

    IF EXISTS (
        SELECT 1
        FROM dbo.PRODUCTO
        WHERE id_producto = @idProducto
          AND (calorias < 0 OR proteinas < 0 OR carbohidratos < 0 OR grasas < 0)
    )
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('El producto tiene valores nutricionales invalidos.', 16, 1);
        RETURN;
    END;

    UPDATE dbo.PRODUCTO
    SET aprobado = 1
    WHERE id_producto = @idProducto;

    COMMIT TRANSACTION;

    SELECT id_producto, nombre, codigo_barras, calorias, proteinas, carbohidratos, grasas, aprobado, fecha_creacion_utc
    FROM dbo.PRODUCTO
    WHERE id_producto = @idProducto;
END;
GO

CREATE PROCEDURE dbo.sp_AsignarPlanPaciente
    @idPlan INT,
    @idUsuario INT,
    @fechaInicio DATE,
    @fechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @cedulaNutricionista VARCHAR(20);
    DECLARE @idAsignacion INT;

    IF @fechaInicio IS NULL OR @fechaFin IS NULL OR @fechaFin < @fechaInicio
    BEGIN
        RAISERROR('El rango de fechas de la asignacion no es valido.', 16, 1);
        RETURN;
    END;

    SELECT @cedulaNutricionista = cedula_nutricionista
    FROM dbo.PLAN_ALIMENTACION
    WHERE id_plan = @idPlan;

    IF @cedulaNutricionista IS NULL
    BEGIN
        RAISERROR('El plan alimenticio no existe.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (
        SELECT 1
        FROM dbo.PACIENTE_NUTRICIONISTA
        WHERE cedula_nutricionista = @cedulaNutricionista
          AND id_usuario = @idUsuario
    )
    BEGIN
        RAISERROR('El paciente no esta asociado al nutricionista del plan.', 16, 1);
        RETURN;
    END;

    BEGIN TRANSACTION;

    IF EXISTS (
        SELECT 1
        FROM dbo.ASIGNACION_PLAN WITH (UPDLOCK, HOLDLOCK)
        WHERE id_plan = @idPlan
          AND id_usuario = @idUsuario
          AND fecha_inicio = @fechaInicio
          AND fecha_fin = @fechaFin
    )
    BEGIN
        ROLLBACK TRANSACTION;
        RAISERROR('Ya existe una asignacion igual para ese paciente.', 16, 1);
        RETURN;
    END;

    INSERT INTO dbo.ASIGNACION_PLAN (id_plan, id_usuario, fecha_inicio, fecha_fin)
    VALUES (@idPlan, @idUsuario, @fechaInicio, @fechaFin);

    SET @idAsignacion = CONVERT(INT, SCOPE_IDENTITY());

    COMMIT TRANSACTION;

    SELECT @idAsignacion AS id_asignacion;
END;
GO

CREATE PROCEDURE dbo.sp_RegistrarMedidaUsuario
    @idUsuario INT,
    @fecha DATE,
    @pesoKg DECIMAL(5,2),
    @estaturaCm DECIMAL(5,2),
    @cintura DECIMAL(5,2) = NULL,
    @cuello DECIMAL(5,2) = NULL,
    @caderas DECIMAL(5,2) = NULL,
    @pctMusculo DECIMAL(5,2) = NULL,
    @pctGrasa DECIMAL(5,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @imc DECIMAL(5,2);
    DECLARE @idMedida INT;

    SET @imc = dbo.fn_CalcularImc(@pesoKg, @estaturaCm);

    IF @fecha IS NULL
    BEGIN
        RAISERROR('La fecha de la medida es obligatoria.', 16, 1);
        RETURN;
    END;

    IF @imc IS NULL
    BEGIN
        RAISERROR('Peso o estatura invalidos para calcular IMC.', 16, 1);
        RETURN;
    END;

    IF NOT EXISTS (SELECT 1 FROM dbo.USUARIO WHERE id_usuario = @idUsuario)
    BEGIN
        RAISERROR('El usuario no existe.', 16, 1);
        RETURN;
    END;

    BEGIN TRANSACTION;

    INSERT INTO dbo.MEDIDA_USUARIO (id_usuario, fecha, cintura, cuello, caderas, pct_musculo, pct_grasa)
    VALUES (@idUsuario, @fecha, @cintura, @cuello, @caderas, @pctMusculo, @pctGrasa);

    SET @idMedida = CONVERT(INT, SCOPE_IDENTITY());

    UPDATE dbo.USUARIO
    SET peso = @pesoKg,
        imc = @imc,
        cintura = @cintura,
        cuello = @cuello,
        caderas = @caderas,
        pct_musculo = @pctMusculo,
        pct_grasa = @pctGrasa
    WHERE id_usuario = @idUsuario;

    COMMIT TRANSACTION;

    SELECT
        @idMedida AS id_medida,
        @idUsuario AS id_usuario,
        @fecha AS fecha,
        @pesoKg AS peso,
        @imc AS imc;
END;
GO

/* =========================================================
   5. TRIGGERS
   ========================================================= */

CREATE TRIGGER dbo.trg_RecalcularTotalesReceta
ON dbo.RECETA_PRODUCTO
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH RecetasAfectadas AS
    (
        SELECT id_receta FROM inserted
        UNION
        SELECT id_receta FROM deleted
    ),
    Totales AS
    (
        SELECT
            ra.id_receta,
            COALESCE(SUM(p.calorias * rp.cantidad_porciones), 0) AS total_calorias,
            COALESCE(SUM(p.carbohidratos * rp.cantidad_porciones), 0) AS total_carbohidratos,
            COALESCE(SUM(p.proteinas * rp.cantidad_porciones), 0) AS total_proteina,
            COALESCE(SUM(p.grasas * rp.cantidad_porciones), 0) AS total_grasa
        FROM RecetasAfectadas ra
        LEFT JOIN dbo.RECETA_PRODUCTO rp ON rp.id_receta = ra.id_receta
        LEFT JOIN dbo.PRODUCTO p ON p.id_producto = rp.id_producto
        GROUP BY ra.id_receta
    )
    UPDATE r
    SET total_calorias = CAST(t.total_calorias AS DECIMAL(7,2)),
        total_carbohidratos = CAST(t.total_carbohidratos AS DECIMAL(7,2)),
        total_proteina = CAST(t.total_proteina AS DECIMAL(7,2)),
        total_grasa = CAST(t.total_grasa AS DECIMAL(7,2))
    FROM dbo.RECETA r
    INNER JOIN Totales t ON t.id_receta = r.id_receta;
END;
GO

CREATE TRIGGER dbo.trg_RecalcularTotalPlan
ON dbo.PLAN_PRODUCTO
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH TiemposAfectados AS
    (
        SELECT id_tiempo FROM inserted
        UNION
        SELECT id_tiempo FROM deleted
    ),
    PlanesAfectados AS
    (
        SELECT DISTINCT tcp.id_plan
        FROM TiemposAfectados ta
        INNER JOIN dbo.TIEMPO_COMIDA_PLAN tcp ON tcp.id_tiempo = ta.id_tiempo
    )
    UPDATE pa
    SET total_calorias = CAST(dbo.fn_TotalCaloriasPlan(pa.id_plan) AS DECIMAL(7,2))
    FROM dbo.PLAN_ALIMENTACION pa
    INNER JOIN PlanesAfectados afectados ON afectados.id_plan = pa.id_plan;
END;
GO

PRINT 'Script ejecutado correctamente.';
GO
