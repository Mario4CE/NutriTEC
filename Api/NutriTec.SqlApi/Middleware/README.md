# Middleware

Esta carpeta contiene middleware transversal para la API SQL.

Responsabilidades principales:

- Traducir excepciones controladas de Application a respuestas HTTP.
- Centralizar manejo de errores esperados.
- Mantener los controllers enfocados en orquestación HTTP.

Restricciones:

- No debe implementar casos de uso.
- No debe acceder a persistencia.
- No debe revelar información sensible en errores.
