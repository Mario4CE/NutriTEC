/**
 * data.js — Capa de datos de la Vista Cliente
 *
 * Unidades de cantidad: gramos (ej: 100g de pinto)
 * Cálculo de calorías: (energia_por_100g * gramos) / 100
 */

class Data {

  // ===========================================================
  // PRODUCTOS — SQL API
  // ===========================================================

  static async buscarProductos(query, tipo = "nombre") {
    try {
      const url = tipo === "nombre"
        ? ENDPOINTS.productos.buscar(query)
        : ENDPOINTS.productos.porCodigo(query);
      const res   = await apiFetch(url);
      const lista = res?.data ?? res ?? [];
      return Array.isArray(lista) ? lista.map(Data._normalizarProducto) : [];
    } catch (err) {
      console.error("buscarProductos error:", err.message);
      return [];
    }
  }

  static async buscarProductosYRecetas(query) {
    try {
      // Buscar productos aprobados
      const productos = await Data.buscarProductos(query, "nombre");

      // Buscar en recetas del usuario
      const session = getSession();
      const recetas = session?.idUsuario ? await Data.getRecetas(null) : [];
      const recetasFiltradas = recetas
        .filter((r) => r.nombre.toLowerCase().includes(query.toLowerCase()))
        .map((r) => ({
          id:            `receta-${r.id}`,
          codigoBarras:  null,
          descripcion:   `🍽️ ${r.nombre} (receta)`,
          porcion:       100,
          energia:       r.energia ?? 0,
          proteina:      0,
          carbohidratos: 0,
          grasa:         0,
          sodio:         null,
          vitaminas:     null,
          calcio:        null,
          hierro:        null,
          esReceta:      true,
          idReceta:      r.id,
          productosReceta: r.productos,
        }));

      return [...productos, ...recetasFiltradas];
    } catch (err) {
      console.error("buscarProductosYRecetas error:", err.message);
      return [];
    }
  }

  static async getProductosAprobados() {
    try {
      const res   = await apiFetch(ENDPOINTS.productos.listar());
      const lista = res?.data ?? res ?? [];
      return Array.isArray(lista) ? lista.map(Data._normalizarProducto) : [];
    } catch (err) {
      console.error("getProductosAprobados error:", err.message);
      return [];
    }
  }

  static async crearProducto(producto) {
    try {
      await apiFetch(ENDPOINTS.productos.crear(), {
        method: "POST",
        body: JSON.stringify({
          Nombre:                  producto.descripcion ?? producto.nombre,
          CodigoBarras:            producto.codigoBarras,
          PorcionGramosMililitros: parseFloat(producto.porcion)    || 100,
          Calorias:                parseFloat(producto.energia),
          Proteinas:               parseFloat(producto.proteina),
          Carbohidratos:           parseFloat(producto.carbohidratos),
          Grasas:                  parseFloat(producto.grasa),
          SodioMiligramos:         parseFloat(producto.sodio)      || null,
          Vitaminas:               producto.vitaminas               || null,
          CalcioMiligramos:        parseFloat(producto.calcio)     || null,
          HierroMiligramos:        parseFloat(producto.hierro)     || null,
        }),
      });
      return true;
    } catch (err) {
      console.error("crearProducto error:", err.message);
      return false;
    }
  }

  static _normalizarProducto(p) {
    return {
      id:            p.id,
      codigoBarras:  p.codigoBarras,
      descripcion:   p.nombre,
      porcion:       p.porcionGramosMililitros ?? 100,
      energia:       p.calorias,
      proteina:      p.proteinas,
      carbohidratos: p.carbohidratos,
      grasa:         p.grasas,
      sodio:         p.sodioMiligramos,
      vitaminas:     p.vitaminas,
      calcio:        p.calcioMiligramos,
      hierro:        p.hierroMiligramos,
      estado:        p.estaAprobado ? "aprobado" : "pendiente",
      esReceta:      false,
    };
  }

  // ===========================================================
  // MEDIDAS
  // ===========================================================

  static async addMedida(userEmail, medida) {
    try {
      const session = getSession();
      if (!session?.idUsuario) return false;
      await apiFetch(ENDPOINTS.medidas.registrar(session.idUsuario), {
        method: "POST",
        body: JSON.stringify({
          Fecha:      new Date().toISOString().split("T")[0],
          Cintura:    parseFloat(medida.cintura)           || null,
          Cuello:     parseFloat(medida.cuello)            || null,
          Caderas:    parseFloat(medida.caderas)           || null,
          PctMusculo: parseFloat(medida.porcentajeMusculo) || null,
          PctGrasa:   parseFloat(medida.porcentajeGrasa)  || null,
        }),
      });
      return true;
    } catch (err) {
      console.error("addMedida error:", err.message);
      return false;
    }
  }

  static async getAllMedidas(userEmail) {
    try {
      const session = getSession();
      if (!session?.idUsuario) return [];
      const res   = await apiFetch(ENDPOINTS.medidas.porUsuario(session.idUsuario));
      const lista = res?.data ?? res ?? [];
      return Array.isArray(lista) ? lista.map((m) => ({
        id:                m.id_medida,
        fecha:             m.fecha,
        cintura:           m.cintura,
        cuello:            m.cuello,
        caderas:           m.caderas,
        porcentajeMusculo: m.pct_musculo,
        porcentajeGrasa:   m.pct_grasa,
      })) : [];
    } catch (err) {
      console.error("getAllMedidas error:", err.message);
      return [];
    }
  }

  static async getMedidasByRange(userEmail, startDate, endDate) {
    const medidas = await Data.getAllMedidas(userEmail);
    const start   = new Date(startDate);
    const end     = new Date(endDate);
    end.setHours(23, 59, 59, 999);
    return medidas.filter((m) => {
      const d = new Date(m.fecha);
      return d >= start && d <= end;
    });
  }

  // ===========================================================
  // CONSUMO DIARIO
  // ===========================================================

  static _agruparRegistros(lista, filtro) {
    const grupos = {};
    lista.forEach((r) => {
      const fechaRegistro = r.fecha?.split("T")[0] ?? r.fecha;
      if (!filtro(fechaRegistro, r)) return;
      if (!grupos[r.id_registro]) {
        grupos[r.id_registro] = {
          id:           r.id_registro,
          fecha:        r.fecha,
          tiempoComida: r.tipo_comida,
          productos:    [],
        };
      }
      if (r.id_producto) {
        grupos[r.id_registro].productos.push({
          productoId: r.id_producto,
          cantidad:   r.cantidad_porciones,
          producto: {
            descripcion:   r.nombre_producto,
            energia:       r.calorias,
            proteina:      r.proteinas,
            carbohidratos: r.carbohidratos,
            grasa:         r.grasas,
            sodio:         r.sodio_mg,
          },
        });
      }
    });
    return Object.values(grupos);
  }

  static async getConsumoHoy(userEmail) {
    try {
      const session = getSession();
      if (!session?.idUsuario) return [];
      const res   = await apiFetch(ENDPOINTS.registros.porUsuario(session.idUsuario));
      const lista = res?.data ?? res ?? [];
      if (!Array.isArray(lista)) return [];
      const hoy = new Date().toLocaleDateString("en-CA");
      return Data._agruparRegistros(lista, (fecha) => fecha === hoy);
    } catch (err) {
      console.error("getConsumoHoy error:", err.message);
      return Data._getConsumosLocal(userEmail).filter(
        (c) => new Date(c.fecha).toDateString() === new Date().toDateString()
      );
    }
  }

  static async getConsumosByFecha(userEmail, fecha) {
    try {
      const session = getSession();
      if (!session?.idUsuario) return [];
      const res   = await apiFetch(ENDPOINTS.registros.porUsuario(session.idUsuario));
      const lista = res?.data ?? res ?? [];
      if (!Array.isArray(lista)) return [];
      return Data._agruparRegistros(lista, (fechaReg) => fechaReg === fecha);
    } catch (err) {
      console.error("getConsumosByFecha error:", err.message);
      return [];
    }
  }

  static async getConsumosByDateRange(userEmail, fechaInicio, fechaFin) {
    try {
      const session = getSession();
      if (!session?.idUsuario) return [];
      const res   = await apiFetch(ENDPOINTS.registros.porUsuario(session.idUsuario));
      const lista = res?.data ?? res ?? [];
      if (!Array.isArray(lista)) return [];
      const inicio = new Date(fechaInicio);
      const fin    = new Date(fechaFin);
      fin.setHours(23, 59, 59, 999);
      return Data._agruparRegistros(lista, (fechaReg) => {
        const d = new Date(fechaReg);
        return d >= inicio && d <= fin;
      });
    } catch (err) {
      console.error("getConsumosByDateRange error:", err.message);
      return [];
    }
  }

  static getNutrientesDia(userEmail, consumos = null) {
    const lista = consumos ?? Data._getConsumosLocal(userEmail).filter(
      (c) => new Date(c.fecha).toDateString() === new Date().toDateString()
    );
    let totales = { calorias: 0, proteinas: 0, grasas: 0, carbohidratos: 0, sodio: 0 };
    lista.forEach((consumo) => {
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

  static async addProductoAlConsumo(userEmail, tiempoComida, productoId, cantidad) {
    try {
      const session = getSession();
      if (!session?.idUsuario) return false;

      // Si es una receta, registrar cada producto de la receta por separado
      if (String(productoId).startsWith("receta-")) {
        const idReceta = String(productoId).replace("receta-", "");
        const recetas  = await Data.getRecetas(null);
        const receta   = recetas.find((r) => String(r.id) === idReceta);
        if (!receta || !receta.productos?.length) return false;

        const hoy = new Date().toLocaleDateString("en-CA");
        for (const prod of receta.productos) {
          await apiFetch(ENDPOINTS.registros.crear(), {
            method: "POST",
            body: JSON.stringify({
              IdUsuario:  parseInt(session.idUsuario),
              Fecha:      hoy,
              TipoComida: tiempoComida,
              Productos:  [{ IdProducto: prod.productoId, CantidadPorciones: prod.cantidad }],
            }),
          });
        }
        return true;
      }

      const hoy = new Date().toLocaleDateString("en-CA");
      const res = await apiFetch(ENDPOINTS.registros.crear(), {
        method: "POST",
        body: JSON.stringify({
          IdUsuario:  parseInt(session.idUsuario),
          Fecha:      hoy,
          TipoComida: tiempoComida,
          Productos:  [{ IdProducto: productoId, CantidadPorciones: cantidad }],
        }),
      });

      const idRegistro = res?.data?.idRegistro ?? res?.idRegistro ?? Date.now();
      const producto   = Data._getProductoCache(productoId);
      Data._addConsumoLocal(userEmail, tiempoComida, idRegistro, productoId, cantidad, producto);
      return true;
    } catch (err) {
      console.error("addProductoAlConsumo error:", err.message);
      return Data._addConsumoLocalFallback(userEmail, tiempoComida, productoId, cantidad);
    }
  }

  static removeProductoDelConsumo(userEmail, consumoId, productoId) {
    const consumos = Data._getConsumosLocal(userEmail);
    const consumo  = consumos.find((c) => c.id == consumoId);
    if (!consumo) return false;
    consumo.productos = consumo.productos.filter((p) => p.productoId != productoId);
    if (consumo.productos.length === 0) consumos.splice(consumos.indexOf(consumo), 1);
    Data._saveConsumosLocal(userEmail, consumos);
    return true;
  }

  static updateCantidadProducto(userEmail, consumoId, productoId, nuevaCantidad) {
    nuevaCantidad = parseInt(nuevaCantidad) || 1;
    const consumos = Data._getConsumosLocal(userEmail);
    const consumo  = consumos.find((c) => c.id == consumoId);
    if (!consumo) return false;
    const prod = consumo.productos.find((p) => p.productoId == productoId);
    if (!prod) return false;
    if (nuevaCantidad <= 0) return Data.removeProductoDelConsumo(userEmail, consumoId, productoId);
    prod.cantidad = nuevaCantidad;
    Data._saveConsumosLocal(userEmail, consumos);
    return true;
  }

  // ===========================================================
  // RECETAS
  // ===========================================================

  static async getRecetas(userEmail) {
    try {
      const session = getSession();
      if (!session?.idUsuario) return [];
      const res   = await apiFetch(ENDPOINTS.recetas.detalle(session.idUsuario));
      const lista = res?.data ?? res ?? [];
      if (!Array.isArray(lista)) return [];

      // Agrupar filas por id_receta (JOIN repite la receta por cada producto)
      const grupos = {};
      lista.forEach((r) => {
        if (!grupos[r.id_receta]) {
          grupos[r.id_receta] = {
            id:        r.id_receta,
            nombre:    r.nombre,
            energia:   r.total_calorias ?? 0,
            productos: [],
          };
        }
        if (r.id_producto) {
          grupos[r.id_receta].productos.push({
            productoId: r.id_producto,
            cantidad:   r.cantidad_porciones,
            producto: {
              descripcion:   r.nombre_producto,
              energia:       r.calorias,
              proteina:      r.proteinas,
              carbohidratos: r.carbohidratos,
              grasa:         r.grasas,
              sodio:         r.sodio_mg,
            },
          });
        }
      });
      return Object.values(grupos);
    } catch (err) {
      console.error("getRecetas error:", err.message);
      return JSON.parse(localStorage.getItem(`recetas_${userEmail}`) || "[]");
    }
  }

  static async saveReceta(userEmail, nombreReceta) {
    if (!nombreReceta || Data._recetaEnEdicion.productos.length === 0) return false;
    try {
      const session = getSession();
      if (!session?.idUsuario) return false;
      await apiFetch(ENDPOINTS.recetas.crear(), {
        method: "POST",
        body: JSON.stringify({
          IdUsuario: parseInt(session.idUsuario),
          Nombre:    nombreReceta,
          Productos: Data._recetaEnEdicion.productos.map((p) => ({
            IdProducto:        p.productoId,
            CantidadPorciones: p.cantidad,
          })),
        }),
      });
      Data._recetaEnEdicion.productos = [];
      return true;
    } catch (err) {
      console.error("saveReceta error:", err.message);
      return false;
    }
  }

  static deleteReceta(userEmail, recetaId) {
    const recetas = JSON.parse(localStorage.getItem(`recetas_${userEmail}`) || "[]")
      .filter((r) => r.id !== recetaId);
    localStorage.setItem(`recetas_${userEmail}`, JSON.stringify(recetas));
    return true;
  }

  // ===========================================================
  // RECETA EN EDICIÓN
  // ===========================================================

  static _recetaEnEdicion = { productos: [] };

  static getProductosRecetaEnEdicion()  { return Data._recetaEnEdicion.productos; }
  static clearRecetaEnEdicion()          { Data._recetaEnEdicion.productos = []; }

  static addProductoARecetaTemporal(productoId, cantidad) {
    const producto = Data._getProductoCache(productoId);
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
  // HELPERS INTERNOS
  // ===========================================================

  static _getConsumosLocal(userEmail) {
    return JSON.parse(localStorage.getItem(`consumos_${userEmail}`) || "[]");
  }

  static _saveConsumosLocal(userEmail, consumos) {
    localStorage.setItem(`consumos_${userEmail}`, JSON.stringify(consumos));
  }

  static _addConsumoLocal(userEmail, tiempoComida, idRegistro, productoId, cantidad, producto) {
    const consumos = Data._getConsumosLocal(userEmail);
    const hoy = new Date().toDateString();
    let consumo = consumos.find(
      (c) => new Date(c.fecha).toDateString() === hoy && c.tiempoComida === tiempoComida
    );
    if (!consumo) {
      consumo = { id: idRegistro, fecha: new Date().toISOString(), tiempoComida, productos: [] };
      consumos.push(consumo);
    }
    const existente = consumo.productos.find((p) => p.productoId == productoId);
    if (existente) { existente.cantidad += cantidad; }
    else { consumo.productos.push({ productoId, cantidad, producto }); }
    Data._saveConsumosLocal(userEmail, consumos);
  }

  static _addConsumoLocalFallback(userEmail, tiempoComida, productoId, cantidad) {
    const producto = Data._getProductoCache(productoId);
    if (!producto) return false;
    Data._addConsumoLocal(userEmail, tiempoComida, Date.now(), productoId, cantidad, producto);
    return true;
  }

  static _getProductoCache(productoId) {
    const cache = JSON.parse(localStorage.getItem("nutritec_productos_cache") || "[]");
    return cache.find((p) => p.id == productoId) ?? null;
  }

  static async cargarCacheProductos() {
    try {
      const productos = await Data.getProductosAprobados();
      localStorage.setItem("nutritec_productos_cache", JSON.stringify(productos));
    } catch (err) {
      console.warn("No se pudo cargar cache de productos:", err.message);
    }
  }

  static initializeTestData() {
    Data.cargarCacheProductos();
  }
}

document.addEventListener("DOMContentLoaded", () => {
  Data.initializeTestData();
});