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

    Task<PageResultResponseDto<CommentEntity>> GetCommentsWithPaginationAsync(CommentFilterDto commentFilter);
}
