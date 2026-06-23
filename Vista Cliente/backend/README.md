# Backend NutriTEC API

API REST desarrollada con ASP.NET Core 10 para la plataforma NutriTEC.

## Tecnologías
- ASP.NET Core 10
- Entity Framework Core
- SQL Server
- MongoDB (para feedback de nutricionistas)
- C# 14

## Estructura del Proyecto

```
NutriTECAPI/
├── Models/                 # Entidades de la base de datos
│   ├── Usuario.cs
│   ├── Medida.cs
│   ├── Producto.cs
│   ├── Consumo.cs
│   └── Receta.cs
├── DTOs/                   # Data Transfer Objects
│   ├── AuthDTO.cs
│   └── EntidadesDTO.cs
├── Services/               # Lógica de negocio
├── Controllers/            # Controladores API
├── Data/                   # Contexto de Base de Datos
│   └── NutriTECContext.cs
├── Program.cs              # Configuración de la aplicación
└── appsettings.json        # Configuración

```

## Instalación

### Requisitos
- .NET 10 SDK
- SQL Server (local o remoto)

### Pasos
1. Navega a la carpeta del proyecto
   ```bash
   cd backend/NutriTECAPI
   ```

2. Restaura las dependencias
   ```bash
   dotnet restore
   ```

3. Configura la conexión a SQL Server en `appsettings.json`

4. Ejecuta las migraciones
   ```bash
   dotnet ef database update
   ```

5. Ejecuta la aplicación
   ```bash
   dotnet run
   ```

## API Endpoints (Próximo)

### Autenticación
- `POST /api/auth/register` - Registrar nuevo usuario
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/logout` - Cerrar sesión

### Usuarios
- `GET /api/usuarios/{id}` - Obtener perfil
- `PUT /api/usuarios/{id}` - Actualizar perfil

### Medidas
- `GET /api/medidas/{usuarioId}` - Obtener medidas
- `POST /api/medidas` - Registrar nueva medida

### Productos
- `GET /api/productos` - Listar productos
- `GET /api/productos/buscar?q=término` - Buscar productos
- `POST /api/productos` - Crear producto

### Consumos
- `GET /api/consumos/{usuarioId}` - Obtener consumos
- `POST /api/consumos` - Registrar consumo

### Recetas
- `GET /api/recetas/{usuarioId}` - Obtener recetas
- `POST /api/recetas` - Crear receta
- `DELETE /api/recetas/{id}` - Eliminar receta

## Configuración de Base de Datos

La conexión a SQL Server debe configurarse en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=NutriTEC;Trusted_Connection=true;"
  }
}
```

## Migraciones

Crear una nueva migración:
```bash
dotnet ef migrations add NombreMigracion
```

Aplicar migraciones:
```bash
dotnet ef database update
```

## Próximos Pasos

1. Crear controladores para cada entidad
2. Implementar servicios de autenticación y autorización
3. Agregar validaciones
4. Crear migraciones iniciales
5. Implementar integración con MongoDB
6. Agregar logging y manejo de errores

## Notas

- El proyecto usa Entity Framework Core para acceso a datos
- Se implementarán JWT para autenticación
- La contraseña debe encriptarse en producción
- Se requiere configurar CORS para comunicación con el frontend

## Autores

Equipo NutriTEC - 2026
