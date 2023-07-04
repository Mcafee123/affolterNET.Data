using System;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace affolterNET.Data.Extensions
{
    public static class WaitForDbExtension
    {
        public static void WaitForDbConnection(
            this string connstring,
            bool logFailures = true,
            TextWriter? outputWriter = null,
            int sleepTime = 500, 
            int retries = 100)
        {
            connstring.WaitForDbConnectionAsync(logFailures, outputWriter, sleepTime, retries).GetAwaiter().GetResult();
        }

        public static async Task WaitForDbConnectionAsync(
            this string connstring,
            bool logFailures = true,
            TextWriter? outputWriter = null,
            int sleepTime = 500, 
            int retries = 100)
        {
            if (outputWriter == null)
            {
                outputWriter = Console.Out;
            }

            int counter = 0;
            var builder = new SqlConnectionStringBuilder(connstring);
            using var connection = new SqlConnection(builder.ConnectionString);
            while (true)
            {
                try
                {
                    await connection.OpenAsync();
                    await outputWriter.WriteLineAsync($@"Db-Connection established: {builder.DataSource}/{builder.InitialCatalog}");
                    await connection.CloseAsync();
                    break;
                }
                catch
                {
                    counter++;
                    if (counter > retries)
                    {
                        throw;
                    }

                    if (logFailures)
                    {
                        await outputWriter.WriteLineAsync(
                            $@"Retry Db-Connection {builder.DataSource}/{builder.InitialCatalog} {counter}...");
                    }

                    Thread.Sleep(sleepTime);
                }
            }
        }
    }
}