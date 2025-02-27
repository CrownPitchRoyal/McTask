using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Extensions;
using UserManagement.API.Middlewares;
using UserManagement.Application.Services;
using UserManagement.Infrastructure;
using UserManagement.Persistence;
using UserManagement.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var configuration = builder.Configuration;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File($"logs/log-{DateTime.UtcNow:yyyy-MM-dd}.txt", rollingInterval: RollingInterval.Day)
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithClientIp()
    .Enrich.FromLogContext()
    .CreateLogger();


// Add Logging
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Learn configuring Swagger: https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Management API", Version = "v1" });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description =
            "API Key needed to access the endpoints. Call Login to acquire, then add it to the request header.",
        In = ParameterLocation.Header,
        Name = "ApiKey",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                Scheme = "ApiKeyScheme",
                Name = "ApiKey",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Register infrastructure dependencies
builder.Services.AddInfrastructureServices(configuration);

// Register custom services
builder.Services.AddSingleton<PasswordService>();

var app = builder.Build();

// Configure the HTTP request pipeline | Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register custom middleware
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Seed the database with starter data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await UserDbContextSeed.SeedAsync(dbContext, new PasswordService());
}

app.Run();