namespace NutriTec.Application.Abstractions.Services;

/*
 * Descripción:
 * Define la abstracción para generar y validar hashes de contraseñas.
 *
 * Entradas:
 * Recibe contraseñas en texto claro solo durante operaciones transitorias y hashes previamente persistidos.
 *
 * Salidas:
 * Devuelve hashes seguros o el resultado de una verificación de credenciales.
 *
 * Restricciones:
 * Las implementaciones concretas deben vivir fuera de los controllers y no deben almacenar contraseñas en texto plano.
 */
public interface IPasswordHasher
{
    /* Descripción: Genera un hash seguro. Entradas: Contraseña en texto claro. Salidas: Hash persistible. Restricciones: No debe devolver la contraseña original. */
    string GenerarHash(string contrasena);

    /* Descripción: Verifica una contraseña. Entradas: Contraseña en texto claro y hash persistido. Salidas: Resultado de validación. Restricciones: No modifica datos. */
    bool Verificar(string contrasena, string contrasenaHash);
}
