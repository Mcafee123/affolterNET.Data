using affolterNET.Data.Interfaces.SessionHandler;
using affolterNET.Data.SessionHandler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace affolterNET.Data.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAffolterNETDataServices(
            this IServiceCollection services,
            IConfiguration cfg,
            string connString,
            EnumHistoryMode historyMode = EnumHistoryMode.None,
            string? historyTableName = null)
        {
            services.AddScoped<ISqlSessionHandler, SqlSessionHandler>();
            services.AddTransient<ISqlSession, SqlSession>();
            services.AddSingleton<ISqlSessionFactory>(provider => new SqlSessionFactory(connString));
            services.AddTransient<IHistorySaver, HistorySaver>(sp => new HistorySaver(connString, historyMode, historyTableName));

            return services;
        }
    }
}