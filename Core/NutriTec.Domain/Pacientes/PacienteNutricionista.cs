namespace NutriTec.Domain.Pacientes;

/*
 * Descripción:
 * Representa la asociación entre un nutricionista y un cliente que pasa a ser su paciente.
 *
 * Entradas:
 * Recibe los identificadores del nutricionista y del cliente, junto con la fecha de asociación.
 *
 * Salidas:
 * Conserva el vínculo activo entre ambos usuarios para habilitar planes y seguimiento.
 *
 * Restricciones:
 * Un mismo cliente no puede estar asociado dos veces al mismo nutricionista.
 */
public sealed class PacienteNutricionista
{
    public Guid Id { get; init; }
    public Guid IdNutricionista { get; init; }
    public Guid IdPaciente { get; init; }
    public DateTime FechaAsociacionUtc { get; init; }
}