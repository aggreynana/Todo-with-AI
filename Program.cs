using Microsoft.EntityFrameworkCore;
using Serilog;
using StackExchange.Redis;
using Todo.Extension;
using Todo.Logging;
using Todo.Storage.Context;

try
{
    // Log application startup using Serilog
    Log.Information("Starting Todo API application");

    var builder = WebApplication.CreateBuilder(args);
    var config = builder.Configuration;

    // Configure Serilog using custom configuration from appsettings.json
    // This sets up console and file logging with custom templates and level overrides
    builder.Host.UseCustomSerilog(config);

    // Registering our Db
    builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(config.GetConnectionString("DbConnection")));

    // Register Redis Distributed Cache
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = config.GetConnectionString("Redis");
        options.InstanceName = "TodoApi_";
    });

    // Register IConnectionMultiplexer for direct Redis access
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        var configuration = config.GetConnectionString("Redis") ?? "localhost:6379";
        return ConnectionMultiplexer.Connect(configuration);
    });

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddAllApiServices(config);
    builder.Services.AddAuthorization();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // Ensure database is created and migrations are applied
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Log successful application startup
    Log.Information("Todo API application started successfully");
    app.Run();
}
catch (Exception ex)
{
    // Log fatal errors that cause application termination
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    // Ensure all log messages are flushed before application exits
    Log.CloseAndFlush();
}
