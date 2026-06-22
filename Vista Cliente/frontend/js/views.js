/**
 * views.js - Vistas de la aplicación NutriTEC
 */

class Views {
    static getLoginView() {
        return `
            <div class="auth-container">
                <div class="auth-card">
                    <div class="card-header">
                        <h2><i class="fas fa-heartbeat"></i> NutriTEC</h2>
                        <p class="text-white mb-0">Tu plataforma de nutrición</p>
                    </div>
                    <form id="loginForm" class="auth-form">
                        <div class="form-group">
                            <label for="email" class="form-label">Correo Electrónico</label>
                            <input type="email" class="form-control" id="email" placeholder="ejemplo@correo.com" required>
                        </div>
                        <div class="form-group">
                            <label for="password" class="form-label">Contraseña</label>
                            <input type="password" class="form-control" id="password" placeholder="••••••••" required>
                        </div>
                        <button type="submit" class="btn btn-primary w-100 mt-3">
                            <i class="fas fa-sign-in-alt"></i> Iniciar Sesión
                        </button>
                    </form>
                    <div class="auth-link">
                        ¿No tienes cuenta? <a href="#" id="switchToRegister">Regístrate aquí</a>
                    </div>
                </div>
            </div>`;
    }

    static getRegisterView() {
        return `
            <div class="auth-container">
                <div class="auth-card" style="max-width: 600px;">
                    <div class="card-header">
                        <h2><i class="fas fa-user-plus"></i> Crear Cuenta</h2>
                        <p class="text-white mb-0">Únete a NutriTEC</p>
                    </div>
                    <form id="registerForm" class="auth-form" style="max-height: 600px; overflow-y: auto; padding-right: 10px;">
                        <div class="form-row">
                            <div class="form-group">
                                <label for="nombre" class="form-label">Nombre</label>
                                <input type="text" class="form-control" id="nombre" required>
                            </div>
                            <div class="form-group">
                                <label for="apellidos" class="form-label">Apellidos</label>
                                <input type="text" class="form-control" id="apellidos" required>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="email" class="form-label">Correo Electrónico</label>
                            <input type="email" class="form-control" id="email" required>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label for="password" class="form-label">Contraseña</label>
                                <input type="password" class="form-control" id="password" required>
                            </div>
                            <div class="form-group">
                                <label for="confirmPassword" class="form-label">Confirmar</label>
                                <input type="password" class="form-control" id="confirmPassword" required>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label for="edad" class="form-label">Edad</label>
                                <input type="number" class="form-control" id="edad" required>
                            </div>
                            <div class="form-group">
                                <label for="fechaNacimiento" class="form-label">Fecha Nacimiento</label>
                                <input type="date" class="form-control" id="fechaNacimiento" required>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label for="peso" class="form-label">Peso (kg)</label>
                                <input type="number" step="0.1" class="form-control" id="peso" required>
                            </div>
                            <div class="form-group">
                                <label for="imc" class="form-label">IMC</label>
                                <input type="number" step="0.1" class="form-control" id="imc" required>
                            </div>
                        </div>
                        <div class="form-group">
                            <label for="pais" class="form-label">País</label>
                            <select class="form-select" id="pais" required>
                                <option value="">Selecciona un país</option>
                                <option value="Costa Rica">Costa Rica</option>
                                <option value="Panama">Panamá</option>
                                <option value="Colombia">Colombia</option>
                                <option value="Mexico">México</option>
                                <option value="Otro">Otro</option>
                            </select>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label for="cintura" class="form-label">Cintura (cm)</label>
                                <input type="number" step="0.1" class="form-control" id="cintura" required>
                            </div>
                            <div class="form-group">
                                <label for="cuello" class="form-label">Cuello (cm)</label>
                                <input type="number" step="0.1" class="form-control" id="cuello" required>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label for="caderas" class="form-label">Caderas (cm)</label>
                                <input type="number" step="0.1" class="form-control" id="caderas" required>
                            </div>
                            <div class="form-group">
                                <label for="porcentajeMusculo" class="form-label">% Músculo</label>
                                <input type="number" step="0.1" class="form-control" id="porcentajeMusculo" required>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group">
                                <label for="porcentajeGrasa" class="form-label">% Grasa</label>
                                <input type="number" step="0.1" class="form-control" id="porcentajeGrasa" required>
                            </div>
                            <div class="form-group">
                                <label for="caloriasDiarias" class="form-label">Meta Calorías</label>
                                <input type="number" class="form-control" id="caloriasDiarias" required>
                            </div>
                        </div>
                        <button type="submit" class="btn btn-primary w-100 mt-3">
                            <i class="fas fa-check"></i> Registrarse
                        </button>
                    </form>
                    <div class="auth-link">
                        ¿Ya tienes cuenta? <a href="#" id="switchToLogin">Inicia sesión aquí</a>
                    </div>
                </div>
            </div>`;
    }

    static getDashboardView(user, consumoHoy = [], medidasRecientes = [], nutrientesHoy = null) {
        const nutrientes        = nutrientesHoy ?? { calorias: 0, proteinas: 0, grasas: 0, carbohidratos: 0, sodio: 0 };
        const metaDiaria        = user.caloriasDiariasMax ?? 2000;
        const porcentajeCaloria = Math.min((nutrientes.calorias / metaDiaria) * 100, 100);
        const ultimaMedida      = medidasRecientes.length > 0 ? medidasRecientes[0] : null;

        return `
            <div class="dashboard-header">
                <div class="container">
                    <div class="row align-items-center">
                        <div class="col">
                            <h1><i class="fas fa-chart-line"></i> Dashboard</h1>
                            <p class="mb-0">Bienvenido, ${user.nombre}!</p>
                        </div>
                        <div class="col-auto">
                            <button class="btn btn-light" id="logoutBtn">
                                <i class="fas fa-sign-out-alt"></i> Cerrar Sesión
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            <div class="container mt-4">
                <div class="stats-container">
                    <div class="stat-card">
                        <div class="stat-icon"><i class="fas fa-fire"></i></div>
                        <div class="stat-value">${nutrientes.calorias.toFixed(0)}</div>
                        <div class="stat-label">Calorías consumidas</div>
                        <small class="text-muted">Meta: ${metaDiaria} kcal</small>
                    </div>
                    <div class="stat-card">
                        <div class="stat-icon"><i class="fas fa-weight"></i></div>
                        <div class="stat-value">${user.peso ?? "—"}</div>
                        <div class="stat-label">Peso actual (kg)</div>
                        <small class="text-muted">IMC: ${user.imc ?? "—"}</small>
                    </div>
                    <div class="stat-card">
                        <div class="stat-icon"><i class="fas fa-utensils"></i></div>
                        <div class="stat-value">${consumoHoy.length}</div>
                        <div class="stat-label">Registros hoy</div>
                        <small class="text-muted">de ${metaDiaria} kcal</small>
                    </div>
                </div>

                <div class="card mb-4">
                    <div class="card-body">
                        <h5 class="card-title">Progreso de Calorías</h5>
                        <div class="progress" style="height: 25px;">
                            <div class="progress-bar bg-success" role="progressbar" style="width: ${porcentajeCaloria}%;">
                                ${porcentajeCaloria.toFixed(0)}%
                            </div>
                        </div>
                        <small class="text-muted">${nutrientes.calorias.toFixed(0)} de ${metaDiaria} kcal</small>
                    </div>
                </div>

                <div class="row">
                    <div class="col-md-2 mb-3">
                        <button class="btn btn-primary w-100" data-view="registro-consumo">
                            <i class="fas fa-utensils"></i><br>Registrar Consumo
                        </button>
                    </div>
                    <div class="col-md-2 mb-3">
                        <button class="btn btn-primary w-100" data-view="registro-medidas">
                            <i class="fas fa-ruler"></i><br>Registrar Medidas
                        </button>
                    </div>
                    <div class="col-md-2 mb-3">
                        <button class="btn btn-primary w-100" data-view="recetas">
                            <i class="fas fa-book"></i><br>Mis Recetas
                        </button>
                    </div>
                    <div class="col-md-2 mb-3">
                        <button class="btn btn-primary w-100" data-view="reporte">
                            <i class="fas fa-file-pdf"></i><br>Reportes
                        </button>
                    </div>
                    <div class="col-md-2 mb-3">
                        <button class="btn btn-info w-100 text-white" data-view="retroalimentacion">
                            <i class="fas fa-comments"></i><br>Mensajes
                        </button>
                    </div>
                </div>

                ${ultimaMedida ? `
                    <div class="card">
                        <div class="card-header"><h5 class="mb-0">Última Medida Registrada</h5></div>
                        <div class="card-body">
                            <div class="row">
                                <div class="col-md-4"><strong>Cintura:</strong> ${ultimaMedida.cintura} cm</div>
                                <div class="col-md-4"><strong>Cuello:</strong> ${ultimaMedida.cuello} cm</div>
                                <div class="col-md-4"><strong>Caderas:</strong> ${ultimaMedida.caderas} cm</div>
                            </div>
                        </div>
                    </div>
                ` : ""}

                <div style="margin-top: 20px; text-align: center;">
                    <button class="btn btn-outline-primary" data-view="perfil">
                        <i class="fas fa-user"></i> Ver Mi Perfil
                    </button>
                </div>
            </div>`;
    }

    static getRegistroConsumoView(user, consumoHoy = []) {
        const nutrientes        = Data.getNutrientesDia(user.email, consumoHoy);
        const metaDiaria        = user.caloriasDiariasMax ?? 2000;
        const porcentajeCaloria = Math.min((nutrientes.calorias / metaDiaria) * 100, 100);

        return `
            <div class="container mt-4">
                <div class="row">
                    <div class="col-md-8">
                        <div class="card mb-4">
                            <div class="card-header">
                                <h5 class="mb-0"><i class="fas fa-utensils"></i> Registrar Consumo Diario</h5>
                            </div>
                            <div class="card-body">
                                <div class="form-group mb-3">
                                    <label for="tiempoComida" class="form-label">Tiempo de Comida</label>
                                    <select class="form-select" id="tiempoComida">
                                        <option value="">Selecciona un tiempo de comida</option>
                                        <option value="Desayuno">🌅 Desayuno</option>
                                        <option value="Merienda Mañana">☕ Merienda Mañana</option>
                                        <option value="Almuerzo">🍽️ Almuerzo</option>
                                        <option value="Merienda Tarde">🍰 Merienda Tarde</option>
                                        <option value="Cena">🌙 Cena</option>
                                    </select>
                                </div>
                                <div class="form-group mb-3">
                                    <label for="searchProducto" class="form-label">
                                        <i class="fas fa-search"></i> Buscar Producto o Receta
                                    </label>
                                    <input type="text" class="form-control" id="searchProducto"
                                        placeholder="Escribe nombre, código de barras o receta...">
                                    <small class="text-muted">Búsqueda en tiempo real — incluye tus recetas</small>
                                </div>
                                <div id="productosLista" class="mb-4">
                                    <div class="text-muted text-center py-4">
                                        <i class="fas fa-search" style="font-size: 2rem;"></i>
                                        <p>Busca un producto o receta para comenzar</p>
                                    </div>
                                </div>
                                <div class="card" style="margin-top: 30px;">
                                    <div class="card-header"><h6 class="mb-0">Productos Agregados</h6></div>
                                    <div class="table-responsive">
                                        <table class="table mb-0">
                                            <thead>
                                                <tr>
                                                    <th>Producto</th>
                                                    <th style="width: 80px;">Gramos</th>
                                                    <th style="width: 100px;">Kcal</th>
                                                    <th style="width: 50px;"></th>
                                                </tr>
                                            </thead>
                                            <tbody id="consumoTableBody">
                                                <tr><td colspan="4" class="text-center text-muted py-3">No hay productos agregados</td></tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <div class="mt-3">
                                    <button class="btn btn-secondary" onclick="window.app.showView('dashboard')">
                                        <i class="fas fa-arrow-left"></i> Volver al Dashboard
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-4">
                        <div class="card sticky-top" style="top: 20px;">
                            <div class="card-header">
                                <h5 class="mb-0"><i class="fas fa-chart-pie"></i> Consumo del Día</h5>
                            </div>
                            <div class="card-body">
                                <div class="mb-4">
                                    <div class="progress" style="height: 30px;">
                                        <div class="progress-bar bg-success" role="progressbar" style="width: ${porcentajeCaloria}%;">
                                            <small>${porcentajeCaloria.toFixed(0)}%</small>
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
                                <div class="alert alert-info" style="font-size: 0.9rem;">
                                    <i class="fas fa-info-circle"></i>
                                    <strong>Total Registros:</strong> ${consumoHoy.length}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>`;
    }

    static getRegistroMedidasView(user) {
        return `
            <div class="container mt-4">
                <div class="row justify-content-center">
                    <div class="col-md-6">
                        <div class="card">
                            <div class="card-header">
                                <h5 class="mb-0"><i class="fas fa-ruler"></i> Registrar Medidas</h5>
                            </div>
                            <div class="card-body">
                                <form id="medidasForm">
                                    <div class="form-group mb-3">
                                        <label for="cintura" class="form-label">Cintura (cm)</label>
                                        <input type="number" step="0.1" class="form-control" id="cintura" required>
                                    </div>
                                    <div class="form-group mb-3">
                                        <label for="cuello" class="form-label">Cuello (cm)</label>
                                        <input type="number" step="0.1" class="form-control" id="cuello" required>
                                    </div>
                                    <div class="form-group mb-3">
                                        <label for="caderas" class="form-label">Caderas (cm)</label>
                                        <input type="number" step="0.1" class="form-control" id="caderas" required>
                                    </div>
                                    <div class="form-group mb-3">
                                        <label for="porcentajeMusculo" class="form-label">Porcentaje de Músculo (%)</label>
                                        <input type="number" step="0.1" class="form-control" id="porcentajeMusculo" required>
                                    </div>
                                    <div class="form-group mb-3">
                                        <label for="porcentajeGrasa" class="form-label">Porcentaje de Grasa (%)</label>
                                        <input type="number" step="0.1" class="form-control" id="porcentajeGrasa" required>
                                    </div>
                                    <button type="submit" class="btn btn-primary w-100 mb-2">
                                        <i class="fas fa-save"></i> Guardar Medidas
                                    </button>
                                    <button type="button" class="btn btn-secondary w-100" onclick="window.app.showView('dashboard')">
                                        <i class="fas fa-arrow-left"></i> Volver
                                    </button>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row mt-4">
                    <div class="col-md-12">
                        <div class="card">
                            <div class="card-header"><h5 class="mb-0">Historial de Medidas</h5></div>
                            <div class="table-responsive">
                                <table class="table mb-0">
                                    <thead>
                                        <tr>
                                            <th>Fecha</th><th>Cintura</th><th>Cuello</th>
                                            <th>Caderas</th><th>% Músculo</th><th>% Grasa</th>
                                        </tr>
                                    </thead>
                                    <tbody id="medidasTable">
                                        <tr><td colspan="6" class="text-center text-muted py-3">No hay medidas registradas</td></tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>`;
    }

    static getPerfilView(user) {
        return `
            <div class="container mt-4">
                <div class="row">
                    <div class="col-md-8 mx-auto">
                        <div class="card">
                            <div class="card-header">
                                <div class="row align-items-center">
                                    <div class="col"><h5 class="mb-0"><i class="fas fa-user"></i> Mi Perfil</h5></div>
                                    <div class="col-auto">
                                        <button class="btn btn-sm btn-primary" id="editPerfilBtn">
                                            <i class="fas fa-edit"></i> Editar
                                        </button>
                                    </div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="row mb-3">
                                    <div class="col-md-6"><h6 class="text-muted small">Nombre</h6><p>${user.nombre}</p></div>
                                    <div class="col-md-6"><h6 class="text-muted small">Apellidos</h6><p>${user.apellidos ?? "—"}</p></div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-md-6"><h6 class="text-muted small">Email</h6><p>${user.correo ?? user.email}</p></div>
                                    <div class="col-md-6"><h6 class="text-muted small">Edad</h6><p>${user.edad ?? "—"} años</p></div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-md-6"><h6 class="text-muted small">Peso</h6><p>${user.peso ?? "—"} kg</p></div>
                                    <div class="col-md-6"><h6 class="text-muted small">IMC</h6><p>${user.imc ?? "—"}</p></div>
                                </div>
                                <div class="row mb-3">
                                    <div class="col-md-6"><h6 class="text-muted small">Meta Diaria de Calorías</h6><p>${user.caloriasDiariasMax ?? 2000} kcal</p></div>
                                    <div class="col-md-6"><h6 class="text-muted small">País</h6><p>${user.pais ?? "—"}</p></div>
                                </div>
                                <form id="perfilForm" class="hidden mt-4">
                                    <div class="form-row">
                                        <div class="form-group mb-3">
                                            <label for="nombre" class="form-label">Nombre</label>
                                            <input type="text" class="form-control" id="nombre" value="${user.nombre}">
                                        </div>
                                        <div class="form-group mb-3">
                                            <label for="apellidos" class="form-label">Apellidos</label>
                                            <input type="text" class="form-control" id="apellidos" value="${user.apellidos ?? ""}">
                                        </div>
                                    </div>
                                    <div class="form-row">
                                        <div class="form-group mb-3">
                                            <label for="peso" class="form-label">Peso (kg)</label>
                                            <input type="number" step="0.1" class="form-control" id="peso" value="${user.peso ?? ""}">
                                        </div>
                                        <div class="form-group mb-3">
                                            <label for="imc" class="form-label">IMC</label>
                                            <input type="number" step="0.1" class="form-control" id="imc" value="${user.imc ?? ""}">
                                        </div>
                                    </div>
                                    <button type="submit" class="btn btn-success me-2">
                                        <i class="fas fa-save"></i> Guardar Cambios
                                    </button>
                                    <button type="button" class="btn btn-secondary" id="cancelEditBtn">Cancelar</button>
                                </form>
                            </div>
                        </div>
                        <button class="btn btn-secondary mt-3 w-100" onclick="window.app.showView('dashboard')">
                            <i class="fas fa-arrow-left"></i> Volver al Dashboard
                        </button>
                    </div>
                </div>
            </div>`;
    }

    static async getRecetasView(user) {
        const recetas = await Data.getRecetas(user.correo ?? user.email);

        return `
            <div class="container mt-4">
                <div class="row">
                    <div class="col-md-7">
                        <div class="card mb-4">
                            <div class="card-header">
                                <h5 class="mb-0"><i class="fas fa-plus"></i> Nueva Receta</h5>
                            </div>
                            <div class="card-body">
                                <div class="form-group mb-3">
                                    <label for="nombreReceta" class="form-label">Nombre de la Receta</label>
                                    <input type="text" class="form-control" id="nombreReceta"
                                        placeholder="Ej: Pinto costarricense" required>
                                </div>
                                <div class="form-group mb-3">
                                    <label for="searchProductoReceta" class="form-label">
                                        <i class="fas fa-search"></i> Buscar Producto
                                    </label>
                                    <input type="text" class="form-control" id="searchProductoReceta"
                                        placeholder="Escribe nombre o código de barras...">
                                    <small class="text-muted">Búsqueda en tiempo real</small>
                                </div>
                                <div id="productosListaReceta" class="mb-3">
                                    <div class="text-muted text-center py-3">
                                        <i class="fas fa-search" style="font-size: 1.5rem;"></i>
                                        <p>Busca un producto para comenzar</p>
                                    </div>
                                </div>
                                <div class="card">
                                    <div class="card-header"><h6 class="mb-0">Productos en Receta</h6></div>
                                    <div class="table-responsive">
                                        <table class="table mb-0">
                                            <thead>
                                                <tr>
                                                    <th>Producto</th>
                                                    <th style="width: 80px;">Cantidad</th>
                                                    <th style="width: 50px;"></th>
                                                </tr>
                                            </thead>
                                            <tbody id="recetaTableBody">
                                                <tr><td colspan="3" class="text-center text-muted py-2">No hay productos</td></tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <button class="btn btn-primary w-100 mt-3" onclick="window.app.guardarReceta()">
                                    <i class="fas fa-save"></i> Guardar Receta
                                </button>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-5">
                        <div class="card mb-4 sticky-top" style="top: 20px;">
                            <div class="card-header">
                                <h5 class="mb-0"><i class="fas fa-chart-pie"></i> Nutrientes de Receta</h5>
                            </div>
                            <div class="card-body" id="resumenNutrientesReceta">
                                <div class="alert alert-info text-center">
                                    <small>Agrega productos para ver nutrientes</small>
                                </div>
                            </div>
                        </div>
                        <div class="card">
                            <div class="card-header"><h5 class="mb-0"><i class="fas fa-book"></i> Mis Recetas</h5></div>
                            <div class="card-body">
                                ${recetas.length > 0 ? `
                                    <div class="list-group">
                                        ${recetas.map(receta => `
                                            <div class="list-group-item">
                                                <div class="d-flex w-100 justify-content-between align-items-start">
                                                    <div style="flex-grow: 1;">
                                                        <h6 class="mb-1">${receta.nombre}</h6>
                                                        <small class="text-muted">${receta.productos ? receta.productos.length : 0} productos</small>
                                                    </div>
                                                    <button class="btn btn-sm btn-danger ms-2"
                                                        onclick="Data.deleteReceta('${user.correo ?? user.email}', '${receta.id}'); window.app.showView('recetas');">
                                                        <i class="fas fa-trash"></i>
                                                    </button>
                                                </div>
                                            </div>
                                        `).join("")}
                                    </div>
                                ` : `
                                    <div class="text-center text-muted py-4">
                                        <p><i class="fas fa-book" style="font-size: 2rem;"></i></p>
                                        <p>No tienes recetas guardadas aún</p>
                                    </div>
                                `}
                            </div>
                        </div>
                    </div>
                </div>
                <button class="btn btn-secondary mt-3 w-100" onclick="window.app.showView('dashboard')">
                    <i class="fas fa-arrow-left"></i> Volver al Dashboard
                </button>
            </div>`;
    }

    static getReporteView(user) {
        return `
            <div class="container mt-4">
                <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0"><i class="fas fa-chart-bar"></i> Reportes de Avance</h5>
                    </div>
                    <div class="card-body">
                        <div class="row mb-4">
                            <div class="col-md-3">
                                <label for="startDate" class="form-label">Fecha Inicio</label>
                                <input type="date" class="form-control" id="startDate">
                            </div>
                            <div class="col-md-3">
                                <label for="endDate" class="form-label">Fecha Fin</label>
                                <input type="date" class="form-control" id="endDate">
                            </div>
                            <div class="col-md-3" style="display: flex; align-items: flex-end;">
                                <button class="btn btn-primary w-100" id="generateReportBtn">
                                    <i class="fas fa-chart-line"></i> Generar Reporte
                                </button>
                            </div>
                            <div class="col-md-3" style="display: flex; align-items: flex-end;">
                                <button class="btn btn-danger w-100" id="generatePDFBtn">
                                    <i class="fas fa-file-pdf"></i> Descargar PDF
                                </button>
                            </div>
                        </div>
                        <div id="reporteData" class="mt-4">
                            <div class="text-center text-muted py-4">
                                <p>Selecciona un rango de fechas para ver el reporte</p>
                            </div>
                        </div>
                    </div>
                </div>
                <button class="btn btn-secondary mt-3 w-100" onclick="window.app.showView('dashboard')">
                    <i class="fas fa-arrow-left"></i> Volver al Dashboard
                </button>
            </div>`;
    }

    static getRetroalimentacionView(user, mensajes = []) {
        const email = user.correo ?? user.email;
        return `
            <div class="container mt-4">
                <div class="card">
                    <div class="card-header">
                        <h5 class="mb-0"><i class="fas fa-comments"></i> Retroalimentación con Nutricionista</h5>
                    </div>
                    <div class="card-body">
                        <!-- Nuevo mensaje -->
                        <div class="mb-4">
                            <label class="form-label fw-bold">Nuevo mensaje</label>
                            <textarea class="form-control" id="nuevoMensaje" rows="3"
                                placeholder="Escribe tu mensaje para tu nutricionista..."></textarea>
                            <button class="btn btn-primary mt-2" onclick="window.app.enviarRetroalimentacion()">
                                <i class="fas fa-paper-plane"></i> Enviar mensaje
                            </button>
                        </div>

                        <hr>
                        <h6 class="mb-3">Historial de conversaciones</h6>

                        ${mensajes.length === 0 ? `
                            <div class="text-center text-muted py-4">
                                <i class="fas fa-comments" style="font-size:2rem;"></i>
                                <p class="mt-2">No hay mensajes aún. ¡Envía el primero a tu nutricionista!</p>
                            </div>
                        ` : mensajes.map((retro) => `
                            <div class="card mb-3">
                                <div class="card-header bg-light">
                                    <small class="text-muted">Conversación del ${new Date(retro.fechaCreacionUtc).toLocaleDateString("es-CR")}</small>
                                </div>
                                <div class="card-body">
                                    ${retro.mensajes.map((m) => `
                                        <div class="d-flex ${m.autor === email ? "justify-content-end" : "justify-content-start"} mb-2">
                                            <div class="card" style="max-width:75%; background:${m.autor === email ? "#d4edda" : "#f8f9fa"};">
                                                <div class="card-body py-2 px-3">
                                                    <small class="text-muted d-block">
                                                        <strong>${m.autor === email ? "Tú" : "Nutricionista"}</strong>
                                                        · ${new Date(m.fechaUtc).toLocaleString("es-CR")}
                                                    </small>
                                                    <p class="mb-0 mt-1">${m.contenido ?? m.mensaje ?? m.Mensaje ?? "—"}</p>
                                                </div>
                                            </div>
                                        </div>
                                    `).join("")}
                                    <!-- Responder a esta conversación -->
                                    <div class="mt-3 pt-2 border-top">
                                        <div class="input-group">
                                            <input type="text" class="form-control form-control-sm"
                                                id="respuesta-${retro.id}"
                                                placeholder="Escribe una respuesta...">
                                            <button class="btn btn-sm btn-outline-primary"
                                                onclick="window.app.responderRetroalimentacion('${retro.id}')">
                                                <i class="fas fa-reply"></i> Responder
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        `).join("")}
                    </div>
                </div>
                <button class="btn btn-secondary mt-3 w-100" onclick="window.app.showView('dashboard')">
                    <i class="fas fa-arrow-left"></i> Volver al Dashboard
                </button>
            </div>`;
    }
}