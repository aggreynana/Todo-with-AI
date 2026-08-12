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
    private readonly IUserRepository _userRepository;

    private readonly IPasswordHasher<UserEntity> _passwordHasher;

    private readonly IJwtTokenService _jwtTokenService;

    private readonly JwtSettings _jwtSettings;

    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IPasswordHasher<UserEntity> passwordHasher, IJwtTokenService jwtTokenService, JwtSettings jwtSettings, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings;
        _logger = logger;
    }


    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest)
    {
        _logger.LogInformation("Login attempt for email: {Email}", loginRequest.Email);

        var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);

        // Check if the user exists in the database
        if (user == null)
        {
            _logger.LogWarning("Login failed for email: {Email} - User not found", loginRequest.Email);
            return ApiResponse<LoginResponseDto>.NotFound("Invalid email or password");
        }


        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);

        // Check if the password verification failed
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            _logger.LogWarning("Login failed for email: {Email} - Invalid password", loginRequest.Email);
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

        return ApiResponse<LoginResponseDto>.OkResponse("Login successful", responseDto);
    }
}
