# =============================================================
# NutriTEC — Script de población de datos de prueba
# Ejecutar con: .\seed-test-data.ps1
# Requisitos: SQL API corriendo en http://localhost:5255
# =============================================================

$BASE = "http://localhost:5255/api"
$headers = @{ "Content-Type" = "application/json" }

function Post($url, $body) {
    try {
        $json = $body | ConvertTo-Json -Depth 5
        $res = Invoke-RestMethod -Uri $url -Method POST -Headers $headers -Body $json -ErrorAction Stop
        return $res
    } catch {
        $raw = $_.ErrorDetails.Message
        Write-Host "  ERROR $($_.Exception.Message)" -ForegroundColor Red
        if ($raw) { Write-Host "  $raw" -ForegroundColor DarkRed }
        return $null
    }
}

function AuthPost($url, $body, $token) {
    try {
        $h = @{ "Content-Type" = "application/json"; "Authorization" = "Bearer $token" }
        $json = $body | ConvertTo-Json -Depth 5
        $res = Invoke-RestMethod -Uri $url -Method POST -Headers $h -Body $json -ErrorAction Stop
        return $res
    } catch {
        Write-Host "  ERROR $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  NutriTEC — Población de pruebas   " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# 1. Login admin
Write-Host "[1/5] Obteniendo token de administrador..." -ForegroundColor Yellow
$loginRes = Post "$BASE/auth/login" @{ Correo = "admin@nutritec.com"; Contrasena = "Admin2026!" }
if (-not $loginRes) { Write-Host "No se pudo hacer login." -ForegroundColor Red; exit 1 }
$adminToken = $loginRes.token
Write-Host "  OK" -ForegroundColor Green

# 2. Nutricionistas
Write-Host ""
Write-Host "[2/5] Registrando nutricionistas..." -ForegroundColor Yellow

$nutricionistas = @(
    @{
        Cedula = "101110111"; Nombre = "Pablo"; Apellidos = "Rojas Mendez"
        CodigoNutricionista = "NUT-001"; Edad = 35
        FechaNacimiento = "1989-03-15"
        Peso = 78.5; Imc = 24.1; Direccion = "San Jose, Costa Rica"
        FotoUrl = $null
        TarjetaCredito = "****-****-****-4521"; TipoCobro = "semanal"
        Correo = "pablo.rojas@nutritec.com"; Contrasena = "Nutricion2026!"
    },
    @{
        Cedula = "202220222"; Nombre = "Ana"; Apellidos = "Jimenez Castro"
        CodigoNutricionista = "NUT-002"; Edad = 42
        FechaNacimiento = "1982-07-20"
        Peso = 65.0; Imc = 22.8; Direccion = "Heredia, Costa Rica"
        FotoUrl = $null
        TarjetaCredito = "****-****-****-7788"; TipoCobro = "mensual"
        Correo = "ana.jimenez@nutritec.com"; Contrasena = "Nutricion2026!"
    },
    @{
        Cedula = "303330333"; Nombre = "Luis"; Apellidos = "Araya Quesada"
        CodigoNutricionista = "NUT-003"; Edad = 50
        FechaNacimiento = "1974-11-05"
        Peso = 82.0; Imc = 26.3; Direccion = "Alajuela, Costa Rica"
        FotoUrl = $null
        TarjetaCredito = "****-****-****-1190"; TipoCobro = "anual"
        Correo = "luis.araya@nutritec.com"; Contrasena = "Nutricion2026!"
    }
)

$tokenesNutricionistas = @{}
foreach ($n in $nutricionistas) {
    $res = Post "$BASE/auth/registrar-nutricionista" $n
    if ($res) {
        Write-Host "  OK — $($n.Nombre) $($n.Apellidos) ($($n.TipoCobro))" -ForegroundColor Green
        $tokenesNutricionistas[$n.Cedula] = $res.token
    }
}

# 3. Usuarios
Write-Host ""
Write-Host "[3/5] Registrando usuarios..." -ForegroundColor Yellow

$usuarios = @(
    @{
        Nombre = "Mariana"; Apellidos = "Solis Vargas"; Edad = 28
        FechaNacimiento = "1996-04-10"
        Peso = 68.0; Imc = 23.5; Pais = "Costa Rica"
        Cintura = 72.0; Cuello = 32.0; Caderas = 95.0
        PctMusculo = 38.0; PctGrasa = 22.0; CaloriasDiariasMax = 1800
        Correo = "mariana.solis@gmail.com"; Contrasena = "Usuario2026!"
    },
    @{
        Nombre = "Esteban"; Apellidos = "Vargas Mora"; Edad = 34
        FechaNacimiento = "1990-09-22"
        Peso = 90.0; Imc = 28.7; Pais = "Costa Rica"
        Cintura = 95.0; Cuello = 40.0; Caderas = 102.0
        PctMusculo = 42.0; PctGrasa = 28.0; CaloriasDiariasMax = 2200
        Correo = "esteban.vargas@gmail.com"; Contrasena = "Usuario2026!"
    },
    @{
        Nombre = "Sofia"; Apellidos = "Mora Fernandez"; Edad = 22
        FechaNacimiento = "2002-01-30"
        Peso = 55.0; Imc = 21.2; Pais = "Costa Rica"
        Cintura = 65.0; Cuello = 30.0; Caderas = 88.0
        PctMusculo = 35.0; PctGrasa = 18.0; CaloriasDiariasMax = 1600
        Correo = "sofia.mora@gmail.com"; Contrasena = "Usuario2026!"
    },
    @{
        Nombre = "Carlos"; Apellidos = "Mendez Arias"; Edad = 45
        FechaNacimiento = "1979-06-15"
        Peso = 95.0; Imc = 30.1; Pais = "Costa Rica"
        Cintura = 100.0; Cuello = 42.0; Caderas = 108.0
        PctMusculo = 38.0; PctGrasa = 32.0; CaloriasDiariasMax = 2400
        Correo = "carlos.mendez@gmail.com"; Contrasena = "Usuario2026!"
    }
)

foreach ($u in $usuarios) {
    $res = Post "$BASE/auth/registrar-cliente" $u
    if ($res) {
        Write-Host "  OK — $($u.Nombre) $($u.Apellidos)" -ForegroundColor Green
    }
}

# 4. Productos
Write-Host ""
Write-Host "[4/5] Creando productos pendientes..." -ForegroundColor Yellow

$tokenNut1 = $tokenesNutricionistas["101110111"]
if ($tokenNut1) {
    $productos = @(
        @{
            Nombre = "Pinto casero"; CodigoBarras = "7501234560011"
            PorcionGramosMililitros = 200; Calorias = 215; Proteinas = 7.5
            Carbohidratos = 38.0; Grasas = 4.2; SodioMiligramos = 320
            Vitaminas = "B1, B6"; CalcioMiligramos = 45; HierroMiligramos = 2.1
        },
        @{
            Nombre = "Batido proteico de avena"; CodigoBarras = "7501234560028"
            PorcionGramosMililitros = 350; Calorias = 310; Proteinas = 22.0
            Carbohidratos = 40.0; Grasas = 6.0; SodioMiligramos = 180
            Vitaminas = $null; CalcioMiligramos = 120; HierroMiligramos = $null
        },
        @{
            Nombre = "Ensalada de garbanzos"; CodigoBarras = "7501234560035"
            PorcionGramosMililitros = 250; Calorias = 180; Proteinas = 9.0
            Carbohidratos = 24.0; Grasas = 5.0; SodioMiligramos = 95
            Vitaminas = "C, K"; CalcioMiligramos = 60; HierroMiligramos = 3.2
        }
    )
    foreach ($p in $productos) {
        $res = AuthPost "$BASE/productos" $p $tokenNut1
        if ($res) { Write-Host "  OK — $($p.Nombre)" -ForegroundColor Green }
    }
} else {
    Write-Host "  SKIP — no hay token de nutricionista disponible" -ForegroundColor DarkYellow
}

# 5. Resumen
Write-Host ""
Write-Host "[5/5] Listo. Ahora ejecuta seed-pacientes.sql en SSMS." -ForegroundColor Yellow
Write-Host ""
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Credenciales de prueba             " -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Admin:    admin@nutritec.com           / Admin2026!" -ForegroundColor Gray
Write-Host "  Nutri 1:  pablo.rojas@nutritec.com     / Nutricion2026! (semanal)" -ForegroundColor Gray
Write-Host "  Nutri 2:  ana.jimenez@nutritec.com     / Nutricion2026! (mensual)" -ForegroundColor Gray
Write-Host "  Nutri 3:  luis.araya@nutritec.com      / Nutricion2026! (anual)" -ForegroundColor Gray
Write-Host "  Usuario:  mariana.solis@gmail.com      / Usuario2026!" -ForegroundColor Gray
Write-Host "  Usuario:  esteban.vargas@gmail.com     / Usuario2026!" -ForegroundColor Gray
Write-Host "  Usuario:  sofia.mora@gmail.com         / Usuario2026!" -ForegroundColor Gray
Write-Host "  Usuario:  carlos.mendez@gmail.com      / Usuario2026!" -ForegroundColor Gray
Write-Host ""