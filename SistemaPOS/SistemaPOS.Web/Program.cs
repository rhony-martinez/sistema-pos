
using SistemaPOS.Application.Sedes;
using SistemaPOS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// MVC (Views). En DEBUG puedes habilitar la recompilación de vistas.
// Si NO tienes instalado el paquete 8.x de RuntimeCompilation, comenta la línea AddRazorRuntimeCompilation().
var mvc = builder.Services.AddControllersWithViews();
#if DEBUG
// Requiere paquete: Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation (v8.x)
mvc.AddRazorRuntimeCompilation();
#endif

// Inyección de la capa de infraestructura (Oracle + Dapper + Repos)
builder.Services.AddInfrastructure(builder.Configuration);

// Casos de uso (Application)
builder.Services.AddScoped<ListarSedesQuery>();
builder.Services.AddScoped<CrearSedeCommand>();

var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// (Si más adelante agregas autenticación/autorización, actívalas aquí)
// app.UseAuthentication();
app.UseAuthorization();

// Ruta por defecto a tu módulo de Sedes (Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sedes}/{action=Index}/{id?}");

app.Run();