using System.Linq;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Todo.Entities;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Model.UserDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

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
    private readonly ICacheService _cacheService;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IPasswordHasher<UserEntity> passwordHasher, IJwtTokenService jwtTokenService, JwtSettings jwtSettings, ICacheService cacheService, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _jwtSettings = jwtSettings;
        _cacheService = cacheService;
        _logger = logger;
    }


    public async Task<ApiResponse<AuthResponseDto>> CreateUserAsync(CreateUserRequestDto userDto)
    {
        try
        {
            _logger.LogInformation("Creating new user with email: {Email}", userDto.Email);
            var user = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (user != null)
            {
                _logger.LogWarning("User creation failed - user already exists with email: {Email}", userDto.Email);
                return ApiResponse<AuthResponseDto>.Conflict();
            }

            var userEntity = new UserEntity
            {
                FirstName = userDto.FirstName,
                MiddleName = userDto.MiddleName,
                LastName = userDto.LastName,
                Email = userDto.Email,
            };

            userEntity.PasswordHash = _passwordHasher.HashPassword(userEntity, userDto.Password);

            var isUserAdded = await _userRepository.AddUserAsync(userEntity);

            if (!isUserAdded)
            {
                _logger.LogError("User creation failed for email: {Email}", userDto.Email);
                return ApiResponse<AuthResponseDto>.InternalServerError();
            }

            var token = _jwtTokenService.GenerateJwtToken(userEntity);

            // Calculate token expiration time using JwtSettings
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            // Map the entity back to DTO for the response
            var responseDto = new AuthResponseDto()
            {
                Token = token,
                Expiration = expiration,
                UserId = userEntity.Id,
                Email = userEntity.Email,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName
            };

            _logger.LogInformation("User created successfully with ID: {UserId}", userEntity.Id);
            return ApiResponse<AuthResponseDto>.CreatedResponse("User", responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "User creation failed with exception for email: {Email}", userDto.Email);
            return ApiResponse<AuthResponseDto>.InternalServerError();
        }
    }


    public async Task<ApiResponse<GetUserResponseDto>?> GetUserByIdAsync(string id)
    {
        _logger.LogInformation("Fetching user with ID: {UserId}", id);
        // Try to get from cache first
        var cacheKey = $"user_{id}";
        var cachedUser = await _cacheService.GetAsync<GetUserResponseDto>(cacheKey);

        if (cachedUser != null)
        {
            _logger.LogInformation("User retrieved from cache with ID: {UserId}", id);
            return ApiResponse<GetUserResponseDto>.OkResponse("User retrieved from cache", cachedUser);
        }

        var userEntity = await _userRepository.GetUserById(id);

        if (userEntity == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return ApiResponse<GetUserResponseDto>.Conflict();
        }

        // Map the entity to DTO
        var responseDto = MapToResponseDto(userEntity);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, responseDto, TimeSpan.FromMinutes(5));

        _logger.LogInformation("User retrieved successfully with ID: {UserId}", id);

        return ApiResponse<GetUserResponseDto>.OkResponse("User retrieved successfully", responseDto);
    }


    public async Task<ApiResponse<PageResultResponseDto<GetUserResponseDto>>> GetUsersAsync(UserFilterDto? userFilter = null)
    {
        _logger.LogInformation("Fetching users with filter: {@Filter}", userFilter);
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

        _logger.LogInformation("Retrieved {Count} users", userDtos.Count);

        return ApiResponse<PageResultResponseDto<GetUserResponseDto>>.OkResponse("Users retrieved successfully", pageResult);
    }


    public async Task<ApiResponse<GetUserResponseDto>> UpdateUserAsync(string id, UpdateUserRequestDto userUpdate)
    {
        _logger.LogInformation("Updating user with ID: {UserId}", id);

        var existingUser = await _userRepository.GetUserById(id);


        if (existingUser == null)
        {
            _logger.LogWarning("User not found for update with ID: {UserId}", id);
            return ApiResponse<GetUserResponseDto>.NotFound("User not found");
        }

        // Email is not updated as it's used as an identifier
        existingUser.FirstName = userUpdate.FirstName;
        existingUser.MiddleName = userUpdate.MiddleName;
        existingUser.LastName = userUpdate.LastName;
        existingUser.ModifiedOn = DateTime.UtcNow;

        var isUserUpdated = await _userRepository.UpdateUSerAsync(existingUser);

        if (!isUserUpdated)
        {
            _logger.LogError("User update failed for ID: {UserId}", id);
            return ApiResponse<GetUserResponseDto>.InternalServerError();
        }

        // Map the updated entity to DTO
        var responseDto = MapToResponseDto(existingUser);

        // Invalidate cache for this user
        var cacheKey = $"user_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("User updated successfully with ID: {UserId}", id);
        return ApiResponse<GetUserResponseDto>.AcceptedResponse();
    }


    public async Task<ApiResponse<bool>> DeleteUserByIdAsync(string id)
    {
        _logger.LogInformation("Deleting user with ID: {UserId}", id);
        var existingUser = await _userRepository.GetUserById(id);

        if (existingUser == null)
        {
            _logger.LogWarning("User not found for deletion with ID: {UserId}", id);
            return ApiResponse<bool>.NotFound("User not found");
        }

        var isUserDeleted = await _userRepository.DeleteUserByIdAsync(existingUser);

        if (!isUserDeleted)
        {
            _logger.LogError("User deletion failed for ID: {UserId}", id);
            return ApiResponse<bool>.InternalServerError();
        }

        // Invalidate cache for this user
        var cacheKey = $"user_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("User deleted successfully with ID: {UserId}", id);
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