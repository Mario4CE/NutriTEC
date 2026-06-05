# Retroalimentaciones

Esta carpeta contiene la lógica de aplicación del módulo de retroalimentaciones.

Responsabilidades principales:

- Crear foros documentales de retroalimentación.
- Consultar foros por paciente o nutricionista.
- Agregar respuestas a foros existentes.
- Mantener el flujo Controller → Service → Repository.

Restricciones:

- No debe depender del cliente Mongo concreto.
- No debe implementar endpoints HTTP.
- No debe mezclar persistencia documental con lógica relacional.
