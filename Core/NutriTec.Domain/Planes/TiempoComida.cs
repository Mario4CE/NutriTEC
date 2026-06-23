namespace NutriTec.Domain.Planes;

/*
 * Descripción:
 * Enumera los cinco tiempos de comida que componen un plan de alimentación.
 *
 * Entradas:
 * No aplica.
 *
 * Salidas:
 * Permite clasificar cada producto asignado a un plan según el momento del día.
 *
 * Restricciones:
 * Un plan debe distribuir sus productos únicamente entre estos cinco valores.
 */
public enum TiempoComida
{
    Desayuno = 1,
    MeriendaManana = 2,
    Almuerzo = 3,
    MeriendaTarde = 4,
    Cena = 5
}