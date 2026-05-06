using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
using WebApplication17.Models;
using WebApplication17.Repositories;
using WebApplication17.Services;

namespace WebApplication17
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ============================
            // Services
            // ============================

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ProjectDB API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer <your_token>"
                });

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
                        new string[] {}
                    }
                });
            });

            // ============================
            // DATABASE CONFIG
            // ============================

            var dbProvider = builder.Configuration["DatabaseProvider"];

            if (dbProvider == "SqlServer")
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString("SqlServerConnection")
                    ));
            }
            else
            {
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(
                        builder.Configuration.GetConnectionString("DefaultConnection")
                    ));
            }

            // ============================
            // JWT Authentication
            // ============================

            var key = builder.Configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(key))
                throw new Exception("JWT Key is missing");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(key)
                        )
                    };
                });

            builder.Services.AddAuthorization();

            // ============================
            // CORS
            // ============================

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // ============================
            // Dependency Injection
            // ============================


            // Dependency Injection
            builder.Services.AddSingleton<CloudinaryService>(); 



            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<IAuthService, AuthService>();

            var app = builder.Build();

            // ============================
            // Middleware Pipeline (ORDER MATTERS!)
            // ============================

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectDB API V1");
                c.RoutePrefix = string.Empty;
            });

            app.MapGet("/swagger", context =>
            {
                context.Response.Redirect("/");
                return Task.CompletedTask;
            });

            app.UseRouting();           // 1️⃣ Routing first

            app.UseCors("AllowAll");    // 2️⃣ CORS second (critical position)

            // app.UseHttpsRedirection();

            app.UseAuthentication();    // 3️⃣ Authentication third

            app.UseAuthorization();     // 4️⃣ Authorization fourth

            app.MapControllers();       // 5️⃣ Map controllers last

            app.Run();
        }
    }
}