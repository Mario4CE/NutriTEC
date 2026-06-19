# Scripts SQL Server

Los scripts están separados por tipo de objeto para facilitar la revisión del profesor y la ejecución controlada sobre una base limpia o existente.

## Orden recomendado de ejecución en base limpia

1. `Tables/000_TIPO_COBRO.sql`
2. `Seed/001_TIPO_COBRO.sql`
3. Resto de scripts de `Tables/` en orden numérico.
4. `Functions/001_fn_CalcularImc.sql`
5. `Functions/002_fn_TotalCaloriasPlan.sql`
6. `Views/001_vw_PacientesPorNutricionista.sql`
7. `Views/002_vw_DetallePlanAlimentacion.sql`
8. `Views/003_vw_ResumenRegistroDiario.sql`
9. `StoredProcedures/001_sp_ReporteCobroNutricionistas.sql`
10. `StoredProcedures/002_sp_AprobarProducto.sql`
11. `StoredProcedures/003_sp_AsignarPlanPaciente.sql`
12. `StoredProcedures/004_sp_RegistrarMedidaUsuario.sql`
13. `Triggers/001_trg_RecalcularTotalesReceta.sql`
14. `Triggers/002_trg_RecalcularTotalPlan.sql`

## Objetos programables agregados

- Funciones:
  - `dbo.fn_CalcularImc`: calcula IMC con validaciones básicas.
  - `dbo.fn_TotalCaloriasPlan`: totaliza calorías desde tiempos de comida y productos.
- Vistas:
  - `dbo.vw_PacientesPorNutricionista`: lectura administrativa sin credenciales ni tarjetas.
  - `dbo.vw_DetallePlanAlimentacion`: detalle de planes, tiempos de comida y productos.
  - `dbo.vw_ResumenRegistroDiario`: resumen diario agregado de consumo por usuario.
- Procedimientos almacenados:
  - `dbo.sp_ReporteCobroNutricionistas`: reporte obligatorio de cobro con descuentos mensual/anual.
  - `dbo.sp_AprobarProducto`: aprueba productos con validaciones y transacción.
  - `dbo.sp_AsignarPlanPaciente`: asigna planes validando relación paciente-nutricionista.
  - `dbo.sp_RegistrarMedidaUsuario`: registra historial de medidas y actualiza datos actuales del usuario.
- Triggers:
  - `dbo.trg_RecalcularTotalesReceta`: mantiene totales de recetas al cambiar productos.
  - `dbo.trg_RecalcularTotalPlan`: mantiene total calórico del plan al cambiar productos.

Ningún procedimiento agregado es un envoltorio trivial de una sola sentencia; todos incluyen validaciones, variables, control de flujo y/o transacciones.
