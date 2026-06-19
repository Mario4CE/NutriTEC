#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT_DIR"

if ! command -v dotnet >/dev/null 2>&1; then
  echo "ERROR: dotnet SDK no está instalado o no está en PATH." >&2
  echo "Instale .NET SDK 8.0 y vuelva a ejecutar: ./Tests/run-tests.sh" >&2
  exit 127
fi

mapfile -t TEST_PROJECTS < <(find Tests -name '*.csproj' -print | sort)

if [ "${#TEST_PROJECTS[@]}" -eq 0 ]; then
  echo "ERROR: no se encontraron proyectos de pruebas en Tests/." >&2
  exit 1
fi

echo "==> Restaurando paquetes"
dotnet restore NutriTEC.sln

echo "==> Compilando solución en Release"
dotnet build NutriTEC.sln --configuration Release --no-restore

for test_project in "${TEST_PROJECTS[@]}"; do
  echo "==> Compilando y ejecutando pruebas: ${test_project}"
  dotnet test "${test_project}" \
    --configuration Release \
    --no-restore
done

echo "==> Ejecutando pruebas a nivel de solución"
dotnet test NutriTEC.sln \
  --configuration Release \
  --no-restore

echo "==> Pruebas completadas correctamente"
