namespace NutriTec.Application.Abstractions.Persistence;

/*
 * Descripción:
 * Define las operaciones mínimas de persistencia requeridas por autenticación.
 *
 * Entradas:
 * Recibe correos, datos internos de registro y tokens de cancelación desde la capa de aplicación.
 *
 * Salidas:
 * Devuelve credenciales internas, existencia de correo o usuario registrado.
 *
 * Restricciones:
 * No expone Entity Framework Core ni MongoDB y no debe recibir contraseñas sin hashear.
 */
public interface IAuthRepository
{

    /* 
     * Descripción: Consulta credenciales por correo. 
     * Entradas: Correo y cancelación.
     * Salidas: Credencial o nulo. 
     * Restricciones: No modifica datos.
     */

    Task<CredencialAutenticacion?> ObtenerCredencialPorCorreoAsync(string correo, CancellationToken cancellationToken);

    /* Descripción: Verifica si un correo ya existe. 
     * Entradas: Correo y cancelación. 
     * Salidas: Existencia.
     * Restricciones: No modifica datos.
     */

    Task<bool> ExisteCorreoAsync(string correo, CancellationToken cancellationToken);

    /*
     * Descripción: Registra un usuario para autenticación.
     * Entradas: Usuario con hash y cancelación. 
     * Salidas: Credencial registrada.
     * Restricciones: La contraseña ya debe estar hasheada. 
     */
    Task<CredencialAutenticacion> RegistrarUsuarioAsync(NuevoUsuarioAutenticacion usuario, CancellationToken cancellationToken);
}
