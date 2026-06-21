import { useState } from "react";
import { Eye, EyeOff, Loader2, ShieldCheck } from "lucide-react";
import { USE_MOCKS, ENDPOINTS, apiFetch } from "../config/api.js";
import { MOCK_ADMIN } from "../config/mockData.js";

/**
 * loginAdmin — abstrae si el login viene del mock o de la API real.
 *
 * API real: POST /api/auth/login
 * Body:     { Correo, Contrasena }   ← PascalCase, sin tilde en Contrasena
 * Respuesta: LoginResponse { idUsuario, nombre, correo, tipoUsuario, token, expiraEn }
 */
async function loginAdmin({ email, password }) {
  if (USE_MOCKS) {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        const ok =
          email.toLowerCase() === MOCK_ADMIN.email.toLowerCase() &&
          password === MOCK_ADMIN.password;
        ok ? resolve(MOCK_ADMIN.session) : reject(new Error("Credenciales inválidas"));
      }, 700);
    });
  }

  return apiFetch(ENDPOINTS.auth.login(), {
    method: "POST",
    body: JSON.stringify({ Correo: email, Contrasena: password }),
  });
}

export default function AdminLogin({ onLoginSuccess }) {
  const [email, setEmail]             = useState("");
  const [password, setPassword]       = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading]         = useState(false);
  const [error, setError]             = useState("");
  const [touched, setTouched]         = useState({ email: false, password: false });

  const emailValid    = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  const passwordValid = password.length >= 8; // MinLength(8) según LoginRequest del backend
  const canSubmit     = emailValid && passwordValid && !loading;

  async function handleSubmit(e) {
    e.preventDefault();
    setTouched({ email: true, password: true });
    setError("");
    if (!emailValid || !passwordValid) return;

    setLoading(true);
    try {
      const session = await loginAdmin({ email, password });
      onLoginSuccess?.(session);
    } catch {
      setError("El correo o la contraseña no son correctos.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={{
      minHeight: "600px", width: "100%", background: "#FAF8F3",
      display: "flex", alignItems: "center", justifyContent: "center",
      padding: "32px 16px",
      fontFamily: "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif",
    }}>
      <div style={{
        width: "100%", maxWidth: "400px", background: "#FFFFFF",
        borderRadius: "20px", border: "1px solid #E8E4D9",
        boxShadow: "0 1px 3px rgba(0,0,0,0.04), 0 8px 24px rgba(0,0,0,0.04)",
        padding: "40px 36px",
      }}>
        {/* Encabezado */}
        <div style={{ display: "flex", flexDirection: "column", alignItems: "center", marginBottom: "28px" }}>
          <div style={{
            width: "52px", height: "52px", borderRadius: "14px", background: "#DCEFE0",
            display: "flex", alignItems: "center", justifyContent: "center", marginBottom: "16px",
          }}>
            <ShieldCheck size={26} color="#3F7A52" strokeWidth={1.8} />
          </div>
          <h1 style={{ fontSize: "20px", fontWeight: 600, color: "#2B2A26", margin: 0, letterSpacing: "-0.01em" }}>
            NutriTEC Admin
          </h1>
          <p style={{ fontSize: "14px", color: "#8C887D", margin: "6px 0 0", textAlign: "center" }}>
            Acceso para administradores de la plataforma
          </p>
        </div>

        <form onSubmit={handleSubmit} noValidate>
          {/* Email */}
          <div style={{ marginBottom: "18px" }}>
            <label htmlFor="admin-email" style={{ display: "block", fontSize: "13px", fontWeight: 500, color: "#5C5848", marginBottom: "6px" }}>
              Correo electrónico
            </label>
            <input
              id="admin-email" type="email" autoComplete="username"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              onBlur={() => setTouched((t) => ({ ...t, email: true }))}
              onFocus={(e) => (e.target.style.borderColor = "#7FB892")}
              placeholder="admin@nutritec.com"
              style={{
                width: "100%", boxSizing: "border-box", padding: "11px 14px",
                fontSize: "14px", borderRadius: "10px",
                border: touched.email && !emailValid ? "1.5px solid #D97757" : "1.5px solid #E8E4D9",
                outline: "none", background: "#FCFBF8", color: "#2B2A26",
              }}
            />
            {touched.email && !emailValid && (
              <p style={{ fontSize: "12px", color: "#C25B3F", margin: "6px 0 0" }}>
                Ingresá un correo electrónico válido.
              </p>
            )}
          </div>

          {/* Password */}
          <div style={{ marginBottom: "8px" }}>
            <label htmlFor="admin-password" style={{ display: "block", fontSize: "13px", fontWeight: 500, color: "#5C5848", marginBottom: "6px" }}>
              Contraseña
            </label>
            <div style={{ position: "relative" }}>
              <input
                id="admin-password"
                type={showPassword ? "text" : "password"}
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                onBlur={() => setTouched((t) => ({ ...t, password: true }))}
                onFocus={(e) => (e.target.style.borderColor = "#7FB892")}
                placeholder="••••••••"
                style={{
                  width: "100%", boxSizing: "border-box", padding: "11px 44px 11px 14px",
                  fontSize: "14px", borderRadius: "10px",
                  border: touched.password && !passwordValid ? "1.5px solid #D97757" : "1.5px solid #E8E4D9",
                  outline: "none", background: "#FCFBF8", color: "#2B2A26",
                }}
              />
              <button
                type="button"
                aria-label={showPassword ? "Ocultar contraseña" : "Mostrar contraseña"}
                onClick={() => setShowPassword((s) => !s)}
                style={{
                  position: "absolute", right: "10px", top: "50%", transform: "translateY(-50%)",
                  background: "none", border: "none", cursor: "pointer", padding: "4px",
                  display: "flex", color: "#9C988A",
                }}
              >
                {showPassword ? <EyeOff size={17} /> : <Eye size={17} />}
              </button>
            </div>
            {touched.password && !passwordValid && (
              <p style={{ fontSize: "12px", color: "#C25B3F", margin: "6px 0 0" }}>
                La contraseña debe tener al menos 8 caracteres.
              </p>
            )}
          </div>

          {error && (
            <div style={{
              background: "#FBEAE5", border: "1px solid #F0C5B5", borderRadius: "10px",
              padding: "10px 12px", fontSize: "13px", color: "#A1442B", margin: "10px 0 0",
            }}>
              {error}
            </div>
          )}

          <button
            type="submit" disabled={!canSubmit}
            style={{
              width: "100%", marginTop: "22px", padding: "12px 0",
              fontSize: "14px", fontWeight: 600, color: "#FFFFFF",
              background: canSubmit ? "#4F9C68" : "#BFD9C7",
              border: "none", borderRadius: "10px",
              cursor: canSubmit ? "pointer" : "not-allowed",
              display: "flex", alignItems: "center", justifyContent: "center", gap: "8px",
            }}
          >
            {loading ? <><Loader2 size={16} /> Verificando...</> : "Iniciar sesión"}
          </button>
        </form>

        <p style={{ fontSize: "12px", color: "#ACA897", textAlign: "center", marginTop: "22px", marginBottom: 0 }}>
          Esta vista es exclusiva para administradores de la plataforma.
        </p>
      </div>
    </div>
  );
}