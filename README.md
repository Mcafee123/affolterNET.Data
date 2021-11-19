# Introduction 
With this project one can access an SQL Database in a type safe manner using Commands and Queries. These Commands and Queries are hand-crafted using all the power of SQL. There are, however, built in helpers to work with a single table.

# Getting Started
1.	Create a Database (I usually do this by using SQL Scripts and [DbUp](https://dbup.readthedocs.io/)).
2.	Create a Console App and use affolterNET.Data.DtoHelper to create DTOs out of the Database.
3.	Create Commands and Queries to work with the Database.

# Build and Test
TODO: Describe and show how to build your code and run the tests. 

Thanks to Wolfgang for the [workaround](https://www.programmingwithwolfgang.com/azure-devops-publish-nuget/) when publishing NuGet packages from Azure Devops.