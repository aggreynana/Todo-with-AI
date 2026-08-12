using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
    {
        _categoryService = categoryService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GetCategoryResponseDto>>> CreateCategory([FromBody] CreateCategoryRequestDto categoryDto)
    {
        _logger.LogInformation("Creating new category with name: {Name}", categoryDto.Name);
        var response = await _categoryService.CreateCategoryAsync(categoryDto);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("Category created successfully with ID: {CategoryId}", response?.Data?.Id);
        }
        else
        {
            _logger.LogWarning("Category creation failed for name: {Name}", categoryDto.Name);
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetCategoryResponseDto>>> GetCategoryById(string id)
    {
        _logger.LogInformation("Fetching category with ID: {CategoryId}", id);
        var response = await _categoryService.GetCategoryByIdAsync(id);
        if (response == null)
        {
            _logger.LogWarning("Category not found with ID: {CategoryId}", id);
            return NotFound(ApiResponse<GetCategoryResponseDto>.NotFound("Category not found"));
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PageResultResponseDto<GetCategoryResponseDto>>>> GetCategories([FromQuery] CategoryFilterDto? categoryFilter = null)
    {
        _logger.LogInformation("Fetching categories with filter: {@Filter}", categoryFilter);
        var response = await _categoryService.GetCategoriesAsync(categoryFilter);
        return StatusCode(Response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<GetCategoryResponseDto>>> UpdateCategory(string id, [FromBody] UpdateCategoryRequestDto categoryUpdate)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId}, new name: {Name}", id, categoryUpdate.Name);
        var response = await _categoryService.UpdateCategoryAsync(id, categoryUpdate);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("Category updated successfully with ID: {CategoryId}", id);
        }
        else
        {
            _logger.LogWarning("Category update failed for ID: {CategoryId}", id);
        }
        return StatusCode(Response.StatusCode, response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(string id)
    {
        _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
        var response = await _categoryService.DeleteCategoryByIdAsync(id);
        if (response.StatusCode >= 200 && response.StatusCode < 300)
        {
            _logger.LogInformation("Category deleted successfully with ID: {CategoryId}", id);
        }
        else
        {
            _logger.LogWarning("Category deletion failed for ID: {CategoryId}", id);
        }
        return StatusCode(Response.StatusCode, response);
    }
}
