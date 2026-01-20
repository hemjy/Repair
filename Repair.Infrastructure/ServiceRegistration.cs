using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Repair.Application.Interfaces;
using Repair.Application.Interfaces.Repositories;
using Repair.Infrastructure.Persistence;
using Repair.Infrastructure.Persistence.Data;
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
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ApplicationDbContext>(options =>

                        options.UseNpgsql(configuration["DefaultConnection"] ?? ""));
            Console.WriteLine($"DefaultConnection: {configuration["DefaultConnection"]}");

            services.AddMemoryCache();
            services.AddScoped(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddSingleton<IFileStorageService, FileStorageService>();

            // Seed data after the context is configured
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            DataSeeder.Seed(context);


        }
    }
}
