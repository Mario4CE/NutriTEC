# Persistence

Esta carpeta contiene interfaces y modelos internos usados por repositorios abstractos.

Responsabilidades principales:

- Definir operaciones que Application necesita de la persistencia.
- Ocultar detalles de Entity Framework Core, MongoDB y consultas concretas.
- Transportar proyecciones internas que no deben exponerse al frontend.

Restricciones:

- No debe incluir repositorios concretos.
- No debe usar `DbContext`, clientes Mongo ni cadenas de conexión.
- No debe recibir contraseñas sin hashear hacia persistencia.
