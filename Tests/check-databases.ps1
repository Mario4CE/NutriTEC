<#
.SYNOPSIS
    Verifica rápidamente que las bases de datos de NutriTEC tengan los objetos esperados.

.DESCRIPTION
    Este smoke check valida SQL Server y MongoDB sin levantar las APIs:
    - SQL Server: conexión, tablas base, funciones, vistas, stored procedures, triggers y cálculo IMC.
    - MongoDB: conexión, base nutritec_feedback, colección Retroalimentaciones e índices mínimos.

.REQUIREMENTS
    - sqlcmd disponible en PATH.
    - mongosh disponible en PATH.
    - Scripts de Database/SqlServer y Database/MongoDB ejecutados previamente.

.EXAMPLE
    pwsh Tests/check-databases.ps1

.EXAMPLE
    pwsh Tests/check-databases.ps1 -SqlServer ".\SQLEXPRESS" -SqlDatabase "NutriTec" -MongoConnectionString "mongodb://localhost:27017" -MongoDatabase "nutritec_feedback"
#>
param(
    [string]$SqlServer = "(localdb)\MSSQLLocalDB",
    [string]$SqlDatabase = "NutriTec",
    [string]$MongoConnectionString = "mongodb://localhost:27017",
    [string]$MongoDatabase = "nutritec_feedback"
)

$ErrorActionPreference = "Stop"

function Write-Ok {
    param([string]$Message)
    Write-Host "OK  $Message" -ForegroundColor Green
}

function Write-Step {
    param([string]$Message)
    Write-Host "--> $Message" -ForegroundColor Cyan
}

function Invoke-SqlCheck {
    param([string]$Query)

    $output = & sqlcmd -S $SqlServer -d $SqlDatabase -b -W -h -1 -Q $Query 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "SQL Server check failed: $output"
    }

    return ($output | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
}

function Invoke-MongoCheck {
    param([string]$Script)

    $output = & mongosh $MongoConnectionString --quiet --eval $Script 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "MongoDB check failed: $output"
    }

    return ($output | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
}

Write-Step "Revisando herramientas requeridas"
if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
    throw "No se encontró sqlcmd en PATH. Instala SQL Server command-line tools."
}
Write-Ok "sqlcmd disponible"

if (-not (Get-Command mongosh -ErrorAction SilentlyContinue)) {
    throw "No se encontró mongosh en PATH. Instala MongoDB Shell."
}
Write-Ok "mongosh disponible"

Write-Step "Revisando SQL Server ($SqlServer / $SqlDatabase)"
Invoke-SqlCheck "SELECT 1;" | Out-Null
Write-Ok "Conexión SQL exitosa"

$sqlMissingObjects = Invoke-SqlCheck @"
SET NOCOUNT ON;
DECLARE @ExpectedObjects TABLE (ObjectName SYSNAME NOT NULL, ObjectType VARCHAR(2) NOT NULL);
INSERT INTO @ExpectedObjects (ObjectName, ObjectType)
VALUES
    ('dbo.TIPO_COBRO', 'U'),
    ('dbo.ADMINISTRADOR', 'U'),
    ('dbo.NUTRICIONISTA', 'U'),
    ('dbo.USUARIO', 'U'),
    ('dbo.PRODUCTO', 'U'),
    ('dbo.PLAN_ALIMENTACION', 'U'),
    ('dbo.RECETA', 'U'),
    ('dbo.REGISTRO_DIARIO', 'U'),
    ('dbo.fn_CalcularImc', 'FN'),
    ('dbo.fn_TotalCaloriasPlan', 'FN'),
    ('dbo.vw_PacientesPorNutricionista', 'V'),
    ('dbo.vw_DetallePlanAlimentacion', 'V'),
    ('dbo.vw_ResumenRegistroDiario', 'V'),
    ('dbo.sp_ReporteCobroNutricionistas', 'P'),
    ('dbo.sp_AprobarProducto', 'P'),
    ('dbo.sp_AsignarPlanPaciente', 'P'),
    ('dbo.sp_RegistrarMedidaUsuario', 'P'),
    ('dbo.trg_RecalcularTotalesReceta', 'TR'),
    ('dbo.trg_RecalcularTotalPlan', 'TR');

SELECT ObjectName
FROM @ExpectedObjects
WHERE OBJECT_ID(ObjectName, ObjectType) IS NULL
ORDER BY ObjectName;
"@

if ($sqlMissingObjects.Count -gt 0) {
    throw "Faltan objetos SQL: $($sqlMissingObjects -join ', ')"
}
Write-Ok "Tablas, funciones, vistas, stored procedures y triggers esperados existen"

$imcResult = Invoke-SqlCheck "SET NOCOUNT ON; SELECT CONVERT(VARCHAR(20), dbo.fn_CalcularImc(70, 175));"
if (-not $imcResult -or [decimal]$imcResult[0] -le 0) {
    throw "fn_CalcularImc no devolvió un valor válido. Resultado: $imcResult"
}
Write-Ok "fn_CalcularImc ejecuta correctamente: $($imcResult[0])"

Write-Step "Revisando MongoDB ($MongoConnectionString / $MongoDatabase)"
$mongoScript = @"
const databaseName = '$MongoDatabase';
const database = db.getSiblingDB(databaseName);
const collections = database.getCollectionNames();
if (!collections.includes('Retroalimentaciones')) {
  throw new Error('Falta la colección Retroalimentaciones en ' + databaseName);
}
const indexes = database.Retroalimentaciones.getIndexes();
const hasPacienteIndex = indexes.some(index => JSON.stringify(index.key) === JSON.stringify({ IdPaciente: 1, FechaCreacionUtc: -1 }));
const hasNutricionistaIndex = indexes.some(index => JSON.stringify(index.key) === JSON.stringify({ IdNutricionista: 1, FechaCreacionUtc: -1 }));
if (!hasPacienteIndex) {
  throw new Error('Falta índice por IdPaciente + FechaCreacionUtc');
}
if (!hasNutricionistaIndex) {
  throw new Error('Falta índice por IdNutricionista + FechaCreacionUtc');
}
print('Mongo OK: ' + databaseName + '.Retroalimentaciones con ' + indexes.length + ' índices.');
"@

Invoke-MongoCheck $mongoScript | Out-Null
Write-Ok "Base Mongo, colección Retroalimentaciones e índices esperados existen"

Write-Host ""
Write-Host "TODO OK: SQL Server y MongoDB tienen la estructura esperada." -ForegroundColor Green
