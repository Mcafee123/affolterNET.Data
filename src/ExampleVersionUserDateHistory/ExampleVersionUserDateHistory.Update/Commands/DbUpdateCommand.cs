using affolterNET.Data.DbUp.Services;
using Spectre.Console.Cli;

namespace ExampleVersion.Update.Commands
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DbUpdateCommand : AsyncCommand<UpdateService.Settings>
    {
        private readonly UpdateService _svc;

        public DbUpdateCommand()
        {
            _svc = new UpdateService();
        }

        public override async Task<int> ExecuteAsync(CommandContext context, UpdateService.Settings settings)
        {
            return await _svc.UpdateDb(context, settings);
        }
    }
}