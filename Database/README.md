# Database

Scripts versionados para estructuras y datos base de las bases de datos del proyecto NutriTEC.

## Estrategia de seeds

Los seeds deben limitarse a catálogos estables que el usuario final normalmente no crea desde pantallas de clientes, nutricionistas o administración diaria.

Seeds permitidos en esta etapa:

- Catálogos técnicos o de negocio muy estables, por ejemplo tipos de cobro.
- Datos requeridos para constraints o llaves foráneas antes de registrar entidades principales.

Seeds versionados actualmente:

- `Database/SqlServer/Seed/001_TIPO_COBRO.sql`: inserta los tipos de cobro `semanal`, `mensual` y `anual`, usados por `NUTRICIONISTA.tipo_cobro`.

Seeds no permitidos en esta etapa:

- Clientes, nutricionistas, administradores con contraseñas versionadas, pacientes o relaciones entre usuarios.
- Productos, recetas, planes de alimentación, registros diarios o mediciones de salud.
- Datos de tarjeta, pagos o información sensible.

El primer administrador se maneja mediante bootstrap controlado desde la API SQL y secretos externos al repositorio, no mediante un seed SQL con contraseña o hash versionado.

## Reglas

- No agregar seeds con información personal o sensible.
- No guardar contraseñas, hashes reales ni secretos en scripts.
- Mantener los seeds idempotentes cuando sea posible.
- Documentar por qué un catálogo requiere seed antes de agregarlo.
