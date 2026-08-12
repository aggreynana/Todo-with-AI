using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Model.TagDto;
using Todo.Services.Interfaces;

namespace Todo.Controllers;

[ApiController]
[Route("api/tags")]
[Authorize]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly ILogger<TagController> _logger;

    public TagController(ITagService tagService, ILogger<TagController> logger)
    {
        _tagService = tagService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GetTagResponseDto>>> CreateTag([FromBody] CreateTagRequestDto tagDto)
    {
        _logger.LogInformation("Creating new tag with name: {Name}", tagDto.Name);
        var response = await _tagService.CreateTagAsync(tagDto);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("Tag created successfully with ID: {TagId}", response?.Data?.Id);
        }
        else
        {
            _logger.LogWarning("Tag creation failed for name: {Name}", tagDto.Name);
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetTagResponseDto>>> GetTagById(string id)
    {
        _logger.LogInformation("Fetching tag with ID: {TagId}", id);
        var response = await _tagService.GetTagByIdAsync(id);
        if (response == null)
        {
            _logger.LogWarning("Tag not found with ID: {TagId}", id);
            return NotFound(ApiResponse<GetTagResponseDto>.NotFound("Tag not found"));
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetTagResponseDto>>>> GetTags([FromQuery] TagFilterDto? tagFilter = null)
    {
        _logger.LogInformation("Fetching tags with filter: {@Filter}", tagFilter);
        var response = await _tagService.GetTagsAsync(tagFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetTagResponseDto>>> UpdateTag(string id, [FromBody] UpdateTagRequestDto tagUpdate)
    {
        _logger.LogInformation("Updating tag with ID: {TagId}, new name: {Name}", id, tagUpdate.Name);
        var response = await _tagService.UpdateTagAsync(id, tagUpdate);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("Tag updated successfully with ID: {TagId}", id);
        }
        else
        {
            _logger.LogWarning("Tag update failed for ID: {TagId}", id);
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTag(string id)
    {
        _logger.LogInformation("Deleting tag with ID: {TagId}", id);
        var response = await _tagService.DeleteTagByIdAsync(id);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("Tag deleted successfully with ID: {TagId}", id);
        }
        else
        {
            _logger.LogWarning("Tag deletion failed for ID: {TagId}", id);
        }
        return StatusCode(Response.StatusCode, response);
    }
}
