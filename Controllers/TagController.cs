using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GetTagResponseDto>>> CreateTag([FromBody] CreateTagRequestDto tagDto)
    {
        var response = await _tagService.CreateTagAsync(tagDto);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetTagResponseDto>>> GetTagById(string id)
    {
        var response = await _tagService.GetTagByIdAsync(id);
        if (response == null)
            return NotFound(ApiResponse<GetTagResponseDto>.NotFound("Tag not found"));
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetTagResponseDto>>>> GetTags([FromQuery] TagFilterDto? tagFilter = null)
    {
        var response = await _tagService.GetTagsAsync(tagFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetTagResponseDto>>> UpdateTag(string id, [FromBody] UpdateTagRequestDto tagUpdate)
    {
        var response = await _tagService.UpdateTagAsync(id, tagUpdate);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTag(string id)
    {
        var response = await _tagService.DeleteTagByIdAsync(id);
        return StatusCode(Response.StatusCode, response);
    }
}
