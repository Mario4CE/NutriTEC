# Explicación de pruebas automatizadas

Este documento resume qué valida cada grupo de pruebas. El objetivo es que el profesor pueda leer rápidamente la intención de cada test sin revisar todo el código.

## Application — Autenticación

Archivo: `Tests/NutriTec.Application.Tests/Autenticacion/AuthServiceTests.cs`

- `LoginAsync_CuandoCredencialesSonValidas_RetornaLoginResponseConToken`: comprueba que un usuario existente con contraseña correcta recibe un `LoginResponse` con datos públicos y token.
- `LoginAsync_CuandoUsuarioNoExiste_RetornaNull`: verifica que el login no revela información cuando el correo no existe.
- `LoginAsync_CuandoContrasenaEsInvalida_RetornaNull`: verifica que una contraseña incorrecta no autentica al usuario.
- `RegistrarClienteAsync_CuandoCorreoEstaDisponible_RegistraClienteYRetornaLoginResponse`: comprueba registro de cliente, normalización de correo y hash de contraseña.
- `RegistrarNutricionistaAsync_CuandoCorreoEstaDisponible_RegistraNutricionistaYRetornaLoginResponse`: comprueba registro de nutricionista, normalización de correo, hash de contraseña y almacenamiento enmascarado de tarjeta.
- `RegistrarClienteAsync_CuandoCorreoYaExiste_LanzaConflictoException`: valida que un correo duplicado se traduce a una excepción de conflicto de Application.

## Application — Productos

Archivo: `Tests/NutriTec.Application.Tests/Productos/ProductoServiceTests.cs`

- `CrearAsync_CuandoDatosSonValidos_CreaProductoPendienteConFichaNutricionalCompleta`: valida que un producto válido se crea pendiente de aprobación y conserva la ficha nutricional completa.
- `CrearAsync_CuandoCodigoBarrasExiste_LanzaConflictoException`: valida que no se permitan códigos de barras duplicados.
- `CrearAsync_CuandoPorcionNoEsPositiva_LanzaArgumentException`: valida que la porción sea mayor que cero.
- `CrearAsync_CuandoNutrienteEsNegativo_LanzaArgumentException`: valida que los nutrientes no sean negativos.

## Application — Administración

Archivo: `Tests/NutriTec.Application.Tests/Administracion/AdministracionServiceTests.cs`

- `ListarProductosPendientesAsync_MapeaProductosPendientes`: comprueba que los productos pendientes del repositorio se transforman a DTOs públicos.
- `AprobarProductoAsync_CuandoIdentificadorEsValido_DelegaAlRepositorio`: valida que la aprobación administrativa delega en el repositorio con el identificador correcto.
- `AprobarProductoAsync_CuandoIdentificadorEstaVacio_LanzaArgumentException`: valida que no se acepten identificadores vacíos para aprobación.

## Application — Retroalimentaciones

Archivo: `Tests/NutriTec.Application.Tests/Retroalimentaciones/RetroalimentacionServiceTests.cs`

- `CrearAsync_CuandoDatosSonValidos_CreaForoConMensajeInicial`: verifica que se cree un foro documental con paciente, nutricionista y mensaje inicial.
- `CrearAsync_CuandoPacienteEstaVacio_LanzaArgumentException`: valida que el identificador del paciente sea obligatorio.
- `ResponderAsync_CuandoDatosSonValidos_AgregaMensajeAlRepositorio`: comprueba que responder una retroalimentación agrega un mensaje limpio al repositorio.

## Infrastructure.Sql — Bootstrap de administrador

Archivo: `Tests/NutriTec.Infrastructure.Sql.Tests/Bootstrap/AdminBootstrapServiceTests.cs`

- `InicializarAsync_CuandoBootstrapEstaDeshabilitado_NoCreaAdministrador`: asegura que el bootstrap no crea administradores cuando está deshabilitado.
- `InicializarAsync_CuandoConfiguracionHabilitadaEstaIncompleta_LanzaInvalidOperationException`: valida que la configuración requerida exista antes de crear el administrador inicial.
- `InicializarAsync_CuandoNoExisteAdministrador_CreaAdministradorConPasswordHash`: comprueba que se cree el administrador inicial con contraseña hasheada.
- `InicializarAsync_CuandoYaExisteAdministrador_NoCreaOtroAdministrador`: evita duplicar administradores existentes.

## Infrastructure.Sql — Hash de contraseñas

Archivo: `Tests/NutriTec.Infrastructure.Sql.Tests/Security/PasswordHasherTests.cs`

- `GenerarHash_CuandoRecibeContrasena_RetornaFormatoPbkdf2Esperado`: valida el formato esperado del hash PBKDF2.
- `GenerarHash_CuandoRecibeMismaContrasena_RetornaHashesDiferentes`: verifica que se use salt aleatorio.
- `Verificar_CuandoContrasenaEsCorrecta_RetornaTrue`: comprueba que una contraseña correcta valida contra su hash.
- `Verificar_CuandoContrasenaEsIncorrecta_RetornaFalse`: comprueba que una contraseña incorrecta no valida.
- `Verificar_CuandoHashTieneFormatoNoSoportado_RetornaFalse`: asegura que hashes inválidos o no soportados no autentiquen.

## Infrastructure.Sql — JWT

Archivo: `Tests/NutriTec.Infrastructure.Sql.Tests/Security/JwtTokenServiceTests.cs`

- `GenerarToken_CuandoConfiguracionEsValida_RetornaJwtConIssuerAudienceExpiracionYClaims`: valida que el JWT generado incluya issuer, audience, expiración y claims esperados.
- `GenerarToken_CuandoConfiguracionEstaIncompleta_LanzaInvalidOperationException`: valida que no se generen tokens cuando falta configuración crítica.
