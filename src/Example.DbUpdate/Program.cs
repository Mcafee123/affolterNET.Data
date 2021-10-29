// See https://aka.ms/new-console-template for more information

using System;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using affolterNET.Data.Extensions;
using Example.DbUpdate;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets("201daa29-6f2b-4aa9-bf7d-67e17429a2bf")
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();
            
var connectionString = configuration.GetValue<string>("CONNSTRING");
if (args?.AsEnumerable().FirstOrDefault(a => a.StartsWith("--show-connectionstring")) != null)
{
    Console.WriteLine($"ConnectionString: {connectionString}");
}
 
if (string.IsNullOrWhiteSpace(connectionString))
{
    var msg = @"ConnString empty. For local development, 
add 'ZADR_CONNSTRING' with the connection to the ZADR master-database to user secrets.
Server=10.211.55.5,1433;Database=ZADR;User Id=zadr;Password=xxxxx";
    throw new InvalidOperationException(msg);
}

var db = args?.AsEnumerable().FirstOrDefault(a => a.StartsWith("--Db"));
var builder = new DbConnectionStringBuilder { ConnectionString = connectionString };
if (db != null)
{
    var rx = new Regex("^--Db[\\=||\\:](.+)$");
    var ma = rx.Match(db);
    if (ma.Success && (ma.Groups.Count > 1))
    {
        builder["Database"] = ma.Groups[1].Value;
    }
}

Console.WriteLine($"ConnectionString: {connectionString}");

connectionString = builder.ConnectionString;
connectionString.WaitForDbConnection();

var updateResult = Database.UpdateDatabase(connectionString);
if (!updateResult.Successful)
{
    Exit.ExitError(new Exception("UpdateDatabase Error", updateResult.Error), args ?? new string[0]);
}

Console.WriteLine();
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Success!");
Console.ResetColor();
Exit.StopOnExit(args ?? new string[0]);
Environment.Exit(0);