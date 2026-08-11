namespace Todo.Logging;

public class LoggingOptions
{
    public const string SectionName = "Logging";

    public string ApplicationName { get; set; } = "TodoApi";

    public string LogFilePath { get; set; } = "logs/todo-.log";

    public int RetainedFileCountLimit { get; set; } = 7;

    public string OutputTemplate { get; set; } = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    public string ConsoleOutputTemplate { get; set; } = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";

    public SerilogLevelOverrides LevelOverrides { get; set; } = new();
}
