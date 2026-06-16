namespace NutriTec.Application.Autenticacion;

/*
Descripción:
Representa el resultado interno de la generación de un token de autenticación.

Entradas:
Token JWT serializado y fecha/hora de expiración calculada por el servicio de tokens.

Salidas:
Transporta el token generado desde Infrastructure hacia Application sin exponer secretos ni credenciales.

Restricciones:
No debe incluir contraseñas, password_hash ni secretos de firma JWT.
*/
public sealed record TokenAutenticacion(
    string Token,
    DateTimeOffset ExpiraEn);
