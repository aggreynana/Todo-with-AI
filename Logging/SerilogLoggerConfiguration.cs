using Serilog;
using Serilog.Enrichers;
using Serilog.Events;

namespace Todo.Logging;


public static class SerilogLoggerConfiguration
{

    //The logging options containing configuration settings
    public static void ConfigureSerilog(LoggingOptions options)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", ParseLogLevel(options.LevelOverrides.Microsoft))
            .MinimumLevel.Override("Microsoft.AspNetCore", ParseLogLevel(options.LevelOverrides.MicrosoftAspNetCore))
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", ParseLogLevel(options.LevelOverrides.MicrosoftEntityFrameworkCore))
            .MinimumLevel.Override("System", ParseLogLevel(options.LevelOverrides.System))

            // Add enrichers to include additional context information in log messages
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProperty("Application", options.ApplicationName)

            // Configure console output
            .WriteTo.Console(
                outputTemplate: options.ConsoleOutputTemplate
            )

            // Configure file output with daily rolling and retention policy
            .WriteTo.File(
                path: options.LogFilePath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: options.OutputTemplate,
                retainedFileCountLimit: options.RetainedFileCountLimit
            )
            .CreateLogger();
    }

    private static LogEventLevel ParseLogLevel(string level)
    {
        return Enum.TryParse<LogEventLevel>(level, true, out var logLevel)
            ? logLevel
            : LogEventLevel.Information;
    }
}
