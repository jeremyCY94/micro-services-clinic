using authServices.Core.Application;
using authServices.Core.Entities;
using authServices.Core.JwtLogic;
using authServices.Core.Persistence;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SeguridadContexto>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("ConexionDB"));//CONEXION A LA BASE DE DATOS
});

var userBuilder = builder.Services.AddIdentityCore<Usuario>();// METODOS DE CRUD en automatico para usuarios
var identityBuilder = new IdentityBuilder(userBuilder.UserType, userBuilder.Services);
identityBuilder.AddEntityFrameworkStores<SeguridadContexto>();//SE ELANZA CON LA BASE DE DATOS
identityBuilder.AddSignInManager<SignInManager<Usuario>>();
builder.Services.TryAddSingleton<ISystemClock, SystemClock>();

builder.Services.AddControllers().AddFluentValidation(x=>x.RegisterValidatorsFromAssemblyContaining<Register>());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Register.UsuarioRegisterCommand).Assembly));
builder.Services.AddAutoMapper(typeof(Register.UsuarioRegisterHandler));
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<IUsuarioSession, UsuarioSession>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsRule", rule =>
    {
        rule.AllowAnyHeader().AllowAnyMethod().WithOrigins("*");
    });
});

builder.Services
    .AddHttpContextAccessor()
    .AddAuthorization()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["token:key"]))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsRule");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var contexto = app.Services.CreateScope())
{
    var services = contexto.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var contextoEF = services.GetRequiredService<SeguridadContexto>();
        SeguridadData.InsertarUsuario(contextoEF, userManager).Wait();
    }
    catch (Exception e) { 
        var logging = services.GetRequiredService<ILogger<Program>>();
        logging.LogError(e, "Error cuando registra usuario");
    }
}

    app.Run();


