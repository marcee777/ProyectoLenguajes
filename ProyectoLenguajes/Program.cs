
// Inicio usings para lo de JWT
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
// Fin usings para lo de JWT

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProyectoLenguajes.Data;
using ProyectoLenguajes.Data.Repository.Interfaces;
using ProyectoLenguajes.Data.Repository;
using ProyectoLenguajes.Utilities;
using ProyectoLenguajes.Services;
using ProyectoLenguajes.Models;
using Microsoft.AspNetCore.Authentication.Cookies;



/**
 * Archivo de configuración principal para la aplicación ASP.NET Core.
 * 
 * Configura los servicios y el pipeline de middleware, incluyendo:
 * - Conexión a la base de datos SQL Server mediante Entity Framework Core.
 * - Configuración de Identity para autenticación y autorización con soporte para JWT y cookies.
 * - Configuración de políticas CORS abiertas para permitir acceso desde cualquier origen.
 * - Configuración y habilitación de sesiones para almacenamiento temporal.
 * - Registro de servicios personalizados, como UnitOfWork, EmailSender y el servicio en background OrderStatusUpdaterService.
 * - Definición de rutas MVC y Razor Pages.
 * 
 * La configuración JWT se lee desde appsettings.json y se utiliza para validar tokens en las peticiones API.
 * 
 * @author Melanie Arce C30634
 * @author Carolina Rodríguez C36640
 * @author Marcela Rojas C36975
 * @version 07/07/25
 */



var builder = WebApplication.CreateBuilder(args);

// Inicio de lectura de configuracion JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
// Fin de lectura de configuracion JWT desde appsettings.json

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// Inicio de configuración de JWT Authentication

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Cambiado
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) // Agregado para web
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => // JWT sigue funcionando
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        NameClaimType = System.Security.Claims.ClaimTypes.Name
    };
});

// Fin de configuracion de JWT Authentication

builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ApplicationUser>();

// Agregamos esto para lo de kitchen/order

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

////////////////////////////////////

builder.Services.AddHostedService<OrderStatusUpdaterService>();

//=======================================CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
//=======================================CORS

var app = builder.Build();

app.UseDeveloperExceptionPage();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//=======================================CORS
app.UseCors("AllowAnyOrigin");
//=======================================CORS

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();  // <<<< Esto es de kitchen/order

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
