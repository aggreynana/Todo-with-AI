using Todo.Entities;
using Todo.Model;
using Todo.Model.FilterDto;

namespace Todo.Storage.Repository.Interfaces;

public interface IUserRepository
{
    Task<bool> AddUserAsync(UserEntity user);
    Task<UserEntity?> GetUserById(string id);
    Task<UserEntity?> GetUserByEmailAsync(string email);
    Task<bool> UpdateUSerAsync(UserEntity user);
    Task<bool> DeleteUserByIdAsync(UserEntity user);

    // Pagination method using FilterDto approach
    // This method supports filtering, sorting, and pagination
    // Returns PageResultResponseDto with all pagination metadata
    Task<PageResultResponseDto<UserEntity>> GetUsersWithPaginationAsync(UserFilterDto userFilter);
}