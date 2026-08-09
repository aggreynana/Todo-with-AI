using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Data;
using Todo.Entities;
using Todo.Extension;
using Todo.Storage.Context;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Registering our Db
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(config.GetConnectionString("DbConnection")));

// Register PasswordHasher separately
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAllApiServices(config);
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<UserEntity>>();
    
    // Ensure database is created
    context.Database.Migrate();
    
    // Seed data
    await DbSeeder.SeedDataAsync(context, passwordHasher);
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

app.Run();
