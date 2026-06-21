using Api.Middleware;
using FluentValidation;
using Identity.Application.Abstractions;
using Identity.Application.Authorization;
using Identity.Application.Services;
using Identity.Application.Validators;
using Microsoft.AspNetCore.Authorization;
using Posts.Application.Authorization;
using SharedInfrastructure;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

const string AllowAnyWebsiteCorsPolicy = "AllowAnyWebsite";

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowAnyWebsiteCorsPolicy, policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOnly, policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy(Policies.CanDeleteAnyPost, policy =>
        policy.RequireClaim("permission", Permissions.PostsDeleteAny));

    options.AddPolicy(Policies.CanModerate, policy =>
        policy.RequireClaim("permission", Permissions.ModerationReview));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

builder.Services.AddScoped<IAuthorizationHandler, PostOwnerOrAdminHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseCors(AllowAnyWebsiteCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
