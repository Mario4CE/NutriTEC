# NutriTec.Infrastructure.Mongo

Proyecto de infraestructura para persistencia documental con MongoDB.

Responsabilidades principales:

- Configurar opciones de conexión Mongo.
- Registrar cliente, base de datos y repositorios documentales.
- Implementar repositorios definidos por Application para documentos.

Restricciones:

- No debe depender de infraestructura SQL.
- No debe ser usado directamente por controllers.
- No debe almacenar secretos reales en archivos versionados.
