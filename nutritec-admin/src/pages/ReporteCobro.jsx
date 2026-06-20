import { useState } from "react";
import { Receipt, FileDown, Loader2 } from "lucide-react";
import { USE_MOCKS, ENDPOINTS, apiFetch } from "../config/api.js";
import { MOCK_REPORTE_COBRO } from "../config/mockData.js";

// =============================================================
// Lógica de cálculo (espeja el stored procedure del backend)
// Semanal:  $1  por paciente, sin descuento
// Mensual:  $4  por paciente, 5%  de descuento
// Anual:    $52 por paciente, 10% de descuento
// =============================================================
const CONFIG_COBRO = {
  semanal: { tarifa: 1,  descuento: 0,    label: "Semanal" },
  mensual: { tarifa: 4,  descuento: 0.05, label: "Mensual" },
  anual:   { tarifa: 52, descuento: 0.10, label: "Anual"   },
};

// =============================================================
// Servicio
// =============================================================
async function fetchReporte() {
  if (USE_MOCKS) {
    return new Promise((res) => setTimeout(() => res(MOCK_REPORTE_COBRO), 600));
  }
  return apiFetch(ENDPOINTS.cobros.reporte());
}

// =============================================================
// Exportar a PDF usando jsPDF + autoTable (cargados en index.html)
// =============================================================
function exportarPDF(grupos) {
  const { jsPDF } = window.jspdf;
  const doc = new jsPDF({ orientation: "landscape" });

  const fechaHoy = new Date().toLocaleDateString("es-CR", {
    day: "2-digit", month: "long", year: "numeric",
  });

  // Encabezado del documento
  doc.setFont("helvetica", "bold");
  doc.setFontSize(16);
  doc.setTextColor(47, 84, 60);
  doc.text("NutriTEC — Reporte de Cobro", 14, 16);

  doc.setFont("helvetica", "normal");
  doc.setFontSize(10);
  doc.setTextColor(120, 116, 108);
  doc.text(`Generado el ${fechaHoy}`, 14, 23);

  let cursorY = 30;

  grupos.forEach((grupo) => {
    const cfg = CONFIG_COBRO[grupo.tipo_cobro];

    // Subtítulo por tipo de cobro
    doc.setFont("helvetica", "bold");
    doc.setFontSize(12);
    doc.setTextColor(47, 84, 60);
    doc.text(`Tipo de cobro: ${cfg.label}`, 14, cursorY + 6);
    cursorY += 10;

    const filas = grupo.nutricionistas.map((n) => [
      n.nombre_completo,
      n.email,
      n.numero_tarjeta,
      n.cantidad_pacientes,
      `$${n.monto_total.toFixed(2)}`,
      cfg.descuento > 0 ? `${(cfg.descuento * 100).toFixed(0)}%` : "—",
      `$${n.monto_a_cobrar.toFixed(2)}`,
    ]);

    doc.autoTable({
      startY: cursorY,
      head: [[
        "Nombre completo",
        "Correo electrónico",
        "N.° tarjeta",
        "Pacientes",
        "Monto total",
        "Descuento",
        "Monto a cobrar",
      ]],
      body: filas,
      styles: { fontSize: 9, cellPadding: 3 },
      headStyles: { fillColor: [79, 156, 104], textColor: 255, fontStyle: "bold" },
      alternateRowStyles: { fillColor: [245, 243, 238] },
      margin: { left: 14, right: 14 },
    });

    cursorY = doc.lastAutoTable.finalY + 12;
  });

  doc.save("reporte-cobro-nutritec.pdf");
}

// =============================================================
// Sub-componente: tabla de un grupo de tipo de cobro
// =============================================================
function TablaGrupo({ grupo }) {
  const cfg = CONFIG_COBRO[grupo.tipo_cobro];

  return (
    <div style={s.grupoCard}>
      {/* Header del grupo */}
      <div style={s.grupoHeader}>
        <div style={s.grupoBadge}>{cfg.label}</div>
        <span style={s.grupoCount}>
          {grupo.nutricionistas.length} nutricionista{grupo.nutricionistas.length !== 1 ? "s" : ""}
        </span>
      </div>

      {/* Tabla */}
      <div style={s.tableWrapper}>
        <table style={s.table}>
          <thead>
            <tr>
              {["Nombre completo", "Correo electrónico", "N.° tarjeta", "Pacientes", "Monto total", "Descuento", "Monto a cobrar"].map((col) => (
                <th key={col} style={s.th}>{col}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {grupo.nutricionistas.map((n, i) => (
              <tr key={n.email} style={{ background: i % 2 === 0 ? "#FFFFFF" : "#FAF8F3" }}>
                <td style={s.td}>{n.nombre_completo}</td>
                <td style={{ ...s.td, color: "#8C887D" }}>{n.email}</td>
                <td style={{ ...s.td, fontFamily: "monospace", fontSize: "12px" }}>{n.numero_tarjeta}</td>
                <td style={{ ...s.td, textAlign: "center" }}>{n.cantidad_pacientes}</td>
                <td style={{ ...s.td, textAlign: "right" }}>${n.monto_total.toFixed(2)}</td>
                <td style={{ ...s.td, textAlign: "center", color: n.descuento > 0 ? "#3F7A52" : "#ACA897" }}>
                  {n.descuento > 0 ? `$${n.descuento.toFixed(2)}` : "—"}
                </td>
                <td style={{ ...s.td, textAlign: "right", fontWeight: 600, color: "#2B2A26" }}>
                  ${n.monto_a_cobrar.toFixed(2)}
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
  const [grupos, setGrupos] = useState(null);
  const [cargando, setCargando] = useState(false);
  const [cargado, setCargado] = useState(false);
  const [exportando, setExportando] = useState(false);

  if (!cargado && !cargando) {
    setCargando(true);
    fetchReporte().then((data) => {
      setGrupos(data);
      setCargando(false);
      setCargado(true);
    });
  }

  async function handleExportarPDF() {
    setExportando(true);
    // Pequeño timeout para que el botón muestre el spinner
    setTimeout(() => {
      exportarPDF(grupos);
      setExportando(false);
    }, 200);
  }

  return (
    <div>
      {/* Encabezado */}
      <div style={s.sectionHeader}>
        <div>
          <h2 style={s.sectionTitle}>Reporte de cobro</h2>
          <p style={s.sectionSub}>
            Monto a cobrar a cada nutricionista según su tipo de suscripción y cantidad de pacientes activos.
          </p>
        </div>
        {grupos && grupos.length > 0 && (
          <button
            style={{ ...s.pdfBtn, opacity: exportando ? 0.7 : 1 }}
            onClick={handleExportarPDF}
            disabled={exportando}
          >
            {exportando
              ? <><Loader2 size={15} /> Generando...</>
              : <><FileDown size={15} /> Exportar PDF</>
            }
          </button>
        )}
      </div>

      {/* Estados */}
      {cargando && (
        <div style={s.estadoCentrado}>
          <p style={s.estadoTexto}>Cargando reporte...</p>
        </div>
      )}

      {!cargando && grupos && (
        <div style={s.listaGrupos}>
          {grupos.map((g) => (
            <TablaGrupo key={g.tipo_cobro} grupo={g} />
          ))}
        </div>
      )}
    </div>
  );
}

// =============================================================
// Estilos
// =============================================================
const s = {
  sectionHeader: {
    display: "flex",
    alignItems: "flex-start",
    justifyContent: "space-between",
    marginBottom: "24px",
    gap: "16px",
    flexWrap: "wrap",
  },
  sectionTitle: {
    fontSize: "20px",
    fontWeight: 600,
    color: "#2B2A26",
    margin: "0 0 4px",
    letterSpacing: "-0.01em",
  },
  sectionSub: {
    fontSize: "13px",
    color: "#8C887D",
    margin: 0,
    maxWidth: "520px",
  },
  pdfBtn: {
    display: "flex",
    alignItems: "center",
    gap: "6px",
    padding: "8px 16px",
    borderRadius: "10px",
    border: "none",
    background: "#4F9C68",
    color: "#FFFFFF",
    fontSize: "13px",
    fontWeight: 600,
    cursor: "pointer",
    flexShrink: 0,
    whiteSpace: "nowrap",
  },
  listaGrupos: {
    display: "flex",
    flexDirection: "column",
    gap: "16px",
  },
  grupoCard: {
    background: "#FFFFFF",
    border: "1px solid #E8E4D9",
    borderRadius: "14px",
    overflow: "hidden",
  },
  grupoHeader: {
    display: "flex",
    alignItems: "center",
    gap: "10px",
    padding: "14px 20px",
    borderBottom: "1px solid #F3F1EA",
  },
  grupoBadge: {
    background: "#DCEFE0",
    color: "#3F7A52",
    fontSize: "12px",
    fontWeight: 600,
    padding: "3px 12px",
    borderRadius: "20px",
  },
  grupoCount: {
    fontSize: "12px",
    color: "#ACA897",
  },
  tableWrapper: {
    overflowX: "auto",
  },
  table: {
    width: "100%",
    borderCollapse: "collapse",
    fontSize: "13px",
  },
  th: {
    padding: "10px 16px",
    textAlign: "left",
    fontSize: "11px",
    fontWeight: 600,
    color: "#8C887D",
    background: "#FAF8F3",
    borderBottom: "1px solid #E8E4D9",
    whiteSpace: "nowrap",
  },
  td: {
    padding: "11px 16px",
    color: "#2B2A26",
    fontSize: "13px",
    borderBottom: "1px solid #F3F1EA",
    whiteSpace: "nowrap",
  },
  estadoCentrado: {
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    padding: "80px 24px",
    background: "#FFFFFF",
    borderRadius: "16px",
    border: "1px solid #E8E4D9",
  },
  estadoTexto: {
    fontSize: "14px",
    color: "#ACA897",
    margin: 0,
  },
};