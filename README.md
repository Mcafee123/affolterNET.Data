# Docs

## Introduction

With this project one can access an SQL Database in a type safe manner using Commands and Queries with [Dapper](https://github.com/DapperLib/Dapper). These Commands and Queries are hand-crafted using all the power of SQL. There are, however, built in helpers to work with a single table.

## Getting Started

1. Create a Database (I usually do this with a Console Application by using SQL Scripts and [DbUp](https://dbup.readthedocs.io/)). Please have a look at the [Example Projects](https://github.com/Mcafee123/affolterNET.Data/tree/main/src). Use "affolterNET.Data.DbUp.Services.UpdateService" to create the db out of SQL scripts. Add a HistoryMode (all scripts or write-operations only) if needed.
2. Use the same Console App to create DTOs out of the Database ([Example Projects](https://github.com/Mcafee123/affolterNET.Data/tree/main/src)). I usually generate this Dto-File in its own Assembly, where I can also put my Commands and Queries later.
3. Create Commands and Queries to work with the Database by inheriting from "CommandQueryBase". Every Command and every Query is a class, containing the sql and the necessary parameters. All generated DTOs contain SQL strings to access "their" table for convenience - these SQL strings can be used by your custom commands and queries. Please use a folder named "Commands" to save your write-operations and a folder named "Queries" for read-access to the database (separation - this will help distinguishing between read- and write-operations, it is relevant when using history).
4. Use ISqlSessionHandler to query the database (simple):

            var cmd = new YourCustomCommandOrQuery(string parameter1);
            var result = await sqlSessionHandler.QueryAsync(cmd);
            return result;

The transaction will be rolled back automatically. Suitable for executing only one command/query.

or with transaction-support:

        var session = sqlSessionHandler.CreateSqlSession();
        session.Begin();
        try
        {
            var cmd = new YourCustomCommandOrQuery(string parameter1);
            var result = await sqlSessionHandler.QueryAsync(cmd);
            session.Commit();
            return result;
        }
        catch
        {
            session.Rollback();
            throw;
        }

You have to roll back your transaction yourself if the session is created explicitly like above. Suited for running multiple commands/queries in one transaction.

## Different Modes

The Examples show different Modes to be enabled:

* simply accessing a table
* adding some metadata (inserted user, changed user, inserted date changed date)
* rowversion to prevent overwriting changes when editing the same record by more than one user at the same time
* history mode to save sql commands and queries in a table of the same or another database.

### Metadata

Every row can have the four metadata-columns for a user and a date of insertion and a user and a date of change (update). Actiate this by:

        GeneratorCfg.WithInsertDate(insDate => insDate == "InsertDate");
        GeneratorCfg.WithInsertUser(insUser => insUser == "InsertUser");
        GeneratorCfg.WithUpdateDate(updDate => updDate == "UpdateDate");
        GeneratorCfg.WithUpdateUser(updUser => updUser == "UpdateUser");

The Argument is the column name.

### Rowversion

_Is a data type that exposes automatically generated, unique binary numbers within a database. rowversion is generally used as a mechanism for version-stamping table rows._ See [Microsoft Docs](https://docs.microsoft.com/en-us/sql/t-sql/data-types/rowversion-transact-sql?view=sql-server-ver16).

This mechanism is supported by checking the version in SQL strings of the DTOs. It is added with:

    GeneratorCfg.WithVersion(version => version == "VersionTimestamp")
The Argument is the column name.

### History Mode

With history Mode you can create kind of event sourcing-solutions, meaning that you can replay a series of sql commands to restore a previous state of your database later in time. Make sure to also add History Mode (with the same database-table) when using the DbUp-Console App. You will get a single table with all database structure changes and user operations in one place.

|Mode|Meaning|
|-|-|
| EnumHistoryMode.None                  | No SQL scripts will be recorded (Default)|
| EnumHistoryMode.All                   | All SQL scripts will be recorded         |
| EnumHistoryMode.CommandsOnly          | All write-operations will be recorded. Write-operations are automatically assumed if a query is in the commands-namespace/subfolder. If it's in "Queries" (or somewhere else), it is a read-operation for the HistorySaver. This automatism can be overruled by explicitly excluding/including a command or query from history (constructor-parameter of CommandQueryBase "bool excludeFromHistory = true/false")  |
| EnumHistoryMode.CommandsOnlyAndCheck  | Same as "CommandsOnly", but writes to the console if a script not in "Commands" contains "insert", "update" or "delete".    |

## Build and Test



Thanks to Wolfgang for the [workaround](https://www.programmingwithwolfgang.com/azure-devops-publish-nuget/) when publishing NuGet packages from Azure Devops.
