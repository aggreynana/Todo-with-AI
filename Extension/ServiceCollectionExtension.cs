using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Todo.Entities;
using Todo.Model;
using Todo.Services.Interfaces;
using Todo.Services.Providers;
using Todo.Storage.Repository.Interfaces;
using Todo.Storage.Repository.Providers;

namespace Todo.Extension;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApiOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);
        return services;
    }

    public static IServiceCollection AddApiRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IActivityRepository, ActivityRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IFileAttachmentRepository, FileAttachmentRepository>();

        return services;
    }

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IActivityService, ActivityService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IFileAttachmentService, FileAttachmentService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }


    public static IServiceCollection AddBearerAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var key = configuration.GetValue<string>("JwtSettings:Key");
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("No secret key provided");

        var encodedKey = Encoding.UTF8.GetBytes(key);

        var audience = configuration.GetValue<string>("JwtSettings:Audience");
        if (string.IsNullOrWhiteSpace(audience)) throw new ArgumentException("No audience Provided");

        var issuer = configuration.GetValue<string>("JwtSettings:Issuer");
        if (string.IsNullOrWhiteSpace(issuer)) throw new ArgumentException("No issuer provided");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAlgorithms = new string[] { SecurityAlgorithms.HmacSha256 },
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(encodedKey)
            };
        });
        return services;
    }

    public static IServiceCollection AddAllApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiOptions(configuration);
        services.AddApiRepositories();
        services.AddApiServices();
        services.AddBearerAuth(configuration);

        return services;
    }
}