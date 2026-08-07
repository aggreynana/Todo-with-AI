using Microsoft.AspNetCore.Identity;
using Todo.Entities;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Extension;

// STEP 1: Create extension method for IServiceCollection
// This extension method centralizes all dependency injection configuration
// It follows the single responsibility principle by keeping DI logic separate
public static class ServiceCollectionExtension
{
    // STEP 2: Define the extension method to register all services
    // This method will be called in Program.cs to configure the application's services
    public static IServiceCollection AddApiOptions(this IServiceCollection services)
    {
        // STEP 3: Register ASP.NET Core Identity PasswordHasher
        // This is used for secure password hashing and verification
        services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();

        // STEP 4: Register Repository services
        // Repositories are registered with scoped lifetime (per HTTP request)
        // This is appropriate for Entity Framework DbContext
        services.AddScoped<IUserRepository, Todo.Storage.Repository.Providers.UserRepository>();
        services.AddScoped<IActivityRepository, Todo.Storage.Repository.Providers.ActivityRepository>();
        services.AddScoped<ICategoryRepository, Todo.Storage.Repository.Providers.CategoryRepository>();
        services.AddScoped<ICommentRepository, Todo.Storage.Repository.Providers.CommentRepository>();
        services.AddScoped<ITagRepository, Todo.Storage.Repository.Providers.TagRepository>();
        services.AddScoped<IFileAttachmentRepository, Todo.Storage.Repository.Providers.FileAttachmentRepository>();

        // STEP 5: Register Service layer
        // Services are also registered with scoped lifetime
        // They orchestrate business logic and coordinate between controllers and repositories
        services.AddScoped<IUserService, Todo.Services.Providers.UserService>();
        services.AddScoped<IActivityService, Todo.Services.Providers.ActivityService>();
        services.AddScoped<ICategoryService, Todo.Services.Providers.CategoryService>();
        services.AddScoped<ICommentService, Todo.Services.Providers.CommentService>();
        services.AddScoped<ITagService, Todo.Services.Providers.TagService>();
        services.AddScoped<IFileAttachmentService, Todo.Services.Providers.FileAttachmentService>();

        // STEP 6: Register Authentication service
        // Handles user authentication and JWT token generation
        services.AddScoped<IAuthService, Todo.Services.Providers.AuthService>();

        // STEP 7: Return the service collection for method chaining
        return services;
    }
}