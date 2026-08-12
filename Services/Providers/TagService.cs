using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using Todo.Entities;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Model.TagDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<TagService> _logger;

    public TagService(ITagRepository tagRepository, ICacheService cacheService, ILogger<TagService> logger)
    {
        _tagRepository = tagRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<ApiResponse<GetTagResponseDto>> CreateTagAsync(CreateTagRequestDto tagDto)
    {
        _logger.LogInformation("Creating tag for user: {UserId}, name: {Name}", tagDto.UserId, tagDto.Name);
        var tagEntity = new TagEntity
        {
            UserId = tagDto.UserId,
            Name = tagDto.Name

        };

        var result = await _tagRepository.AddTagAsync(tagEntity);

        if (!result)
        {
            _logger.LogError("Tag creation failed for user: {UserId}", tagDto.UserId);
            return ApiResponse<GetTagResponseDto>.InternalServerError();
        }

        var responseDto = MapToResponseDto(tagEntity);
        _logger.LogInformation("Tag created successfully with ID: {TagId}", tagEntity.Id);

        return ApiResponse<GetTagResponseDto>.CreatedResponse("Tag", responseDto);
    }


    public async Task<ApiResponse<GetTagResponseDto>?> GetTagByIdAsync(string id)
    {
        _logger.LogInformation("Fetching tag with ID: {TagId}", id);
        // Try to get from cache first
        var cacheKey = $"tag_{id}";
        var cachedTag = await _cacheService.GetAsync<GetTagResponseDto>(cacheKey);

        if (cachedTag != null)
        {
            _logger.LogInformation("Tag retrieved from cache with ID: {TagId}", id);
            return ApiResponse<GetTagResponseDto>.OkResponse("Tag retrieved from cache", cachedTag);
        }

        var tagEntity = await _tagRepository.GetTagById(id);

        if (tagEntity == null)
        {
            _logger.LogWarning("Tag not found with ID: {TagId}", id);
            return ApiResponse<GetTagResponseDto>.InternalServerError();
        }

        var responseDto = MapToResponseDto(tagEntity);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, responseDto, TimeSpan.FromMinutes(5));

        _logger.LogInformation("Tag retrieved successfully with ID: {TagId}", id);
        return ApiResponse<GetTagResponseDto>.OkResponse("Tag retrieved successfully", responseDto);
    }


    public async Task<ApiResponse<PageResultResponseDto<GetTagResponseDto>>> GetTagsAsync(TagFilterDto? tagFilter = null)
    {
        _logger.LogInformation("Fetching tags with filter: {@Filter}", tagFilter);
        var filter = tagFilter ?? new TagFilterDto();

        var tagPageResult = await _tagRepository.GetTagsWithPaginationAsync(filter);

        // Map entities to DTOs
        var tagDtos = tagPageResult.Records.Select(MapToResponseDto).ToList();

        // Create paginated response with metadata
        var pageResult = new PageResultResponseDto<GetTagResponseDto>
        {
            Page = tagPageResult.Page,
            PageSize = tagPageResult.PageSize,
            Records = tagDtos,
            TotalCount = tagPageResult.TotalCount,
            TotalPages = tagPageResult.TotalPages
        };

        _logger.LogInformation("Retrieved {Count} tags", tagDtos.Count);
        return ApiResponse<PageResultResponseDto<GetTagResponseDto>>.OkResponse("Tags retrieved successfully", pageResult);
    }

    public async Task<ApiResponse<GetTagResponseDto>> UpdateTagAsync(string id, UpdateTagRequestDto tagUpdate)
    {
        _logger.LogInformation("Updating tag with ID: {TagId}", id);

        var existingTag = await _tagRepository.GetTagById(id);

        if (existingTag == null)
        {
            _logger.LogWarning("Tag not found for update with ID: {TagId}", id);
            return ApiResponse<GetTagResponseDto>.NotFound("Tag not found");
        }

        // Update only the fields that are provided (partial update)
        if (tagUpdate.Name != null)
            existingTag.Name = tagUpdate.Name;

        existingTag.ModifiedOn = DateTime.UtcNow;

        var result = await _tagRepository.UpdateTagAsync(existingTag);

        if (!result)
        {
            _logger.LogError("Tag update failed for ID: {TagId}", id);
            return ApiResponse<GetTagResponseDto>.InternalServerError();
        }

        // STEP 33: Map the updated entity to DTO
        var responseDto = MapToResponseDto(existingTag);

        // Invalidate cache for this tag
        var cacheKey = $"tag_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("Tag updated successfully with ID: {TagId}", id);
        return ApiResponse<GetTagResponseDto>.AcceptedResponse();
    }


    public async Task<ApiResponse<bool>> DeleteTagByIdAsync(string id)
    {
        _logger.LogInformation("Deleting tag with ID: {TagId}", id);
        var existingTag = await _tagRepository.GetTagById(id);

        if (existingTag == null)
        {
            _logger.LogWarning("Tag not found for deletion with ID: {TagId}", id);
            return ApiResponse<bool>.NotFound("Tag not found");
        }

        var result = await _tagRepository.DeleteTagByIdAsync(existingTag);

        if (!result)
        {
            _logger.LogError("Tag deletion failed for ID: {TagId}", id);
            return ApiResponse<bool>.InternalServerError();
        }

        // Invalidate cache for this tag
        var cacheKey = $"tag_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        _logger.LogInformation("Tag deleted successfully with ID: {TagId}", id);
        return ApiResponse<bool>.NoContent();
    }


    private static GetTagResponseDto MapToResponseDto(TagEntity entity)
    {
        return new GetTagResponseDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            CreatedOn = entity.CreatedOn,
            ModifiedOn = entity.ModifiedOn
        };
    }
}
