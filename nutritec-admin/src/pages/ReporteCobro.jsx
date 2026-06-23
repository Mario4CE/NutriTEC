import { useState } from "react";
import { FileDown, Loader2 } from "lucide-react";
import { USE_MOCKS, ENDPOINTS, apiFetch } from "../config/api.js";
import { MOCK_REPORTE_COBRO } from "../config/mockData.js";

// =============================================================
// Reporte de cobro usa el SP directamente:
//   GET /api/sql-programable/stored-procedures/reporte-cobro-nutricionistas
//   → Lista plana de ReporteCobroNutricionistaResponse
//
// El SP sp_ReporteCobroNutricionistas ya aplica la lógica de:
//   Semanal:  $1  × pacientes, sin descuento
//   Mensual:  $4  × pacientes, 5% descuento
//   Anual:    $52 × pacientes, 10% descuento
//
// El frontend solo agrupa las filas planas por tipoCobro.
// =============================================================

const LABEL_TIPO = { semanal: "Semanal", mensual: "Mensual", anual: "Anual" };
const ORDEN_TIPO = ["semanal", "mensual", "anual"];

function agruparFilas(filas) {
  const mapa = {};
  for (const fila of filas) {
    const tipo = fila.tipoCobro?.toLowerCase() ?? fila.TipoCobro?.toLowerCase();
    if (!mapa[tipo]) mapa[tipo] = [];
    mapa[tipo].push(fila);
  }
  return ORDEN_TIPO.filter((t) => mapa[t]).map((t) => ({ tipoCobro: t, nutricionistas: mapa[t] }));
}

async function fetchReporte() {
  if (USE_MOCKS) {
    const plano = MOCK_REPORTE_COBRO.flatMap((g) =>
      g.nutricionistas.map((n) => ({ ...n, tipoCobro: g.tipoCobro }))
    );
    return new Promise((res) => setTimeout(() => res(plano), 600));
  }
  // SP directo — montoBasePorPaciente=1 porque el SP calcula
  // los factores (×4, ×52) internamente según tipo_cobro
  const data = await apiFetch(ENDPOINTS.sp.reporteCobro(1, true));
  return data?.data ?? data;
}

// =============================================================
// PDF con jsPDF + autoTable
// =============================================================
function exportarPDF(grupos) {
  const { jsPDF } = window.jspdf;
  const doc = new jsPDF({ orientation: "landscape" });
  const hoy = new Date().toLocaleDateString("es-CR", {
    day: "2-digit", month: "long", year: "numeric",
  });

  doc.setFont("helvetica", "bold");
  doc.setFontSize(16);
  doc.setTextColor(47, 84, 60);
  doc.text("NutriTEC — Reporte de Cobro", 14, 16);

  doc.setFont("helvetica", "normal");
  doc.setFontSize(9);
  doc.setTextColor(120, 116, 108);
  doc.text(`Generado el ${hoy}  ·  Datos obtenidos via SP sp_ReporteCobroNutricionistas`, 14, 23);

  let y = 30;
  grupos.forEach((grupo) => {
    doc.setFont("helvetica", "bold");
    doc.setFontSize(12);
    doc.setTextColor(47, 84, 60);
    doc.text(`Tipo de cobro: ${LABEL_TIPO[grupo.tipoCobro] ?? grupo.tipoCobro}`, 14, y + 6);
    y += 10;

    doc.autoTable({
      startY: y,
      head: [["Nombre", "Cédula", "Pacientes", "Monto base", "Subtotal", "Descuento", "Total a cobrar"]],
      body: grupo.nutricionistas.map((n) => [
        n.nombreNutricionista,
        n.cedulaNutricionista,
        n.cantidadPacientes,
        `$${Number(n.montoBasePorPaciente).toFixed(2)}`,
        `$${Number(n.subtotal).toFixed(2)}`,
        n.porcentajeDescuento > 0
          ? `${(n.porcentajeDescuento * 100).toFixed(0)}%  ($${Number(n.montoDescuento).toFixed(2)})`
          : "—",
        `$${Number(n.totalCobrar).toFixed(2)}`,
      ]),
      styles:          { fontSize: 9, cellPadding: 3 },
      headStyles:      { fillColor: [79, 156, 104], textColor: 255, fontStyle: "bold" },
      alternateRowStyles: { fillColor: [245, 243, 238] },
      margin: { left: 14, right: 14 },
    });

    y = doc.lastAutoTable.finalY + 14;
  });

  doc.save("reporte-cobro-nutritec.pdf");
}

// =============================================================
// Tabla de un grupo
// =============================================================
function TablaGrupo({ grupo }) {
  return (
    <div style={s.grupoCard}>
      <div style={s.grupoHeader}>
        <div style={s.grupoBadge}>{LABEL_TIPO[grupo.tipoCobro] ?? grupo.tipoCobro}</div>
        <span style={s.grupoCount}>
          {grupo.nutricionistas.length} nutricionista{grupo.nutricionistas.length !== 1 ? "s" : ""}
        </span>
      </div>
      <div style={s.tableWrapper}>
        <table style={s.table}>
          <thead>
            <tr>
              {["Nombre", "Cédula", "Pacientes", "Monto base", "Subtotal", "Descuento", "Total a cobrar"].map((col) => (
                <th key={col} style={s.th}>{col}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {grupo.nutricionistas.map((n, i) => (
              <tr key={n.cedulaNutricionista} style={{ background: i % 2 === 0 ? "#FFFFFF" : "#FAF8F3" }}>
                <td style={{ ...s.td, fontWeight: 500 }}>{n.nombreNutricionista}</td>
                <td style={{ ...s.td, fontFamily: "monospace", color: "#8C887D" }}>{n.cedulaNutricionista}</td>
                <td style={{ ...s.td, textAlign: "center" }}>{n.cantidadPacientes}</td>
                <td style={{ ...s.td, textAlign: "right" }}>${Number(n.montoBasePorPaciente).toFixed(2)}</td>
                <td style={{ ...s.td, textAlign: "right" }}>${Number(n.subtotal).toFixed(2)}</td>
                <td style={{ ...s.td, textAlign: "center", color: n.porcentajeDescuento > 0 ? "#3F7A52" : "#ACA897" }}>
                  {n.porcentajeDescuento > 0
                    ? `${(n.porcentajeDescuento * 100).toFixed(0)}%  ($${Number(n.montoDescuento).toFixed(2)})`
                    : "—"}
                </td>
                <td style={{ ...s.td, textAlign: "right", fontWeight: 700, color: "#2B2A26" }}>
                  ${Number(n.totalCobrar).toFixed(2)}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

// =============================================================
// Componente principal
// =============================================================
export default function ReporteCobro() {
  const [grupos, setGrupos]         = useState(null);
  const [cargando, setCargando]     = useState(false);
  const [cargado, setCargado]       = useState(false);
  const [exportando, setExportando] = useState(false);

  if (!cargado && !cargando) {
    setCargando(true);
    fetchReporte().then((filas) => {
      setGrupos(agruparFilas(filas));
      setCargando(false);
      setCargado(true);
    });
  }

  function handleExportarPDF() {
    setExportando(true);
    setTimeout(() => { exportarPDF(grupos); setExportando(false); }, 200);
  }

  return (
    <div>
      <div style={s.sectionHeader}>
        <div>
          <h2 style={s.sectionTitle}>Reporte de cobro</h2>
          <p style={s.sectionSub}>
            Monto a cobrar a cada nutricionista según su tipo de suscripción y cantidad de pacientes activos. Generado mediante el stored procedure <code>sp_ReporteCobroNutricionistas</code>.
          </p>
        </div>
        {grupos?.length > 0 && (
          <button
            style={{ ...s.pdfBtn, opacity: exportando ? 0.7 : 1 }}
            onClick={handleExportarPDF}
            disabled={exportando}
          >
            {exportando ? <><Loader2 size={15} /> Generando...</> : <><FileDown size={15} /> Exportar PDF</>}
          </button>
        )}
      </div>

      {cargando && (
        <div style={s.estadoCentrado}><p style={s.estadoTexto}>Cargando reporte...</p></div>
      )}

      {!cargando && grupos && (
        <div style={s.listaGrupos}>
          {grupos.map((g) => <TablaGrupo key={g.tipoCobro} grupo={g} />)}
        </div>
      )}
    </div>
  );
}

const s = {
  sectionHeader:  { display: "flex", alignItems: "flex-start", justifyContent: "space-between", marginBottom: "24px", gap: "16px", flexWrap: "wrap" },
  sectionTitle:   { fontSize: "20px", fontWeight: 600, color: "#2B2A26", margin: "0 0 4px", letterSpacing: "-0.01em" },
  sectionSub:     { fontSize: "13px", color: "#8C887D", margin: 0, maxWidth: "560px" },
  pdfBtn:         { display: "flex", alignItems: "center", gap: "6px", padding: "8px 16px", borderRadius: "10px", border: "none", background: "#4F9C68", color: "#FFFFFF", fontSize: "13px", fontWeight: 600, cursor: "pointer", flexShrink: 0, whiteSpace: "nowrap" },
  listaGrupos:    { display: "flex", flexDirection: "column", gap: "16px" },
  grupoCard:      { background: "#FFFFFF", border: "1px solid #E8E4D9", borderRadius: "14px", overflow: "hidden" },
  grupoHeader:    { display: "flex", alignItems: "center", gap: "10px", padding: "14px 20px", borderBottom: "1px solid #F3F1EA" },
  grupoBadge:     { background: "#DCEFE0", color: "#3F7A52", fontSize: "12px", fontWeight: 600, padding: "3px 12px", borderRadius: "20px" },
  grupoCount:     { fontSize: "12px", color: "#ACA897" },
  tableWrapper:   { overflowX: "auto" },
  table:          { width: "100%", borderCollapse: "collapse", fontSize: "13px" },
  th:             { padding: "10px 16px", textAlign: "left", fontSize: "11px", fontWeight: 600, color: "#8C887D", background: "#FAF8F3", borderBottom: "1px solid #E8E4D9", whiteSpace: "nowrap" },
  td:             { padding: "11px 16px", color: "#2B2A26", fontSize: "13px", borderBottom: "1px solid #F3F1EA", whiteSpace: "nowrap" },
  estadoCentrado: { display: "flex", alignItems: "center", justifyContent: "center", padding: "80px 24px", background: "#FFFFFF", borderRadius: "16px", border: "1px solid #E8E4D9" },
  estadoTexto:    { fontSize: "14px", color: "#ACA897", margin: 0 },
};