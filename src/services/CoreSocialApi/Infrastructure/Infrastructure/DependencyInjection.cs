using Application.Abstractions;
using Identity.Application.Abstractions.Authentication;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Settings;
using Infrastructure.Services;
using Infrastructure.Time;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Profiles.Application.Abstractions;
using SharedInfrastructure.Authentication;
using SharedInfrastructure.Persistence;
using SharedInfrastructure.Repositories.Identity;
using SharedInfrastructure.Repositories.Profiles;
using System.Text;

namespace SharedInfrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddPersistence(configuration)
                .AddIdentityInfrastructure(configuration).
                AddProfileInfrastructure(configuration);


            return services;
        }

        private static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                   ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());


            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();


            return services;
        }

        private static IServiceCollection AddIdentityInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.SectionName));

            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException($"Configuration section '{JwtOptions.SectionName}' was not found.");

            services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
            services.AddSingleton<ITokenService, TokenService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            services.AddAuthorization();

            return services;
        }

        private static IServiceCollection AddProfileInfrastructure(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            services.AddScoped<IProfileRepository, ProfileRepository>();

            return services;
        }
    }
}
