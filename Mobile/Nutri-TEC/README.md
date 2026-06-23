# NutriTEC - Aplicación Móvil MAUI

Aplicación móvil multiplataforma para el seguimiento de consumo diario de alimentos y gestión de recetas personalizadas.

## 🎯 Funcionalidades Principales

### 1. **Autenticación** 
- Login de usuarios existentes
- Registro de nuevas cuentas
- Almacenamiento local seguro de credenciales

### 2. **Registro Diario de Consumo**
- Seleccionar tiempo de comida (Desayuno, Almuerzo, Cena, Merienda)
- Búsqueda de productos por nombre o código de barras
- Agregar/editar productos al consumo
- Visualización de nutrientes en tiempo real (calorías, proteínas, carbohidratos)
- Guardar consumo diario

### 3. **Gestión de Recetas**
- Crear recetas personalizadas con múltiples productos
- Especificar cantidades en gramos
- Cálculo automático de nutrientes totales
- Ver lista de recetas guardadas
- Usar recetas en el registro diario
- Eliminar recetas

## 🛠 Requisitos Previos

- .NET 10 SDK instalado
- Visual Studio 2022 o VS Code con extensión MAUI
- Emulador Android o iOS (según la plataforma)

## 📦 Instalación y Compilación

### 1. Restaurar dependencias
```bash
dotnet restore
```

### 2. Compilar el proyecto
```bash
dotnet build
```

### 3. Ejecutar en emulador Android
```bash
dotnet run -f net10.0-android
```

### 4. Ejecutar en emulador iOS (macOS)
```bash
dotnet run -f net10.0-ios
```

### 5. Ejecutar en Windows
```bash
dotnet run -f net10.0-windows10.0.19041.0
```

## 📁 Estructura del Proyecto

```
Nutri-TEC/
├── Models/                    # Modelos de datos
│   ├── Usuario.cs
│   ├── Producto.cs
│   ├── Consumo.cs
│   └── Receta.cs
├── Services/                  # Servicios y lógica
│   └── DataService.cs        # Almacenamiento local JSON
├── Views/                     # Páginas XAML
│   ├── LoginPage.xaml
│   ├── RegistroPage.xaml
│   ├── RegistroConsumoPage.xaml
│   ├── CrearRecetaPage.xaml
│   └── MisRecetasPage.xaml
├── AppShell.xaml             # Navegación principal
├── MauiProgram.cs            # Configuración de DI
└── App.xaml                  # Estilos globales
```

## 🔐 Credenciales Demo

Para probar la aplicación:
- **Email:** juan@test.com
- **Contraseña:** 123456

## 💾 Almacenamiento Local

Los datos se guardan en `FileSystem.AppDataDirectory` como archivos JSON:

- `usuarios.json` - Usuarios registrados
- `productos.json` - Base de datos de productos
- `consumos.json` - Registro de consumos diarios
- `recetas.json` - Recetas creadas por usuarios

## 📊 Productos Disponibles por Defecto

La aplicación incluye 8 productos de prueba:
1. Arroz blanco (206 kcal/100g)
2. Pechuga de pollo (165 kcal/100g)
3. Manzana roja (95 kcal/100g)
4. Plátano (105 kcal/100g)
5. Huevo (78 kcal/100g)
6. Pan blanco (80 kcal/100g)
7. Leche entera (80 kcal/100ml)
8. Brócoli (55 kcal/100g)

## 🔄 Flujo de la Aplicación

```
Login/Registro
    ↓
Dashboard (TabBar)
├─ Registrar Consumo
│  ├─ Seleccionar tiempo de comida
│  ├─ Buscar productos
│  ├─ Agregar a consumo
│  └─ Crear receta (si es necesario)
│
└─ Mis Recetas
   ├─ Ver recetas guardadas
   ├─ Usar receta en consumo
   ├─ Crear nueva receta
   └─ Eliminar receta
```

## 🎨 Paleta de Colores

- **Verde (#2ecc71)** - Acciones positivas, elementos principales
- **Azul (#3498db)** - Información secundaria
- **Rojo (#ff6b6b)** - Acciones de eliminación
- **Gris (#999, #ddd)** - Textos secundarios y bordes

## 🚀 Próximas Mejoras

- [ ] Historial de consumos
- [ ] Gráficos y reportes de progreso
- [ ] Sincronización con backend
- [ ] Notificaciones de recordatorios
- [ ] Exportar datos a PDF
- [ ] Compartir recetas

## 📝 Notas de Desarrollo

- La app utiliza inyección de dependencias de Microsoft
- El almacenamiento es completamente local (sin internet requerido)
- Los datos se persisten entre sesiones
- El diseño es responsive para diferentes tamaños de pantalla

## 🐛 Troubleshooting

**"El emulador no inicia"**
- Verifica que tienes Android SDK instalado
- Intenta crear un nuevo AVD (Android Virtual Device)

**"Error al restaurar dependencias"**
```bash
dotnet nuget locals all --clear
dotnet restore
```

**"Página en blanco al iniciar"**
- Asegúrate de estar en Login (verificar en AppShell.xaml)
- Revisa la consola de debug para errores

## 📞 Soporte

Para reportar problemas o sugerencias, contacta al equipo de desarrollo.

---

**Versión:** 1.0
**Última actualización:** 2026-06-19
**Target Framework:** .NET 10
