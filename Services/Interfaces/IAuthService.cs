using Todo.Model;
using Todo.Model.AuthDto;

namespace Todo.Services.Interfaces;

// STEP 1: Define the interface for Authentication service
// This interface defines the contract for authentication operations
public interface IAuthService
{
    // STEP 2: Create method for user login
    // This method validates credentials and generates a JWT token
    Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequest);
}
