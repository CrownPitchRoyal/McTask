using UserManagement.Application.Services;
using UserManagement.Infrastructure;
using UserManagement.Persistence;
using UserManagement.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

// Load configuration
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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

//app.UseHttpsRedirection();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Seed
// Seed the database with starter data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await UserDbContextSeed.SeedAsync(dbContext);
    await UserDbContextSeed.SeedAsync(dbContext, new PasswordService());
}

app.Run();