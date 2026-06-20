// =============================================
// NutriTEC - Colección de retroalimentaciones
// MongoDB
// =============================================
// Este script está alineado con el modelo actual de la API Mongo:
// - Colección: Retroalimentaciones
// - Documento agregado con mensajes embebidos
// - Campos en PascalCase porque el driver .NET serializa las propiedades actuales sin atributos BSON personalizados.

use("nutritec_feedback");

const retroalimentacionesValidator = {
  $jsonSchema: {
    bsonType: "object",
    required: ["IdPaciente", "IdNutricionista", "FechaCreacionUtc", "Mensajes"],
    properties: {
      IdPaciente: {
        description: "Identificador GUID del paciente manejado por la API."
      },
      IdNutricionista: {
        description: "Identificador GUID del nutricionista manejado por la API."
      },
      FechaCreacionUtc: {
        bsonType: "date",
        description: "Fecha UTC de creación del foro de retroalimentación."
      },
      Mensajes: {
        bsonType: "array",
        minItems: 1,
        description: "Mensajes embebidos del foro de retroalimentación.",
        items: {
          bsonType: "object",
          required: ["Autor", "Mensaje", "FechaUtc"],
          properties: {
            Autor: {
              bsonType: "string",
              description: "Nombre o identificador visible del autor del mensaje."
            },
            Mensaje: {
              bsonType: "string",
              description: "Texto del mensaje de retroalimentación."
            },
            FechaUtc: {
              bsonType: "date",
              description: "Fecha UTC del mensaje."
            }
          }
        }
      }
    }
  }
};

// =============================================
// 1. RETROALIMENTACIONES
// =============================================
if (!db.getCollectionNames().includes("Retroalimentaciones")) {
  db.createCollection("Retroalimentaciones", {
    validator: retroalimentacionesValidator
  });
} else {
  db.runCommand({
    collMod: "Retroalimentaciones",
    validator: retroalimentacionesValidator
  });
}

// =============================================
// 2. Índices recomendados
// =============================================

// Para consultar todas las retroalimentaciones de un paciente, de más reciente a más antigua.
db.Retroalimentaciones.createIndex({ IdPaciente: 1, FechaCreacionUtc: -1 });

// Para consultar todas las retroalimentaciones de un nutricionista, de más reciente a más antigua.
db.Retroalimentaciones.createIndex({ IdNutricionista: 1, FechaCreacionUtc: -1 });
