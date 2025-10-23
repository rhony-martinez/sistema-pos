using Microsoft.AspNetCore.Builder;
using SistemaPOS.Application.Sedes;
using SistemaPOS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
 
//  Añadir servicios al contenedor
builder.Services.AddControllers(); // ← importante para API
builder.Services.AddEndpointsApiExplorer(); // Para detectar controladores
builder.Services.AddSwaggerGen(); // Activa Swagger

//  Inyección de dependencias (repositorios y casos de uso)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ListarSedesQuery>();
builder.Services.AddScoped<CrearSedeCommand>();

var app = builder.Build();

//  Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    // Swagger visible solo en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SistemaPOS API v1");
        options.RoutePrefix = string.Empty; 
    });
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers(); // Importante para rutas de controladores
app.Run();
