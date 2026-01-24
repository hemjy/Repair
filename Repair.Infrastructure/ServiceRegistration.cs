using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Repair.Application.Interfaces;
using Repair.Application.Interfaces.Auth;
using Repair.Application.Interfaces.Repositories;
using Repair.Domain;
using Repair.Infrastructure.Persistence;
using Repair.Infrastructure.Persistence.Data;
using Repair.Infrastructure.Persistence.Identity;
using Repair.Infrastructure.Persistence.Respositories;
using Repair.Infrastructure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repair.Infrastructure
{
    public static class ServiceRegistration
    {
        public static async Task  AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ApplicationDbContext>(options =>

                        options.UseNpgsql(configuration["DefaultConnection"] ?? ""));
            Console.WriteLine($"DefaultConnection: {configuration["DefaultConnection"]}");

            services.AddMemoryCache();
            services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddSingleton<IFileStorageService, FileStorageService>();

            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;
            })
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders();
            Console.WriteLine($"SecretKey: {configuration["JwtSettings:SecretKey"]}");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {

                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"])),
                   ValidIssuer = configuration["JwtSettings:Issuer"],
                   ValidAudience = configuration["JwtSettings:Audience"],
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true, // This automatically checks the token expiration
                   ClockSkew = TimeSpan.Zero,  // Optional: Set clock skew (default is 5 minutes)
               };

               options.Events = new JwtBearerEvents
               {
                   OnMessageReceived = context =>
                   {
                       var token = context.Request.Headers["Authorization"].ToString();
                       Console.WriteLine($"JWT Token received: {token}");
                      // logger.Error("JWT Token received: {token}", token);
                       return Task.CompletedTask;
                   },
                   OnAuthenticationFailed = context =>
                   {
                       if (context.Exception is SecurityTokenExpiredException)
                       {
                           context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                           context.Response.ContentType = "application/json";
                          // logger.Error("Authentication failed: {Message}", context.Exception.Message);
                           return context.Response.WriteAsync("{\"error\":\"Token has expired\"}");
                       }

                       if (context.Exception is SecurityTokenException)
                       {
                           context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                           context.Response.ContentType = "application/json";
                           return context.Response.WriteAsync("{\"error\":\"Invalid token\"}");
                       }

                       return Task.CompletedTask;
                   }
               };
           });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Seed data after the context is configured
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var serives = scope.ServiceProvider;
            var context = serives.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            DataSeeder.Seed(context);
            var roleManager = serives.GetRequiredService<RoleManager<Role>>();
            var userManager = serives.GetRequiredService<UserManager<User>>();
            await SeedRolesAsync(roleManager);
            await SeedAdminUserAsync(userManager, roleManager);


        }

        // seed roles - Customer, Admin
        static async Task SeedRolesAsync(RoleManager<Role> roleManager)
        {
            var roleNames = new[] { "Customer", "Admin" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = Role.Create(roleName);
                    await roleManager.CreateAsync(role);
                }
            }
        }

        // Seed an Admin user 
        static async Task SeedAdminUserAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@admin.com");

            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = "admin@admin.com",
                    Email = "admin@admin.com",
                    IsActive = true,
                    FirstName = "Admin",
                    LastName = "Admin"
                };

                var result = await userManager.CreateAsync(user, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            // Regular user 1
            var user1 = await userManager.FindByEmailAsync("user1@example.com");
            if (user1 == null)
            {
                var newUser1 = new User
                {
                    UserName = "james@example.com",
                    Email = "user1@example.com",
                    LastName = "James",
                    FirstName = "Justin"
                };
                var result = await userManager.CreateAsync(newUser1, "User1@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser1, "Customer");
                }
            }

        }
    }
}
