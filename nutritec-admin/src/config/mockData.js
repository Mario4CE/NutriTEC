/**
 * mockData.js — Datos falsos para desarrollar sin backend real.
 *
 * Cuando USE_MOCKS (en api.js) sea false, estos datos dejan de usarse
 * y los servicios llaman a la API real en su lugar.
 */

export const MOCK_ADMIN = {
  email: "admin@nutritec.com",
  password: "Admin2026!",
};

export const MOCK_PRODUCTOS_PENDIENTES = [
  {
    id_producto: "a1b2c3d4-0001",
    nombre: "Pinto casero",
    codigo_barras: "7501234560011",
    calorias: 215,
    proteinas: 7.5,
    carbohidratos: 38,
    grasas: 4.2,
    creado_por: { tipo: "usuario", nombre: "Mariana Solís" },
    fecha_creacion_utc: "2026-06-10T14:22:00Z",
  },
  {
    id_producto: "a1b2c3d4-0002",
    nombre: "Batido proteico de avena",
    codigo_barras: "7501234560028",
    calorias: 310,
    proteinas: 22,
    carbohidratos: 40,
    grasas: 6,
    creado_por: { tipo: "nutricionista", nombre: "Lic. Pablo Rojas" },
    fecha_creacion_utc: "2026-06-12T09:05:00Z",
  },
  {
    id_producto: "a1b2c3d4-0003",
    nombre: "Ensalada de garbanzos",
    codigo_barras: "7501234560035",
    calorias: 180,
    proteinas: 9,
    carbohidratos: 24,
    grasas: 5,
    creado_por: { tipo: "usuario", nombre: "Esteban Vargas" },
    fecha_creacion_utc: "2026-06-13T17:40:00Z",
  },
];

export const MOCK_REPORTE_COBRO = [
  {
    tipo_cobro: "semanal",
    nutricionistas: [
      {
        email: "pablo.rojas@nutritec.com",
        nombre_completo: "Pablo Rojas Méndez",
        numero_tarjeta: "**** **** **** 4521",
        cantidad_pacientes: 6,
        monto_total: 6,
        descuento: 0,
        monto_a_cobrar: 6,
      },
    ],
  },
  {
    tipo_cobro: "mensual",
    nutricionistas: [
      {
        email: "ana.jimenez@nutritec.com",
        nombre_completo: "Ana Jiménez Castro",
        numero_tarjeta: "**** **** **** 7788",
        cantidad_pacientes: 10,
        monto_total: 40,
        descuento: 2,
        monto_a_cobrar: 38,
      },
    ],
  },
  {
    tipo_cobro: "anual",
    nutricionistas: [
      {
        email: "luis.araya@nutritec.com",
        nombre_completo: "Luis Araya Quesada",
        numero_tarjeta: "**** **** **** 1190",
        cantidad_pacientes: 4,
        monto_total: 208,
        descuento: 20.8,
        monto_a_cobrar: 187.2,
      },
    ],
  },
];