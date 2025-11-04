# ⚙️ MANUAL TÉCNICO – SISTEMA POS

## 🧾 Descripción general
El **Sistema POS (Point of Sale)** es una aplicación web desarrollada con una arquitectura **Cliente–Servidor**.  
Permite la gestión de **ventas, productos, usuarios y sedes** de forma centralizada, con autenticación basada en **JWT** y control de roles.

---

## 🏗️ Arquitectura del sistema

### 🔹 Backend
- **Framework:** ASP.NET Core 8.0  
- **Patrón de diseño:** Capas (API – Application – Domain – Infrastructure)  
- **ORM:** Entity Framework Core  
- **Autenticación:** JSON Web Token (JWT)  
- **Base de datos:** Oracle o Microsoft SQL Server  

**Capas principales:**
| Capa | Función |
|------|----------|
| **API** | Expone los controladores RESTful. |
| **Application** | Contiene la lógica de negocio y servicios. |
| **Domain** | Define las entidades del modelo de dominio. |
| **Infrastructure** | Gestiona la persistencia y configuración de datos. |

---

### 🔹 Frontend
- **Tecnología:** HTML5, CSS3 y JavaScript (Vanilla JS)  
- **Estilo:** Diseño modular 
- **Validaciones:** Scripts globales (`validaciones.js`) y por módulo (`validaciones-producto.js`)  
- **Consumo de API:** `fetch` con token JWT almacenado en `sessionStorage`.

---

## 🧩 Componentes principales

### 🔸 Autenticación
- Servicio: `AuthService`
- Flujo:
  1. El usuario inicia sesión (`/api/Auth/Login`).
  2. Se genera un **JWT** con `sedeId`, `rol` y `uid`.
  3. El frontend almacena el token en `sessionStorage`.
  4. Las peticiones posteriores incluyen `Authorization: Bearer <token>`.

---

### 🔸 Gestión de Productos
- Controlador: `ProductoController`
- DTO: `ProductoRequest`
- Servicio: `ProductoService`
- Funcionalidad:
  - Registro de nuevos productos.
  - Asociación automática al **catálogo de la sede** usando el `sedeId` del token.
  - Validaciones de duplicados y categoría.

---

### 🔸 Gestión de Catálogo
- Renderizado dinámico de productos con su categoría correspondiente.
- Búsqueda por nombre implementada en frontend.
- Validaciones visuales y mensajes de error bajo cada campo.

---

## 🧮 Base de datos (modelo general)

| Tabla | Descripción |
|--------|--------------|
| **USUARIO** | Información de usuarios y roles. |
| **SEDE** | Datos de las sedes registradas. |
| **CATEGORIA_PRODUCTO** | Dominios de categorías. |
| **PRODUCTO** | Datos de productos (nombre, descripción, precio, unidad, estado, categoría). |
| **CATALOGO** | Asociación entre sede y producto. |
| **VENTA / DETALLE_VENTA** | Registro de ventas y sus ítems. |

---

## ⚙️ Configuración del entorno

### 1️⃣ Requisitos previos
- **.NET SDK 8.0 o superior**
- **Node.js (opcional para desarrollo frontend local)**
- **Oracle o SQL Server** con la base de datos creada.
- **Visual Studio 2022** o **VS Code** con extensiones C#.

---

### 2️⃣ Variables de entorno
En el archivo `appsettings.json`:
