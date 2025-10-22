using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// No necesitas Razor Pages si solo usarás HTML
// builder.Services.AddRazorPages();

var app = builder.Build();

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// ?? Servir archivos estáticos del frontend
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseRouting();

// No necesitas autorización para HTML estático
// app.UseAuthorization();

// ?? Cuando el usuario acceda a la raíz, abrirá consultar_sede.html
app.MapFallbackToFile("consultar_sede.html");

app.Run();
