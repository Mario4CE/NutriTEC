# Retroalimentaciones

Esta carpeta contiene entidades de dominio del foro de retroalimentación.

Responsabilidades principales:

- Representar foros entre pacientes y nutricionistas.
- Representar mensajes dentro de una conversación.
- Mantener el modelo documental independiente de la API.

Restricciones:

- No debe depender directamente del controller Mongo.
- No debe incluir consultas al driver de MongoDB.
- No debe exponer documentos internos como respuesta HTTP sin mapeo.
