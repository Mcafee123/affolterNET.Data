using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace affolterNET.Data.Extensions
{
    public static class WaitForDbExtension
    {
        public static async Task WaitForDbConnection(this string connstring, bool logFailures = true)
        {
            int counter = 0;
            var builder = new SqlConnectionStringBuilder(connstring);
            using var connection = new SqlConnection(builder.ConnectionString);
            while (true)
            {
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine($@"Db-Connection established: {builder.DataSource}/{builder.InitialCatalog}");
                    await connection.CloseAsync();
                    break;
                }
                catch
                {
                    counter++;
                    if (counter > 100)
                    {
                        throw;
                    }

                    if (logFailures)
                    {
                        Console.WriteLine(
                            $@"Retry Db-Connection {builder.DataSource}/{builder.InitialCatalog} {counter}...");
                    }

                    Thread.Sleep(500);
                }
            }
        }
    }
}