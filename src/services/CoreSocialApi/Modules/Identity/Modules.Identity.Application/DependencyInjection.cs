using FluentValidation;
using Identity.Application.Abstractions;
using Identity.Application.Dtos;
using Identity.Application.Services;
using Identity.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

            return services;
        }
    }
}
