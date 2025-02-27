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
builder.Services.AddSwaggerGen();

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