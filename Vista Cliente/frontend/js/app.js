/**
 * app.js - Controlador principal de la aplicación NutriTEC
 * Gestiona las vistas y la navegación
 */

class NutriTECApp {
    constructor() {
        this.currentUser = null;
        this.currentView = 'login';
        this.appContainer = document.getElementById('app');
        this.init();
    }

    init() {
        // Verificar si hay usuario autenticado
        this.currentUser = this.getStoredUser();
        
        if (this.currentUser) {
            this.showView('dashboard');
        } else {
            this.showView('login');
        }

        // Escuchar eventos de autenticación
        document.addEventListener('user-login', (e) => {
            this.currentUser = e.detail;
            this.showView('dashboard');
        });

        document.addEventListener('user-logout', () => {
            this.currentUser = null;
            this.showView('login');
        });
    }

    showView(viewName) {
        this.currentView = viewName;
        
        switch (viewName) {
            case 'login':
                this.appContainer.innerHTML = Views.getLoginView();
                this.setupLoginHandlers();
                break;
            case 'register':
                this.appContainer.innerHTML = Views.getRegisterView();
                this.setupRegisterHandlers();
                break;
            case 'dashboard':
                this.appContainer.innerHTML = Views.getDashboardView(this.currentUser);
                this.setupDashboardHandlers();
                break;
            case 'registro-consumo':
                this.appContainer.innerHTML = Views.getRegistroConsumoView(this.currentUser);
                this.setupRegistroConsumoHandlers();
                break;
            case 'registro-medidas':
                this.appContainer.innerHTML = Views.getRegistroMedidasView(this.currentUser);
                this.setupRegistroMedidasHandlers();
                break;
            case 'perfil':
                this.appContainer.innerHTML = Views.getPerfilView(this.currentUser);
                this.setupPerfilHandlers();
                break;
            case 'recetas':
                this.appContainer.innerHTML = Views.getRecetasView(this.currentUser);
                this.setupRecetasHandlers();
                break;
            case 'reporte':
                this.appContainer.innerHTML = Views.getReporteView(this.currentUser);
                this.setupReporteHandlers();
                break;
            default:
                this.showView('login');
        }
    }

    setupLoginHandlers() {
        const form = document.getElementById('loginForm');
        const switchToRegisterBtn = document.getElementById('switchToRegister');

        if (form) {
            form.addEventListener('submit', (e) => {
                e.preventDefault();
                const email = document.getElementById('email').value;
                const password = document.getElementById('password').value;
                
                if (Auth.login(email, password)) {
                    // El evento se dispara en Auth.login()
                } else {
                    this.showAlert('Email o contraseña inválidos', 'danger');
                }
            });
        }

        if (switchToRegisterBtn) {
            switchToRegisterBtn.addEventListener('click', () => {
                this.showView('register');
            });
        }
    }

    setupRegisterHandlers() {
        const form = document.getElementById('registerForm');
        const switchToLoginBtn = document.getElementById('switchToLogin');

        if (form) {
            form.addEventListener('submit', (e) => {
                e.preventDefault();
                
                const userData = {
                    nombre: document.getElementById('nombre').value,
                    apellidos: document.getElementById('apellidos').value,
                    email: document.getElementById('email').value,
                    password: document.getElementById('password').value,
                    confirmPassword: document.getElementById('confirmPassword').value,
                    edad: document.getElementById('edad').value,
                    fechaNacimiento: document.getElementById('fechaNacimiento').value,
                    peso: document.getElementById('peso').value,
                    imc: document.getElementById('imc').value,
                    pais: document.getElementById('pais').value,
                    cintura: document.getElementById('cintura').value,
                    cuello: document.getElementById('cuello').value,
                    caderas: document.getElementById('caderas').value,
                    porcentajeMusculo: document.getElementById('porcentajeMusculo').value,
                    porcentajeGrasa: document.getElementById('porcentajeGrasa').value,
                    caloriasDiarias: document.getElementById('caloriasDiarias').value
                };

                if (userData.password !== userData.confirmPassword) {
                    this.showAlert('Las contraseñas no coinciden', 'danger');
                    return;
                }

                if (Auth.register(userData)) {
                    this.showAlert('Registro exitoso. Por favor, inicia sesión.', 'success');
                    setTimeout(() => this.showView('login'), 2000);
                } else {
                    this.showAlert('Error en el registro. El email ya existe.', 'danger');
                }
            });
        }

        if (switchToLoginBtn) {
            switchToLoginBtn.addEventListener('click', () => {
                this.showView('login');
            });
        }
    }

    setupDashboardHandlers() {
        // Botones de navegación
        const navButtons = document.querySelectorAll('[data-view]');
        navButtons.forEach(btn => {
            btn.addEventListener('click', () => {
                const view = btn.dataset.view;
                this.showView(view);
            });
        });

        // Botón de logout
        const logoutBtn = document.getElementById('logoutBtn');
        if (logoutBtn) {
            logoutBtn.addEventListener('click', () => {
                Auth.logout();
            });
        }
    }

    setupRegistroConsumoHandlers() {
        const searchInput = document.getElementById('searchProducto');
        const tiempoComidaSelect = document.getElementById('tiempoComida');
        const productosLista = document.getElementById('productosLista');
        const consumoTableBody = document.getElementById('consumoTableBody');

        // Búsqueda en tiempo real
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                const query = e.target.value.trim();

                if (query.length === 0) {
                    productosLista.innerHTML = `
                        <div class="text-muted text-center py-4">
                            <i class="fas fa-search" style="font-size: 2rem;"></i>
                            <p>Busca un producto para comenzar</p>
                        </div>
                    `;
                    return;
                }

                // Realizar búsqueda
                let resultados = [];
                
                // Buscar por nombre
                resultados = Data.buscarProductos(query, 'nombre');

                // Si no hay resultados por nombre, buscar por código de barras
                if (resultados.length === 0 && !isNaN(query)) {
                    resultados = Data.buscarProductos(query, 'codigoBarras');
                }

                // Mostrar resultados
                if (resultados.length === 0) {
                    productosLista.innerHTML = `
                        <div class="alert alert-warning text-center">
                            <i class="fas fa-info-circle"></i> No se encontraron productos
                        </div>
                    `;
                } else {
                    productosLista.innerHTML = `
                        <div class="list-group">
                            ${resultados.map(producto => `
                                <div class="list-group-item list-group-item-action">
                                    <div class="d-flex w-100 justify-content-between align-items-start">
                                        <div style="flex-grow: 1;">
                                            <h6 class="mb-1">${producto.descripcion}</h6>
                                            <small class="text-muted">
                                                Código: ${producto.codigoBarras} | 
                                                Kcal: ${producto.energia}
                                            </small>
                                        </div>
                                        <div style="margin-left: 10px;">
                                            <input type="number" min="1" value="100" 
                                                class="form-control form-control-sm" 
                                                style="width: 60px; display: inline-block;"
                                                id="cant-${producto.id}">
                                            <button class="btn btn-sm btn-success ms-2" 
                                                onclick="window.app.agregarProductoAlConsumo(${producto.id})">
                                                <i class="fas fa-plus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            `).join('')}
                        </div>
                    `;
                }
            });
        }

        // Actualizar tabla al cargar la vista
        this.actualizarTablaConsumo();
    }

    agregarProductoAlConsumo(productoId) {
        const tiempoComida = document.getElementById('tiempoComida').value;
        const cantidadInput = document.getElementById(`cant-${productoId}`);
        const cantidad = parseInt(cantidadInput.value) || 1;

        if (!tiempoComida) {
            this.showAlert('Por favor selecciona un tiempo de comida', 'warning');
            return;
        }

        if (Data.addProductoAlConsumo(this.currentUser.email, tiempoComida, productoId, cantidad)) {
            this.showAlert(`Producto agregado al ${tiempoComida}`, 'success');
            
            // Limpiar búsqueda
            document.getElementById('searchProducto').value = '';
            document.getElementById('productosLista').innerHTML = `
                <div class="text-muted text-center py-4">
                    <i class="fas fa-search" style="font-size: 2rem;"></i>
                    <p>Busca un producto para comenzar</p>
                </div>
            `;

            // Actualizar tabla y datos
            this.actualizarTablaConsumo();
        } else {
            this.showAlert('Error al agregar producto', 'danger');
        }
    }

    eliminarProductoDelConsumo(consumoId, productoId) {
        if (Data.removeProductoDelConsumo(this.currentUser.email, consumoId, productoId)) {
            this.showAlert('Producto eliminado', 'info');
            this.actualizarTablaConsumo();
        }
    }

    actualizarTablaConsumo() {
        const consumoHoy = Data.getConsumoHoy(this.currentUser.email);
        const tableBody = document.getElementById('consumoTableBody');
        const nutrientes = Data.getNutrientesDia(this.currentUser.email);
        const metaDiaria = this.currentUser.caloriasDiarias || 2000;
        const porcentajeCaloria = Math.min((nutrientes.calorias / metaDiaria) * 100, 100);

        if (!tableBody) return;

        if (consumoHoy.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="4" class="text-center text-muted py-3">No hay productos agregados</td></tr>';
        } else {
            let html = '';
            
            consumoHoy.forEach(consumo => {
                consumo.productos.forEach(prod => {
                    const totalCaloriasProducto = (prod.producto.energia * prod.cantidad) / 100;
                    html += `
                        <tr>
                            <td>
                                <small>${prod.producto.descripcion}</small>
                            </td>
                            <td>
                                <input type="number" min="1" value="${prod.cantidad}" 
                                    class="form-control form-control-sm" style="width: 70px;"
                                    onchange="window.app.updateCantidadProducto('${consumo.id}', ${prod.productoId}, this.value)">
                            </td>
                            <td>${totalCaloriasProducto.toFixed(0)}</td>
                            <td>
                                <button class="btn btn-sm btn-danger" 
                                    onclick="window.app.eliminarProductoDelConsumo('${consumo.id}', ${prod.productoId})">
                                    <i class="fas fa-trash"></i>
                                </button>
                            </td>
                        </tr>
                    `;
                });
            });

            tableBody.innerHTML = html;
        }

        // Actualizar panel derecho de nutrientes
        const panelNutrientes = document.querySelector('.card.sticky-top .card-body');
        if (panelNutrientes) {
            panelNutrientes.innerHTML = `
                <!-- Barra de Progreso -->
                <div class="mb-4">
                    <div class="progress" style="height: 30px;">
                        <div class="progress-bar bg-success" role="progressbar" 
                            style="width: ${porcentajeCaloria}%;" 
                            aria-valuenow="${porcentajeCaloria}" aria-valuemin="0" aria-valuemax="100">
                            <small>${porcentajeCaloria.toFixed(0)}%</small>
                        </div>
                    </div>
                    <small class="text-muted">
                        ${nutrientes.calorias.toFixed(0)} / ${metaDiaria} kcal
                    </small>
                </div>

                <!-- Desglose de Nutrientes -->
                <div class="mb-2">
                    <div class="d-flex justify-content-between">
                        <strong>Proteínas:</strong>
                        <span>${nutrientes.proteinas.toFixed(1)}g</span>
                    </div>
                </div>
                <div class="mb-2">
                    <div class="d-flex justify-content-between">
                        <strong>Grasas:</strong>
                        <span>${nutrientes.grasas.toFixed(1)}g</span>
                    </div>
                </div>
                <div class="mb-2">
                    <div class="d-flex justify-content-between">
                        <strong>Carbohidratos:</strong>
                        <span>${nutrientes.carbohidratos.toFixed(1)}g</span>
                    </div>
                </div>
                <hr>
                <div class="mb-2">
                    <div class="d-flex justify-content-between">
                        <strong>Sodio:</strong>
                        <span>${nutrientes.sodio.toFixed(0)}mg</span>
                    </div>
                </div>
                <hr>
                <div class="alert alert-info" style="font-size: 0.9rem;">
                    <i class="fas fa-info-circle"></i>
                    <strong>Total Registros:</strong> ${consumoHoy.length}
                </div>
            `;
        }
    }

    updateCantidadProducto(consumoId, productoId, nuevaCantidad) {
        nuevaCantidad = parseInt(nuevaCantidad) || 1;
        
        if (Data.updateCantidadProducto(this.currentUser.email, consumoId, productoId, nuevaCantidad)) {
            this.actualizarTablaConsumo();
        }
    }

    setupRegistroMedidasHandlers() {
        const form = document.getElementById('medidasForm');
        if (form) {
            form.addEventListener('submit', (e) => {
                e.preventDefault();
                
                const medida = {
                    fecha: new Date().toISOString(),
                    cintura: document.getElementById('cintura').value,
                    cuello: document.getElementById('cuello').value,
                    caderas: document.getElementById('caderas').value,
                    porcentajeMusculo: document.getElementById('porcentajeMusculo').value,
                    porcentajeGrasa: document.getElementById('porcentajeGrasa').value
                };

                if (Data.addMedida(this.currentUser.email, medida)) {
                    this.showAlert('Medidas registradas exitosamente', 'success');
                    form.reset();
                } else {
                    this.showAlert('Error al registrar medidas', 'danger');
                }
            });
        }
    }

    setupPerfilHandlers() {
        const editBtn = document.getElementById('editPerfilBtn');
        const form = document.getElementById('perfilForm');

        if (editBtn) {
            editBtn.addEventListener('click', () => {
                form.classList.toggle('hidden');
            });
        }

        if (form) {
            form.addEventListener('submit', (e) => {
                e.preventDefault();
                
                this.currentUser.nombre = document.getElementById('nombre').value;
                this.currentUser.apellidos = document.getElementById('apellidos').value;
                this.currentUser.peso = document.getElementById('peso').value;
                this.currentUser.imc = document.getElementById('imc').value;

                if (Data.updateUser(this.currentUser)) {
                    this.showAlert('Perfil actualizado exitosamente', 'success');
                    this.showView('perfil');
                } else {
                    this.showAlert('Error al actualizar perfil', 'danger');
                }
            });
        }
    }

    setupRecetasHandlers() {
        const searchInput = document.getElementById('searchProductoReceta');
        const productosLista = document.getElementById('productosListaReceta');
        const recetaTableBody = document.getElementById('recetaTableBody');

        // Búsqueda en tiempo real
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                const query = e.target.value.trim();

                if (query.length === 0) {
                    productosLista.innerHTML = `
                        <div class="text-muted text-center py-3">
                            <i class="fas fa-search" style="font-size: 1.5rem;"></i>
                            <p>Busca un producto para comenzar</p>
                        </div>
                    `;
                    return;
                }

                let resultados = [];
                resultados = Data.buscarProductos(query, 'nombre');
                if (resultados.length === 0 && !isNaN(query)) {
                    resultados = Data.buscarProductos(query, 'codigoBarras');
                }

                if (resultados.length === 0) {
                    productosLista.innerHTML = `
                        <div class="alert alert-warning text-center">
                            <i class="fas fa-info-circle"></i> No se encontraron productos
                        </div>
                    `;
                } else {
                    productosLista.innerHTML = `
                        <div class="list-group">
                            ${resultados.map(producto => `
                                <div class="list-group-item list-group-item-action">
                                    <div class="d-flex w-100 justify-content-between align-items-start">
                                        <div style="flex-grow: 1;">
                                            <h6 class="mb-1">${producto.descripcion}</h6>
                                            <small class="text-muted">
                                                ${producto.energia} kcal
                                            </small>
                                        </div>
                                        <div style="margin-left: 10px;">
                                            <input type="number" min="1" value="1" 
                                                class="form-control form-control-sm" 
                                                style="width: 60px; display: inline-block;"
                                                id="cant-receta-${producto.id}">
                                            <button class="btn btn-sm btn-success ms-2" 
                                                onclick="window.app.agregarProductoAReceta(${producto.id})">
                                                <i class="fas fa-plus"></i>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            `).join('')}
                        </div>
                    `;
                }
            });
        }

        // Actualizar tabla al cargar la vista
        this.actualizarTablaReceta();
    }

    agregarProductoAReceta(productoId) {
        const cantidadInput = document.getElementById(`cant-receta-${productoId}`);
        const cantidad = parseInt(cantidadInput.value) || 1;

        if (Data.addProductoARecetaTemporal(productoId, cantidad)) {
            this.showAlert('Producto agregado a receta', 'success');
            
            // Limpiar búsqueda
            document.getElementById('searchProductoReceta').value = '';
            document.getElementById('productosListaReceta').innerHTML = `
                <div class="text-muted text-center py-3">
                    <i class="fas fa-search" style="font-size: 1.5rem;"></i>
                    <p>Busca un producto para comenzar</p>
                </div>
            `;

            // Actualizar tabla y resumen
            this.actualizarTablaReceta();
        } else {
            this.showAlert('Error al agregar producto', 'danger');
        }
    }

    eliminarProductoDeReceta(productoId) {
        Data.removeProductoDeRecetaTemporal(productoId);
        this.showAlert('Producto eliminado de receta', 'info');
        this.actualizarTablaReceta();
    }

    actualizarTablaReceta() {
        const productosReceta = Data.getProductosRecetaEnEdicion();
        const tableBody = document.getElementById('recetaTableBody');
        const nutrientes = Data.calcularNutrientesRecetaTemporal();
        const resumenPanel = document.getElementById('resumenNutrientesReceta');

        if (!tableBody) return;

        if (productosReceta.length === 0) {
            tableBody.innerHTML = '<tr><td colspan="3" class="text-center text-muted py-2">No hay productos</td></tr>';
        } else {
            let html = '';
            
            productosReceta.forEach(prod => {
                const totalCaloriasProducto = prod.producto.energia * prod.cantidad;
                html += `
                    <tr>
                        <td>
                            <small>${prod.producto.descripcion}</small>
                        </td>
                        <td>
                            <input type="number" min="1" value="${prod.cantidad}" 
                                class="form-control form-control-sm" style="width: 70px;"
                                onchange="window.app.updateCantidadProductoReceta(${prod.productoId}, this.value)">
                        </td>
                        <td>
                            <button class="btn btn-sm btn-danger" 
                                onclick="window.app.eliminarProductoDeReceta(${prod.productoId})">
                                <i class="fas fa-trash"></i>
                            </button>
                        </td>
                    </tr>
                `;
            });

            tableBody.innerHTML = html;
        }

        // Actualizar panel derecho
        if (resumenPanel) {
            if (productosReceta.length === 0) {
                resumenPanel.innerHTML = `
                    <div class="alert alert-info text-center">
                        <small>Agrega productos para ver nutrientes</small>
                    </div>
                `;
            } else {
                resumenPanel.innerHTML = `
                    <div class="mb-2">
                        <div class="d-flex justify-content-between">
                            <strong>Calorías:</strong>
                            <span>${nutrientes.calorias.toFixed(0)}</span>
                        </div>
                    </div>
                    <div class="mb-2">
                        <div class="d-flex justify-content-between">
                            <strong>Proteínas:</strong>
                            <span>${nutrientes.proteinas.toFixed(1)}g</span>
                        </div>
                    </div>
                    <div class="mb-2">
                        <div class="d-flex justify-content-between">
                            <strong>Grasas:</strong>
                            <span>${nutrientes.grasas.toFixed(1)}g</span>
                        </div>
                    </div>
                    <div class="mb-2">
                        <div class="d-flex justify-content-between">
                            <strong>Carbohidratos:</strong>
                            <span>${nutrientes.carbohidratos.toFixed(1)}g</span>
                        </div>
                    </div>
                    <hr>
                    <div class="mb-2">
                        <div class="d-flex justify-content-between">
                            <strong>Sodio:</strong>
                            <span>${nutrientes.sodio.toFixed(0)}mg</span>
                        </div>
                    </div>
                `;
            }
        }
    }

    updateCantidadProductoReceta(productoId, nuevaCantidad) {
        nuevaCantidad = parseInt(nuevaCantidad) || 1;
        
        if (Data.updateCantidadProductoRecetaTemporal(productoId, nuevaCantidad)) {
            this.actualizarTablaReceta();
        }
    }

    guardarReceta() {
        const nombreReceta = document.getElementById('nombreReceta').value.trim();
        
        if (!nombreReceta) {
            this.showAlert('Por favor ingresa un nombre para la receta', 'warning');
            return;
        }

        if (Data.saveReceta(this.currentUser.email, nombreReceta)) {
            this.showAlert('Receta guardada exitosamente', 'success');
            document.getElementById('nombreReceta').value = '';
            this.actualizarTablaReceta();
            this.showView('recetas');
        } else {
            this.showAlert('Error al guardar receta o no hay productos', 'danger');
        }
    }

    setupReporteHandlers() {
        const generateBtn = document.getElementById('generateReportBtn');
        const generatePDFBtn = document.getElementById('generatePDFBtn');

        if (generateBtn) {
            generateBtn.addEventListener('click', () => {
                const startDate = document.getElementById('startDate').value;
                const endDate = document.getElementById('endDate').value;

                if (!startDate || !endDate) {
                    this.showAlert('Por favor selecciona rango de fechas', 'warning');
                    return;
                }

                // Mostrar datos en tabla
                this.showReporteData(startDate, endDate);
            });
        }

        if (generatePDFBtn) {
            generatePDFBtn.addEventListener('click', () => {
                const startDate = document.getElementById('startDate').value;
                const endDate = document.getElementById('endDate').value;

                if (!startDate || !endDate) {
                    this.showAlert('Por favor selecciona rango de fechas primero', 'warning');
                    return;
                }

                // Generate PDF from current report data
                const element = document.getElementById('reporteData');
                if (!element || element.innerHTML.trim() === '') {
                    this.showAlert('Primero genera el reporte antes de descargar PDF', 'warning');
                    return;
                }

                const opt = {
                    margin: 10,
                    filename: `reporte_nutricion_${startDate}_${endDate}.pdf`,
                    image: { type: 'jpeg', quality: 0.98 },
                    html2canvas: { scale: 2 },
                    jsPDF: { orientation: 'landscape', unit: 'mm', format: 'a4' }
                };

                // Clone element to avoid modifying original
                const clonedElement = element.cloneNode(true);
                const title = document.createElement('h3');
                title.textContent = 'Reporte de Nutrición NutriTEC';
                clonedElement.insertBefore(title, clonedElement.firstChild);

                const dateInfo = document.createElement('p');
                dateInfo.innerHTML = `<strong>Período:</strong> ${startDate} a ${endDate}`;
                clonedElement.insertBefore(dateInfo, clonedElement.children[1]);

                html2pdf().set(opt).from(clonedElement).save();
                this.showAlert('PDF generado exitosamente', 'success');
            });
        }
    }

    showReporteData(startDate, endDate) {
        const consumos = Data.getConsumosByDateRange(this.currentUser.email, startDate, endDate);
        const medidas = Data.getMedidasByRange(this.currentUser.email, startDate, endDate);
        const container = document.getElementById('reporteData');
        
        if (consumos.length === 0 && medidas.length === 0) {
            container.innerHTML = '<div class="alert alert-info">No hay datos en este rango de fechas</div>';
            return;
        }

        let html = '';

        // Mostrar consumos de alimentos
        if (consumos.length > 0) {
            html += '<h6 class="mt-4 mb-3">📊 Consumo Diario de Alimentos</h6>';
            html += '<table class="table table-striped"><thead><tr><th>Fecha</th><th>Hora de Comida</th><th>Productos</th><th>Calorías</th><th>Proteínas (g)</th><th>Grasas (g)</th><th>Carbohidratos (g)</th></tr></thead><tbody>';
            
            consumos.forEach(consumo => {
                const fecha = new Date(consumo.fecha).toLocaleDateString();
                const productos = consumo.productos.map(p => `${p.producto.descripcion} (${p.cantidad}g)`).join(', ');
                
                // Calculate totals from products
                let totalCalorias = 0, totalProteinas = 0, totalGrasas = 0, totalCarbohidratos = 0;
                consumo.productos.forEach(p => {
                    totalCalorias += (p.producto.energia || 0) * (p.cantidad / 100);
                    totalProteinas += (p.producto.proteina || 0) * (p.cantidad / 100);
                    totalGrasas += (p.producto.grasa || 0) * (p.cantidad / 100);
                    totalCarbohidratos += (p.producto.carbohidratos || 0) * (p.cantidad / 100);
                });

                html += `<tr>
                    <td>${fecha}</td>
                    <td>${consumo.tiempoComida || 'No especificado'}</td>
                    <td>${productos}</td>
                    <td><strong>${totalCalorias.toFixed(0)} kcal</strong></td>
                    <td>${totalProteinas.toFixed(1)}</td>
                    <td>${totalGrasas.toFixed(1)}</td>
                    <td>${totalCarbohidratos.toFixed(1)}</td>
                </tr>`;
            });

            html += '</tbody></table>';
        }

        // Mostrar medidas corporales
        if (medidas.length > 0) {
            html += '<h6 class="mt-4 mb-3">📏 Medidas Corporales</h6>';
            html += '<table class="table table-striped"><thead><tr><th>Fecha</th><th>Cintura (cm)</th><th>Cuello (cm)</th><th>Caderas (cm)</th><th>% Músculo</th><th>% Grasa</th></tr></thead><tbody>';
            
            medidas.forEach(medida => {
                const fecha = new Date(medida.fecha).toLocaleDateString();
                html += `<tr>
                    <td>${fecha}</td>
                    <td>${medida.cintura}</td>
                    <td>${medida.cuello}</td>
                    <td>${medida.caderas}</td>
                    <td>${medida.porcentajeMusculo}%</td>
                    <td>${medida.porcentajeGrasa}%</td>
                </tr>`;
            });

            html += '</tbody></table>';
        }

        container.innerHTML = html;
    }

    showAlert(message, type = 'info') {
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

        const container = document.querySelector('.container, main, [class*="content"]');
        if (container) {
            container.insertBefore(alertDiv, container.firstChild);
            setTimeout(() => alertDiv.remove(), 5000);
        }
    }

    getStoredUser() {
        const user = localStorage.getItem('currentUser');
        return user ? JSON.parse(user) : null;
    }
}

// Inicializar la aplicación cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', () => {
    window.app = new NutriTECApp();
});
