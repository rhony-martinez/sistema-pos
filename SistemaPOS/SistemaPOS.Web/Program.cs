using Microsoft.Extensions.FileProviders;
using SistemaPOS.Application.Sedes;// si usas comandos/queries
using SistemaPOS.Infrastructure; // si usas AddInfrastructure

using System.IO;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Si este proyecto necesita servicios de tu capa Application/Infrastructure:
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ListarSedesQuery>();
builder.Services.AddScoped<CrearSedeCommand>();

// 🔹 Si planeas tener controladores API dentro del proyecto Web
builder.Services.AddControllers();

var app = builder.Build();

// 🔹 Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 🔹 Servir archivos estáticos desde wwwroot (por ejemplo, HTML, JS, CSS)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseRouting();

// (Descomenta si agregas autenticación luego)
// app.UseAuthentication();
app.UseAuthorization();

// 🔹 Exponer controladores API
app.MapControllers();

// 🔹 Fallback para archivos HTML (por ejemplo consultar_sede.html)
app.MapFallbackToFile("consultar_sede.html");

app.Run();
