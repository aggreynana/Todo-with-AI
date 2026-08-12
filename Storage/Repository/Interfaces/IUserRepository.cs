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

    Task<PageResultResponseDto<UserEntity>> GetUsersWithPaginationAsync(UserFilterDto userFilter);
}