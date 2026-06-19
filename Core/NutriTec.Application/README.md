# NutriTec.Application

Proyecto que contiene los casos de uso y reglas de aplicación de NutriTEC.

Responsabilidades principales:

- Orquestar operaciones entre contratos, dominio y repositorios abstractos.
- Definir interfaces que serán implementadas por infraestructura.
- Aplicar validaciones de negocio independientes de HTTP y bases de datos.
- Registrar servicios de aplicación compartidos mediante Dependency Injection.

Restricciones:

- No debe referenciar Entity Framework Core ni MongoDB directamente.
- No debe conocer controllers ni detalles HTTP.
- No debe almacenar contraseñas en texto plano.
