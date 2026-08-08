using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GetCommentResponseDto>>> CreateComment([FromBody] CreateCommentRequestDto commentDto)
    {
        var response = await _commentService.CreateCommentAsync(commentDto);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetCommentResponseDto>>> GetCommentById(string id)
    {
        var response = await _commentService.GetCommentByIdAsync(id);
        if (response == null)
            return NotFound(ApiResponse<GetCommentResponseDto>.NotFound("Comment not found"));
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetCommentResponseDto>>>> GetComments([FromQuery] CommentFilterDto? commentFilter = null)
    {
        var response = await _commentService.GetCommentsAsync(commentFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetCommentResponseDto>>> UpdateComment(string id, [FromBody] UpdateCommentRequestDto commentUpdate)
    {
        var response = await _commentService.UpdateCommentAsync(id, commentUpdate);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteComment(string id)
    {
        var response = await _commentService.DeleteCommentByIdAsync(id);
        return StatusCode(Response.StatusCode, response);
    }
}
