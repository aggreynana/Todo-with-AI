namespace Todo.Model;

// STEP 1: Create generic PageResultResponseDto for paginated responses
// This DTO wraps paginated data with metadata about the pagination state
// The generic type T allows it to work with any entity type
public class PageResultResponseDto<T> where T : class
{
    // STEP 2: Add current page number
    // Indicates which page of results this response represents
    public int Page { get; set; } = 1;

    // STEP 3: Add page size
    // Indicates how many items are included in each page
    public int PageSize { get; set; } = 10;

    // STEP 4: Add the actual data records
    // Contains the list of items for the current page
    public List<T> Records { get; set; } = new List<T>();

    // STEP 5: Add total count of all records
    // Represents the total number of records across all pages
    // Used for calculating total pages and displaying "showing X of Y results"
    public int TotalCount { get; set; }

    // STEP 6: Add total pages
    // Calculated as ceiling(TotalCount / PageSize)
    // Used for pagination controls and determining if there are more pages
    public int TotalPages { get; set; }

    // STEP 7: Add computed property for HasPreviousPage
    // Indicates whether there's a previous page available
    public bool HasPreviousPage => Page > 1;

    // STEP 8: Add computed property for HasNextPage
    // Indicates whether there's a next page available
    public bool HasNextPage => Page < TotalPages;
}