using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Todo.Model;
using Todo.Model.CommentDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;

namespace Todo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentController> _logger;

    public CommentController(ICommentService commentService, ILogger<CommentController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GetCommentResponseDto>>> CreateComment([FromBody] CreateCommentRequestDto commentDto)
    {
        _logger.LogInformation("Creating new comment for activity: {ActivityId}", commentDto.ActivityId);
        var response = await _commentService.CreateCommentAsync(commentDto);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("Comment created successfully with ID: {CommentId}", response?.Data?.Id);
        }
        else
        {
            _logger.LogWarning("Comment creation failed for activity: {ActivityId}", commentDto.ActivityId);
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetCommentResponseDto>>> GetCommentById(string id)
    {
        _logger.LogInformation("Fetching comment with ID: {CommentId}", id);
        var response = await _commentService.GetCommentByIdAsync(id);
        if (response == null)
        {
            _logger.LogWarning("Comment not found with ID: {CommentId}", id);
            return NotFound(ApiResponse<GetCommentResponseDto>.NotFound("Comment not found"));
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetCommentResponseDto>>>> GetComments([FromQuery] CommentFilterDto? commentFilter = null)
    {
        _logger.LogInformation("Fetching comments with filter: {@Filter}", commentFilter);
        var response = await _commentService.GetCommentsAsync(commentFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetCommentResponseDto>>> UpdateComment(string id, [FromBody] UpdateCommentRequestDto commentUpdate)
    {
        _logger.LogInformation("Updating comment with ID: {CommentId}", id);
        var response = await _commentService.UpdateCommentAsync(id, commentUpdate);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("Comment updated successfully with ID: {CommentId}", id);
        }
        else
        {
            _logger.LogWarning("Comment update failed for ID: {CommentId}", id);
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteComment(string id)
    {
        _logger.LogInformation("Deleting comment with ID: {CommentId}", id);
        var response = await _commentService.DeleteCommentByIdAsync(id);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("Comment deleted successfully with ID: {CommentId}", id);
        }
        else
        {
            _logger.LogWarning("Comment deletion failed for ID: {CommentId}", id);
        }
        return StatusCode(Response.StatusCode, response);
    }
}
