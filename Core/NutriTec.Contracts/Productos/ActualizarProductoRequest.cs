using System.ComponentModel.DataAnnotations;

namespace NutriTec.Contracts.Productos;

/*
 * Descripción:
 * Define los datos editables de un producto alimenticio existente.
 *
 * Entradas:
 * Recibe nombre, código de barras y valores nutricionales actualizados.
 *
 * Salidas:
 * Transporta la modificación desde la API SQL hacia la capa de aplicación.
 *
 * Restricciones:
 * Los textos son obligatorios y los valores nutricionales no pueden ser negativos.
 */

public sealed record ActualizarProductoRequest(
    [Required, MaxLength(150)] string Nombre,
    [Required, MaxLength(64)] string CodigoBarras,
    [Range(0, double.MaxValue)] decimal Calorias,
    [Range(0, double.MaxValue)] decimal Proteinas,
    [Range(0, double.MaxValue)] decimal Carbohidratos,
    [Range(0, double.MaxValue)] decimal Grasas);
