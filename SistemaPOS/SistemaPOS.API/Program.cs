using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Persistence;
using SistemaPOS.Domain.Repositories;
using SistemaPOS.Infrastructure.Repositories;
using Microsoft.Extensions.FileProviders;
using System.IO;
var builder = WebApplication.CreateBuilder(args);

// 🔹 Configurar conexión a Oracle
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SistemaPOSDbContext>(options =>
    options.UseOracle(connectionString)
);

// 🔹 Inyección de dependencias (repositorios)
builder.Services.AddScoped<ISedeRepository, SedeRepository>();

// 🔹 Agregar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(
            "http://127.0.0.1:5501", // Live Server
            "http://localhost:5500"  // Alternativo
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});
var app = builder.Build();

// 🔹 Habilitar Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowLocalFront");
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseStaticFiles();
app.UseDefaultFiles();

// 🔹 Servir archivos estáticos de wwwroot (si los hay en el API)
app.UseStaticFiles();

// 🔹 Servir archivos del frontend (SistemaPOS.web/wwwroot)
var frontendPath = Path.Combine(
    @"C:\Users\Carlos E. Dorado\Desktop\Eduardo\Carlos Software\sistema-pos\SistemaPOS\SistemaPOS.web",
    "wwwroot"
);

if (Directory.Exists(frontendPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frontendPath),
        RequestPath = ""
    });

    // Si no encuentra una ruta, devuelve el archivo principal del frontend
    app.MapFallbackToFile("consultar_sede.html");
}
else
{
    Console.WriteLine($"⚠️ No se encontró la carpeta del frontend en: {frontendPath}");
}
app.UseCors("AllowFrontend");
// 🔹 Mapear controladores (API)
app.MapControllers();

app.Run();
