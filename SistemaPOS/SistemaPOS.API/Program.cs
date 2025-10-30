using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SistemaPOS.Application.CategoriasProducto;
using SistemaPOS.Application.Sedes;
using SistemaPOS.Application.Services;
using SistemaPOS.Application.Services.Implementations;
using SistemaPOS.Application.Services.Interfaces;
using SistemaPOS.Infrastructure;
using SistemaPOS.Infrastructure.Data;
using SistemaPOS.Infrastructure.Persistence;
using SistemaPOS.Infrastructure.Repositories;
using System.Text;

// Crear el builder
var builder = WebApplication.CreateBuilder(args);
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

// ----------------------------------------------------
// 🔹 Configuración de servicios
// ----------------------------------------------------

// Controladores y endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ----------------------------------------------------
// 🔹 Configurar conexión a SQL Server
builder.Services.AddDbContext<SistemaPosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------------------------------------
// 🔹 Repositorios y servicios personalizados
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISedeRepository, SedeRepository>();
builder.Services.AddScoped<ISedeService, SedeService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaProductoService, CategoriaProductoService>();

// 🔹 Registrar casos de uso de “crear sede”
builder.Services.AddScoped<ListarSedesQuery>();
builder.Services.AddScoped<CrearSedeCommand>();

// 🔹 Cargar infraestructura (de la rama de jsolarte)
builder.Services.AddInfrastructure(builder.Configuration);

// ----------------------------------------------------
// 🔹 Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://127.0.0.1:5501", "http://localhost:5501")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ----------------------------------------------------
// 🔹 Autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // evitar tolerancia temporal
    };

    // Validación de tokens revocados
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async ctx =>
        {
            var jti = ctx.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
            {
                ctx.Fail("Invalid token");
                return;
            }

            var db = ctx.HttpContext.RequestServices.GetRequiredService<SistemaPosContext>();
            var exists = await db.RevokedTokens.FindAsync(jti);
            if (exists != null)
                ctx.Fail("Token revoked");
        }
    };
});

// ----------------------------------------------------
// 🔹 Swagger con autenticación Bearer
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SistemaPOS API",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Introduce el token JWT así: Bearer {tu token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ----------------------------------------------------
// 🔹 Construir aplicación
var app = builder.Build();

// ----------------------------------------------------
// 🔹 Middleware
// ----------------------------------------------------

// CORS debe ir antes del pipeline
app.UseCors("AllowFrontend");

// HTTPS y archivos estáticos
app.UseHttpsRedirection();
app.UseStaticFiles();

// Ruteo
app.UseRouting();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Swagger 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SistemaPOS API v1");
        options.RoutePrefix = string.Empty;
    });
}
else
{
    // Habilita Swagger temporalmente también en producción
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SistemaPOS API v1");
        options.RoutePrefix = "swagger"; // así se accede desde /swagger
    });

    app.UseExceptionHandler("/Error");
    app.UseHsts();
}



// Mapeo de controladores
app.MapControllers();

// Ejecutar aplicación
app.Run();
