using Microsoft.EntityFrameworkCore;
using SistemaPOS.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SistemaPOS.Infrastructure.Persistence;
using SistemaPOS.Domain.Repositories;
using SistemaPOS.Infrastructure.Repositories;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);

// Controladores (API REST)
builder.Services.AddControllers();

// Contexto con Oracle
builder.Services.AddDbContext<SistemaPosContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

// Repos y servicios
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRevokedTokenRepository, RevokedTokenRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISedeRepository, SedeRepository>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// 🔹 CORS: permitir ambos dominios de desarrollo
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://127.0.0.1:5501", "http://localhost:5501")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

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
        ClockSkew = TimeSpan.Zero // 🔹 evita tolerancia de tiempo
    };

    // 🔹 Validar tokens revocados
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

// 🔹 Swagger con esquema de seguridad JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SistemaPOS API",
        Version = "v1"
    });

    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Introduce el token JWT así: Bearer {tu token}",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 🔹 Usar CORS ANTES del pipeline
app.UseCors("AllowFrontend");



///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// 🔹 Configurar conexión a Oracle
/*var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SistemaPOSDbContext>(options =>
    options.UseOracle(connectionString)
);

*/

// 🔹 Habilitar Swagger solo en desarrollo
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseCors("AllowLocalFront");
app.UseDefaultFiles();

/*
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
*/
app.MapControllers();

app.Run();
