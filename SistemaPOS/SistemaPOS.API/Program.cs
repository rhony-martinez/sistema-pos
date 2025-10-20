using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Registrar los controladores (API REST)
builder.Services.AddControllers();

// Registrar el contexto con la cadena de conexión de Oracle
builder.Services.AddDbContext<SistemaPosContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

// Agregar soporte a Swagger para probar los endpoints
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // puedes mantenerlo si algún día sirves archivos estáticos (opcional)

app.UseRouting();

app.UseAuthorization();

// Mapea los controladores de la API 
app.MapControllers();

app.Run();
