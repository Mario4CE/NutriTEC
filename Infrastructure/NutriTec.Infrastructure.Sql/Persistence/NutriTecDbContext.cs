using Microsoft.EntityFrameworkCore;

namespace NutriTec.Infrastructure.Sql.Persistence;

/*
 * Descripción:
 * Representa la sesión de Entity Framework Core utilizada por la infraestructura SQL Server de NutriTEC.
 *
 * Entradas:
 * Recibe las opciones de configuración preparadas por el contenedor de inyección de dependencias.
 *
 * Salidas:
 * Proporciona el punto central para consultar y persistir entidades relacionales cuando se agreguen los módulos funcionales.
 *
 * Restricciones:
 * Debe utilizarse únicamente desde la infraestructura SQL; los controllers no deben acceder directamente a este contexto.
 */
public sealed class NutriTecDbContext(DbContextOptions<NutriTecDbContext> options) : DbContext(options);
