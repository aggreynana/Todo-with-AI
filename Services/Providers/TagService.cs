using System.Linq;
using System;
using Todo.Entities;
using Todo.Model;
using Todo.Model.FilterDto;
using Todo.Model.TagDto;
using Todo.Services.Interfaces;
using Todo.Storage.Repository.Interfaces;

namespace Todo.Services.Providers;

// STEP 1: Implement the ITagService interface
// This class contains the business logic for Tag operations
// It acts as a bridge between the controller and the repository layer
public class TagService : ITagService
{
    // STEP 2: Inject the ITagRepository through constructor injection
    // This follows the dependency injection pattern for loose coupling
    private readonly ITagRepository _tagRepository;

    public TagService(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    // STEP 3: Implement CreateTagAsync method
    // This method handles the creation of a new tag
    public async Task<ApiResponse<GetTagResponseDto>> CreateTagAsync(CreateTagRequestDto tagDto)
    {
        // STEP 4: Map the DTO to the entity
        // Convert the incoming DTO to the domain entity for database operations
        var tagEntity = new TagEntity
        {
            UserId = tagDto.UserId,
            Name = tagDto.Name
            // Id and CreatedOn are set automatically in BaseEntity
        };

        // STEP 5: Call the repository to add the entity to the database
        var result = await _tagRepository.AddTagAsync(tagEntity);

        // STEP 6: Check if the operation was successful
        if (!result)
        {
            // STEP 7: Return an error response if creation failed
            return ApiResponse<GetTagResponseDto>.InternalServerError();
        }

        // STEP 8: Map the entity back to DTO for the response
        var responseDto = MapToResponseDto(tagEntity);

        // STEP 9: Return a success response with the created tag
        return ApiResponse<GetTagResponseDto>.CreatedResponse("Tag", responseDto);
    }

    // STEP 10: Implement GetTagByIdAsync method
    // This method retrieves a single tag by its unique identifier
    public async Task<ApiResponse<GetTagResponseDto>?> GetTagByIdAsync(string id)
    {
        // STEP 11: Call the repository to get the tag
        var tagEntity = await _tagRepository.GetTagById(id);

        // STEP 12: Check if the tag exists
        if (tagEntity == null)
        {
            // STEP 13: Return null if tag not found
            return null;
        }

        // STEP 14: Map the entity to DTO
        var responseDto = MapToResponseDto(tagEntity);

        // STEP 15: Return the tag in a success response
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
