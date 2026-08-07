using Todo.Entities;
using Todo.Model;
using Todo.Model.FilterDto;

namespace Todo.Storage.Repository.Interfaces;

public interface IActivityRepository
{
    Task<bool> AddActivityAsync(ActivityEntity activity);
    Task<ActivityEntity?> GetActivityById(string id);
    Task<bool> UpdateActivityAsync(ActivityEntity activity);
    Task<bool> DeleteActivityByIdAsync(ActivityEntity activity);

    // Pagination method using FilterDto approach
    // This method supports filtering, sorting, and pagination
    // Returns PageResultResponseDto with all pagination metadata
    Task<PageResultResponseDto<ActivityEntity>> GetActivitiesWithPaginationAsync(ActivityFilterDto activityFilter);
}
