#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "ERROR: dotnet SDK no está instalado o no está en PATH." >&2
  echo "Instale .NET SDK 8.0 y vuelva a ejecutar: ./Tests/run-tests.sh" >&2
  exit 127
fi

echo "==> Restaurando paquetes"
dotnet restore NutriTEC.sln

echo "==> Compilando solución en Release"
dotnet build NutriTEC.sln --configuration Release --no-restore

echo "==> Ejecutando pruebas de Application"
dotnet test Tests/NutriTec.Application.Tests/NutriTec.Application.Tests.csproj \
  --configuration Release \
  --no-build

echo "==> Ejecutando pruebas de Infrastructure.Sql"
dotnet test Tests/NutriTec.Infrastructure.Sql.Tests/NutriTec.Infrastructure.Sql.Tests.csproj \
  --configuration Release \
  --no-build

echo "==> Ejecutando pruebas a nivel de solución"
dotnet test NutriTEC.sln \
  --configuration Release \
  --no-build

echo "==> Pruebas completadas correctamente"
