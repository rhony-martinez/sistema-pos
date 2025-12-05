# ⚙️ MANUAL TÉCNICO – SISTEMA POS

## 🧾 Descripción general
El **Sistema POS (Point of Sale)** es una aplicación web desarrollada con una arquitectura **Cliente–Servidor**.  
Permite la gestión de **ventas, productos, usuarios y sedes** de forma centralizada, con autenticación basada en **JWT** y control de roles.

---

## 🏗️ Arquitectura del sistema

El sistema se organiza en una arquitectura **Cliente–Servidor**:

- **Frontend (Cliente Web):** interfaz para los roles *Admin General*, *Admin Local* y *Cajero*. Consume la API y renderiza módulos según permisos.
- **Backend (API REST):** expone endpoints para autenticación y operaciones del POS. Aplica reglas de negocio, validaciones y control de acceso.
- **Base de datos:** persistencia de sedes, usuarios, productos, ventas y caja. La segregación por sede se gestiona con `sedeId`.

### 🔹 Backend
- **Framework:** ASP.NET Core 8
- **Patrón sugerido:** Controller → Service → Persistencia
- **Seguridad:** JWT + autorización por rol/claims (`uid`, `rol`, `sedeId`)
- **Responsabilidad principal:** centralizar lógica de negocio y validaciones server-side

### 🔹 Frontend
- **Cliente web:** SPA (según implementación actual)
- **Gestión de sesión:** `sessionStorage` para token JWT
- **Responsabilidad principal:** UX, validaciones visuales, consumo de API y descarga de PDFs

---

## 🧩 Componentes principales

### 🔸 Autenticación
**Servicio:** `AuthService`  
**Flujo:**
1. El usuario inicia sesión (`POST /api/Auth/Login`).
2. El backend genera un **JWT** con `sedeId`, `rol` y `uid`.
3. El frontend almacena el token en `sessionStorage`.
4. Peticiones posteriores agregan: `Authorization: Bearer <token>`.

**Notas técnicas:**
- El `sedeId` del token se usa para filtrar datos multi-sede.
- Un token expirado provoca cierre de sesión/solicitud de re-login.

---

### 🔸 Gestión de Productos
**Controlador:** `ProductoController`  
**DTO:** `ProductoRequest`  
**Servicio:** `ProductoService`  

**Funcionalidad:**
- Registro de nuevos productos.
- Asociación a la sede (catálogo) usando `sedeId` del token.
- Validaciones: duplicados y categoría.

---

### 🔸 Gestión de Catálogo
**Frontend:**
- Renderizado dinámico de productos con categoría.
- Búsqueda por nombre “Buscar producto…” (frontend).
- Acciones por fila: editar (✏️) y eliminar (🗑) según permisos.

**Backend (reglas sugeridas):**
- Validar que el precio sea numérico y > 0.
- Validar que la categoría exista o cumpla regla definida.
- Evitar duplicados por ID/nombre según criterio del negocio.

---

### 🔸 Gestión de Usuarios
**Controlador:** `UsuarioController`  
**Servicio:** `UsuarioService`  

**Funcionalidad por rol:**
- **Admin General:** crea **Admin Local** y asigna sede (`sedeId`).
- **Admin Local:** crea **Cajero** asociado a su sede (tomada del token).

**Notas UI (según pantallas documentadas):**
- Ícono 🚫 **no tiene acción**: solo indica que el usuario ya está **Inactivo**.
- Ícono 🗑 (cesta) **desactiva** al cajero directamente (cambia estado a Inactivo).
- Ícono ✏️ permite editar.

---

### 🔸 Gestión de Sedes
**Controlador:** `SedeController`  
**Servicio:** `SedeService`  

**Funcionalidad:**
- Listado de sedes.
- Crear sede (Admin General).
- Consultar sede.
- Eliminar sede (si el sistema lo permite y no rompe dependencias).

---

### 🔸 Gestión de Caja
**Controlador:** `CajaController`  
**Servicio:** `CajaService`  

**Pantalla inicial del Cajero:** el cajero inicia sesión y es recibido por **Gestión de Caja**.

**Funcionalidad:**
- **Abrir Caja:** registra saldo inicial y marca caja como abierta.
- **Estado de caja:** muestra saldo inicial, ventas netas y saldo final estimado.
- **Arqueo de Caja:** permite cuadrar efectivo real vs. estimado.
- **Cerrar Caja:** finaliza el turno.

**Cálculos:**
- `saldo_final_estimado = saldo_inicial + ventas_netas`

---

### 🔸 Gestión de Ventas
**Controlador:** `VentaController`  
**Servicio:** `VentaService`  

**Funcionalidad:**
- Registrar venta con detalle de productos (cant., precio, IVA%).
- Cálculo de **Subtotal**, **IVA 19%** y **Total**.
- Registro de métodos de pago: **Efectivo / Tarjeta / Transferencia**.
- Asociación automática a sede con `sedeId` del token.

**Flujo Cajero (UI):**
1. Desde Gestión de Caja → **Crear Venta**.
2. Agregar productos por ID y ajustar cantidad.
3. Seleccionar medio de pago.
4. Confirmar venta → modal con:
   - Aceptar y volver a Gestión de Caja
   - Visualizar factura y luego volver

---

### 🔸 Reportes e Informes (PDF)
 
**Funcionalidad:**
- Informe de ventas por **rango de fechas** (Desde/Hasta).
- Exportación a **PDF**.

**Comportamiento UI:**
- Botón **Generar Informe** descarga el PDF **automáticamente** con las fechas seleccionadas.

---

### 🔸 Dashboard

**Admin General (global):**
- Ventas Hoy
- Ingresos del Mes
- Usuarios Activos
- Sedes Activas

**Admin Local (por sede):**
- Ventas Hoy
- Ingresos del Mes
- Cajeros Activos
- Caja Abierta

---

### 🔸 Autorización y Seguridad (Roles)
**Mecanismo:** JWT + autorización por rol y sede.

**Reglas clave:**
- Admin General puede operar globalmente.
- Admin Local opera solo en su sede (`sedeId`).
- Cajero opera caja/ventas de su sede.

---

## 🧮 Base de datos (modelo general)

### Entidades principales (sugeridas)
- **Sede**
- **Usuario** (con rol y estado)
- **Producto**
- **Categoría**
- **Venta**
- **DetalleVenta**
- **Caja** (apertura/cierre)
- **ArqueoCaja** (si aplica)
- **Factura/Comprobante** (si aplica)

### Relaciones clave
- `Usuario.sedeId → Sede.id`
- `Producto.sedeId → Sede.id` (o relación catálogo-sede)
- `Venta.sedeId → Sede.id`
- `Venta.usuarioId → Usuario.id`
- `DetalleVenta.ventaId → Venta.id`

---

## ⚙️ Configuración del entorno

### 1️⃣ Requisitos previos
- .NET SDK (según backend)
- Node.js (si el frontend requiere build)
- Motor de base de datos configurado (según proyecto)
- Variables de entorno / appsettings configurados para:
  - Connection string
  - JWT secret / issuer / audience
  - URLs permitidas (CORS)

### 2️⃣ Variables de entorno (sugeridas)
- `CONNECTION_STRING`
- `JWT_SECRET`
- `JWT_ISSUER`
- `JWT_AUDIENCE`
- `JWT_EXPIRES_MINUTES`

---

## ✅ Validaciones del sistema

### Validaciones funcionales (según manual)
- Sin caracteres especiales ni espacios dobles.
- Campos numéricos: solo números.
- Correos: sin espacios y con formato válido.
- Bloqueo de palabras reservadas SQL (prevención de inyección).
- Mensajes de error bajo cada campo (frontend).

### Validaciones recomendadas (backend)
- Validación completa server-side (never trust client).
- Validación de permisos por rol/claims.
- Validación de rangos de fechas y existencia de registros.

---

## 🧪 Pruebas sugeridas (checklist)

- **Auth:** token inválido → 401; rol sin permisos → 403.
- **Sede:** Admin Local no consulta datos fuera de su sede.
- **Caja:** prevenir doble apertura; arqueo antes de cierre; cierre controlado.
- **Ventas:** impedir venta sin productos; efectivo sin monto recibido; monto < total.
- **Reportes:** rango vacío produce PDF vacío o mensaje controlado; descarga OK.

---

## 🧰 Resolución de problemas (técnico)
- PDF no descarga: revisar permisos del navegador, headers y tipo MIME.
- Sesión expira: revisar expiración JWT y manejo de errores en frontend.
- Totales incorrectos: revisar regla de IVA 19% y redondeos.
- Usuario no puede operar: revisar estado Activo/Inactivo y rol.

---

## 📌 Versionado y operación
- Recomendado: SemVer `MAJOR.MINOR.PATCH`.
- Logs/auditoría recomendada: login, apertura/cierre caja, creación/anulación ventas, CRUD usuarios y productos.
