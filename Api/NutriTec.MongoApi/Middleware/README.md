# Middleware

Esta carpeta contiene middleware transversal para la API Mongo.

Responsabilidades principales:

- Manejar errores de validación y argumentos inválidos.
- Convertir excepciones esperadas en respuestas HTTP consistentes.
- Reducir duplicación de manejo de errores en controllers.

Restricciones:

- No debe acceder a repositorios.
- No debe construir respuestas con información sensible.
- No debe reemplazar validaciones de negocio de Application.
