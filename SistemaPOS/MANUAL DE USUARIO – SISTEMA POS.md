# 🧾 MANUAL DE USUARIO – SISTEMA POS

## 📘 Introducción
El **Sistema POS (Point of Sale)** es una aplicación web diseñada para la **gestión integral de ventas, productos, usuarios y sedes** de una cadena de tiendas.  
Permite a los **administradores generales**, **administradores locales** y **cajeros** realizar sus tareas de forma centralizada, segura y eficiente.

Este manual tiene como objetivo guiar al usuario en el uso de las principales funcionalidades del sistema, explicando de forma sencilla cómo interactuar con cada módulo.

---

## 🔐 Inicio de Sesión

### Acceso al sistema
1. Ingrese a la URL oficial del sistema (`http://www.possistema.somee.com/index.html`).
2. En la pantalla de inicio, introduzca:
   - **Nombre de Usuario**
   - **Contraseña**
3. Presione el botón **“Iniciar sesión”**.

### Autenticación y roles
Una vez autenticado, el sistema identifica su **rol** y lo redirige automáticamente al **panel correspondiente**:

| Rol de Usuario | Página inicial | Permisos principales |
|----------------|----------------|----------------------|
| **ADMIN_GENERAL** | Dashboard General | Gestión del sistema en administradores locales y sedes. |
| **ADMIN_LOCAL** | Dashboard Local | Gestión de catálogo, ventas y cajeros de su sede |
| **CAJERO** | Módulo de Ventas | Registro de ventas |

---

## 🧭 Navegación General

La aplicación posee una **barra lateral (sidebar)** desde donde puede acceder a los distintos módulos disponibles según su rol.

### Estructura de la interfaz

- **Barra lateral izquierda:** contiene los accesos principales (Dashboard, Catálogo, Ventas, Caja, Sedes, Usuarios).
- **Barra superior:** muestra el buscador, botones de acción y el nombre del módulo activo.
- **Sección principal:** despliega el contenido y las tablas dinámicas según la acción realizada.

---

## 📦 Gestión de Catálogo (ADMIN_LOCAL)

El módulo **Gestión de Catálogo** permite **visualizar, registrar y consultar productos** disponibles en la sede actual.

### 🔹 Visualización del catálogo
1. Desde el menú lateral, seleccione **Gestión de Catálogo**.  
2. Se mostrará una tabla con los productos registrados para su sede.  
   - Columnas: ID, Nombre, Descripción, Precio y Categoría.

### 🔍 Búsqueda de productos
- Use la **barra de búsqueda** superior para filtrar productos por nombre de manera instantánea.

### ➕ Registrar un nuevo producto
1. Haga clic en el botón **“Cargar Producto”**.  
2. Se abrirá un formulario modal donde debe ingresar:
   - **Nombre del producto**
   - **Descripción**
   - **Unidad de medida**
   - **Precio de venta**
   - **Categoría** (seleccionable desde la lista desplegable)
3. Presione **Registrar Producto** para guardar los cambios.

📌 **Notas importantes:**
- El sistema aplica validaciones automáticas para evitar caracteres no permitidos o palabras reservadas SQL.
- En caso de error, los mensajes se mostrarán debajo de cada campo o a través de modales.
- Una vez registrado, el producto se asocia automáticamente al **catálogo**.

---

## 💰 Gestión de Ventas (CAJERO)

El módulo de **Ventas** permite registrar operaciones de compra de forma rápida y controlada.

### 🧾 Crear una nueva venta
1. Seleccione **Crear Venta** en su página inicial.
2. Agregue productos al carrito indicando:
   - Cantidad
   - Precio unitario (precargado desde el catálogo)
   - Id del producto
3. El sistema calcula automáticamente el **total de la venta**.

### 💳 Finalizar venta
1. Seleccione el **método de pago**:
   - Efectivo
   - Tarjeta
   - Mixto
2. Si el método es efectivo, el sistema mostrará el **cambio a entregar**.
3. Presione **Finalizar Venta**.
4. Se generará un **comprobante o factura** con los detalles de la transacción.

---

## 👥 Gestión de Usuarios (ADMIN_GENERAL / ADMIN_LOCAL)

### Crear un nuevo usuario
1. Ingrese a **Gestión de Usuarios**.
2. Haga clic en **Registrar Usuario (Admin Local o Cajero según aplique)**.
3. Complete los datos requeridos:
   - Nombre y apellido
   - Correo electrónico
   - Rol del usuario
   - Sede (en caso de ADMIN_LOCAL)
4. Presione **Guardar**.

### Consultar o filtrar usuarios
- Puede buscar usuarios por nombre mediante la barra de búsqueda.

---

## 🏬 Gestión de Sedes (ADMIN_GENERAL)

Permite administrar las sedes registradas en el sistema.

### Crear una nueva sede
1. Ingrese a **Gestión de Sedes**.
2. Presione el botón **Registrar Sede**.
3. Complete los campos:
   - Nombre de la sede
   - Dirección
   - Ciudad
   - Departamento
4. Confirme la creación.

---

## 💵 Gestión de Caja (CAJERO)

### Funciones disponibles
- **Crear Venta:** registra la venta del negocio.  

---

## ⚙️ Validaciones del Sistema

El sistema aplica **validaciones globales** y **por módulo** para mantener la integridad de los datos:
- No se permiten caracteres especiales ni espacios dobles.
- Los campos numéricos solo aceptan números.
- Los correos no admiten espacios.
- Las palabras reservadas SQL están bloqueadas para prevenir inyección de código.

Los errores se muestran **debajo del campo correspondiente**, con mensajes claros y visualmente resaltados.

---

## 🔒 Seguridad

- El sistema utiliza **autenticación JWT** para proteger las rutas API.  
- Cada usuario tiene un token con su información y sede asociada.  
- El acceso a módulos y datos está **limitado por rol** y **sedeId**.

---

## 🧰 Resolución de problemas comunes

| Situación | Posible causa | Solución |
|------------|----------------|-----------|
| No carga el catálogo | Error de conexión o token expirado | Recargue la página e inicie sesión nuevamente |
| No puedo registrar producto | Faltan campos obligatorios o duplicado | Revise los campos y el mensaje de error |
| La sesión se cierra automáticamente | Token vencido | Vuelva a iniciar sesión |
| No aparecen las categorías | El servidor no respondió correctamente | Verifique conexión o contacte al administrador |

---

## 🧾 Créditos
👨‍💻 **Equipo de Desarrollo**  

📦 **Proyecto:** Sistema POS  
📍 **Versión actual:** v2.0.0 – *Publicación desplegada en producción*  
🕓 **Última actualización:** Noviembre 2025

---

> 💡 **Recomendación:**  
> Mantenga su sesión activa solo mientras use el sistema y evite compartir sus credenciales.  
> Para cualquier incidencia técnica, comuníquese con el equipo de soporte del proyecto.
