/**
 * data.js — Capa de datos de la Vista Cliente
 *
 * Reemplaza la versión anterior que guardaba todo en localStorage.
 * Ahora llama a la SQL API (puerto 5255) y la Mongo API (puerto 5272).
 *
 * Productos: se usan los endpoints /productos de la SQL API.
 * Medidas: se registran via sp_RegistrarMedidaUsuario.
 * Consumos y Recetas: estos endpoints aún no están implementados
 *   en el backend, así que por ahora se mantienen en localStorage
 *   como fallback temporal hasta que el compa los implemente.
 *   Están marcados con // TODO: conectar al backend.
 */

class Data {

  // ===========================================================
  // PRODUCTOS — SQL API
  // ===========================================================

  /**
   * Buscar productos aprobados por nombre o código de barras.
   * @param {string} query
   * @param {string} tipo — 'nombre' | 'codigoBarras'
   * @returns {Promise<Array>}
   */
  static async buscarProductos(query, tipo = "nombre") {
    try {
      let url;
      if (tipo === "nombre") {
        url = ENDPOINTS.productos.buscar(query);
      } else {
        url = ENDPOINTS.productos.porCodigo(query);
      }
      const res = await apiFetch(url);
      const lista = res?.data ?? res ?? [];
      // Normalizar campos al formato que espera views.js
      return Array.isArray(lista) ? lista.map(Data._normalizarProducto) : [];
    } catch (err) {
      console.error("buscarProductos error:", err.message);
      return [];
    }
  }

  /**
   * Obtener todos los productos aprobados.
   * @returns {Promise<Array>}
   */
  static async getProductosAprobados() {
    try {
      const res = await apiFetch(ENDPOINTS.productos.listar());
      const lista = res?.data ?? res ?? [];
      return Array.isArray(lista) ? lista.map(Data._normalizarProducto) : [];
    } catch (err) {
      console.error("getProductosAprobados error:", err.message);
      return [];
    }
  }

  /**
   * Crear un producto nuevo (queda pendiente de aprobación).
   * @param {object} producto
   * @returns {Promise<boolean>}
   */
  static async crearProducto(producto) {
    try {
      await apiFetch(ENDPOINTS.productos.crear(), {
        method: "POST",
        body: JSON.stringify({
          Nombre:                  producto.descripcion ?? producto.nombre,
          CodigoBarras:            producto.codigoBarras,
          PorcionGramosMililitros: parseFloat(producto.porcion) || 100,
          Calorias:                parseFloat(producto.energia),
          Proteinas:               parseFloat(producto.proteina),
          Carbohidratos:           parseFloat(producto.carbohidratos),
          Grasas:                  parseFloat(producto.grasa),
          SodioMiligramos:         parseFloat(producto.sodio)   || null,
          Vitaminas:               producto.vitaminas            || null,
          CalcioMiligramos:        parseFloat(producto.calcio)  || null,
          HierroMiligramos:        parseFloat(producto.hierro)  || null,
        }),
      });
      return true;
    } catch (err) {
      console.error("crearProducto error:", err.message);
      return false;
    }
  }

  /**
   * Normaliza un ProductoResponse del backend al formato
   * que ya usa views.js (campos como descripcion, energia, etc.)
   */
  static _normalizarProducto(p) {
    return {
      id:            p.id,
      codigoBarras:  p.codigoBarras,
      descripcion:   p.nombre,           // views.js usa "descripcion"
      porcion:       p.porcionGramosMililitros ?? 100,
      energia:       p.calorias,         // views.js usa "energia"
      proteina:      p.proteinas,        // views.js usa "proteina"
      carbohidratos: p.carbohidratos,
      grasa:         p.grasas,           // views.js usa "grasa"
      sodio:         p.sodioMiligramos,
      vitaminas:     p.vitaminas,
      calcio:        p.calcioMiligramos,
      hierro:        p.hierroMiligramos,
      estado:        p.estaAprobado ? "aprobado" : "pendiente",
    };
  }

  // ===========================================================
  // MEDIDAS — SP sp_RegistrarMedidaUsuario via SQL API
  // ===========================================================

  /**
   * Registrar una medida corporal.
   * Usa el SP sp_RegistrarMedidaUsuario directamente.
   * @param {string} userEmail — no usado directamente, el idUsuario
   *   viene de la sesión activa.
   * @param {object} medida — { cintura, cuello, caderas,
   *   porcentajeMusculo, porcentajeGrasa }
   * @returns {Promise<boolean>}
   */
  static async addMedida(userEmail, medida) {
    try {
      const session = getSession();
      if (!session?.idUsuario) return false;

      await apiFetch(ENDPOINTS.sp.registrarMedida(), {
        method: "POST",
        body: JSON.stringify({
          IdUsuario:   parseInt(session.idUsuario),
          Fecha:       new Date().toISOString().split("T")[0],
          PesoKg:      parseFloat(session.peso ?? 0),
          EstaturaCm:  parseFloat(session.estaturaCm ?? 170), // fallback
          Cintura:     parseFloat(medida.cintura)             || null,
          Cuello:      parseFloat(medida.cuello)              || null,
          Caderas:     parseFloat(medida.caderas)             || null,
          PctMusculo:  parseFloat(medida.porcentajeMusculo)   || null,
          PctGrasa:    parseFloat(medida.porcentajeGrasa)     || null,
        }),
      });

      // También guardamos localmente para historial inmediato
      Data._addMedidaLocal(userEmail, medida);
      return true;
    } catch (err) {
      console.error("addMedida error:", err.message);
      return false;
    }
  }

  /**
   * Obtener medidas del historial local (fallback hasta que
   * el backend tenga endpoint GET /medidas/usuario/:id).
   */
  static getAllMedidas(userEmail) {
    return Data._getMedidasLocal(userEmail);
  }

  static getMedidasByRange(userEmail, startDate, endDate) {
    const medidas = Data._getMedidasLocal(userEmail);
    const start = new Date(startDate);
    const end   = new Date(endDate);
    return medidas.filter((m) => {
      const d = new Date(m.fecha);
      return d >= start && d <= end;
    });
  }

  // ===========================================================
  // CONSUMOS — localStorage temporal
  // TODO: conectar a backend cuando el compa implemente los endpoints
  // ===========================================================

  static getConsumoHoy(userEmail) {
    const consumos = Data._getConsumosLocal(userEmail);
    const hoy = new Date().toDateString();
    return consumos.filter((c) => new Date(c.fecha).toDateString() === hoy);
  }

  static getConsumosByFecha(userEmail, fecha) {
    const consumos = Data._getConsumosLocal(userEmail);
    return consumos.filter(
      (c) => new Date(c.fecha).toISOString().split("T")[0] === fecha
    );
  }

  static getConsumosByDateRange(userEmail, fechaInicio, fechaFin) {
    const consumos = Data._getConsumosLocal(userEmail);
    const inicio = new Date(fechaInicio);
    const fin    = new Date(fechaFin);
    fin.setHours(23, 59, 59, 999);
    return consumos.filter((c) => {
      const d = new Date(c.fecha);
      return d >= inicio && d <= fin;
    });
  }

  static getNutrientesDia(userEmail, fecha = null) {
    const consumos = fecha
      ? Data.getConsumosByFecha(userEmail, fecha)
      : Data.getConsumoHoy(userEmail);

    let totales = { calorias: 0, proteinas: 0, grasas: 0, carbohidratos: 0, sodio: 0 };

    consumos.forEach((consumo) => {
      (consumo.productos ?? []).forEach((prod) => {
        if (prod.producto) {
          const cant = prod.cantidad || 100;
          totales.calorias      += (prod.producto.energia       || 0) * (cant / 100);
          totales.proteinas     += (prod.producto.proteina      || 0) * (cant / 100);
          totales.grasas        += (prod.producto.grasa         || 0) * (cant / 100);
          totales.carbohidratos += (prod.producto.carbohidratos || 0) * (cant / 100);
          totales.sodio         += (prod.producto.sodio         || 0) * (cant / 100);
        }
      });
    });

    return totales;
  }

  static addProductoAlConsumo(userEmail, tiempoComida, productoId, cantidad) {
    const consumos = Data._getConsumosLocal(userEmail);
    const hoy      = new Date().toISOString().split("T")[0];

    let consumo = consumos.find((c) => {
      return new Date(c.fecha).toISOString().split("T")[0] === hoy
        && c.tiempoComida === tiempoComida;
    });

    if (!consumo) {
      consumo = {
        id: Date.now().toString(),
        fecha: new Date().toISOString(),
        tiempoComida,
        productos: [],
      };
      consumos.push(consumo);
    }

    const producto = Data._getProductoLocal(productoId);
    if (!producto) return false;

    const existente = consumo.productos.find((p) => p.productoId == productoId);
    if (existente) {
      existente.cantidad += cantidad;
    } else {
      consumo.productos.push({ productoId, cantidad, producto });
    }

    Data._saveConsumosLocal(userEmail, consumos);
    return true;
  }

  static removeProductoDelConsumo(userEmail, consumoId, productoId) {
    const consumos = Data._getConsumosLocal(userEmail);
    const consumo  = consumos.find((c) => c.id === consumoId);
    if (!consumo) return false;
    consumo.productos = consumo.productos.filter((p) => p.productoId != productoId);
    if (consumo.productos.length === 0) {
      const idx = consumos.indexOf(consumo);
      consumos.splice(idx, 1);
    }
    Data._saveConsumosLocal(userEmail, consumos);
    return true;
  }

  static updateCantidadProducto(userEmail, consumoId, productoId, nuevaCantidad) {
    nuevaCantidad = parseInt(nuevaCantidad) || 1;
    const consumos = Data._getConsumosLocal(userEmail);
    const consumo  = consumos.find((c) => c.id === consumoId);
    if (!consumo) return false;
    const prod = consumo.productos.find((p) => p.productoId == productoId);
    if (!prod) return false;
    if (nuevaCantidad <= 0) return Data.removeProductoDelConsumo(userEmail, consumoId, productoId);
    prod.cantidad = nuevaCantidad;
    Data._saveConsumosLocal(userEmail, consumos);
    return true;
  }

  // ===========================================================
  // RECETAS — localStorage temporal
  // TODO: conectar a backend cuando el compa implemente los endpoints
  // ===========================================================

  static getRecetas(userEmail) {
    const key = `recetas_${userEmail}`;
    return JSON.parse(localStorage.getItem(key) || "[]");
  }

  static saveReceta(userEmail, nombreReceta) {
    if (!nombreReceta || Data._recetaEnEdicion.productos.length === 0) return false;
    const recetas = Data.getRecetas(userEmail);
    recetas.push({
      id: Date.now().toString(),
      nombre: nombreReceta,
      fechaCreacion: new Date().toISOString(),
      productos: [...Data._recetaEnEdicion.productos],
    });
    localStorage.setItem(`recetas_${userEmail}`, JSON.stringify(recetas));
    Data._recetaEnEdicion.productos = [];
    return true;
  }

  static deleteReceta(userEmail, recetaId) {
    const recetas = Data.getRecetas(userEmail).filter((r) => r.id !== recetaId);
    localStorage.setItem(`recetas_${userEmail}`, JSON.stringify(recetas));
    return true;
  }

  // ===========================================================
  // RECETA EN EDICIÓN (temporal en memoria)
  // ===========================================================

  static _recetaEnEdicion = { productos: [] };

  static getProductosRecetaEnEdicion()   { return Data._recetaEnEdicion.productos; }
  static clearRecetaEnEdicion()           { Data._recetaEnEdicion.productos = []; }

  static addProductoARecetaTemporal(productoId, cantidad) {
    const producto = Data._getProductoLocal(productoId);
    if (!producto) return false;
    const existente = Data._recetaEnEdicion.productos.find((p) => p.productoId == productoId);
    if (existente) { existente.cantidad += cantidad; }
    else { Data._recetaEnEdicion.productos.push({ productoId, cantidad, producto }); }
    return true;
  }

  static removeProductoDeRecetaTemporal(productoId) {
    Data._recetaEnEdicion.productos = Data._recetaEnEdicion.productos.filter(
      (p) => p.productoId != productoId
    );
  }

  static updateCantidadProductoRecetaTemporal(productoId, nuevaCantidad) {
    if (nuevaCantidad <= 0) { Data.removeProductoDeRecetaTemporal(productoId); return true; }
    const prod = Data._recetaEnEdicion.productos.find((p) => p.productoId == productoId);
    if (prod) { prod.cantidad = nuevaCantidad; return true; }
    return false;
  }

  static calcularNutrientesRecetaTemporal() {
    let totales = { calorias: 0, proteinas: 0, grasas: 0, carbohidratos: 0, sodio: 0 };
    Data._recetaEnEdicion.productos.forEach((prod) => {
      if (prod.producto) {
        const cant = prod.cantidad || 1;
        totales.calorias      += (prod.producto.energia       || 0) * cant;
        totales.proteinas     += (prod.producto.proteina      || 0) * cant;
        totales.grasas        += (prod.producto.grasa         || 0) * cant;
        totales.carbohidratos += (prod.producto.carbohidratos || 0) * cant;
        totales.sodio         += (prod.producto.sodio         || 0) * cant;
      }
    });
    return totales;
  }

  // ===========================================================
  // USUARIO
  // ===========================================================

  static updateUser(updatedUser) {
    const session = getSession();
    if (!session) return false;
    setSession({ ...session, ...updatedUser });
    return true;
  }

  // ===========================================================
  // HELPERS INTERNOS (localStorage)
  // ===========================================================

  static _getConsumosLocal(userEmail) {
    return JSON.parse(localStorage.getItem(`consumos_${userEmail}`) || "[]");
  }

  static _saveConsumosLocal(userEmail, consumos) {
    localStorage.setItem(`consumos_${userEmail}`, JSON.stringify(consumos));
  }

  static _getMedidasLocal(userEmail) {
    return JSON.parse(localStorage.getItem(`medidas_${userEmail}`) || "[]");
  }

  static _addMedidaLocal(userEmail, medida) {
    const medidas = Data._getMedidasLocal(userEmail);
    medidas.unshift({ id: Date.now().toString(), fecha: new Date().toISOString(), ...medida });
    localStorage.setItem(`medidas_${userEmail}`, JSON.stringify(medidas));
  }

  static _getProductoLocal(productoId) {
    const productos = JSON.parse(localStorage.getItem("nutritec_productos_cache") || "[]");
    return productos.find((p) => p.id == productoId) ?? null;
  }

  /**
   * Cachear productos de la API en localStorage para búsquedas offline
   * y para el selector de consumo/recetas.
   */
  static async cargarCacheProductos() {
    try {
      const productos = await Data.getProductosAprobados();
      localStorage.setItem("nutritec_productos_cache", JSON.stringify(productos));
    } catch (err) {
      console.warn("No se pudo cargar cache de productos:", err.message);
    }
  }

  /**
   * Inicializar datos de prueba — ya no crea usuarios falsos,
   * solo carga el cache de productos desde la API.
   */
  static initializeTestData() {
    Data.cargarCacheProductos();
  }
}

// Cargar productos al iniciar
document.addEventListener("DOMContentLoaded", () => {
  Data.initializeTestData();
});