# NutriTec.Contracts

Proyecto que contiene DTOs y respuestas compartidas entre APIs y Application.

Responsabilidades principales:

- Definir contratos de entrada y salida serializables.
- Mantener los modelos expuestos al frontend separados del dominio.
- Incluir validaciones declarativas básicas cuando corresponda.

Restricciones:

- No debe contener entidades de dominio.
- No debe contener lógica de negocio.
- No debe incluir secretos, hashes ni datos internos de persistencia.
