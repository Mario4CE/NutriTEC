# Configuration

Esta carpeta contiene opciones de configuración para MongoDB.

Responsabilidades principales:

- Definir modelos de opciones enlazados desde configuración externa.
- Validar datos mínimos para conectar con MongoDB.
- Mantener nombres y cadenas de conexión fuera de código de negocio.

Restricciones:

- No debe contener secretos reales.
- No debe implementar repositorios.
- No debe depender de controllers.
