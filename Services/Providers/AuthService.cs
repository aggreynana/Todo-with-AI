using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Todo.Entities;
using Todo.Model;
using Todo.Model.AuthDto;
using Todo.Model.UserDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

// STEP 1: Implement the IAuthService interface
// This class contains the business logic for authentication operations
// It handles user authentication and JWT token generation
public class AuthService : IAuthService
{
    // STEP 2: Inject required dependencies through constructor injection
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;

    public AuthService(IUserRepository userRepository, IConfiguration configuration, IPasswordHasher<UserEntity> passwordHasher)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
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

        // STEP 10: Generate JWT token for the authenticated user
        var token = GenerateJwtToken(user);

        // STEP 11: Calculate token expiration time
        var expirationInMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");
        var expiration = DateTime.UtcNow.AddMinutes(expirationInMinutes);

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

    // STEP 14: Create helper method to generate JWT token
    // This private method handles the JWT token generation logic
    private string GenerateJwtToken(UserEntity user)
    {
        // STEP 15: Retrieve JWT settings from configuration
        var secretKey = _configuration["JwtSettings:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured");

        var issuer = _configuration["JwtSettings:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");

        var audience = _configuration["JwtSettings:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
        
        var expirationInMinutes = _configuration.GetValue<int>("JwtSettings:ExpirationInMinutes");

        // STEP 16: Create security key from the secret
        // The secret is converted to bytes for signing
        var key = Encoding.UTF8.GetBytes(secretKey);

        // STEP 17: Create claims for the JWT token
        // Claims contain information about the user embedded in the token
        var claims = new List<Claim>
        {
            // STEP 18: Add user ID claim
            new(ClaimTypes.NameIdentifier, user.Id),

            // STEP 19: Add email claim
            new(ClaimTypes.Email, user.Email),

            // STEP 20: Add name claim
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),

            // STEP 21: Add custom claims for user details
            new("FirstName", user.FirstName),
            new("LastName", user.LastName)
        };

        // STEP 22: Create signing credentials
        // Uses HMAC SHA256 algorithm for token signing
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        );

        // STEP 23: Create the JWT token descriptor
        // This describes the token's properties
        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
            signingCredentials: credentials
        );

        // STEP 24: Write and return the JWT token as a string
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenDescriptor);

        return token;
    }
}
