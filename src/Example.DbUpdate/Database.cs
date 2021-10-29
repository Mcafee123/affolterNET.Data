using System.Reflection;
using DbUp;
using DbUp.Engine;

namespace Example.DbUpdate
{
    public static class Database
    {
        public static DatabaseUpgradeResult UpdateDatabase(string connectionString)
        {
            var upgrader = DeployChanges.To.SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole().Build();

            var result = upgrader.PerformUpgrade();
            return result;
        }
    }
}