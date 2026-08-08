using Microsoft.EntityFrameworkCore;
using Todo.Extension;
using Todo.Storage.Context;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Registering our Db
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(config.GetConnectionString("DbConnection")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAllApiServices(config);
builder.Services.AddAuthorization();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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
