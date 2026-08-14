using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Application.Settings;
using AssignmentSystem.Infrastructure.Auth;
using AssignmentSystem.Infrastructure.Common;
using AssignmentSystem.Infrastructure.Persistence;
using AssignmentSystem.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = PostgresConnectionString.Resolve(configuration);

            services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
           
            services.AddScoped<IPasswordHasherService, PasswordHasherWrapper>();
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            return services;
        }
    }
}