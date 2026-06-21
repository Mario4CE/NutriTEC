/**
 * mockData.js — Datos falsos que imitan exactamente los contratos del backend.
 *
 * Nombres de campos = nombres que devuelve la API real (PascalCase en C# → 
 * JSON serializado en camelCase por ASP.NET por defecto).
 *
 * LoginResponse:   { idUsuario, nombre, correo, tipoUsuario, token, expiraEn }
 * ProductoResponse:{ id, nombre, codigoBarras, porcionGramosMililitros,
 *                    calorias, proteinas, carbohidratos, grasas, sodioMiligramos,
 *                    vitaminas, calcioMiligramos, hierroMiligramos,
 *                    estaAprobado, fechaCreacionUtc }
 */

export const MOCK_ADMIN = {
  email:    "admin@nutritec.com",
  password: "Admin2026!",
  // Respuesta que simula LoginResponse del backend
  session: {
    idUsuario:   "1",
    nombre:      "Administrador",
    correo:      "admin@nutritec.com",
    tipoUsuario: "Administrador",
    token:       "mock-jwt-token",
    expiraEn:    new Date(Date.now() + 3600 * 1000).toISOString(),
  },
};

// Imita IReadOnlyCollection<ProductoResponse> de /api/administracion/productos/pendientes
export const MOCK_PRODUCTOS_PENDIENTES = [
  {
    id:                    "a1b2c3d4-0001-0000-0000-000000000001",
    nombre:                "Pinto casero",
    codigoBarras:          "7501234560011",
    porcionGramosMililitros: 200,
    calorias:              215,
    proteinas:             7.5,
    carbohidratos:         38,
    grasas:                4.2,
    sodioMiligramos:       320,
    vitaminas:             "B1, B6",
    calcioMiligramos:      45,
    hierroMiligramos:      2.1,
    estaAprobado:          false,
    fechaCreacionUtc:      "2026-06-10T14:22:00Z",
    // Campo extra solo en mock para mostrar quién creó — el backend real
    // no lo incluye en ProductoResponse todavía, así que el componente
    // lo trata como opcional.
    creadoPor:             "Mariana Solís (Usuario)",
  },
  {
    id:                    "a1b2c3d4-0002-0000-0000-000000000002",
    nombre:                "Batido proteico de avena",
    codigoBarras:          "7501234560028",
    porcionGramosMililitros: 350,
    calorias:              310,
    proteinas:             22,
    carbohidratos:         40,
    grasas:                6,
    sodioMiligramos:       180,
    vitaminas:             null,
    calcioMiligramos:      120,
    hierroMiligramos:      null,
    estaAprobado:          false,
    fechaCreacionUtc:      "2026-06-12T09:05:00Z",
    creadoPor:             "Lic. Pablo Rojas (Nutricionista)",
  },
  {
    id:                    "a1b2c3d4-0003-0000-0000-000000000003",
    nombre:                "Ensalada de garbanzos",
    codigoBarras:          "7501234560035",
    porcionGramosMililitros: 250,
    calorias:              180,
    proteinas:             9,
    carbohidratos:         24,
    grasas:                5,
    sodioMiligramos:       95,
    vitaminas:             "C, K",
    calcioMiligramos:      60,
    hierroMiligramos:      3.2,
    estaAprobado:          false,
    fechaCreacionUtc:      "2026-06-13T17:40:00Z",
    creadoPor:             "Esteban Vargas (Usuario)",
  },
];

// Imita la respuesta del SP sp_ReporteCobroNutricionistas agrupada por tipo_cobro.
// El SP devuelve filas planas — el backend las agrupa antes de enviarlas al frontend.
// Campos: nombre_nutricionista, tipo_cobro, cantidad_pacientes,
//         monto_base_por_paciente, subtotal, porcentaje_descuento,
//         monto_descuento, total_cobrar
export const MOCK_REPORTE_COBRO = [
  {
    tipoCobro: "semanal",
    nutricionistas: [
      {
        nombreNutricionista:   "Pablo Rojas Méndez",
        correo:                "pablo.rojas@nutritec.com",
        cantidadPacientes:     6,
        montoBasePorPaciente:  1,
        subtotal:              6,
        porcentajeDescuento:   0,
        montoDescuento:        0,
        totalCobrar:           6,
      },
    ],
  },
  {
    tipoCobro: "mensual",
    nutricionistas: [
      {
        nombreNutricionista:   "Ana Jiménez Castro",
        correo:                "ana.jimenez@nutritec.com",
        cantidadPacientes:     10,
        montoBasePorPaciente:  4,
        subtotal:              40,
        porcentajeDescuento:   0.05,
        montoDescuento:        2,
        totalCobrar:           38,
      },
    ],
  },
  {
    tipoCobro: "anual",
    nutricionistas: [
      {
        nombreNutricionista:   "Luis Araya Quesada",
        correo:                "luis.araya@nutritec.com",
        cantidadPacientes:     4,
        montoBasePorPaciente:  52,
        subtotal:              208,
        porcentajeDescuento:   0.10,
        montoDescuento:        20.8,
        totalCobrar:           187.2,
      },
    ],
  },
];