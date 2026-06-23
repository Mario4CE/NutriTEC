import { useState } from "react";
import { ShieldCheck, PackageSearch, Receipt, LogOut } from "lucide-react";
import AprobacionProductos from "./AprobacionProductos.jsx";
import ReporteCobro from "./ReporteCobro.jsx";

const SECCIONES = [
  { id: "productos", label: "Aprobación de productos", icon: PackageSearch, component: AprobacionProductos },
  { id: "cobro",    label: "Reporte de cobro",         icon: Receipt,        component: ReporteCobro },
];

export default function AdminDashboard({ session, onLogout }) {
  const [seccionActiva, setSeccionActiva] = useState("productos");
  const SeccionActual = SECCIONES.find((s) => s.id === seccionActiva).component;

  return (
    <div style={styles.wrapper}>
      {/* ── Navbar ── */}
      <nav style={styles.navbar}>
        {/* Logo / marca */}
        <div style={styles.brand}>
          <div style={styles.brandIcon}>
            <ShieldCheck size={18} color="#3F7A52" strokeWidth={2} />
          </div>
          <span style={styles.brandName}>NutriTEC</span>
          <span style={styles.brandRole}>Admin</span>
        </div>

        {/* Links de navegación */}
        <div style={styles.navLinks}>
          {SECCIONES.map((s) => {
            const activo = seccionActiva === s.id;
            const Icon = s.icon;
            return (
              <button
                key={s.id}
                onClick={() => setSeccionActiva(s.id)}
                style={{
                  ...styles.navLink,
                  ...(activo ? styles.navLinkActivo : {}),
                }}
              >
                <Icon size={15} strokeWidth={activo ? 2.2 : 1.8} />
                {s.label}
              </button>
            );
          })}
        </div>

        {/* Usuario + cerrar sesión */}
        <div style={styles.navRight}>
          <span style={styles.sessionEmail}>{session.email}</span>
          <button onClick={onLogout} style={styles.logoutBtn}>
            <LogOut size={15} strokeWidth={1.8} />
            Salir
          </button>
        </div>
      </nav>

      {/* ── Contenido ── */}
      <main style={styles.main}>
        <SeccionActual />
      </main>
    </div>
  );
}

// ── Estilos ──────────────────────────────────────────────────
const styles = {
  wrapper: {
    minHeight: "100vh",
    background: "#FAF8F3",
    fontFamily: "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif",
  },
  navbar: {
    height: "56px",
    background: "#FFFFFF",
    borderBottom: "1px solid #E8E4D9",
    display: "flex",
    alignItems: "center",
    padding: "0 24px",
    gap: "8px",
    position: "sticky",
    top: 0,
    zIndex: 100,
  },
  brand: {
    display: "flex",
    alignItems: "center",
    gap: "8px",
    marginRight: "24px",
    flexShrink: 0,
  },
  brandIcon: {
    width: "30px",
    height: "30px",
    borderRadius: "8px",
    background: "#DCEFE0",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
  },
  brandName: {
    fontSize: "15px",
    fontWeight: 700,
    color: "#2B2A26",
    letterSpacing: "-0.01em",
  },
  brandRole: {
    fontSize: "11px",
    fontWeight: 500,
    color: "#FFFFFF",
    background: "#4F9C68",
    borderRadius: "20px",
    padding: "1px 8px",
  },
  navLinks: {
    display: "flex",
    alignItems: "center",
    gap: "4px",
    flex: 1,
  },
  navLink: {
    display: "flex",
    alignItems: "center",
    gap: "6px",
    padding: "6px 12px",
    borderRadius: "8px",
    border: "none",
    background: "none",
    fontSize: "13px",
    fontWeight: 500,
    color: "#8C887D",
    cursor: "pointer",
    transition: "all 0.15s ease",
    whiteSpace: "nowrap",
  },
  navLinkActivo: {
    background: "#DCEFE0",
    color: "#3F7A52",
  },
  navRight: {
    display: "flex",
    alignItems: "center",
    gap: "12px",
    flexShrink: 0,
    marginLeft: "auto",
  },
  sessionEmail: {
    fontSize: "12px",
    color: "#ACA897",
  },
  logoutBtn: {
    display: "flex",
    alignItems: "center",
    gap: "5px",
    padding: "6px 12px",
    borderRadius: "8px",
    border: "1px solid #E8E4D9",
    background: "none",
    fontSize: "13px",
    fontWeight: 500,
    color: "#8C887D",
    cursor: "pointer",
  },
  main: {
    padding: "32px 24px",
    maxWidth: "1100px",
    margin: "0 auto",
  },
  placeholder: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "center",
    gap: "12px",
    padding: "80px 24px",
    background: "#FFFFFF",
    borderRadius: "16px",
    border: "1px solid #E8E4D9",
  },
  placeholderText: {
    fontSize: "14px",
    color: "#ACA897",
    margin: 0,
  },
};