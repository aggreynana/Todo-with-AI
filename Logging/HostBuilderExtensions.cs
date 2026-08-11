using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Todo.Logging;

public static class HostBuilderExtensions
{
    public static IHostBuilder UseCustomSerilog(this IHostBuilder builder, IConfiguration configuration)
    {
        // Retrieve logging options from the configuration section, fallback to defaults if not found
        var loggingOptions = configuration.GetSection(LoggingOptions.SectionName).Get<LoggingOptions>()
            ?? new LoggingOptions();

        // Configure Serilog with the retrieved options
        SerilogLoggerConfiguration.ConfigureSerilog(loggingOptions);

        // Apply Serilog to the host builder
        return builder.UseSerilog();
    }
}
