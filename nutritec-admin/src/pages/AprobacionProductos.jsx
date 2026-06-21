import { useState } from "react";
import { PackageSearch, Check, X, ChevronDown, ChevronUp } from "lucide-react";
import { USE_MOCKS, ENDPOINTS, apiFetch } from "../config/api.js";
import { MOCK_PRODUCTOS_PENDIENTES } from "../config/mockData.js";

// =============================================================
// Servicios — campos exactos de ProductoResponse del backend:
//   id, nombre, codigoBarras, porcionGramosMililitros,
//   calorias, proteinas, carbohidratos, grasas, sodioMiligramos,
//   vitaminas, calcioMiligramos, hierroMiligramos,
//   estaAprobado, fechaCreacionUtc
//
// Aprobar: PUT /api/administracion/productos/{id}/aprobacion → 204
// Rechazar: DELETE /api/productos/{id} → 204 (rol Administrador)
// =============================================================

async function fetchProductosPendientes() {
  if (USE_MOCKS) {
    return new Promise((res) => setTimeout(() => res([...MOCK_PRODUCTOS_PENDIENTES]), 500));
  }
  const data = await apiFetch(ENDPOINTS.administracion.productosPendientes());
  // El backend envuelve en ApiResponse<T> — extraemos .data
  return data?.data ?? data;
}

async function aprobarProducto(id) {
  if (USE_MOCKS) return new Promise((res) => setTimeout(() => res(null), 400));
  return apiFetch(ENDPOINTS.administracion.aprobarProducto(id), { method: "PUT" });
}

async function rechazarProducto(id) {
  if (USE_MOCKS) return new Promise((res) => setTimeout(() => res(null), 400));
  return apiFetch(ENDPOINTS.productos.eliminar(id), { method: "DELETE" });
}

// =============================================================
// Fila expandible
// =============================================================
function FilaProducto({ producto, onAprobar, onRechazar }) {
  const [expandido, setExpandido]   = useState(false);
  const [accionando, setAccionando] = useState(null);

  async function handleAprobar() {
    setAccionando("aprobar");
    await onAprobar(producto.id);
    setAccionando(null);
  }

  async function handleRechazar() {
    setAccionando("rechazar");
    await onRechazar(producto.id);
    setAccionando(null);
  }

  const fecha = new Date(producto.fechaCreacionUtc).toLocaleDateString("es-CR", {
    day: "2-digit", month: "short", year: "numeric",
  });

  return (
    <div style={s.card}>
      <div style={s.cardHeader}>
        <div style={s.cardInfo}>
          <span style={s.productNombre}>{producto.nombre}</span>
          <span style={s.productMeta}>
            Código: {producto.codigoBarras}
            {producto.creadoPor && <> &nbsp;·&nbsp; {producto.creadoPor}</>}
            &nbsp;·&nbsp; {fecha}
          </span>
        </div>

        <div style={s.cardActions}>
          <span style={s.calChip}>{producto.calorias} kcal</span>

          <button style={s.iconBtn} onClick={() => setExpandido((e) => !e)} title="Ver detalles nutricionales">
            {expandido ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
          </button>

          <button style={{ ...s.actionBtn, ...s.btnAprobar }} onClick={handleAprobar} disabled={!!accionando}>
            {accionando === "aprobar" ? "..." : <><Check size={14} strokeWidth={2.5} /> Aprobar</>}
          </button>

          <button style={{ ...s.actionBtn, ...s.btnRechazar }} onClick={handleRechazar} disabled={!!accionando}>
            {accionando === "rechazar" ? "..." : <><X size={14} strokeWidth={2.5} /> Rechazar</>}
          </button>
        </div>
      </div>

      {expandido && (
        <div style={s.nutriGrid}>
          {[
            { label: "Porción",        valor: producto.porcionGramosMililitros, unidad: "g/ml" },
            { label: "Proteínas",      valor: producto.proteinas,               unidad: "g"    },
            { label: "Carbohidratos",  valor: producto.carbohidratos,           unidad: "g"    },
            { label: "Grasas",         valor: producto.grasas,                  unidad: "g"    },
            { label: "Sodio",          valor: producto.sodioMiligramos,         unidad: "mg"   },
            { label: "Calcio",         valor: producto.calcioMiligramos,        unidad: "mg"   },
            { label: "Hierro",         valor: producto.hierroMiligramos,        unidad: "mg"   },
            { label: "Vitaminas",      valor: producto.vitaminas,               unidad: ""     },
          ]
            .filter(({ valor }) => valor !== null && valor !== undefined)
            .map(({ label, valor, unidad }) => (
              <div key={label} style={s.nutriItem}>
                <span style={s.nutriLabel}>{label}</span>
                <span style={s.nutriValor}>{valor} {unidad}</span>
              </div>
            ))}
        </div>
      )}
    </div>
  );
}

// =============================================================
// Componente principal
// =============================================================
export default function AprobacionProductos() {
  const [productos, setProductos] = useState(null);
  const [cargando, setCargando]   = useState(false);
  const [cargado, setCargado]     = useState(false);

  if (!cargado && !cargando) {
    setCargando(true);
    fetchProductosPendientes().then((data) => {
      setProductos(data);
      setCargando(false);
      setCargado(true);
    });
  }

  async function handleAprobar(id) {
    await aprobarProducto(id);
    setProductos((prev) => prev.filter((p) => p.id !== id));
  }

  async function handleRechazar(id) {
    await rechazarProducto(id);
    setProductos((prev) => prev.filter((p) => p.id !== id));
  }

  return (
    <div>
      <div style={s.sectionHeader}>
        <div>
          <h2 style={s.sectionTitle}>Aprobación de productos</h2>
          <p style={s.sectionSub}>
            Revisá y aprobá los productos enviados por usuarios y nutricionistas antes de que estén disponibles para toda la comunidad.
          </p>
        </div>
        {productos && (
          <span style={s.badge}>
            {productos.length} pendiente{productos.length !== 1 ? "s" : ""}
          </span>
        )}
      </div>

      {cargando && (
        <div style={s.estadoCentrado}>
          <p style={s.estadoTexto}>Cargando productos...</p>
        </div>
      )}

      {!cargando && productos?.length === 0 && (
        <div style={s.estadoCentrado}>
          <PackageSearch size={36} color="#BFD9C7" strokeWidth={1.5} />
          <p style={s.estadoTexto}>No hay productos pendientes de aprobación.</p>
        </div>
      )}

      {!cargando && productos?.length > 0 && (
        <div style={s.lista}>
          {productos.map((p) => (
            <FilaProducto key={p.id} producto={p} onAprobar={handleAprobar} onRechazar={handleRechazar} />
          ))}
        </div>
      )}
    </div>
  );
}

const s = {
  sectionHeader:  { display: "flex", alignItems: "flex-start", justifyContent: "space-between", marginBottom: "24px", gap: "16px" },
  sectionTitle:   { fontSize: "20px", fontWeight: 600, color: "#2B2A26", margin: "0 0 4px", letterSpacing: "-0.01em" },
  sectionSub:     { fontSize: "13px", color: "#8C887D", margin: 0, maxWidth: "520px" },
  badge:          { flexShrink: 0, background: "#DCEFE0", color: "#3F7A52", fontSize: "12px", fontWeight: 600, padding: "4px 12px", borderRadius: "20px", whiteSpace: "nowrap" },
  lista:          { display: "flex", flexDirection: "column", gap: "10px" },
  card:           { background: "#FFFFFF", border: "1px solid #E8E4D9", borderRadius: "14px", padding: "16px 20px" },
  cardHeader:     { display: "flex", alignItems: "center", gap: "12px", flexWrap: "wrap" },
  cardInfo:       { flex: 1, minWidth: "200px", display: "flex", flexDirection: "column", gap: "4px" },
  productNombre:  { fontSize: "15px", fontWeight: 600, color: "#2B2A26" },
  productMeta:    { fontSize: "12px", color: "#ACA897" },
  cardActions:    { display: "flex", alignItems: "center", gap: "8px", flexWrap: "wrap" },
  calChip:        { fontSize: "12px", fontWeight: 500, color: "#5C5848", background: "#F3F1EA", padding: "3px 10px", borderRadius: "20px" },
  iconBtn:        { background: "none", border: "1px solid #E8E4D9", borderRadius: "8px", padding: "5px 7px", cursor: "pointer", display: "flex", alignItems: "center", color: "#8C887D" },
  actionBtn:      { display: "flex", alignItems: "center", gap: "5px", padding: "6px 14px", borderRadius: "8px", border: "none", fontSize: "13px", fontWeight: 500, cursor: "pointer" },
  btnAprobar:     { background: "#DCEFE0", color: "#3F7A52" },
  btnRechazar:    { background: "#FBEAE5", color: "#A1442B" },
  nutriGrid:      { display: "flex", gap: "10px", marginTop: "14px", paddingTop: "14px", borderTop: "1px solid #F3F1EA", flexWrap: "wrap" },
  nutriItem:      { display: "flex", flexDirection: "column", gap: "2px", background: "#FAF8F3", borderRadius: "10px", padding: "8px 14px", minWidth: "90px" },
  nutriLabel:     { fontSize: "11px", color: "#ACA897", fontWeight: 500 },
  nutriValor:     { fontSize: "14px", fontWeight: 600, color: "#2B2A26" },
  estadoCentrado: { display: "flex", flexDirection: "column", alignItems: "center", justifyContent: "center", gap: "12px", padding: "80px 24px", background: "#FFFFFF", borderRadius: "16px", border: "1px solid #E8E4D9" },
  estadoTexto:    { fontSize: "14px", color: "#ACA897", margin: 0 },
};