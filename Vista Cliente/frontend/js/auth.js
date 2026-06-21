/**
 * auth.js - Gestión de autenticación
 * Funciones para login, register y logout
 */

class Auth {
    /**
     * Login de usuario
     * @param {string} email - Email del usuario
     * @param {string} password - Contraseña
     * @returns {boolean} - True si el login fue exitoso
     */
    static login(email, password) {
        const users = this.getAllUsers();
        
        // Buscar usuario por email
        const user = users.find(u => u.email === email);

        if (!user) {
            console.log('Usuario no encontrado');
            return false;
        }

        // Validar contraseña (en producción usar hashing)
        if (user.password !== password) {
            console.log('Contraseña inválida');
            return false;
        }

        // Guardar usuario actual (sin contraseña)
        const loggedInUser = { ...user };
        delete loggedInUser.password;
        
        localStorage.setItem('currentUser', JSON.stringify(loggedInUser));
        localStorage.setItem('authToken', this.generateToken());

        // Disparar evento
        document.dispatchEvent(new CustomEvent('user-login', { 
            detail: loggedInUser 
        }));

        console.log('Login exitoso para:', email);
        return true;
    }

    /**
     * Registro de nuevo usuario
     * @param {object} userData - Datos del usuario
     * @returns {boolean} - True si el registro fue exitoso
     */
    static register(userData) {
        const users = this.getAllUsers();

        // Verificar si el email ya existe
        if (users.some(u => u.email === userData.email)) {
            console.log('Email ya registrado');
            return false;
        }

        // Crear nuevo usuario
        const newUser = {
            id: Date.now().toString(),
            nombre: userData.nombre,
            apellidos: userData.apellidos,
            email: userData.email,
            password: userData.password, // En producción encriptar
            edad: userData.edad,
            fechaNacimiento: userData.fechaNacimiento,
            peso: userData.peso,
            imc: userData.imc,
            pais: userData.pais,
            cintura: userData.cintura,
            cuello: userData.cuello,
            caderas: userData.caderas,
            porcentajeMusculo: userData.porcentajeMusculo,
            porcentajeGrasa: userData.porcentajeGrasa,
            caloriasDiarias: userData.caloriasDiarias,
            foto: null,
            fechaRegistro: new Date().toISOString(),
            medidas: [],
            consumos: [],
            recetas: [],
            nutricionista: null
        };

        // Guardar usuario
        users.push(newUser);
        localStorage.setItem('users', JSON.stringify(users));

        console.log('Nuevo usuario registrado:', userData.email);
        return true;
    }

    /**
     * Logout del usuario
     */
    static logout() {
        localStorage.removeItem('currentUser');
        localStorage.removeItem('authToken');
        
        document.dispatchEvent(new Event('user-logout'));
        
        console.log('Logout exitoso');
    }

    /**
     * Obtener todos los usuarios registrados
     * @returns {array} - Array de usuarios
     */
    static getAllUsers() {
        const users = localStorage.getItem('users');
        return users ? JSON.parse(users) : [];
    }

    /**
     * Obtener usuario actual
     * @returns {object} - Usuario actual o null
     */
    static getCurrentUser() {
        const user = localStorage.getItem('currentUser');
        return user ? JSON.parse(user) : null;
    }

    /**
     * Generar token de autenticación (mock)
     * @returns {string} - Token
     */
    static generateToken() {
        return 'token_' + Math.random().toString(36).substr(2, 9);
    }

    /**
     * Verificar si el usuario está autenticado
     * @returns {boolean}
     */
    static isAuthenticated() {
        return !!localStorage.getItem('authToken');
    }

    /**
     * Actualizar contraseña
     * @param {string} email - Email del usuario
     * @param {string} oldPassword - Contraseña anterior
     * @param {string} newPassword - Nueva contraseña
     * @returns {boolean}
     */
    static changePassword(email, oldPassword, newPassword) {
        const users = this.getAllUsers();
        const userIndex = users.findIndex(u => u.email === email);

        if (userIndex === -1 || users[userIndex].password !== oldPassword) {
            return false;
        }

        users[userIndex].password = newPassword;
        localStorage.setItem('users', JSON.stringify(users));
        return true;
    }
}
