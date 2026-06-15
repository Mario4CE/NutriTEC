# Tests

Proyectos de pruebas automatizadas de NutriTEC.

## Reglas

- Las pruebas unitarias de Application deben usar dobles de prueba o mocks para evitar acceso directo a base de datos.
- Las pruebas de Infrastructure pueden usar proveedores de prueba o entornos controlados, pero no deben crear ni modificar scripts en `Database/` sin confirmación explícita.
- Las pruebas de API e integración se agregarán en incrementos posteriores cuando el alcance lo requiera.
