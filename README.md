# 💼 Sistema POS — Proyecto Académico de Arquitectura de Software

## 📋 Descripción general

**Sistema POS (Point of Sale)** es un proyecto académico desarrollado con **C# (.NET 8)**, **Oracle Database**, **HTML**, **CSS** y **JavaScript**, siguiendo una **arquitectura multicapa**.  
Su propósito es modelar un sistema de punto de venta que gestione **sedes, usuarios, productos, ventas y control de caja**, aplicando buenas prácticas de desarrollo, integración y documentación.

---

## 🧩 Arquitectura del proyecto

El sistema está estructurado bajo una **arquitectura por capas** que separa responsabilidades:

| Capa | Proyecto | Descripción |
|------|-----------|-------------|
| 🎨 **Presentación** | `SistemaPOS.Web` | Contiene el frontend (HTML, CSS, JS). |
| ⚙️ **API / Backend** | `SistemaPOS.API` | Expone servicios REST que interactúan con la lógica de negocio y Oracle. |
| 🧠 **Aplicación** | `SistemaPOS.Application` | Contiene la lógica de negocio y orquestación de procesos. |
| 🧱 **Dominio** | `SistemaPOS.Domain` | Define las entidades principales del sistema (modelos). |
| 💾 **Infraestructura** | `SistemaPOS.Infrastructure` | Implementa la conexión a la base de datos Oracle mediante EF Core. |

---

## ⚙️ Fase 1 — Instalación del entorno de desarrollo

### 🔹 Requisitos
- **Visual Studio 2022** (Community o superior)  
- **.NET SDK 8.0**  
- **Oracle Database 21c XE o superior**  
- **Oracle Data Provider for .NET (ODP.NET)**  
- **Git y GitHub** configurados  

### 🔹 Workloads requeridas en Visual Studio
Durante la instalación, marca las siguientes opciones:

- ✅ **Desarrollo de ASP.NET y web**  
- ✅ **Desarrollo de escritorio con .NET**  
- 📦 *(Opcional)* **Almacenamiento y procesamiento de datos**

---

## 🏗️ Fase 2 — Creación de la solución base

Se generó una estructura multicapa como sigue:
```
SistemaPOS
├── SistemaPOS.API
├── SistemaPOS.Web
├── SistemaPOS.Application
├── SistemaPOS.Domain
└── SistemaPOS.Infrastructure
```

### 🔹 Comandos base (alternativa CLI)

```bash
dotnet new sln -n SistemaPOS
dotnet new webapi -n SistemaPOS.API
dotnet new classlib -n SistemaPOS.Application
dotnet new classlib -n SistemaPOS.Domain
dotnet new classlib -n SistemaPOS.Infrastructure
dotnet sln add SistemaPOS.*
```
### 🔹 Referencias entre proyectos
```bash
dotnet add SistemaPOS.API reference SistemaPOS.Infrastructure
dotnet add SistemaPOS.Infrastructure reference SistemaPOS.Domain
dotnet add SistemaPOS.Application reference SistemaPOS.Domain
```
---
## 🗃️ Fase 3 — Configuración de la base de datos Oracle
🔹 Script base

El script SQL con las tablas del sistema (`SEDE`, `CAJA`, `PRODUCTO`, `USUARIO`, `VENTA`, etc.) se encuentra en
📁 `/docs/sql/estructura_base.sql`

Service Name (SID): Según creación en Oracle (`XE` ó `XEPDB1` u otro)

🔹 Cadena de conexión

Archivo:
`SistemaPOS.API/appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "OracleDb": "User Id=POS;Password=software1;Data Source=localhost:1521/XE"
  },

  "DetailedErrors": true,
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```
⚠️ Importante:

No incluir credenciales reales en appsettings.json.

El `appsettings.Development.json` está listado en `.gitignore.`

---

## 🧠 Fase 4 — Conexión API ↔ Oracle
🔹 Paquetes NuGet instalados
```bash
dotnet add SistemaPOS.Infrastructure package Oracle.EntityFrameworkCore
dotnet add SistemaPOS.Infrastructure package Microsoft.EntityFrameworkCore.Tools
dotnet add SistemaPOS.API package Swashbuckle.AspNetCore
```
🔹 Contexto de datos (`SistemaPosContext.cs`)

Ruta:
`SistemaPOS.Infrastructure/Data/`

```cs
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Domain.Entities;

namespace SistemaPOS.Infrastructure.Data
{
    public class SistemaPosContext : DbContext
    {
        public SistemaPosContext(DbContextOptions<SistemaPosContext> options) : base(options) { }

        // Tablas (DbSets)
        public DbSet<Sede> Sedes { get; set; }
        public DbSet<Caja> Cajas { get; set; }
        public DbSet<CategoriaProducto> CategoriasProducto { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Catalogo> Catalogos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetallesVenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ejemplo: clave compuesta en CATALOGO
            modelBuilder.Entity<Catalogo>()
                .HasKey(c => new { c.SedeId, c.ProId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
```

🔹 Configuración de `Program.cs` (API)
```cs
using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Controladores REST
builder.Services.AddControllers();

// Conexión a Oracle
builder.Services.AddDbContext<SistemaPosContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

🔹 Controlador de prueba (`SedeController.cs`)

Ruta:
`SistemaPOS.API/Controllers/SedeController.cs`
```cs
using Microsoft.AspNetCore.Mvc;
using SistemaPOS.Infrastructure.Data;

namespace SistemaPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SedeController : ControllerBase
    {
        private readonly SistemaPosContext _context;

        public SedeController(SistemaPosContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetSedes()
        {
            var sedes = _context.Sedes.ToList();
            return Ok(sedes);
        }
    }
}
```
---
## 🚀 Fase 5 — Ejecución y prueba

Compila la solución (Ctrl + Shift + B)

Ejecuta `SistemaPOS.API` (Ctrl + F5)

Accede a:
🔗 https://localhost:7096/swagger

Expande el endpoint **/api/Sede**

Presiona **“Execute”**

Resultado esperado:
```json
[
  {
    "sedeId": 1,
    "sedeNombre": "Sede Central",
    "sedeDireccion": "Av. Principal #123",
    "sedeCiudad": "Bogotá",
    "sedeDepartamento": "Cundinamarca",
    "sedeUbicacion": "Centro Comercial El Sol, Local 12",
    "sedeTelefono": "3021345677",
    "sedeCorreo": "central@pos.com",
    "sedeEstado": "ACTIVA"
  }
]
```

**ENLACES EXTERNOS**
https://www.figma.com/design/LvxiFRraDB2ZAWkXMbFDKP/Sistema-pos?node-id=0-1&t=pkG9HilTqFNv6D9K-1 
## Información
Proyecto con fines educativos

**Asignatura**: Ingeniería de Software I

**Institución**: Universidad del Cauca

**Año**: 2025
