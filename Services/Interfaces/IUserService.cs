using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Model.UserDto;

namespace Todo.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<AuthResponseDto>> CreateUserAsync(CreateUserRequestDto userDto);
    Task<ApiResponse<GetUserResponseDto>?> GetUserByIdAsync(string id);
    Task<ApiResponse<PageResultResponseDto<GetUserResponseDto>>> GetUsersAsync(UserFilterDto? userFilter = null);
    Task<ApiResponse<GetUserResponseDto>> UpdateUserAsync(string id, UpdateUserRequestDto userUpdate);
    Task<ApiResponse<bool>> DeleteUserByIdAsync(string id);
}