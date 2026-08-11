namespace Todo.Logging;

// These help reduce log noise from framework components

public class SerilogLevelOverrides
{
    public string Microsoft { get; set; } = "Warning";

    public string MicrosoftAspNetCore { get; set; } = "Warning";

    public string MicrosoftEntityFrameworkCore { get; set; } = "Warning";

    public string System { get; set; } = "Warning";
}
