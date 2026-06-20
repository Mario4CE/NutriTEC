namespace NutriTec.Infrastructure.Sql.Bootstrap;

/*
Descripción:
Opciones de configuración para crear de forma controlada el primer administrador del sistema.

Entradas:
Valores enlazados desde la sección BootstrapAdmin de la configuración.

Salidas:
Datos necesarios para habilitar o deshabilitar el bootstrap del administrador inicial.

Restricciones:
No debe contener secretos versionados; la contraseña temporal debe venir de variables de entorno, user-secrets o gestor de secretos.
*/
public sealed class AdminBootstrapOptions
{
    public const string SectionName = "BootstrapAdmin";

    public bool Enabled { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
