# Vista Nutricionista en Azure

Esta carpeta queda preparada para publicar la vista web de nutricionista como una Azure Static Web App independiente.

## Publicación recomendada

- App location: `Web/nutricionista`
- Api location: dejar vacío
- Output location: dejar vacío o `.`

Antes de publicar, coloca aquí el `index.html` y los recursos (`css`, `js`, `assets`, etc.) de la vista nutricionista.

## Configuración de APIs

Si la vista usa las APIs de NutriTEC desde JavaScript, carga `config.js` antes del script principal y cambia las URLs por las APIs publicadas en Azure App Service:

```js
window.NUTRITEC_CONFIG = {
  USE_MOCKS: false,
  SQL_API_BASE_URL: "https://<tu-api-sql>.azurewebsites.net/api",
  MONGO_API_BASE_URL: "https://<tu-api-mongo>.azurewebsites.net/api",
};
```
