# Scripts SQL Server

Los scripts están separados por tipo de objeto para facilitar la revisión del profesor y la ejecución controlada sobre una base limpia o existente.

## Orden recomendado de ejecución en base limpia

1. `Tables/000_TIPO_COBRO.sql`
2. `Seed/001_TIPO_COBRO.sql`
3. Resto de scripts de `Tables/` en orden numérico.
4. Para bases existentes, aplicar `Migrations/001_Ampliar_PRODUCTO_nutricion.sql` si PRODUCTO fue creado antes de agregar la ficha nutricional completa.
5. `Functions/001_fn_CalcularImc.sql`
6. `Functions/002_fn_TotalCaloriasPlan.sql`
7. `Views/001_vw_PacientesPorNutricionista.sql`
8. `Views/002_vw_DetallePlanAlimentacion.sql`
9. `Views/003_vw_ResumenRegistroDiario.sql`
10. `StoredProcedures/001_sp_ReporteCobroNutricionistas.sql`
11. `StoredProcedures/002_sp_AprobarProducto.sql`
12. `StoredProcedures/003_sp_AsignarPlanPaciente.sql`
13. `StoredProcedures/004_sp_RegistrarMedidaUsuario.sql`
14. `Triggers/001_trg_RecalcularTotalesReceta.sql`
15. `Triggers/002_trg_RecalcularTotalPlan.sql`

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

## Uso desde la API

No todos los objetos programables se llaman directamente desde HTTP. La API usa algunos mediante repositories y otros quedan como lógica automática dentro de SQL Server.

| Objeto SQL | Estado de integración | Endpoint o uso |
| --- | --- | --- |
| `dbo.sp_ReporteCobroNutricionistas` | Usado desde API | `GET /api/administracion/reporte-cobro?montoBasePorPaciente=1500&incluirSinPacientes=true` |
| `dbo.fn_CalcularImc` | Usado desde API | `GET /api/administracion/imc?pesoKg=70&estaturaCm=170` |
| `dbo.sp_AprobarProducto` | Usado desde API | `PUT /api/administracion/productos/{idProducto}/aprobacion` |
| `dbo.fn_TotalCaloriasPlan` | Usado desde base de datos | Lo usa el trigger de plan para recalcular totales. |
| `dbo.trg_RecalcularTotalesReceta` | Automático | Se dispara al cambiar productos de una receta. |
| `dbo.trg_RecalcularTotalPlan` | Automático | Se dispara al cambiar productos de un plan. |
| Vistas SQL | Lectura/reportes | Pueden consultarse directamente en SQL Server para revisión o reportes. |

Los triggers no tienen endpoint propio porque SQL Server los ejecuta automáticamente cuando se insertan, actualizan o eliminan filas en las tablas asociadas.
