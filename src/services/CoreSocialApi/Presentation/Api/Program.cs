using Api.Middleware;
using Identity.Infrastructure;
using MediaService.Infrastructure;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi;
using Posts.Infrastructure;
using Profiles.Infrastructure;
using SharedInfrastructure;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

const string AllowAnyWebsiteCorsPolicy = "AllowAnyWebsite";


var builder = WebApplication.CreateBuilder(args);

var uploadsPath = ResolveLocalStorageRootPath(
    builder.Configuration["LocalStorage:RootPath"],
    builder.Environment.ContentRootPath);

if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT access token."
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            []
        }
    });
});
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddIdentityInfrastructure(builder.Configuration)
    .AddProfilesInfrastructure(builder.Configuration)
    .AddPostsInfrastructure(builder.Configuration)
    .AddMediaInfrastructure(builder.Configuration, builder.Environment.ContentRootPath);

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



app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});
app.MapControllers();

app.Run();

static string ResolveLocalStorageRootPath(string? rootPath, string contentRootPath)
{
    var configuredRootPath = string.IsNullOrWhiteSpace(rootPath)
        ? "Uploads"
        : rootPath;

    return Path.IsPathRooted(configuredRootPath)
        ? Path.GetFullPath(configuredRootPath)
        : Path.GetFullPath(Path.Combine(contentRootPath, configuredRootPath));
}
