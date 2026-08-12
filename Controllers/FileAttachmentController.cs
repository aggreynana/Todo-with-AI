using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<FileAttachmentController> _logger;

    public FileAttachmentController(IFileAttachmentService fileAttachmentService, ILogger<FileAttachmentController> logger)
    {
        _fileAttachmentService = fileAttachmentService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GetFileAttachmentResponseDto>>> CreateFileAttachment([FromBody] CreateFileAttachmentRequestDto fileAttachmentDto)
    {
        _logger.LogInformation("Creating new file attachment: {FileName}", fileAttachmentDto.FileName);
        var response = await _fileAttachmentService.CreateFileAttachmentAsync(fileAttachmentDto);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("File attachment created successfully with ID: {FileAttachmentId}", response?.Data?.Id);
        }
        else
        {
            _logger.LogWarning("File attachment creation failed for file: {FileName}", fileAttachmentDto.FileName);
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetFileAttachmentResponseDto>>> GetFileAttachmentById(string id)
    {
        _logger.LogInformation("Fetching file attachment with ID: {FileAttachmentId}", id);
        var response = await _fileAttachmentService.GetFileAttachmentByIdAsync(id);
        if (response == null)
        {
            _logger.LogWarning("File attachment not found with ID: {FileAttachmentId}", id);
            return NotFound(ApiResponse<GetFileAttachmentResponseDto>.NotFound("File attachment not found"));
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetFileAttachmentResponseDto>>>> GetFileAttachments([FromQuery] FileAttachmentFilterDto? fileAttachmentFilter = null)
    {
        _logger.LogInformation("Fetching file attachments with filter: {@Filter}", fileAttachmentFilter);
        var response = await _fileAttachmentService.GetFileAttachmentsAsync(fileAttachmentFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetFileAttachmentResponseDto>>> UpdateFileAttachment(string id, [FromBody] UpdateFileAttachmentRequestDto fileAttachmentUpdate)
    {
        _logger.LogInformation("Updating file attachment with ID: {FileAttachmentId}", id);
        var response = await _fileAttachmentService.UpdateFileAttachmentAsync(id, fileAttachmentUpdate);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("File attachment updated successfully with ID: {FileAttachmentId}", id);
        }
        else
        {
            _logger.LogWarning("File attachment update failed for ID: {FileAttachmentId}", id);
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteFileAttachment(string id)
    {
        _logger.LogInformation("Deleting file attachment with ID: {FileAttachmentId}", id);
        var response = await _fileAttachmentService.DeleteFileAttachmentByIdAsync(id);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("File attachment deleted successfully with ID: {FileAttachmentId}", id);
        }
        else
        {
            _logger.LogWarning("File attachment deletion failed for ID: {FileAttachmentId}", id);
        }
        return StatusCode(Response.StatusCode, response);
    }
}
