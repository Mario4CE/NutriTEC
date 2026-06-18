import { useState } from "react";
import AdminLogin from "./pages/AdminLogin.jsx";

// =============================================================
// App — controla la sesión del administrador.
//
// Por ahora solo existe AdminLogin. Cuando armemos el Dashboard
// (aprobación de productos + reporte de cobro), este archivo va a
// mostrar uno u otro según si `session` tiene datos o es null.
//
// sessionStorage (no localStorage) se usa para que la sesión se
// borre automáticamente al cerrar el navegador — más apropiado
// para un panel administrativo.
// =============================================================
function getStoredSession() {
  const raw = sessionStorage.getItem("nutritec_admin_session");
  return raw ? JSON.parse(raw) : null;
}

export default function App() {
  const [session, setSession] = useState(getStoredSession());

  function handleLoginSuccess(newSession) {
    sessionStorage.setItem("nutritec_admin_session", JSON.stringify(newSession));
    setSession(newSession);
  }

  function handleLogout() {
    sessionStorage.removeItem("nutritec_admin_session");
    setSession(null);
  }

  if (!session) {
    return <AdminLogin onLoginSuccess={handleLoginSuccess} />;
  }

  // Placeholder temporal — se reemplaza por <AdminDashboard /> cuando
  // construyamos aprobación de productos y reporte de cobro.
  return (
    <div style={{ padding: "40px", fontFamily: "Inter, sans-serif" }}>
      <p>Sesión activa: {session.email}</p>
      <button onClick={handleLogout}>Cerrar sesión</button>
    </div>
  );
}