using NutriTec.Contracts.Productos;
using NutriTec.Domain.Productos;

namespace NutriTec.Application.Productos;

/*
 * Descripción:
 * Centraliza la transformación de entidades Producto hacia DTOs de salida.
 *
 * Entradas:
 * Recibe entidades del dominio de productos.
 *
 * Salidas:
 * Devuelve contratos serializables para las APIs.
 *
 * Restricciones:
 * No aplica reglas de negocio ni modifica las entidades recibidas.
 */

internal static class ProductoMapper
{
    /*
     * Descripción: Convierte una entidad Producto en ProductoResponse.
     * Entradas: Recibe la entidad del dominio.
     * Salidas: Devuelve el DTO equivalente.
     * Restricciones: La entidad no puede ser nula.
     */

    public static ProductoResponse Mapear(Producto producto)
    {
        return new ProductoResponse(
            producto.Id,
            producto.Nombre,
            producto.CodigoBarras,
            producto.PorcionGramosMililitros,
            producto.Calorias,
            producto.Proteinas,
            producto.Carbohidratos,
            producto.Grasas,
            producto.SodioMiligramos,
            producto.Vitaminas,
            producto.CalcioMiligramos,
            producto.HierroMiligramos,
            producto.EstaAprobado,
            producto.FechaCreacionUtc);
    }
}
