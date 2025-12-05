# 📦 Manual de Despliegue  
## Sistema POS — Publicación General

## 1. ✔️ Preparación del Entorno

### 1.1. Base de Datos
- Crear o actualizar la base de datos en el hosting.  
- Ejecutar los scripts necesarios (creación, actualización y datos base).  
- Verificar la conexión a la base de datos.

### 1.2. Configuración de la API
Editar el archivo `appsettings.json` o `appsettings.Production.json`:

```
"ConnectionStrings": {
  "DefaultConnection": "Server=<host>;Database=<db>;User Id=<user>;Password=<pass>;"
}
```

Asegurar:
- Cadena de conexión correcta  
- Entorno configurado en `Production`

## 2. 🚀 Publicación del Backend (API .NET)

1. Abrir Visual Studio 2022.  
2. En el Explorador de soluciones, clic derecho sobre `SistemaPOS.API`.  
3. Seleccionar **Publicar**.  
4. Crear o seleccionar un perfil de publicación tipo **Carpeta (Folder)**.  
5. Configurar modo **Release**, framework y carpeta de salida.  
6. Hacer clic en **Publicar**.

## 3. 📤 Subida del Backend al Hosting

1. Acceder al administrador de archivos o usar FTP.  
2. Subir el contenido publicado.  
3. Verificar:
   - Archivos `.dll`  
   - Archivo ejecutable principal  
   - Carpeta `wwwroot/`  
4. Reiniciar la aplicación si el hosting lo requiere.

## 4. 🎨 Despliegue del Frontend

1. Comprimir la carpeta `wwwroot/`.  
2. Subir el archivo `.zip` al hosting.  
3. Extraerlo en la carpeta pública (`wwwroot/`).  
4. Verificar estructura:

```
wwwroot/
 ├── index.html
 ├── css/
 ├── js/
 ├── assets/
 └── .....
```

## 5. 🔗 Conexión Frontend → API

Asegurar que el frontend apunte a:

```
https://tu-dominio.com/api
```

## 6. 🧪 Validación del Despliegue

### Frontend
- Carga correcta  
- Navegación funcional  

### API
- Login  
- Productos  
- Ventas  
- Dashboard  

## 7. 🔄 Actualización del Sistema

1. Actualizar BD si aplica.  
2. Publicar de nuevo desde Visual Studio.  
3. Subir nuevos archivos de API y frontend.  

## 8. 🛠️ Problemas Comunes

### API no responde
- Revisar cadena de conexión  
- Configuración del hosting  

### Frontend no conecta
- URL incorrecta  
- CORS  

### Pantalla en blanco
- Archivos mal ubicados  
