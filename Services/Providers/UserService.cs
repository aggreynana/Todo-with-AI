using System.Linq;
using System;
using Todo.Entities;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Model.UserDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Todo.Services.Providers;

// STEP 1: Implement the IUserService interface
// This class contains the business logic for User operations
// It acts as a bridge between the controller and the repository layer
public class UserService : IUserService
{
    // STEP 2: Inject the IUserRepository through constructor injection
    // This follows the dependency injection pattern for loose coupling
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<UserEntity> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtSettings _jwtSettings;

    public UserService(IUserRepository userRepository, IPasswordHasher<UserEntity> passwordHasher, IJwtTokenService jwtTokenService, JwtSettings jwtSettings)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings;
    }

    // STEP 3: Implement CreateUserAsync method
    // This method handles the creation of a new user with auto-login functionality
    public async Task<ApiResponse<AuthResponseDto>> CreateUserAsync(CreateUserRequestDto userDto)
    {
        try
        {
            // First check if the user already exists in our database
            var user = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (user != null)
                return ApiResponse<AuthResponseDto>.Conflict();

            // STEP 4: Map the DTO to the entity
            // Convert the incoming DTO to the domain entity for database operations
            var userEntity = new UserEntity
            {
                FirstName = userDto.FirstName,
                MiddleName = userDto.MiddleName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                // Id and CreatedOn are set automatically in BaseEntity
            };

            // STEP 5: Hash the password using ASP.NET Core Identity's password hasher
            // This provides secure password hashing before storing in the database
            userEntity.PasswordHash = _passwordHasher.HashPassword(userEntity, userDto.Password);

            // STEP 6: Call the repository to add the entity to the database
            var isUserAdded = await _userRepository.AddUserAsync(userEntity);

            // STEP 7: Check if the operation was successful
            if (!isUserAdded)
            {
                // STEP 8: Return an error response if creation failed
                return ApiResponse<AuthResponseDto>.InternalServerError();
            }

            // STEP 9: Generate JWT token for the newly created user
            // This provides auto-login functionality after registration
            var token = _jwtTokenService.GenerateJwtToken(userEntity);

            // STEP 10: Calculate token expiration time using JwtSettings
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            // STEP 11: Map the entity back to DTO for the response
            var responseDto = new AuthResponseDto()
            {
                Token = token,
                Expiration = expiration,
                UserId = userEntity.Id,
                Email = userEntity.Email,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName
            };

            // STEP 12: Return a success response with the created user
            return ApiResponse<AuthResponseDto>.CreatedResponse("User", responseDto);
        }
        catch (Exception)
        {
            // Log the exception (consider adding logging here)
            // Return a generic error response to avoid exposing sensitive information
            return ApiResponse<AuthResponseDto>.InternalServerError();
        }
    }

    // STEP 11: Implement GetUserByIdAsync method
    // This method retrieves a single user by its unique identifier
    public async Task<ApiResponse<GetUserResponseDto>?> GetUserByIdAsync(string id)
    {
        // STEP 12: Call the repository to get the user
        var userEntity = await _userRepository.GetUserById(id);

        // STEP 13: Check if the user exists
        if (userEntity == null)
        {
            // STEP 14: Return null if user not found
            return ApiResponse<GetUserResponseDto>.Conflict();
        }

        // STEP 15: Map the entity to DTO
        var responseDto = MapToResponseDto(userEntity);

        // STEP 16: Return the user in a success response
        return ApiResponse<GetUserResponseDto>.OkResponse("User retrieved successfully", responseDto);
    }

    // Implement GetUsersAsync method with pagination and filtering
    // This method retrieves users from the database with pagination and filtering support
    public async Task<ApiResponse<PageResultResponseDto<GetUserResponseDto>>> GetUsersAsync(UserFilterDto? userFilter = null)
    {
        // Use default filter if not provided
        var filter = userFilter ?? new UserFilterDto();

        // Call the repository with filter
        var userPageResult = await _userRepository.GetUsersWithPaginationAsync(filter);

        // Map entities to DTOs
        var userDtos = userPageResult.Records.Select(MapToResponseDto).ToList();

        // Create paginated response with metadata
        var pageResult = new PageResultResponseDto<GetUserResponseDto>
        {
            Page = userPageResult.Page,
            PageSize = userPageResult.PageSize,
            Records = userDtos,
            TotalCount = userPageResult.TotalCount,
            TotalPages = userPageResult.TotalPages
        };

        // Return paginated response
        return ApiResponse<PageResultResponseDto<GetUserResponseDto>>.OkResponse("Users retrieved successfully", pageResult);
    }

    // STEP 21: Implement UpdateUserAsync method
    // This method updates an existing user
    public async Task<ApiResponse<GetUserResponseDto>> UpdateUserAsync(string id, UpdateUserRequestDto userUpdate)
    {
        // STEP 22: First, retrieve the existing user by ID
        var existingUser = await _userRepository.GetUserById(id);

        // STEP 23: Check if the user exists
        if (existingUser == null)
        {
            // STEP 24: Return an error response if user not found
            return ApiResponse<GetUserResponseDto>.NotFound("User not found");
        }

        // STEP 25: Update only the fields that are provided (partial update)
        // Note: Email is not updated as it's used as an identifier
        existingUser.FirstName = userUpdate.FirstName;
        existingUser.MiddleName = userUpdate.MiddleName;
        existingUser.LastName = userUpdate.LastName;

        // STEP 26: Set the ModifiedOn timestamp
        existingUser.ModifiedOn = DateTime.UtcNow;

        // STEP 27: Call the repository to update the entity
        var isUserUpdated = await _userRepository.UpdateUSerAsync(existingUser);

        // STEP 28: Check if the update was successful
        if (!isUserUpdated)
        {
            // STEP 29: Return an error response if update failed
            return ApiResponse<GetUserResponseDto>.InternalServerError();
        }

        // STEP 30: Map the updated entity to DTO
        var responseDto = MapToResponseDto(existingUser);

        // STEP 31: Return a success response with the updated user
        return ApiResponse<GetUserResponseDto>.AcceptedResponse();
    }

    // STEP 32: Implement DeleteUserByIdAsync method
    // This method deletes a user by its ID
    public async Task<ApiResponse<bool>> DeleteUserByIdAsync(string id)
    {
        // STEP 33: First, retrieve the existing user
        var existingUser = await _userRepository.GetUserById(id);

        // STEP 34: Check if the user exists
        if (existingUser == null)
        {
            // STEP 35: Return an error response if user not found
            return ApiResponse<bool>.NotFound("User not found");
        }

        // STEP 36: Call the repository to delete the entity
        var isUserDeleted = await _userRepository.DeleteUserByIdAsync(existingUser);

        // STEP 37: Check if the deletion was successful
        if (!isUserDeleted)
        {
            // STEP 38: Return an error response if deletion failed
            return ApiResponse<bool>.InternalServerError();
        }

        // STEP 39: Return a success response indicating successful deletion
        return ApiResponse<bool>.NoContent();
    }

    // STEP 40: Create a helper method to map Entity to DTO
    // This private method reduces code duplication and ensures consistent mapping
    private static GetUserResponseDto MapToResponseDto(UserEntity entity)
    {
        return new GetUserResponseDto
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            MiddleName = entity.MiddleName,
            LastName = entity.LastName,
            Email = entity.Email
        };
    }
}