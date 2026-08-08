using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Model;
using Todo.Model.CategoryDto;
using Todo.Model.FilterDto;
using Todo.Services.Interfaces;

namespace Todo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GetCategoryResponseDto>>> CreateCategory([FromBody] CreateCategoryRequestDto categoryDto)
    {
        var response = await _categoryService.CreateCategoryAsync(categoryDto);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetCategoryResponseDto>>> GetCategoryById(string id)
    {
        var response = await _categoryService.GetCategoryByIdAsync(id);
        if (response == null)
            return NotFound(ApiResponse<GetCategoryResponseDto>.NotFound("Category not found"));
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetCategoryResponseDto>>>> GetCategories([FromQuery] CategoryFilterDto? categoryFilter = null)
    {
        var response = await _categoryService.GetCategoriesAsync(categoryFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetCategoryResponseDto>>> UpdateCategory(string id, [FromBody] UpdateCategoryRequestDto categoryUpdate)
    {
        var response = await _categoryService.UpdateCategoryAsync(id, categoryUpdate);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(string id)
    {
        var response = await _categoryService.DeleteCategoryByIdAsync(id);
        return StatusCode(Response.StatusCode, response);
    }
}
