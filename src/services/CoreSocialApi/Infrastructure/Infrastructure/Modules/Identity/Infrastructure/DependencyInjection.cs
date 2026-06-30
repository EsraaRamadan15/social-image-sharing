using Identity.Application;
using Identity.Application.Abstractions.Authentication;
using Identity.Application.Abstractions.Repositories;
using Identity.Application.Authorization;
using Identity.Application.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedInfrastructure.Authentication;
using SharedInfrastructure.Repositories.Identity;
using System.Text;

namespace Identity.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddIdentityApplication();

            services
                .AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.SectionName));

            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException($"Configuration section '{JwtOptions.SectionName}' was not found.");

            services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
            services.AddSingleton<IRefreshTokenHasher, Sha256RefreshTokenHasher>();
            services.AddSingleton<ITokenService, TokenService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserSessionRepository, UserSessionRepository>();
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

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy(Policies.AdminOnly, policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy(Policies.CanDeleteAnyPost, policy =>
                    policy.RequireClaim("permission", Permissions.PostsDeleteAny));

                options.AddPolicy(Policies.CanModerate, policy =>
                    policy.RequireClaim("permission", Permissions.ModerationReview));
            });

            return services;
        }
    }
}
