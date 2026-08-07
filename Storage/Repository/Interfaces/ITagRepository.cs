using Todo.Entities;
using Todo.Model;
using Todo.Model.FilterDto;

namespace Todo.Storage.Repository.Interfaces;

public interface ITagRepository
{
    Task<bool> AddTagAsync(TagEntity tag);
    Task<TagEntity?> GetTagById(string id);
    Task<bool> UpdateTagAsync(TagEntity tag);
    Task<bool> DeleteTagByIdAsync(TagEntity tag);

    // Pagination method using FilterDto approach
    // This method supports filtering, sorting, and pagination
    // Returns PageResultResponseDto with all pagination metadata
    Task<PageResultResponseDto<TagEntity>> GetTagsWithPaginationAsync(TagFilterDto tagFilter);
}
