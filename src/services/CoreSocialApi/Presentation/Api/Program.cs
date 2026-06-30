using Api.Middleware;
using Identity.Infrastructure;
using Posts.Infrastructure;
using Profiles.Infrastructure;
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddIdentityInfrastructure(builder.Configuration)
    .AddProfilesInfrastructure(builder.Configuration)
    .AddPostsInfrastructure(builder.Configuration);

builder.Services.AddFluentValidationAutoValidation();

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
