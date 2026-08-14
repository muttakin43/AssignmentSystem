using AssignmentSystem.API.Middleware;
using AssignmentSystem.Application;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Application.Settings;
using AssignmentSystem.Infrastructure;
using AssignmentSystem.Infrastructure.Persistence;
using AssignmentSystem.Infrastructure.Seed;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://+:{port}");
}

//----logging--
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));



builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddFluentValidationAutoValidation();


builder.Services.AddHealthChecks();

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
        Description = "Enter: Bearer {your JWT token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


//JWT
var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings section missing in appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

builder.Services.AddAuthorization();

const string CorsPolicyName = "AllowFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? new[] { "http://localhost:4200" };
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    KnownNetworks = { },
    KnownProxies = { }
});

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsPolicyName);

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapHealthChecks("/health");

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();
var db = services.GetRequiredService<AppDbContext>();
var passwordHasher = services.GetRequiredService<IPasswordHasherService>();

try
{
    logger.LogInformation("Applying database migrations...");
    await db.Database.MigrateAsync();
    await DbInitializer.SeedAsync(db, passwordHasher, logger);
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Database migration or seeding failed — the application cannot start.");
    throw;
}

app.Lifetime.ApplicationStarted.Register(() =>
{
    var baseUrl = BrowsableBaseUrl(app);

    logger.LogInformation("AssignmentSystem API is ready.");
    logger.LogInformation("  Swagger UI   -> {Url}", $"{baseUrl}/swagger");
    logger.LogInformation("  Health check -> {Url}", $"{baseUrl}/health");
    logger.LogInformation("  API base     -> {Url}", $"{baseUrl}/api");
});

app.Run();

static string BrowsableBaseUrl(WebApplication app)
{
    var external = Environment.GetEnvironmentVariable("RENDER_EXTERNAL_URL");
    if (!string.IsNullOrWhiteSpace(external))
        return external.TrimEnd('/');

    var address = app.Urls.FirstOrDefault() ?? "http://localhost:8080";

    return address
        .Replace("://[::]", "://localhost")
        .Replace("://0.0.0.0", "://localhost")
        .Replace("://+", "://localhost")
        .Replace("://*", "://localhost")
        .TrimEnd('/');
}