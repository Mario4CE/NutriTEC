namespace NutriTec.Application.Common;

/*
 * Descripción:
 * Representa un conflicto esperado entre una solicitud válida y el estado actual de los datos.
 *
 * Entradas:
 * Recibe un mensaje seguro que puede devolverse al consumidor de la API.
 *
 * Salidas:
 * Permite que los hosts traduzcan el conflicto a HTTP 409 sin ocultar excepciones internas inesperadas.
 *
 * Restricciones:
 * Debe utilizarse únicamente para conflictos de negocio controlados, como códigos de barras duplicados.
 */
public sealed class ConflictoException(string message) : Exception(message);
