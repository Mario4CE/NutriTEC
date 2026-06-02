namespace NutriTec.Contracts.Productos;

/*
 * Descripción:
 * Expone un producto alimenticio mediante un DTO independiente de Entity Framework Core.
 *
 * Entradas:
 * Recibe identificación, información nutricional, estado de aprobación y fecha UTC de creación.
 *
 * Salidas:
 * Transporta un producto serializable hacia el frontend.
 *
 * Restricciones:
 * No debe exponer tipos propios de persistencia ni permitir modificar el estado de aprobación desde el módulo de productos.
 */
public sealed record ProductoResponse(
    Guid Id,
    string Nombre,
    string CodigoBarras,
    decimal Calorias,
    decimal Proteinas,
    decimal Carbohidratos,
    decimal Grasas,
    bool EstaAprobado,
    DateTime FechaCreacionUtc);
