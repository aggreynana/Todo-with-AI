using System.Linq;
using System;
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

    public TagService(ITagRepository tagRepository, ICacheService cacheService)
    {
        _tagRepository = tagRepository;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<GetTagResponseDto>> CreateTagAsync(CreateTagRequestDto tagDto)
    {
        var tagEntity = new TagEntity
        {
            UserId = tagDto.UserId,
            Name = tagDto.Name

        };

        var result = await _tagRepository.AddTagAsync(tagEntity);

        if (!result)
        {
            return ApiResponse<GetTagResponseDto>.InternalServerError();
        }

        var responseDto = MapToResponseDto(tagEntity);

        return ApiResponse<GetTagResponseDto>.CreatedResponse("Tag", responseDto);
    }


    public async Task<ApiResponse<GetTagResponseDto>?> GetTagByIdAsync(string id)
    {
        // Try to get from cache first
        var cacheKey = $"tag_{id}";
        var cachedTag = await _cacheService.GetAsync<GetTagResponseDto>(cacheKey);

        if (cachedTag != null)
        {
            return ApiResponse<GetTagResponseDto>.OkResponse("Tag retrieved from cache", cachedTag);
        }

        var tagEntity = await _tagRepository.GetTagById(id);

        if (tagEntity == null)
        {
            return ApiResponse<GetTagResponseDto>.InternalServerError();
        }

        var responseDto = MapToResponseDto(tagEntity);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, responseDto, TimeSpan.FromMinutes(5));

        return ApiResponse<GetTagResponseDto>.OkResponse("Tag retrieved successfully", responseDto);
    }

    // Implement GetTagsAsync method with pagination and filtering
    // This method retrieves tags from the database with pagination and filtering support
    public async Task<ApiResponse<PageResultResponseDto<GetTagResponseDto>>> GetTagsAsync(TagFilterDto? tagFilter = null)
    {
        // Use default filter if not provided
        var filter = tagFilter ?? new TagFilterDto();

        // Call the repository with filter
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

        // Return paginated response
        return ApiResponse<PageResultResponseDto<GetTagResponseDto>>.OkResponse("Tags retrieved successfully", pageResult);
    }

    // STEP 24: Implement UpdateTagAsync method
    // This method updates an existing tag
    public async Task<ApiResponse<GetTagResponseDto>> UpdateTagAsync(string id, UpdateTagRequestDto tagUpdate)
    {
        // STEP 25: First, retrieve the existing tag
        var existingTag = await _tagRepository.GetTagById(id);

        // STEP 26: Check if the tag exists
        if (existingTag == null)
        {
            // STEP 27: Return an error response if tag not found
            return ApiResponse<GetTagResponseDto>.NotFound("Tag not found");
        }

        // STEP 28: Update only the fields that are provided (partial update)
        if (tagUpdate.Name != null)
            existingTag.Name = tagUpdate.Name;

        // STEP 29: Set the ModifiedOn timestamp
        existingTag.ModifiedOn = DateTime.UtcNow;

        // STEP 30: Call the repository to update the entity
        var result = await _tagRepository.UpdateTagAsync(existingTag);

        // STEP 31: Check if the update was successful
        if (!result)
        {
            // STEP 32: Return an error response if update failed
            return ApiResponse<GetTagResponseDto>.InternalServerError();
        }

        // STEP 33: Map the updated entity to DTO
        var responseDto = MapToResponseDto(existingTag);

        // Invalidate cache for this tag
        var cacheKey = $"tag_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        // STEP 34: Return a success response with the updated tag
        return ApiResponse<GetTagResponseDto>.AcceptedResponse();
    }

    // STEP 35: Implement DeleteTagByIdAsync method
    // This method deletes a tag by its ID
    public async Task<ApiResponse<bool>> DeleteTagByIdAsync(string id)
    {
        // STEP 36: First, retrieve the existing tag
        var existingTag = await _tagRepository.GetTagById(id);

        // STEP 37: Check if the tag exists
        if (existingTag == null)
        {
            // STEP 38: Return an error response if tag not found
            return ApiResponse<bool>.NotFound("Tag not found");
        }

        // STEP 39: Call the repository to delete the entity
        var result = await _tagRepository.DeleteTagByIdAsync(existingTag);

        // STEP 40: Check if the deletion was successful
        if (!result)
        {
            // STEP 41: Return an error response if deletion failed
            return ApiResponse<bool>.InternalServerError();
        }

        // Invalidate cache for this tag
        var cacheKey = $"tag_{id}";
        await _cacheService.RemoveAsync(cacheKey);

        // STEP 42: Return a success response indicating successful deletion
        return ApiResponse<bool>.NoContent();
    }

    // STEP 43: Create a helper method to map Entity to DTO
    // This private method reduces code duplication and ensures consistent mapping
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
