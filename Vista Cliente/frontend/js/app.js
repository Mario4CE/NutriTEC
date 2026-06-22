/**
 * app.js - Controlador principal de la aplicación NutriTEC
 */

class NutriTECApp {
  constructor() {
    this.currentUser = null;
    this.currentView = "login";
    this.appContainer = document.getElementById("app");
    this.init();
  }

  init() {
    this.currentUser = this.getStoredUser();
    if (this.currentUser) {
      this.showView("dashboard");
    } else {
      this.showView("login");
    }

    document.addEventListener("user-login", (e) => {
      this.currentUser = e.detail;
      this.showView("dashboard");
    });

    document.addEventListener("user-logout", () => {
      this.currentUser = null;
      this.showView("login");
    });
  }

  async showView(viewName) {
    this.currentView = viewName;

    switch (viewName) {
      case "login":
        this.appContainer.innerHTML = Views.getLoginView();
        this.setupLoginHandlers();
        break;

      case "register":
        this.appContainer.innerHTML = Views.getRegisterView();
        this.setupRegisterHandlers();
        break;

      case "dashboard": {
        const email           = this.currentUser.correo ?? this.currentUser.email;
        const consumoHoy      = await Data.getConsumoHoy(email);
        const medidasRecientes = await Data.getAllMedidas(email);
        const nutrientesHoy   = Data.getNutrientesDia(email, consumoHoy);
        this.appContainer.innerHTML = Views.getDashboardView(this.currentUser, consumoHoy, medidasRecientes, nutrientesHoy);
        this.setupDashboardHandlers();
        break;
      }

      case "registro-consumo": {
        const emailC      = this.currentUser.correo ?? this.currentUser.email;
        const consumoHoyC = await Data.getConsumoHoy(emailC);
        this.appContainer.innerHTML = Views.getRegistroConsumoView(this.currentUser, consumoHoyC);
        await this.setupRegistroConsumoHandlers();
        break;
      }

      case "registro-medidas":
        this.appContainer.innerHTML = Views.getRegistroMedidasView(this.currentUser);
        await this.setupRegistroMedidasHandlers();
        break;

      case "perfil":
        this.appContainer.innerHTML = Views.getPerfilView(this.currentUser);
        this.setupPerfilHandlers();
        break;

      case "recetas":
        this.appContainer.innerHTML = await Views.getRecetasView(this.currentUser);
        this.setupRecetasHandlers();
        break;

      case "reporte":
        this.appContainer.innerHTML = Views.getReporteView(this.currentUser);
        this.setupReporteHandlers();
        break;

      default:
        this.showView("login");
    }
  }

  // ── Login ─────────────────────────────────────────────────

  setupLoginHandlers() {
    const form = document.getElementById("loginForm");
    const switchToRegisterBtn = document.getElementById("switchToRegister");

    if (form) {
      form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const email    = document.getElementById("email").value;
        const password = document.getElementById("password").value;
        const ok = await Auth.login(email, password);
        if (!ok) this.showAlert("Correo o contraseña inválidos", "danger");
      });
    }

    if (switchToRegisterBtn) {
      switchToRegisterBtn.addEventListener("click", () => this.showView("register"));
    }
  }

  // ── Registro ──────────────────────────────────────────────

  setupRegisterHandlers() {
    const form = document.getElementById("registerForm");
    const switchToLoginBtn = document.getElementById("switchToLogin");

    if (form) {
      form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const userData = {
          nombre:            document.getElementById("nombre").value,
          apellidos:         document.getElementById("apellidos").value,
          email:             document.getElementById("email").value,
          password:          document.getElementById("password").value,
          confirmPassword:   document.getElementById("confirmPassword").value,
          edad:              document.getElementById("edad").value,
          fechaNacimiento:   document.getElementById("fechaNacimiento").value,
          peso:              document.getElementById("peso").value,
          imc:               document.getElementById("imc").value,
          pais:              document.getElementById("pais").value,
          cintura:           document.getElementById("cintura").value,
          cuello:            document.getElementById("cuello").value,
          caderas:           document.getElementById("caderas").value,
          porcentajeMusculo: document.getElementById("porcentajeMusculo").value,
          porcentajeGrasa:   document.getElementById("porcentajeGrasa").value,
          caloriasDiarias:   document.getElementById("caloriasDiarias").value,
        };

        if (userData.password !== userData.confirmPassword) {
          this.showAlert("Las contraseñas no coinciden", "danger");
          return;
        }

        const ok = await Auth.register(userData);
        if (ok) {
          this.showAlert("Registro exitoso. Por favor, inicia sesión.", "success");
          setTimeout(() => this.showView("login"), 2000);
        } else {
          this.showAlert("Error en el registro. El correo ya existe o hay datos inválidos.", "danger");
        }
      });
    }

    if (switchToLoginBtn) {
      switchToLoginBtn.addEventListener("click", () => this.showView("login"));
    }
  }

  // ── Dashboard ─────────────────────────────────────────────

  setupDashboardHandlers() {
    document.querySelectorAll("[data-view]").forEach((btn) => {
      btn.addEventListener("click", () => this.showView(btn.dataset.view));
    });

    const logoutBtn = document.getElementById("logoutBtn");
    if (logoutBtn) {
      logoutBtn.addEventListener("click", () => Auth.logout());
    }
  }

  // ── Registro de consumo ───────────────────────────────────

  async setupRegistroConsumoHandlers() {
    const searchInput    = document.getElementById("searchProducto");
    const productosLista = document.getElementById("productosLista");

    if (searchInput) {
      let debounceTimer;
      searchInput.addEventListener("input", (e) => {
        clearTimeout(debounceTimer);
        const query = e.target.value.trim();

        if (query.length === 0) {
          productosLista.innerHTML = `
            <div class="text-muted text-center py-4">
              <i class="fas fa-search" style="font-size:2rem;"></i>
              <p>Busca un producto para comenzar</p>
            </div>`;
          return;
        }

        debounceTimer = setTimeout(async () => {
          productosLista.innerHTML = `<div class="text-center py-3"><div class="spinner-border spinner-border-sm text-success"></div></div>`;

          let resultados = await Data.buscarProductosYRecetas(query);
          if (resultados.length === 0 && !isNaN(query)) {
            resultados = await Data.buscarProductos(query, "codigoBarras");
          }

          const cache = JSON.parse(localStorage.getItem("nutritec_productos_cache") || "[]");
          resultados.forEach((p) => { if (!cache.find((c) => c.id === p.id)) cache.push(p); });
          localStorage.setItem("nutritec_productos_cache", JSON.stringify(cache));

          if (resultados.length === 0) {
            productosLista.innerHTML = `
              <div class="alert alert-warning text-center">
                <i class="fas fa-info-circle"></i> No se encontraron productos
              </div>`;
          } else {
            productosLista.innerHTML = `
              <div class="list-group">
                ${resultados.map((p) => `
                  <div class="list-group-item list-group-item-action">
                    <div class="d-flex w-100 justify-content-between align-items-start">
                      <div style="flex-grow:1;">
                        <h6 class="mb-1">${p.descripcion}</h6>
                        <small class="text-muted">${p.esReceta ? `🍽️ Receta · ${p.energia} kcal` : `Código: ${p.codigoBarras} | Kcal: ${p.energia}`}</small>
                      </div>
                      <div style="margin-left:10px;">
                        <input type="number" min="1" max="9999" value="100"
                          class="form-control form-control-sm"
                          style="width:70px;display:inline-block;"
                          id="cant-${p.id}">
                        <button class="btn btn-sm btn-success ms-2"
                          onclick="window.app.agregarProductoAlConsumo('${p.id}')">
                          <i class="fas fa-plus"></i>
                        </button>
                      </div>
                    </div>
                  </div>`).join("")}
              </div>`;
          }
        }, 350);
      });
    }

    await this.actualizarTablaConsumo();
  }

  async agregarProductoAlConsumo(productoId) {
    const tiempoComida  = document.getElementById("tiempoComida").value;
    const cantidadInput = document.getElementById(`cant-${productoId}`);
    const cantidad      = parseInt(cantidadInput?.value) || 100;

    if (!tiempoComida) {
      this.showAlert("Por favor selecciona un tiempo de comida", "warning");
      return;
    }

    const ok = await Data.addProductoAlConsumo(
      this.currentUser.correo ?? this.currentUser.email,
      tiempoComida, productoId, cantidad
    );

    if (ok) {
      this.showAlert(`Producto agregado al ${tiempoComida}`, "success");
      document.getElementById("searchProducto").value = "";
      document.getElementById("productosLista").innerHTML = `
        <div class="text-muted text-center py-4">
          <i class="fas fa-search" style="font-size:2rem;"></i>
          <p>Busca un producto para comenzar</p>
        </div>`;
      await this.actualizarTablaConsumo();
    } else {
      this.showAlert("Error al agregar producto", "danger");
    }
  }

  async eliminarProductoDelConsumo(consumoId, productoId) {
    const email = this.currentUser.correo ?? this.currentUser.email;
    if (Data.removeProductoDelConsumo(email, consumoId, productoId)) {
      this.showAlert("Producto eliminado", "info");
      await this.actualizarTablaConsumo();
    }
  }

  async actualizarTablaConsumo() {
    const email      = this.currentUser.correo ?? this.currentUser.email;
    const consumoHoy = await Data.getConsumoHoy(email);
    const tableBody  = document.getElementById("consumoTableBody");
    const nutrientes = Data.getNutrientesDia(email, consumoHoy);
    const metaDiaria = this.currentUser.caloriasDiariasMax ?? 2000;
    const porcentaje = Math.min((nutrientes.calorias / metaDiaria) * 100, 100);

    if (!tableBody) return;

    if (consumoHoy.length === 0) {
      tableBody.innerHTML = '<tr><td colspan="4" class="text-center text-muted py-3">No hay productos agregados</td></tr>';
    } else {
      let html = "";
      consumoHoy.forEach((consumo) => {
        (consumo.productos ?? []).forEach((prod) => {
          const kcal = ((prod.producto.energia ?? 0) * prod.cantidad) / 100;
          html += `
            <tr>
              <td><small>${prod.producto.descripcion}</small></td>
              <td>
                <input type="number" min="1" max="9999" value="${prod.cantidad}"
                  class="form-control form-control-sm" style="width:70px;"
                  onchange="window.app.updateCantidadProducto('${consumo.id}', '${prod.productoId}', this.value)">
              </td>
              <td>${kcal.toFixed(0)}</td>
              <td>
                <button class="btn btn-sm btn-danger"
                  onclick="window.app.eliminarProductoDelConsumo('${consumo.id}', '${prod.productoId}')">
                  <i class="fas fa-trash"></i>
                </button>
              </td>
            </tr>`;
        });
      });
      tableBody.innerHTML = html;
    }

    const panel = document.querySelector(".card.sticky-top .card-body");
    if (panel) {
      panel.innerHTML = `
        <div class="mb-4">
          <div class="progress" style="height:30px;">
            <div class="progress-bar bg-success" role="progressbar"
              style="width:${porcentaje}%;">
              <small>${porcentaje.toFixed(0)}%</small>
            </div>
          </div>
          <small class="text-muted">${nutrientes.calorias.toFixed(0)} / ${metaDiaria} kcal</small>
        </div>
        <div class="mb-2"><div class="d-flex justify-content-between"><strong>Proteínas:</strong><span>${nutrientes.proteinas.toFixed(1)}g</span></div></div>
        <div class="mb-2"><div class="d-flex justify-content-between"><strong>Grasas:</strong><span>${nutrientes.grasas.toFixed(1)}g</span></div></div>
        <div class="mb-2"><div class="d-flex justify-content-between"><strong>Carbohidratos:</strong><span>${nutrientes.carbohidratos.toFixed(1)}g</span></div></div>
        <hr>
        <div class="mb-2"><div class="d-flex justify-content-between"><strong>Sodio:</strong><span>${nutrientes.sodio.toFixed(0)}mg</span></div></div>
        <hr>
        <div class="alert alert-info" style="font-size:0.9rem;">
          <i class="fas fa-info-circle"></i>
          <strong>Total Registros:</strong> ${consumoHoy.length}
        </div>`;
    }
  }

  async updateCantidadProducto(consumoId, productoId, nuevaCantidad) {
    const email = this.currentUser.correo ?? this.currentUser.email;
    if (Data.updateCantidadProducto(email, consumoId, productoId, parseInt(nuevaCantidad) || 1)) {
      await this.actualizarTablaConsumo();
    }
  }

  // ── Registro de medidas ───────────────────────────────────

  async setupRegistroMedidasHandlers() {
    await this._renderMedidasTable();

    const form = document.getElementById("medidasForm");
    if (form) {
      form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const email  = this.currentUser.correo ?? this.currentUser.email;
        const medida = {
          cintura:           document.getElementById("cintura").value,
          cuello:            document.getElementById("cuello").value,
          caderas:           document.getElementById("caderas").value,
          porcentajeMusculo: document.getElementById("porcentajeMusculo").value,
          porcentajeGrasa:   document.getElementById("porcentajeGrasa").value,
        };

        const ok = await Data.addMedida(email, medida);
        if (ok) {
          this.showAlert("Medidas registradas exitosamente", "success");
          form.reset();
          await this._renderMedidasTable();
        } else {
          this.showAlert("Error al registrar medidas", "danger");
        }
      });
    }
  }

  async _renderMedidasTable() {
    const email   = this.currentUser.correo ?? this.currentUser.email;
    const medidas = await Data.getAllMedidas(email);
    const tbody   = document.getElementById("medidasTable");
    if (!tbody) return;

    if (medidas.length === 0) {
      tbody.innerHTML = '<tr><td colspan="6" class="text-center text-muted py-3">No hay medidas registradas</td></tr>';
    } else {
      tbody.innerHTML = medidas.map((m) => `
        <tr>
          <td>${new Date(m.fecha).toLocaleDateString("es-CR")}</td>
          <td>${m.cintura ?? "—"}</td>
          <td>${m.cuello ?? "—"}</td>
          <td>${m.caderas ?? "—"}</td>
          <td>${m.porcentajeMusculo ?? "—"}%</td>
          <td>${m.porcentajeGrasa ?? "—"}%</td>
        </tr>`).join("");
    }
  }

  // ── Perfil ────────────────────────────────────────────────

  setupPerfilHandlers() {
    const editBtn = document.getElementById("editPerfilBtn");
    const form    = document.getElementById("perfilForm");

    if (editBtn) {
      editBtn.addEventListener("click", () => form.classList.toggle("hidden"));
    }

    if (form) {
      form.addEventListener("submit", (e) => {
        e.preventDefault();
        this.currentUser.nombre    = document.getElementById("nombre").value;
        this.currentUser.apellidos = document.getElementById("apellidos").value;
        this.currentUser.peso      = document.getElementById("peso").value;
        this.currentUser.imc       = document.getElementById("imc").value;

        if (Data.updateUser(this.currentUser)) {
          this.showAlert("Perfil actualizado exitosamente", "success");
          this.showView("perfil");
        } else {
          this.showAlert("Error al actualizar perfil", "danger");
        }
      });
    }
  }

  // ── Recetas ───────────────────────────────────────────────

  setupRecetasHandlers() {
    const searchInput    = document.getElementById("searchProductoReceta");
    const productosLista = document.getElementById("productosListaReceta");

    if (searchInput) {
      let debounceTimer;
      searchInput.addEventListener("input", (e) => {
        clearTimeout(debounceTimer);
        const query = e.target.value.trim();

        if (query.length === 0) {
          productosLista.innerHTML = `
            <div class="text-muted text-center py-3">
              <i class="fas fa-search" style="font-size:1.5rem;"></i>
              <p>Busca un producto para comenzar</p>
            </div>`;
          return;
        }

        debounceTimer = setTimeout(async () => {
          let resultados = await Data.buscarProductosYRecetas(query);
          if (resultados.length === 0 && !isNaN(query)) {
            resultados = await Data.buscarProductos(query, "codigoBarras");
          }

          const cache = JSON.parse(localStorage.getItem("nutritec_productos_cache") || "[]");
          resultados.forEach((p) => { if (!cache.find((c) => c.id === p.id)) cache.push(p); });
          localStorage.setItem("nutritec_productos_cache", JSON.stringify(cache));

          if (resultados.length === 0) {
            productosLista.innerHTML = `<div class="alert alert-warning text-center"><i class="fas fa-info-circle"></i> No se encontraron productos</div>`;
          } else {
            productosLista.innerHTML = `
              <div class="list-group">
                ${resultados.map((p) => `
                  <div class="list-group-item list-group-item-action">
                    <div class="d-flex w-100 justify-content-between align-items-start">
                      <div style="flex-grow:1;">
                        <h6 class="mb-1">${p.descripcion}</h6>
                        <small class="text-muted">${p.energia} kcal</small>
                      </div>
                      <div style="margin-left:10px;">
                        <input type="number" min="1" max="9999" value="1"
                          class="form-control form-control-sm"
                          style="width:70px;display:inline-block;"
                          id="cant-receta-${p.id}">
                        <button class="btn btn-sm btn-success ms-2"
                          onclick="window.app.agregarProductoAReceta('${p.id}')">
                          <i class="fas fa-plus"></i>
                        </button>
                      </div>
                    </div>
                  </div>`).join("")}
              </div>`;
          }
        }, 350);
      });
    }

    this.actualizarTablaReceta();
  }

  agregarProductoAReceta(productoId) {
    const cantidadInput = document.getElementById(`cant-receta-${productoId}`);
    const cantidad      = parseInt(cantidadInput?.value) || 1;

    if (Data.addProductoARecetaTemporal(productoId, cantidad)) {
      this.showAlert("Producto agregado a receta", "success");
      document.getElementById("searchProductoReceta").value = "";
      document.getElementById("productosListaReceta").innerHTML = `
        <div class="text-muted text-center py-3">
          <i class="fas fa-search" style="font-size:1.5rem;"></i>
          <p>Busca un producto para comenzar</p>
        </div>`;
      this.actualizarTablaReceta();
    } else {
      this.showAlert("Error al agregar producto", "danger");
    }
  }

  eliminarProductoDeReceta(productoId) {
    Data.removeProductoDeRecetaTemporal(productoId);
    this.showAlert("Producto eliminado de receta", "info");
    this.actualizarTablaReceta();
  }

  actualizarTablaReceta() {
    const productos  = Data.getProductosRecetaEnEdicion();
    const tableBody  = document.getElementById("recetaTableBody");
    const nutrientes = Data.calcularNutrientesRecetaTemporal();
    const panel      = document.getElementById("resumenNutrientesReceta");

    if (!tableBody) return;

    if (productos.length === 0) {
      tableBody.innerHTML = '<tr><td colspan="3" class="text-center text-muted py-2">No hay productos</td></tr>';
    } else {
      tableBody.innerHTML = productos.map((prod) => `
        <tr>
          <td><small>${prod.producto.descripcion}</small></td>
          <td>
            <input type="number" min="1" max="9999" value="${prod.cantidad}"
              class="form-control form-control-sm" style="width:70px;"
              onchange="window.app.updateCantidadProductoReceta('${prod.productoId}', this.value)">
          </td>
          <td>
            <button class="btn btn-sm btn-danger"
              onclick="window.app.eliminarProductoDeReceta('${prod.productoId}')">
              <i class="fas fa-trash"></i>
            </button>
          </td>
        </tr>`).join("");
    }

    if (panel) {
      if (productos.length === 0) {
        panel.innerHTML = `<div class="alert alert-info text-center"><small>Agrega productos para ver nutrientes</small></div>`;
      } else {
        panel.innerHTML = `
          <div class="mb-2"><div class="d-flex justify-content-between"><strong>Calorías:</strong><span>${nutrientes.calorias.toFixed(0)}</span></div></div>
          <div class="mb-2"><div class="d-flex justify-content-between"><strong>Proteínas:</strong><span>${nutrientes.proteinas.toFixed(1)}g</span></div></div>
          <div class="mb-2"><div class="d-flex justify-content-between"><strong>Grasas:</strong><span>${nutrientes.grasas.toFixed(1)}g</span></div></div>
          <div class="mb-2"><div class="d-flex justify-content-between"><strong>Carbohidratos:</strong><span>${nutrientes.carbohidratos.toFixed(1)}g</span></div></div>
          <hr>
          <div class="mb-2"><div class="d-flex justify-content-between"><strong>Sodio:</strong><span>${nutrientes.sodio.toFixed(0)}mg</span></div></div>`;
      }
    }
  }

  updateCantidadProductoReceta(productoId, nuevaCantidad) {
    if (Data.updateCantidadProductoRecetaTemporal(productoId, parseInt(nuevaCantidad) || 1)) {
      this.actualizarTablaReceta();
    }
  }

  async guardarReceta() {
    const nombreReceta = document.getElementById("nombreReceta")?.value.trim();
    const email        = this.currentUser.correo ?? this.currentUser.email;

    if (!nombreReceta) {
      this.showAlert("Por favor ingresa un nombre para la receta", "warning");
      return;
    }

    const ok = await Data.saveReceta(email, nombreReceta);
    if (ok) {
      this.showAlert("Receta guardada exitosamente", "success");
      document.getElementById("nombreReceta").value = "";
      this.actualizarTablaReceta();
      await this.showView("recetas");
    } else {
      this.showAlert("Error al guardar receta o no hay productos", "danger");
    }
  }

  // ── Reporte ───────────────────────────────────────────────

  setupReporteHandlers() {
    const generateBtn    = document.getElementById("generateReportBtn");
    const generatePDFBtn = document.getElementById("generatePDFBtn");

    if (generateBtn) {
      generateBtn.addEventListener("click", async () => {
        const startDate = document.getElementById("startDate").value;
        const endDate   = document.getElementById("endDate").value;
        if (!startDate || !endDate) {
          this.showAlert("Por favor selecciona rango de fechas", "warning");
          return;
        }
        await this.showReporteData(startDate, endDate);
      });
    }

    if (generatePDFBtn) {
      generatePDFBtn.addEventListener("click", () => {
        const startDate = document.getElementById("startDate").value;
        const endDate   = document.getElementById("endDate").value;
        const element   = document.getElementById("reporteData");

        if (!startDate || !endDate) {
          this.showAlert("Por favor selecciona rango de fechas primero", "warning");
          return;
        }
        if (!element || !element.innerHTML.trim()) {
          this.showAlert("Primero genera el reporte antes de descargar PDF", "warning");
          return;
        }

        const clonedElement = element.cloneNode(true);
        const title         = document.createElement("h3");
        title.textContent   = "Reporte de Nutrición NutriTEC";
        clonedElement.insertBefore(title, clonedElement.firstChild);

        const dateInfo     = document.createElement("p");
        dateInfo.innerHTML = `<strong>Período:</strong> ${startDate} a ${endDate}`;
        clonedElement.insertBefore(dateInfo, clonedElement.children[1]);

        html2pdf().set({
          margin:      10,
          filename:    `reporte_nutricion_${startDate}_${endDate}.pdf`,
          image:       { type: "jpeg", quality: 0.98 },
          html2canvas: { scale: 2 },
          jsPDF:       { orientation: "landscape", unit: "mm", format: "a4" },
        }).from(clonedElement).save();

        this.showAlert("PDF generado exitosamente", "success");
      });
    }
  }

  async showReporteData(startDate, endDate) {
    const email    = this.currentUser.correo ?? this.currentUser.email;
    const consumos = await Data.getConsumosByDateRange(email, startDate, endDate);
    const medidas  = await Data.getMedidasByRange(email, startDate, endDate);
    const container = document.getElementById("reporteData");

    if (consumos.length === 0 && medidas.length === 0) {
      container.innerHTML = '<div class="alert alert-info">No hay datos en este rango de fechas</div>';
      return;
    }

    let html = "";

    if (consumos.length > 0) {
      html += '<h6 class="mt-4 mb-3">Consumo Diario de Alimentos</h6>';
      html += '<table class="table table-striped"><thead><tr><th>Fecha</th><th>Tiempo de Comida</th><th>Productos</th><th>Calorías</th><th>Proteínas (g)</th><th>Grasas (g)</th><th>Carbohidratos (g)</th></tr></thead><tbody>';

      consumos.forEach((consumo) => {
        const fecha    = new Date(consumo.fecha).toLocaleDateString("es-CR");
        const productos = (consumo.productos ?? []).map((p) => `${p.producto.descripcion} (${p.cantidad}g)`).join(", ");
        let totalCal = 0, totalProt = 0, totalGrasa = 0, totalCarb = 0;
        (consumo.productos ?? []).forEach((p) => {
          totalCal   += (p.producto.energia       ?? 0) * (p.cantidad / 100);
          totalProt  += (p.producto.proteina      ?? 0) * (p.cantidad / 100);
          totalGrasa += (p.producto.grasa         ?? 0) * (p.cantidad / 100);
          totalCarb  += (p.producto.carbohidratos ?? 0) * (p.cantidad / 100);
        });
        html += `<tr>
          <td>${fecha}</td>
          <td>${consumo.tiempoComida || "—"}</td>
          <td>${productos}</td>
          <td><strong>${totalCal.toFixed(0)} kcal</strong></td>
          <td>${totalProt.toFixed(1)}</td>
          <td>${totalGrasa.toFixed(1)}</td>
          <td>${totalCarb.toFixed(1)}</td>
        </tr>`;
      });
      html += "</tbody></table>";
    }

    if (medidas.length > 0) {
      html += '<h6 class="mt-4 mb-3">Medidas Corporales</h6>';
      html += '<table class="table table-striped"><thead><tr><th>Fecha</th><th>Cintura (cm)</th><th>Cuello (cm)</th><th>Caderas (cm)</th><th>% Músculo</th><th>% Grasa</th></tr></thead><tbody>';
      medidas.forEach((m) => {
        html += `<tr>
          <td>${new Date(m.fecha).toLocaleDateString("es-CR")}</td>
          <td>${m.cintura}</td>
          <td>${m.cuello}</td>
          <td>${m.caderas}</td>
          <td>${m.porcentajeMusculo}%</td>
          <td>${m.porcentajeGrasa}%</td>
        </tr>`;
      });
      html += "</tbody></table>";
    }

    container.innerHTML = html;
  }

  // ── Utilidades ────────────────────────────────────────────

  showAlert(message, type = "info") {
    const alertDiv     = document.createElement("div");
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `${message}<button type="button" class="btn-close" data-bs-dismiss="alert"></button>`;
    const container = document.querySelector(".container, main, [class*='content']");
    if (container) {
      container.insertBefore(alertDiv, container.firstChild);
      setTimeout(() => alertDiv.remove(), 5000);
    }
  }

  getStoredUser() {
    return Auth.getCurrentUser();
  }
}

document.addEventListener("DOMContentLoaded", () => {
  window.app = new NutriTECApp();
});