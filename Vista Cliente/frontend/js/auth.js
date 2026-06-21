/**
 * auth.js — Autenticación de la Vista Cliente
 *
 * Reemplaza la versión anterior que usaba localStorage con contraseñas
 * en texto plano. Ahora llama a la SQL API real.
 *
 * La sesión se guarda en sessionStorage (se borra al cerrar el navegador).
 *
 * Campos que devuelve LoginResponse (camelCase desde ASP.NET):
 *   idUsuario, nombre, correo, tipoUsuario, token, expiraEn
 */

class Auth {
  /**
   * Login del usuario contra la API real.
   * @param {string} email
   * @param {string} password
   * @returns {Promise<boolean>}
   */
  static async login(email, password) {
    try {
      const res = await apiFetch(ENDPOINTS.auth.login(), {
        method: "POST",
        body: JSON.stringify({ Correo: email, Contrasena: password }),
      });

      // La API devuelve directamente un LoginResponse
      const session = res?.data ?? res;

      if (!session?.token) {
        console.error("Login: respuesta inesperada", res);
        return false;
      }

      // Guardamos la sesión completa (incluye token y datos del usuario)
      setSession(session);

      document.dispatchEvent(new CustomEvent("user-login", { detail: session }));
      return true;
    } catch (err) {
      console.error("Login error:", err.message);
      return false;
    }
  }

  /**
   * Registro de nuevo cliente.
   * Campos requeridos por RegistrarClienteRequest:
   *   Nombre, Apellidos, Edad, FechaNacimiento, Peso, Imc, Pais,
   *   Cintura, Cuello, Caderas, PctMusculo, PctGrasa,
   *   CaloriasDiariasMax (int), Correo, Contrasena
   * @param {object} userData
   * @returns {Promise<boolean>}
   */
  static async register(userData) {
    try {
      const body = {
        Nombre:             userData.nombre,
        Apellidos:          userData.apellidos,
        Edad:               parseInt(userData.edad),
        FechaNacimiento:    userData.fechaNacimiento,  // "YYYY-MM-DD"
        Peso:               parseFloat(userData.peso),
        Imc:                parseFloat(userData.imc),
        Pais:               userData.pais,
        Cintura:            parseFloat(userData.cintura)          || null,
        Cuello:             parseFloat(userData.cuello)           || null,
        Caderas:            parseFloat(userData.caderas)          || null,
        PctMusculo:         parseFloat(userData.porcentajeMusculo) || null,
        PctGrasa:           parseFloat(userData.porcentajeGrasa)  || null,
        CaloriasDiariasMax: parseInt(userData.caloriasDiarias),
        Correo:             userData.email,
        Contrasena:         userData.password,
      };

      await apiFetch(ENDPOINTS.auth.registrarCliente(), {
        method: "POST",
        body: JSON.stringify(body),
      });

      return true;
    } catch (err) {
      console.error("Register error:", err.message);
      return false;
    }
  }

  /**
   * Logout — limpia la sesión.
   */
  static logout() {
    clearSession();
    document.dispatchEvent(new Event("user-logout"));
  }

  /**
   * Devuelve el usuario actual desde sessionStorage.
   * Equivalente al currentUser del app.js original.
   */
  static getCurrentUser() {
    return getSession();
  }

  /**
   * Verifica si hay sesión activa.
   */
  static isAuthenticated() {
    return !!getSession()?.token;
  }
}