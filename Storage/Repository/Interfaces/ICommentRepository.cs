using Todo.Entities;
using Todo.Model;
using Todo.Model.FilterDto;

namespace Todo.Storage.Repository.Interfaces;

public interface ICommentRepository
{
    Task<bool> AddCommentAsync(CommentEntity comment);
    Task<CommentEntity?> GetCommentById(string id);
    Task<bool> UpdateCommentAsync(CommentEntity comment);
    Task<bool> DeleteCommentByIdAsync(CommentEntity comment);

    // Pagination method using FilterDto approach
    // This method supports filtering, sorting, and pagination
    // Returns PageResultResponseDto with all pagination metadata
    Task<PageResultResponseDto<CommentEntity>> GetCommentsWithPaginationAsync(CommentFilterDto commentFilter);
}
