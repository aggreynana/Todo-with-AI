using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Model;
using Todo.Model.FileAttachmentDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;

namespace Todo.Controllers;

[ApiController]
[Route("api/FileAttachments")]
[Authorize]
public class FileAttachmentController : ControllerBase
{
    private readonly IFileAttachmentService _fileAttachmentService;

    public FileAttachmentController(IFileAttachmentService fileAttachmentService)
    {
        _fileAttachmentService = fileAttachmentService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GetFileAttachmentResponseDto>>> CreateFileAttachment([FromBody] CreateFileAttachmentRequestDto fileAttachmentDto)
    {
        var response = await _fileAttachmentService.CreateFileAttachmentAsync(fileAttachmentDto);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetFileAttachmentResponseDto>>> GetFileAttachmentById(string id)
    {
        var response = await _fileAttachmentService.GetFileAttachmentByIdAsync(id);
        if (response == null)
            return NotFound(ApiResponse<GetFileAttachmentResponseDto>.NotFound("File attachment not found"));
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetFileAttachmentResponseDto>>>> GetFileAttachments([FromQuery] FileAttachmentFilterDto? fileAttachmentFilter = null)
    {
        var response = await _fileAttachmentService.GetFileAttachmentsAsync(fileAttachmentFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetFileAttachmentResponseDto>>> UpdateFileAttachment(string id, [FromBody] UpdateFileAttachmentRequestDto fileAttachmentUpdate)
    {
        var response = await _fileAttachmentService.UpdateFileAttachmentAsync(id, fileAttachmentUpdate);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteFileAttachment(string id)
    {
        var response = await _fileAttachmentService.DeleteFileAttachmentByIdAsync(id);
        return StatusCode(Response.StatusCode, response);
    }
}
