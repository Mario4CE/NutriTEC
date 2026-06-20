# Frontend NutriTEC

Interfaz web de la plataforma NutriTEC para clientes/pacientes.

## Tecnologías
- HTML5
- CSS3
- JavaScript Vanilla
- Bootstrap 5
- Font Awesome 6

## Estructura del Proyecto

```
frontend/
├── index.html              # Archivo principal
├── css/
│   └── styles.css         # Estilos personalizados
├── js/
│   ├── app.js             # Controlador principal
│   ├── auth.js            # Gestión de autenticación
│   ├── data.js            # Gestión de datos locales (localStorage)
│   └── views.js           # Vistas HTML dinámicas
├── pages/                 # Futuras páginas adicionales
└── assets/                # Imágenes y recursos
```

## Cómo Usar

1. **Abre el archivo `index.html` en un navegador**
   - Simplemente abre el archivo en tu navegador preferido
   - No requiere servidor web para funcionar localmente

2. **Datos de Prueba**
   - Email: `juan@test.com`
   - Contraseña: `123456`

## Funcionalidades Implementadas

### ✅ Autenticación
- Login
- Registro de nuevos usuarios
- Logout

### ✅ Dashboard
- Vista general con estadísticas
- Progreso de calorías del día
- Accesos rápidos a funciones principales

### ✅ Gestión de Medidas
- Registrar medidas corporales
- Historial de medidas
- Rango de fechas customizable

### ✅ Perfil de Usuario
- Ver información personal
- Editar datos

### ✅ Gestión de Recetas
- Crear recetas personalizadas
- Listar recetas guardadas
- Eliminar recetas

### ✅ Reportes
- Generar reportes por rango de fechas
- Vista de datos en tabla
- Preparado para exportar PDF

### 🔄 Registro de Consumo
- Búsqueda de productos
- Registrar consumo por tiempo de comida
- Ver datos nutricionales diarios

## Almacenamiento de Datos

Los datos se guardan en `localStorage` del navegador:
- `users` - Lista de usuarios registrados
- `currentUser` - Usuario actualmente logueado
- `authToken` - Token de sesión
- `productos` - Base de datos de productos

## Próximos Pasos

1. **Integración con Backend** - Conectar con API en .NET
2. **Búsqueda de Productos** - Implementar búsqueda funcional
3. **Generación de PDF** - Usar librería como jsPDF
4. **Gráficos** - Usar Chart.js para visualizaciones
5. **App Móvil** - Versión responsive mejorada

## Notas de Desarrollo

- Los datos actualmente se guardan en `localStorage` (temporal)
- Las contraseñas NO están encriptadas (para desarrollo)
- Se usarán las APIs del backend cuando esté disponible
- Implementar validaciones más robustas cuando sea necesario

## Dependencias Externas

- **Bootstrap 5.3.0** - Framework CSS
- **Font Awesome 6.4.0** - Iconos
- Ambas se cargan desde CDN

## Autores

Equipo NutriTEC - 2026
