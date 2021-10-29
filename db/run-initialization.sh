#!/bin/bash

# author: martin@affolter.net

pw="$1"

count=0
while [[ $(/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$pw" -Q "SET NOCOUNT ON;SELECT count([name]) as n FROM master.dbo.sysdatabases where [name] = 'master'" -h -1 -W) != 1 && $count -lt 60 ]]; do count=$[ count + 1] && echo "waiting... $count $(/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$pw" -Q "SET NOCOUNT ON;SELECT count([name]) as n FROM master.dbo.sysdatabases where [name] = 'master'" -h -1 -W)" && sleep 5; done
sleep 2

echo
echo
echo "wait duration: $count"
echo
echo

# Run the setup script to create the DB and the schema in the DB
# Note: make sure that your password matches what is in the Dockerfile
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$pw" -d master -i init.sql
