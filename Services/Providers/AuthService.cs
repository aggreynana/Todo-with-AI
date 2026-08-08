using Microsoft.AspNetCore.Identity;
using Todo.Entities;
using Todo.Model;
using Todo.Model.AuthDto;
using Todo.Model.UserDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

// STEP 1: Implement the IAuthService interface
// This class contains the business logic for authentication operations
// It handles user authentication and coordinates JWT token generation via JwtTokenService
public class AuthService : IAuthService
{
    // STEP 2: Inject required dependencies through constructor injection
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IPasswordHasher<UserEntity> passwordHasher, IJwtTokenService jwtTokenService, JwtSettings jwtSettings)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings;
    }

    // STEP 3: Implement LoginAsync method
    // This method validates user credentials and generates a JWT token
    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest)
    {
        // STEP 4: Retrieve the user by email
        // We use email as the unique identifier for login
        var user = await _userRepository.GetUserByEmailAsync(loginRequest.Email);

        // STEP 5: Check if the user exists
        if (user == null)
        {
            // STEP 6: Return an error response if user not found
            return ApiResponse<LoginResponseDto>.NotFound("Invalid email or password");
        }

        // STEP 7: Verify the password using ASP.NET Core Identity's password hasher
        // This provides secure password verification with proper hashing
        var passwordResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);

        // STEP 8: Check if the password verification failed
        if (passwordResult == PasswordVerificationResult.Failed)
        {
            // STEP 9: Return an error response if password is incorrect
            return ApiResponse<LoginResponseDto>.Unauthorized();
        }

        // STEP 10: Generate JWT token for the authenticated user using JwtTokenService
        var token = _jwtTokenService.GenerateJwtToken(user);

        // STEP 11: Calculate token expiration time using JwtSettings
        var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

        // STEP 12: Create the login response DTO
        var responseDto = new LoginResponseDto
        {
            Token = token,
            Expiration = expiration,
            UserId = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        // STEP 13: Return success response with the token
        return ApiResponse<LoginResponseDto>.OkResponse("Login successful", responseDto);
    }
}
