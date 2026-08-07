namespace Todo.Model;

public class ApiResponse<T> 
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public ApiResponse(string message,int code, T? data)
    {
        StatusCode = code;
        Message = message;
        Data = data;
    }

    public static ApiResponse<T> OkResponse(string message, T data) => new(message, 200, data);
    public static ApiResponse<T> CreatedResponse(string type, T data) => new($"{type} is created", 201, data);

    public static ApiResponse<T> BadRequest() => new("Bad request", 400, default);

    public static ApiResponse<T> Unauthorized() => new("Unauthorized", 401, default);

    public static ApiResponse<T> NotFound(string message) => new(message, 404, default);

    public static ApiResponse<T> NoContent() => new("No Content", 204, default);

    public static ApiResponse<T> Conflict() => new("Conflict", 409, default);

    public static ApiResponse<T> FailedDependency() => new("Failed Dependency", 424, default);

    public static ApiResponse<T> InternalServerError() => new("Something went wrong. Kindly wait for a while and try again.", 500, default);

    public static ApiResponse<T> AcceptedResponse() => new("Updated successful", 202, default);
}