/**
 * data.js - Gestión de datos locales
 * Maneja usuarios, medidas, consumos y recetas
 */

class Data {
    /**
     * Agregar una medida al usuario
     * @param {string} userEmail - Email del usuario
     * @param {object} medida - Datos de la medida
     * @returns {boolean}
     */
    static addMedida(userEmail, medida) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user) return false;

        if (!user.medidas) user.medidas = [];
        
        user.medidas.push({
            id: Date.now().toString(),
            ...medida
        });

        localStorage.setItem('users', JSON.stringify(users));
        
        // Actualizar usuario actual si es el que está logueado
        if (Auth.getCurrentUser().email === userEmail) {
            const currentUser = Auth.getCurrentUser();
            currentUser.medidas = user.medidas;
            localStorage.setItem('currentUser', JSON.stringify(currentUser));
        }

        return true;
    }

    /**
     * Obtener medidas de un usuario en un rango de fechas
     * @param {string} userEmail - Email del usuario
     * @param {string} startDate - Fecha inicio (YYYY-MM-DD)
     * @param {string} endDate - Fecha fin (YYYY-MM-DD)
     * @returns {array}
     */
    static getMedidasByRange(userEmail, startDate, endDate) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !user.medidas) return [];

        const start = new Date(startDate);
        const end = new Date(endDate);

        return user.medidas.filter(medida => {
            const medidaDate = new Date(medida.fecha);
            return medidaDate >= start && medidaDate <= end;
        });
    }

    /**
     * Obtener todas las medidas de un usuario
     * @param {string} userEmail - Email del usuario
     * @returns {array}
     */
    static getAllMedidas(userEmail) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !user.medidas) return [];

        return user.medidas.sort((a, b) => new Date(b.fecha) - new Date(a.fecha));
    }

    /**
     * Agregar un consumo diario
     * @param {string} userEmail - Email del usuario
     * @param {object} consumo - Datos del consumo
     * @returns {boolean}
     */
    static addConsumo(userEmail, consumo) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user) return false;

        if (!user.consumos) user.consumos = [];

        user.consumos.push({
            id: Date.now().toString(),
            ...consumo,
            fecha: new Date().toISOString()
        });

        localStorage.setItem('users', JSON.stringify(users));
        return true;
    }

    /**
     * Obtener consumos del día actual
     * @param {string} userEmail - Email del usuario
     * @returns {array}
     */
    static getConsumoHoy(userEmail) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !user.consumos) return [];

        const hoy = new Date().toDateString();
        return user.consumos.filter(c => {
            const consumoDate = new Date(c.fecha).toDateString();
            return consumoDate === hoy;
        });
    }

    /**
     * Obtener consumos por fecha
     * @param {string} userEmail - Email del usuario
     * @param {string} fecha - Fecha (YYYY-MM-DD)
     * @returns {array}
     */
    static getConsumosByFecha(userEmail, fecha) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !user.consumos) return [];

        return user.consumos.filter(c => {
            const consumoDate = new Date(c.fecha).toISOString().split('T')[0];
            return consumoDate === fecha;
        });
    }

    /**
     * Obtener consumos por rango de fechas
     * @param {string} userEmail - Email del usuario
     * @param {string} fechaInicio - Fecha inicio (YYYY-MM-DD)
     * @param {string} fechaFin - Fecha fin (YYYY-MM-DD)
     * @returns {array}
     */
    static getConsumosByDateRange(userEmail, fechaInicio, fechaFin) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !user.consumos) return [];

        const inicio = new Date(fechaInicio);
        const fin = new Date(fechaFin);
        fin.setHours(23, 59, 59, 999); // Include entire end date

        return user.consumos.filter(c => {
            const consumoDate = new Date(c.fecha);
            return consumoDate >= inicio && consumoDate <= fin;
        });
    }

    /**
     * Agregar una receta
     * @param {string} userEmail - Email del usuario
     * @param {object} receta - Datos de la receta
     * @returns {boolean}
     */
    static addReceta(userEmail, receta) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user) return false;

        if (!user.recetas) user.recetas = [];

        user.recetas.push({
            id: Date.now().toString(),
            ...receta,
            fechaCreacion: new Date().toISOString()
        });

        localStorage.setItem('users', JSON.stringify(users));
        return true;
    }

    /**
     * Obtener todas las recetas de un usuario
     * @param {string} userEmail - Email del usuario
     * @returns {array}
     */
    static getRecetas(userEmail) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !user.recetas) return [];

        return user.recetas;
    }

    /**
     * Eliminar una receta
     * @param {string} userEmail - Email del usuario
     * @param {string} recetaId - ID de la receta
     * @returns {boolean}
     */
    static deleteReceta(userEmail, recetaId) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !user.recetas) return false;

        user.recetas = user.recetas.filter(r => r.id !== recetaId);
        localStorage.setItem('users', JSON.stringify(users));
        return true;
    }

    /**
     * Actualizar datos del usuario
     * @param {object} updatedUser - Datos actualizados del usuario
     * @returns {boolean}
     */
    static updateUser(updatedUser) {
        const users = Auth.getAllUsers();
        const userIndex = users.findIndex(u => u.email === updatedUser.email);

        if (userIndex === -1) return false;

        users[userIndex] = { ...users[userIndex], ...updatedUser };
        localStorage.setItem('users', JSON.stringify(users));

        // Actualizar usuario actual
        localStorage.setItem('currentUser', JSON.stringify(updatedUser));

        return true;
    }

    /**
     * Calcular calorías totales del día
     * @param {string} userEmail - Email del usuario
     * @param {string} fecha - Fecha (YYYY-MM-DD) - si no se proporciona, usa hoy
     * @returns {number}
     */
    static calcularCaloriasDelDia(userEmail, fecha = null) {
        const consumos = fecha 
            ? this.getConsumosByFecha(userEmail, fecha)
            : this.getConsumoHoy(userEmail);

        return consumos.reduce((total, consumo) => total + (consumo.calorias || 0), 0);
    }

    /**
     * Obtener datos de nutrientes del día
     * @param {string} userEmail - Email del usuario
     * @param {string} fecha - Fecha (YYYY-MM-DD)
     * @returns {object}
     */
    static getNutrientesDia(userEmail, fecha = null) {
        const consumos = fecha 
            ? this.getConsumosByFecha(userEmail, fecha)
            : this.getConsumoHoy(userEmail);

        let totalNutrientes = {
            calorias: 0,
            proteinas: 0,
            grasas: 0,
            carbohidratos: 0,
            sodio: 0
        };

        // Iterar sobre cada consumo y sumar los nutrientes de sus productos
        consumos.forEach(consumo => {
            if (consumo.productos && Array.isArray(consumo.productos)) {
                consumo.productos.forEach(prod => {
                    if (prod.producto) {
                        const cantidad = prod.cantidad || 100; // Default 100g
                        // Divide by 100 because nutrients are per 100g
                        totalNutrientes.calorias += (prod.producto.energia || 0) * (cantidad / 100);
                        totalNutrientes.proteinas += (prod.producto.proteina || 0) * (cantidad / 100);
                        totalNutrientes.grasas += (prod.producto.grasa || 0) * (cantidad / 100);
                        totalNutrientes.carbohidratos += (prod.producto.carbohidratos || 0) * (cantidad / 100);
                        totalNutrientes.sodio += (prod.producto.sodio || 0) * (cantidad / 100);
                    }
                });
            }
        });

        return totalNutrientes;
    }

    /**
     * Agregar un producto a la base de datos de productos
     * @param {object} producto - Datos del producto
     * @returns {boolean}
     */
    static addProducto(producto) {
        let productos = JSON.parse(localStorage.getItem('productos') || '[]');

        // Verificar que no exista el código de barras
        if (productos.some(p => p.codigoBarras === producto.codigoBarras)) {
            return false;
        }

        productos.push({
            id: Date.now().toString(),
            ...producto,
            estado: 'pendiente', // pendiente, aprobado, rechazado
            fechaCreacion: new Date().toISOString()
        });

        localStorage.setItem('productos', JSON.stringify(productos));
        return true;
    }

    /**
     * Obtener todos los productos aprobados
     * @returns {array}
     */
    static getProductosAprobados() {
        const productos = JSON.parse(localStorage.getItem('productos') || '[]');
        return productos.filter(p => p.estado === 'aprobado');
    }

    /**
     * Buscar productos
     * @param {string} query - Texto de búsqueda
     * @param {string} tipo - 'nombre' o 'codigoBarras'
     * @returns {array}
     */
    static buscarProductos(query, tipo = 'nombre') {
        const productos = this.getProductosAprobados();

        if (tipo === 'nombre') {
            return productos.filter(p => 
                p.descripcion.toLowerCase().includes(query.toLowerCase())
            );
        } else if (tipo === 'codigoBarras') {
            return productos.filter(p => p.codigoBarras === query);
        }

        return [];
    }

    /**
     * Obtener un producto por ID
     * @param {string} productoId - ID del producto
     * @returns {object}
     */
    static getProducto(productoId) {
        const productos = JSON.parse(localStorage.getItem('productos') || '[]');
        return productos.find(p => p.id == productoId);  // Comparación flexible para soportar string/number
    }

    /**
     * Agregar un producto al consumo de un día específico
     * @param {string} userEmail - Email del usuario
     * @param {string} tiempoComida - Tiempo de comida
     * @param {number} productoId - ID del producto
     * @param {number} cantidad - Cantidad de porciones
     * @returns {boolean}
     */
    static addProductoAlConsumo(userEmail, tiempoComida, productoId, cantidad) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !tiempoComida || !productoId) return false;

        if (!user.consumos) user.consumos = [];

        // Buscar o crear consumo del día para ese tiempo de comida
        const hoy = new Date().toISOString().split('T')[0];
        let consumo = user.consumos.find(c => {
            const fechaConsumo = new Date(c.fecha).toISOString().split('T')[0];
            return fechaConsumo === hoy && c.tiempoComida === tiempoComida;
        });

        if (!consumo) {
            consumo = {
                id: Date.now().toString(),
                fecha: new Date().toISOString(),
                tiempoComida: tiempoComida,
                productos: []
            };
            user.consumos.push(consumo);
        }

        // Agregar producto al consumo
        const producto = this.getProducto(productoId);
        if (!producto) return false;

        // Verificar si el producto ya existe, si sí, actualizar cantidad
        const productoExistente = consumo.productos.find(p => p.productoId == productoId);
        
        if (productoExistente) {
            productoExistente.cantidad += cantidad;
        } else {
            consumo.productos.push({
                productoId: productoId,
                cantidad: cantidad,
                producto: producto
            });
        }

        localStorage.setItem('users', JSON.stringify(users));
        return true;
    }

    /**
     * Eliminar un producto del consumo
     * @param {string} userEmail - Email del usuario
     * @param {string} consumoId - ID del consumo
     * @param {number} productoId - ID del producto
     * @returns {boolean}
     */
    static removeProductoDelConsumo(userEmail, consumoId, productoId) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !user.consumos) return false;

        const consumo = user.consumos.find(c => c.id === consumoId);
        if (!consumo) return false;

        consumo.productos = consumo.productos.filter(p => p.productoId != productoId);

        // Si no hay productos, eliminar el consumo
        if (consumo.productos.length === 0) {
            user.consumos = user.consumos.filter(c => c.id !== consumoId);
        }

        localStorage.setItem('users', JSON.stringify(users));
        return true;
    }

    /**
     * Actualizar cantidad de un producto en consumo
     * @param {string} userEmail - Email del usuario
     * @param {string} consumoId - ID del consumo
     * @param {number} productoId - ID del producto
     * @param {number} nuevaCantidad - Nueva cantidad
     * @returns {boolean}
     */
    static updateCantidadProducto(userEmail, consumoId, productoId, nuevaCantidad) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !user.consumos) return false;

        const consumo = user.consumos.find(c => c.id === consumoId);
        if (!consumo) return false;

        const producto = consumo.productos.find(p => p.productoId == productoId);
        if (!producto) return false;

        if (nuevaCantidad <= 0) {
            return this.removeProductoDelConsumo(userEmail, consumoId, productoId);
        }

        producto.cantidad = nuevaCantidad;
        localStorage.setItem('users', JSON.stringify(users));
        return true;
    }

    /**
     * Obtener consumo por ID
     * @param {string} userEmail - Email del usuario
     * @param {string} consumoId - ID del consumo
     * @returns {object}
     */
    static getConsumo(userEmail, consumoId) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user || !user.consumos) return null;

        return user.consumos.find(c => c.id === consumoId);
    }

    /**
     * Inicializar con productos de prueba
     */
    static initializeTestData() {
        const testUser = {
            id: '1',
            nombre: 'Juan',
            apellidos: 'Pérez',
            email: 'juan@test.com',
            password: '123456',
            edad: 30,
            fechaNacimiento: '1994-06-15',
            peso: 75,
            imc: 24.5,
            pais: 'Costa Rica',
            cintura: 85,
            cuello: 38,
            caderas: 98,
            porcentajeMusculo: 45,
            porcentajeGrasa: 18,
            caloriasDiarias: 2200,
            foto: null,
            fechaRegistro: new Date().toISOString(),
            medidas: [],
            consumos: [],
            recetas: [],
            nutricionista: null
        };

        const testProducts = [
            {
                id: '1',
                codigoBarras: '1234567890',
                descripcion: 'Arroz blanco (1 taza)',
                porcion: '150g',
                energia: 206,
                grasa: 0.3,
                sodio: 2,
                carbohidratos: 45,
                proteina: 4,
                vitaminas: 'B1, B2',
                calcio: 10,
                hierro: 0.2,
                estado: 'aprobado'
            },
            {
                id: '2',
                codigoBarras: '1234567891',
                descripcion: 'Pechuga de pollo (100g)',
                porcion: '100g',
                energia: 165,
                grasa: 3.6,
                sodio: 74,
                carbohidratos: 0,
                proteina: 31,
                vitaminas: 'B3, B6',
                calcio: 11,
                hierro: 1,
                estado: 'aprobado'
            },
            {
                id: '3',
                codigoBarras: '1234567892',
                descripcion: 'Manzana roja (1 mediana)',
                porcion: '182g',
                energia: 95,
                grasa: 0.3,
                sodio: 2,
                carbohidratos: 25,
                proteina: 0.5,
                vitaminas: 'C',
                calcio: 11,
                hierro: 0.2,
                estado: 'aprobado'
            },
            {
                id: '4',
                codigoBarras: '1234567893',
                descripcion: 'Plátano mediano (1)',
                porcion: '118g',
                energia: 105,
                grasa: 0.3,
                sodio: 2,
                carbohidratos: 27,
                proteina: 1.3,
                vitaminas: 'B6, C',
                calcio: 5,
                hierro: 0.3,
                estado: 'aprobado'
            },
            {
                id: '5',
                codigoBarras: '1234567894',
                descripcion: 'Huevo cocido (1 grande)',
                porcion: '50g',
                energia: 78,
                grasa: 5.3,
                sodio: 63,
                carbohidratos: 0.6,
                proteina: 6.3,
                vitaminas: 'A, D, E',
                calcio: 25,
                hierro: 0.9,
                estado: 'aprobado'
            },
            {
                id: '6',
                codigoBarras: '1234567895',
                descripcion: 'Pan integral (1 rebanada)',
                porcion: '28g',
                energia: 80,
                grasa: 1.5,
                sodio: 150,
                carbohidratos: 14,
                proteina: 4,
                vitaminas: 'B1, B2',
                calcio: 20,
                hierro: 1,
                estado: 'aprobado'
            },
            {
                id: '7',
                codigoBarras: '1234567896',
                descripcion: 'Leche descremada (1 taza)',
                porcion: '240ml',
                energia: 80,
                grasa: 0.1,
                sodio: 130,
                carbohidratos: 12,
                proteina: 8,
                vitaminas: 'A, D',
                calcio: 300,
                hierro: 0.1,
                estado: 'aprobado'
            },
            {
                id: '8',
                codigoBarras: '1234567897',
                descripcion: 'Brócoli cocido (1 taza)',
                porcion: '156g',
                energia: 55,
                grasa: 0.6,
                sodio: 71,
                carbohidratos: 11,
                proteina: 3.7,
                vitaminas: 'C, K',
                calcio: 71,
                hierro: 0.8,
                estado: 'aprobado'
            }
        ];

        // Guardar datos de prueba
        const users = Auth.getAllUsers();
        if (!users.some(u => u.email === testUser.email)) {
            users.push(testUser);
            localStorage.setItem('users', JSON.stringify(users));
        }

        const existingProducts = JSON.parse(localStorage.getItem('productos') || '[]');
        const newProducts = testProducts.filter(tp => 
            !existingProducts.some(ep => ep.codigoBarras === tp.codigoBarras)
        );
        
        if (newProducts.length > 0) {
            existingProducts.push(...newProducts);
            localStorage.setItem('productos', JSON.stringify(existingProducts));
        }
    }

    /**
     * Variable temporal para almacenar productos de receta en edición
     */
    static _recetaEnEdicion = {
        productos: []
    };

    /**
     * Obtener productos temporales de receta en edición
     */
    static getProductosRecetaEnEdicion() {
        return this._recetaEnEdicion.productos;
    }

    /**
     * Agregar producto a receta en edición
     */
    static addProductoARecetaTemporal(productoId, cantidad) {
        const producto = this.getProducto(productoId);
        if (!producto) return false;

        const existente = this._recetaEnEdicion.productos.find(p => p.productoId == productoId);
        
        if (existente) {
            existente.cantidad += cantidad;
        } else {
            this._recetaEnEdicion.productos.push({
                productoId: productoId,
                cantidad: cantidad,
                producto: producto
            });
        }
        return true;
    }

    /**
     * Eliminar producto de receta en edición
     */
    static removeProductoDeRecetaTemporal(productoId) {
        this._recetaEnEdicion.productos = this._recetaEnEdicion.productos.filter(p => p.productoId != productoId);
    }

    /**
     * Actualizar cantidad de producto en receta en edición
     */
    static updateCantidadProductoRecetaTemporal(productoId, nuevaCantidad) {
        if (nuevaCantidad <= 0) {
            this.removeProductoDeRecetaTemporal(productoId);
            return true;
        }
        const prod = this._recetaEnEdicion.productos.find(p => p.productoId == productoId);
        if (prod) {
            prod.cantidad = nuevaCantidad;
            return true;
        }
        return false;
    }

    /**
     * Calcular nutrientes totales de productos en receta temporal
     */
    static calcularNutrientesRecetaTemporal() {
        let totales = {
            calorias: 0,
            proteinas: 0,
            grasas: 0,
            carbohidratos: 0,
            sodio: 0
        };

        this._recetaEnEdicion.productos.forEach(prod => {
            if (prod.producto) {
                const cantidad = prod.cantidad || 1;
                totales.calorias += (prod.producto.energia || 0) * cantidad;
                totales.proteinas += (prod.producto.proteina || 0) * cantidad;
                totales.grasas += (prod.producto.grasa || 0) * cantidad;
                totales.carbohidratos += (prod.producto.carbohidratos || 0) * cantidad;
                totales.sodio += (prod.producto.sodio || 0) * cantidad;
            }
        });

        return totales;
    }

    /**
     * Guardar receta
     */
    static saveReceta(userEmail, nombreReceta) {
        const users = Auth.getAllUsers();
        const user = users.find(u => u.email === userEmail);

        if (!user) return false;

        if (!user.recetas) user.recetas = [];

        if (!nombreReceta || this._recetaEnEdicion.productos.length === 0) {
            return false;
        }

        const receta = {
            id: Date.now().toString(),
            nombre: nombreReceta,
            fechaCreacion: new Date().toISOString(),
            productos: [...this._recetaEnEdicion.productos]
        };

        user.recetas.push(receta);
        localStorage.setItem('users', JSON.stringify(users));

        // Limpiar receta en edición
        this._recetaEnEdicion.productos = [];

        return true;
    }

    /**
     * Limpiar receta en edición
     */
    static clearRecetaEnEdicion() {
        this._recetaEnEdicion.productos = [];
    }
}

// Inicializar datos de prueba
document.addEventListener('DOMContentLoaded', () => {
    Data.initializeTestData();
});
