using ferreteria_catalog.Data;
using ferreteria_catalog.Repositories;
using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog para logging en archivos
var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log.txt");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configurar logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configuración de Razor Pages con convenciones de rutas
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/Productos/SubirImagen", "Productos/SubirImagen");
    options.Conventions.AddPageRoute("/Productos/CargarImagenesLote", "Productos/CargarImagenesLote");
});

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories and services
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add controllers and Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Configuración de IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Configuración de JWT
var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSecretKey"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Configura el servicio de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

// End Register repositories and services

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseRouting();

// Middleware para agregar el token JWT de las cookies a los encabezados de las solicitudes
app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["jwtToken"];
    if (!string.IsNullOrEmpty(token))
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Request.Headers.Add("Authorization", "Bearer " + token);
            Console.WriteLine("Token añadido al encabezado: " + token); // Log para depuración
        }
    }
    await next();
});


app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// Configurar la página de inicio
app.MapGet("/", () => Results.Redirect("/Productos/Index"));

app.Run();
