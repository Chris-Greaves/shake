using Serilog;
using Shake.CLI.Commands;
using System.CommandLine;

var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var root = new RootCommand
{
    AudioCommand.GetCommand(logger.ForContext<AudioCommand>()),
    ScreenCommand.GetCommand(logger.ForContext<ScreenCommand>()),
    ConfigCommand.GetCommand(logger.ForContext<ConfigCommand>()),
};

return root.Invoke(args);