using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Todo.Entities;
using Todo.Model;
using Todo.Model.AuthDto;
using Todo.Model.UserDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

/// <summary>
/// Service for handling authentication operations including user login and JWT token generation
/// </summary>
public class AuthService : IAuthService
{
    // Repository for accessing user data from the database
    private readonly IUserRepository _userRepository;
    // Password hasher for secure password verification using ASP.NET Core Identity
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    // Service for generating JWT tokens for authenticated users
    private readonly IJwtTokenService _jwtTokenService;
    // JWT configuration settings containing expiration time and other token parameters
    private readonly JwtSettings _jwtSettings;
    // Logger for tracking authentication attempts and security events
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IPasswordHasher<UserEntity> passwordHasher, IJwtTokenService jwtTokenService, JwtSettings jwtSettings, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user with email and password credentials
    /// Returns a JWT token upon successful authentication
    /// </summary>
    /// <param name="loginRequest">The login credentials containing email and password</param>
    /// <returns>API response with JWT token on success, or error response on failure</returns>
    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest)
    {
        // Log the login attempt for security monitoring and debugging
        _logger.LogInformation("Login attempt for email: {Email}", loginRequest.Email);

        // Retrieve the user by email from the database
        // Email is used as the unique identifier for login
        var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);

        // Check if the user exists in the database
        if (user == null)
        {
            // Log a warning for failed login attempt due to non-existent user
            // This helps detect potential brute force attacks or account enumeration
            _logger.LogWarning("Login failed for email: {Email} - User not found", loginRequest.Email);
            // Return generic error message to avoid revealing user existence
            return ApiResponse<LoginResponseDto>.NotFound("Invalid email or password");
        }

        // Verify the password using ASP.NET Core Identity's secure password hasher
        // This handles proper password verification with bcrypt or other secure algorithms
        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);

        // Check if the password verification failed
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            // Log a warning for failed login attempt due to invalid password
            // This helps detect potential brute force attacks
            _logger.LogWarning("Login failed for email: {Email} - Invalid password", loginRequest.Email);
            // Return unauthorized response without specifying the exact reason
            return ApiResponse<LoginResponseDto>.Unauthorized();
        }

        // Generate JWT token for the authenticated user using the JwtTokenService
        var token = _jwtTokenService.GenerateJwtToken(user);

        // Calculate token expiration time based on JWT settings
        var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

        // Create the login response DTO with token and user information
        var responseDto = new LoginResponseDto
        {
            Token = token,
            Expiration = expiration,
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        // Log successful authentication for audit trail
        _logger.LogInformation("Login successful for email: {Email}, UserId: {UserId}", loginRequest.Email, user.Id);
        // Return success response with the JWT token and user details
        return ApiResponse<LoginResponseDto>.OkResponse("Login successful", responseDto);
    }
}
