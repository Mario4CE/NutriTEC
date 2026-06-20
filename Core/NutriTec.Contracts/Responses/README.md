# Responses

Esta carpeta contiene contratos de respuesta transversales.

Responsabilidades principales:

- Normalizar la estructura de respuestas HTTP.
- Facilitar mensajes consistentes para éxito y error.
- Evitar duplicación de envoltorios de respuesta en cada módulo.

Restricciones:

- No debe contener lógica de negocio.
- No debe depender de infraestructura.
- No debe transportar entidades internas como contrato público.
