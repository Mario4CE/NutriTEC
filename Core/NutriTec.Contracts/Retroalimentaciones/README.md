# Retroalimentaciones

Esta carpeta contiene DTOs públicos del módulo de retroalimentaciones.

Responsabilidades principales:

- Definir solicitudes para crear y responder foros.
- Definir respuestas serializables para foros y mensajes.
- Mantener separado el contrato HTTP del documento de dominio.

Restricciones:

- No debe depender del driver de MongoDB.
- No debe exponer detalles de colecciones o filtros concretos.
- No debe implementar reglas de conversación.
