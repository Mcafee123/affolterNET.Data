using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using affolterNET.Data.Extensions;
using affolterNET.Data.SessionHandler;
using DbUp;
using Spectre.Console;
using Spectre.Console.Cli;

namespace affolterNET.Data.DbUp.Services;

public class UpdateService
{
    // ReSharper disable once ClassNeverInstantiated.Global

    public class Settings: CommandSettings
    {
        [CommandArgument(0, "<connstring>")]
        public string ConnString { get; set; } = null!;

        [CommandOption("-h|--historyMode")]
        [DefaultValue(EnumHistoryMode.None)]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public EnumHistoryMode HistoryMode{ get; set;  } 
        
        [CommandOption("-t|--historyTableName")]
        [DefaultValue("T_History")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string? HistoryTableName { get; set;  } 
    }

    public async Task<int> UpdateDb(CommandContext context, Settings settings)
    {
        AnsiConsole.MarkupLine($"[blue]DbUpdateCommand[/]");
        var connectionString = settings.ConnString;
        var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

        AnsiConsole.MarkupLine($"[orange3]ConnectionString: {connectionString}[/]");

        connectionString = builder.ConnectionString;
        await connectionString.WaitForDbConnectionAsync();

        var ass = Assembly.GetEntryAssembly();
        var upg = DeployChanges.To.SqlDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(ass)
            .LogToConsole();
        if (!string.IsNullOrWhiteSpace(settings.HistoryTableName))
        {
            var saver = new HistorySaver(connectionString, settings.HistoryMode, settings.HistoryTableName);
            var writer = new HistoryLog(saver, connectionString);
            upg.LogTo(writer);
        }
        var upgrader = upg.Build();

        var updateResult = upgrader.PerformUpgrade();
        if (!updateResult.Successful)
        {
            throw new InvalidOperationException("db update failed", updateResult.Error);
        }

        return await Task.FromResult(0);
    }
}