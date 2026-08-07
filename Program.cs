using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Todo.Extension;
using Todo.Storage.Context;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddApiOptions();
builder.Services.AddControllers();


// Registering our Db
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(config.GetConnectionString("DbConnection")));

// STEP 1: Configure JWT Bearer Authentication
// This configures the application to use JWT tokens for authentication
builder.Services.AddAuthentication(options =>
{
    // STEP 2: Set the default authentication scheme to JWT Bearer
    // This tells the application to use JWT tokens for authentication
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // STEP 3: Configure JWT token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // STEP 4: Validate the token issuer
        // Ensures the token was issued by our application
        ValidateIssuer = true,
        ValidIssuer = config["JwtSettings:Issuer"],

        // STEP 5: Validate the token audience
        // Ensures the token is intended for our application
        ValidateAudience = true,
        ValidAudience = config["JwtSettings:Audience"],

        // STEP 6: Validate the token lifetime
        // Ensures the token has not expired
        ValidateLifetime = true,

        // STEP 7: Validate the signing key
        // Ensures the token was signed with our secret key
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured")))
    };
});

// STEP 8: Add Authorization service
// This enables authorization attributes on controllers and actions
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

// STEP 9: Add Authentication middleware to the pipeline
// This must be added before Authorization middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
