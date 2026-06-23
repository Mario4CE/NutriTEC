# Publicación de páginas web en Azure

Este repositorio contiene dos frontends que se pueden publicar como **Azure Static Web Apps** independientes:

- **Vista Administrador**: `nutritec-admin` (React + Vite)
- **Vista Cliente**: `Vista Cliente/frontend` (HTML/CSS/JavaScript estático)
- **Vista Nutricionista**: `Web/nutricionista` (web estática)

## 1. Vista Administrador

### Configuración de APIs

La aplicación lee las URLs desde variables de entorno de Vite:

```bash
VITE_SQL_API_BASE_URL=https://<tu-api-sql>.azurewebsites.net/api
VITE_MONGO_API_BASE_URL=https://<tu-api-mongo>.azurewebsites.net/api
```

Si no se configuran, se usan `/api/sql` y `/api/mongo`, que funcionan con el proxy local de Vite durante desarrollo.

### Build local

```bash
cd nutritec-admin
npm ci
npm run build
```

La carpeta para publicar en Azure es:

```text
nutritec-admin/dist
```

### Azure Static Web Apps

Valores recomendados al crear el recurso o el workflow:

- App location: `nutritec-admin`
- Api location: dejar vacío
- Output location: `dist`
- App settings:
  - `VITE_SQL_API_BASE_URL`
  - `VITE_MONGO_API_BASE_URL`

## 2. Vista Cliente

La vista cliente no requiere compilación. Azure debe servir directamente la carpeta:

```text
Vista Cliente/frontend
```

### Configuración de APIs

Edita `Vista Cliente/frontend/config.js` antes de publicar o automatiza el reemplazo en tu pipeline:

```js
window.NUTRITEC_CONFIG = {
  USE_MOCKS: false,
  SQL_API_BASE_URL: "https://<tu-api-sql>.azurewebsites.net/api",
  MONGO_API_BASE_URL: "https://<tu-api-mongo>.azurewebsites.net/api",
};
```

Si dejas `/api/sql` y `/api/mongo`, necesitarás configurar rutas/proxy equivalentes en Azure.

### Azure Static Web Apps

Valores recomendados:

- App location: `Vista Cliente/frontend`
- Api location: dejar vacío
- Output location: dejar vacío o `.`

## 3. Vista Nutricionista

La vista de nutricionista se publica como una tercera **Azure Static Web App** independiente desde:

```text
Web/nutricionista
```

Si esta vista usa APIs desde JavaScript, carga o adapta `Web/nutricionista/config.js` con las URLs públicas de Azure:

```js
window.NUTRITEC_CONFIG = {
  USE_MOCKS: false,
  SQL_API_BASE_URL: "https://<tu-api-sql>.azurewebsites.net/api",
  MONGO_API_BASE_URL: "https://<tu-api-mongo>.azurewebsites.net/api",
};
```

Valores recomendados para Azure Static Web Apps:

- App location: `Web/nutricionista`
- Api location: dejar vacío
- Output location: dejar vacío o `.`

> Nota: actualmente la carpeta `Web/nutricionista` queda preparada con archivos de configuración para Azure. Antes de publicarla, asegúrate de que contenga el `index.html` y los recursos de la web del nutricionista.

## 4. Backend y CORS

Cuando publiques las APIs en Azure App Service, habilita CORS para los dominios generados por Azure Static Web Apps, por ejemplo:

```text
https://<admin>.azurestaticapps.net
https://<cliente>.azurestaticapps.net
https://<nutricionista>.azurestaticapps.net
```

También puedes agregar tus dominios personalizados si los configuras en Azure.
