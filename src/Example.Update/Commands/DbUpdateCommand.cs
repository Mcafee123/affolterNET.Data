using System;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using System.Threading.Tasks;
using affolterNET.Data.Extensions;
using DbUp;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Example.Update.Commands
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DbUpdateCommand : AsyncCommand<DbUpdateCommand.Settings>
    {
        // ReSharper disable once ClassNeverInstantiated.Global
        public class Settings: CommandSettings
        {
            [CommandArgument(0, "<connstring>")]
            public string ConnString { get; set; } = null!;
        }

        public override Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            AnsiConsole.MarkupLine($"[blue]DbUpdateCommand[/]");
            var connectionString = settings.ConnString;
            var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            AnsiConsole.MarkupLine($"[orange3]ConnectionString: {connectionString}[/]");

            connectionString = builder.ConnectionString;
            connectionString.WaitForDbConnection();

            var upgrader = DeployChanges.To.SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole().Build();

            var updateResult = upgrader.PerformUpgrade();
            if (!updateResult.Successful)
            {
                throw new InvalidOperationException("db update failed", updateResult.Error);
            }

            return Task.FromResult(0);
        }
    }
}