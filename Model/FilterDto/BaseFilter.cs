using Todo.Enums;

namespace Todo.Model.FilterDto;

public class BaseFilter
{
    public SortDirection Sort { get; set; } = SortDirection.Asc;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}