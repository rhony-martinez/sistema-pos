# MANUAL DE USUARIO — SISTEMA POS

## Introducción
El **Sistema POS (Point of Sale)** es una aplicación web diseñada para la gestión integral de **ventas, productos, usuarios, sedes y caja** de una cadena de tiendas.

Permite que **administradores generales**, **administradores locales** y **cajeros** puedan operar de forma ordenada, rápida y segura, centralizando las operaciones del punto de venta.

Este manual incluye:
- Inicio y cierre de sesión  
- Gestión de catálogo  
- Gestión de ventas  
- Gestión de usuarios  
- Gestión de sedes  
- Apertura de caja  
- Arqueo de caja  
- Cierre de caja  
- Registro y anulación de ventas  
- Manejo de roles y seguridad  
- Validaciones, advertencias y resolución de errores  

---

## Inicio de Sesión

### Acceso al sistema
1. Ingrese en la URL oficial:  
   `http://www.possistema.somee.com/index.html`
2. Introduzca:
   - Nombre de usuario
   - Contraseña
3. Seleccione el botón **Iniciar sesión**.

Si los datos son correctos, el sistema validará el token y lo dirigirá al panel correspondiente según su rol.

### Roles del sistema
| Rol | Área inicial | Funciones principales |
|---|---|---|
| **ADMIN_GENERAL** | Dashboard General | Gestión de sedes y administradores locales y usuarios generales |
| **ADMIN_LOCAL** | Dashboard Local | Gestión del catálogo, caja, ventas y usuarios de su sede |
| **CAJERO** | Ventas / Caja | Registrar ventas, arqueos y cierre de caja |

### Cerrar sesión
1. Haga clic en **Cerrar Sesión**.  
2. El sistema elimina el token y regresa al login.

**Recomendación:** siempre cerrar sesión al terminar el turno.

---

## Navegación General
La interfaz del POS se organiza en:

### Barra lateral izquierda
Accesos principales:
- Dashboard
- Gestión de Catálogo
- Ventas
- Sedes
- Usuarios

### Barra superior
Incluye:
- Nombre del módulo activo
- Buscador
- Acciones rápidas

### Área central de trabajo
Aquí se muestran:
- Tablas
- Formularios
- Ventanas modales
- Reportes
- Detalles de ventas
- Información de caja

---

# ADMIN_GENERAL

## Dashboard (Admin General)

### 1) Objetivo
El Dashboard permite al Admin General:
- Consultar indicadores principales (**ventas**, **ingresos**, **usuarios** y **sedes**).
- Navegar hacia los módulos de administración (**Sedes** y **Usuarios**).
- Cerrar sesión de forma segura.

### 2) Elementos de la pantalla
**Barra superior**
- Título: “Dashboard”.
- Usuario conectado: “Admin_General”.
- Botón **Cerrar sesión** (rojo): finaliza la sesión.

**Menú lateral**
- Dashboard
- Gestión de Sedes
- Gestión de Usuarios

**Tarjetas de indicadores**
1. **Ventas Hoy**: total de ventas registradas en el día  
2. **Ingresos del Mes**: total acumulado del mes  
3. **Usuarios Activos**: usuarios activos/en operación  
4. **Sedes Activas**: sedes en funcionamiento  

---

## Gestión de Sedes (Admin General)

### 1) Acceso al módulo
1. Inicia sesión como **Admin General**.  
2. En el menú lateral izquierdo, haz clic en **Gestión de Sedes**.

### 2) Elementos de la pantalla
**Encabezado**
- **Consultar Sede**: buscar/consultar una sede registrada.
- **Crear Sede**: registrar una nueva sede.

**Tabla de sedes**
- ID de Sede
- Nombre
- Dirección
- Ubicación
- Teléfono
- Acciones (ícono para eliminar)

### 3) Crear una sede
1. Clic en **Crear Sede**.  
2. Completa la información solicitada (ej. nombre, dirección, ubicación, teléfono).  
3. Guarda/Confirma.

**Resultado esperado:** la sede aparece en la tabla.

### 4) Consultar una sede
1. Clic en **Consultar Sede**.  
2. Ingresa el dato de búsqueda (ID o nombre).  
3. Visualiza el resultado.

### 5) Eliminar una sede
1. Ubica la sede.  
2. Clic en el ícono de **eliminar** en Acciones.  
3. Confirma.

**Importante:** eliminar una sede puede afectar usuarios/ventas/inventario asociados.

---

## Gestión de Usuarios (Admin General)

### 1) Acceso al módulo
1. Inicia sesión como **Admin General**.  
2. En el menú lateral, selecciona **Gestión de Usuarios**.

### 2) Elementos de la pantalla
**Barra superior**
- Buscador **“Buscar usuario…”** (usuario, nombre/apellido, ID usuario)
- Botón **“+ Crear Admin Local”**

**Tabla de usuarios**
- Usuario
- Nombre
- Apellido
- Email
- Estado
- Sede Id
- Acciones (Editar / Eliminar)

### 3) Buscar un usuario
1. Clic en **Buscar usuario…**  
2. Escribe el criterio (ej. juan, Perez, 2001).  
3. Se filtra el listado.

### 4) Crear Admin Local
1. Clic en **+ Crear Admin Local**.  
2. Completa datos (usuario, nombre, apellido, correo, sede, estado/credenciales).  
3. Cancela/Confirma.

### 5) Editar usuario
1. Ubica el usuario.  
2. Clic en **Editar**.  
3. Modifica campos permitidos.  
4. Guarda.

### 6) Eliminar usuario
1. Ubica el usuario.  
2. Clic en **Eliminar**.  
3. Confirma.

**Importante:** eliminar un usuario puede impedir su acceso y afectar la operación.

---

# ADMIN_LOCAL

## Dashboard (Admin Local)

### 1) Objetivo
Permite:
- Ver indicadores clave de **su sede** (ventas, ingresos, cajeros activos y estado de caja).
- Acceder a Catálogo, Usuarios y Ventas desde el menú.
- Cerrar sesión.

### 2) Elementos de la pantalla
**Barra superior**
- “Dashboard - Administrador Local”
- Usuario conectado
- Botón **Cerrar sesión**

**Menú lateral**
- Dashboard
- Gestión de Catálogo
- Gestión de Usuarios
- Gestión de Ventas

### 3) Tarjetas de indicadores
1. **Ventas Hoy** (sede)  
2. **Ingresos del Mes** (sede)  
3. **Cajeros Activos** (sede)  
4. **Caja Abierta** (sede)  

---

## Gestión de Catálogo (Admin Local)

### 1) Acceso al módulo
1. Inicia sesión como **Administrador Local**.  
2. En el menú lateral, selecciona **Gestión de Catálogo**.

### 2) Elementos de la pantalla
**Barra superior**
- Buscador **“Buscar producto…”** (por nombre)
- Botón **“+ Cargar Producto”**

**Tabla de productos**
- ID Producto
- Nombre
- Descripción
- Precio
- Categoría
- Acciones (Editar / Eliminar)

### 3) Buscar un producto
1. Clic en **Buscar producto…**  
2. Escribe el dato (ej. “Martillo”).  
3. Se filtra la tabla.

### 4) Cargar producto
1. Clic en **+ Cargar Producto**.  
2. Completa datos (nombre, descripción, precio, categoría).  
3. Cancela/Confirma.

### 5) Editar producto
1. Ubica el producto.  
2. Clic en **Editar**.  
3. Actualiza datos permitidos (ej. precio).  
4. Guarda.

### 6) Eliminar producto
1. Ubica el producto.  
2. Clic en **Eliminar**.  
3. Confirma.

**Importante:** al eliminar un producto puede dejar de estar disponible para ventas.

---

## Gestión de Usuarios (Admin Local)

### 1) Acceso al módulo
1. Inicia sesión como **Administrador Local**.  
2. En el menú lateral, selecciona **Gestión de Usuarios**.

### 2) Elementos de la pantalla
**Barra superior**
- Buscador **“Buscar usuario…”** (nombre/apellido, ID de usuario)
- Botón **“+ Crear Cajero”**

**Tabla de usuarios**
- Usuario
- Nombre
- Apellido
- Email
- Estado (Activo/Inactivo)
- Sede Id
- Acciones

### 3) Buscar un usuario
1. Clic en **Buscar usuario…**  
2. Escribe criterio (ej. juli, rojas, 205).  
3. Se filtra la tabla.

### 4) Crear cajero
1. Clic en **+ Crear Cajero**.  
2. Completa datos (usuario, nombre, apellido, email, estado).  
   - **La sede se toma automáticamente** del admin_local logueado.
3. Cancela/Confirma.

### 5) Editar usuario
1. Ubica el usuario.  
2. Clic en **Editar**.  
3. Modifica datos permitidos.  
4. Guarda.

### 6) Acciones (según íconos)
- **Eliminar (cesta)**: desactiva al cajero (estado = Inactivo).  
- **Prohibido (🚫)**: *no es botón*; solo indica visualmente que está inactivo.  
- **Editar**: actualiza datos (incluye estado si está habilitado).

---

## Gestión de Ventas (Admin Local)

### 1) Acceso al módulo
1. Inicia sesión como **Administrador Local**.  
2. En el menú lateral, haz clic en **Gestión de Ventas**.

### 2) Elementos de la pantalla
**Filtros por fechas**
- **Desde**: fecha inicial del reporte/consulta.
- **Hasta**: fecha final del reporte/consulta.

**Botón “Generar Informe”**
- Descarga automáticamente un **PDF** del rango de fechas seleccionado (Desde–Hasta).

**Tabla de registro de ventas**
- ID de venta
- Fecha (fecha y hora)
- Monto total
- Método de pago (Efectivo/Tarjeta/Transferencia)
- Acciones

### 3) Generar informe PDF
1. Selecciona **Desde**.  
2. Selecciona **Hasta**.  
3. Clic en **Generar Informe**.

**Resultado:** descarga automática del PDF.

### 4) Acción sobre una venta
1. Ubica la venta.  
2. Clic en el ícono en **Acciones**.  
3. Confirma si el sistema lo solicita.

---

# CAJERO

## Gestión de Caja (pantalla inicial)

### 1) Objetivo
Muestra el estado actual de la caja y concentra las acciones del cajero.

### 2) Elementos de la pantalla
**Acciones principales**
- **Crear Venta**: abre el flujo de venta.
- **Abrir Caja**: inicia turno ingresando saldo inicial.
- **Cerrar Caja**: finaliza caja (recomendado tras arqueo).

**Estado de Caja Actual**
- Saldo Inicial
- Ventas Netas
- Saldo Final Estimado = Saldo Inicial + Ventas Netas

**Arqueo de Caja**
- Permite cuadrar efectivo real vs. estimado (recomendado antes de cerrar caja).

**Cerrar sesión**
- Finaliza el acceso del cajero.

---

## Crear Venta (Cajero)

### 1) Datos del cliente
Campos:
- Cc. Documento
- Nombre
- Teléfono
- Correo

### 2) Agregar productos
1. Ingresa el **ID del producto**.
2. Clic en **+ Agregar**.
3. Se agrega a la tabla:
   - Producto
   - Cantidad (ajustable)
   - Precio
   - IVA%
   - Subtotal
   - Acciones (eliminar ítem)

> El subtotal se calcula automáticamente según cantidad, precio e IVA.

### 3) Seleccionar medio de pago
Opciones:
- Efectivo
- Tarjeta
- Transferencia

**Si es Efectivo**
- Ingrese **Monto recibido**
- El sistema calcula **Cambio**

### 4) Resumen y observaciones
En el panel Resumen:
- Subtotal
- IVA 19%
- TOTAL  
Además: campo **Observaciones**.

### 5) Confirmar / Cancelar
- **Cancelar**: no registra la venta.
- **Confirmar**: registra y genera comprobante.

### 6) Modal posterior a confirmar
Opciones:
- **Aceptar y volver a Gestión de Caja**
- **Visualizar factura** (y luego volver a Gestión de Caja)

---

## Validaciones del Sistema
- No se permiten caracteres especiales ni espacios dobles.
- Campos numéricos: solo números.
- Correos: sin espacios.
- Palabras reservadas SQL bloqueadas (prevención de inyección).
- Los errores se muestran debajo del campo correspondiente.

---

## Seguridad
- Autenticación JWT para proteger rutas API.
- Token con información del usuario y sede asociada.
- Acceso a módulos/datos limitado por **rol** y **sedeId**.

---

## Resolución de problemas comunes
| Situación | Posible causa | Solución |
|---|---|---|
| No puedo iniciar sesión | Usuario/contraseña incorrectos | Verifique credenciales (mayúsculas) e intente nuevamente |
| No puedo iniciar sesión | Usuario desactivado/inactivo | Solicite al administrador activar el usuario |
| La sesión se cierra automáticamente | Token vencido / inactividad | Vuelva a iniciar sesión |
| No carga el Dashboard o un módulo | Conexión inestable / servidor no responde | Recargue la página; verifique internet; intente más tarde |
| No carga el catálogo | Error de conexión o token expirado | Recargue la página e inicie sesión nuevamente |
| No puedo cargar un producto | Faltan campos obligatorios | Complete todos los campos requeridos y guarde |
| No puedo cargar un producto | Producto duplicado (ID o nombre) | Cambie el identificador o edite el producto existente |
| No puedo editar un producto/usuario | Permisos insuficientes | Verifique el rol; contacte al Admin General si aplica |
| No puedo abrir caja | Caja ya está abierta | Verifique el estado de caja; no abra una segunda caja |
| No puedo crear venta | Caja cerrada | Abra caja antes de registrar ventas |
| No puedo agregar producto a la venta | ID de producto no existe | Verifique el ID en Catálogo o consulte al Admin Local |
| El total/IVA no coincide | Cantidad o precio mal digitado | Revise cantidad, precio e IVA% del producto |
| No calcula el cambio | No se ingresó “Monto recibido” | Digite el monto recibido; debe ser mayor o igual al total |
| Cambio sale negativo | Monto recibido menor al total | Corrija el monto o cambie el método de pago |
| No puedo confirmar la venta | No hay productos o faltan datos | Agregue al menos un producto y complete lo obligatorio |
| Confirmé la venta y no aparece en registro | Filtro de fechas no incluye la venta | Ajuste Desde/Hasta para incluir la fecha correcta |
| Generar Informe no descarga el PDF | Descargas bloqueadas | Habilite descargas y reintente “Generar Informe” |
| El PDF sale vacío | No hay ventas en el rango | Seleccione un rango con ventas registradas |

---

## Créditos
**Equipo de Desarrollo**  
Proyecto: Sistema POS  
Versión actual: **v3.0.0** – Publicación en producción  
Última actualización: **Diciembre 2025**

**Recomendación final:** mantenga su sesión activa solo mientras use el sistema y evite compartir sus credenciales.
