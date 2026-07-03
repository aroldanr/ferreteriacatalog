using ferreteria_catalog.Data;
using ferreteria_catalog.Repositories;
using ferreteria_catalog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog file logging.
var logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log.txt");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Configure logging providers.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 50 MB
});

// Configure Razor Pages routes.
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories and services.
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IModulosRepository, ModulosRepository>();
builder.Services.AddScoped<IModuloService, ModuloService>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Add controllers and Razor Pages.
builder.Services.AddControllers();
builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();

// Configure JWT.
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

// Configure authorization policies.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Colab", policy => policy.RequireRole("Colab"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        corsBuilder =>
        {
            corsBuilder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseRouting();

// Add the JWT token from the cookie into request headers when needed.
app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["jwtToken"];
    if (!string.IsNullOrEmpty(token))
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            context.Request.Headers.Add("Authorization", "Bearer " + token);
            Console.WriteLine("Token added to request header: " + token);
        }
    }

    await next();
});

app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "images")),
    RequestPath = "/images"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/bootstrap")),
    RequestPath = "/bootstrap"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/Login/Login"));

app.Run();
