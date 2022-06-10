#!/bin/bash

# author: martin@affolter.net

dbname="example"
projectname="Example"
default_sql_port="1433"
password="Som3V3ryS3cretP4ssw0rd!"
update_tool_path="../src/$projectname.Update"
update_tool_dll="$projectname.Update.dll"

echo
echo
echo "starting $dbname db in docker"
echo
echo

# write init sql
echo "create database $dbname" > init.sql

# set variables if not set "outside"
if [ -z "$restart" ]; then
    restart=${1:-"0"}
fi
if [ -z "$network" ]; then
    network=${2:-"$projectname-network"}
fi
if [ -z "$port" ]; then
    port=${3:-"1436"}
fi

dbcontainername="$dbname-db-$port" #commas make it lowercase

echo
echo "start $dbcontainername / network: $network / db-port: $port"
echo

localconnstring="Server=localhost,$port;Database=$dbname;User Id=sa;Password=${password};"
connstring="Server=$dbcontainername,$default_sql_port;Database=$dbname;User Id=sa;Password=${password};"
echo "localconnstring: $localconnstring"
echo "##vso[task.setvariable variable=localconnstring]$localconnstring"

dck="_docker.sh"
if test -f "$dck"; then
. $dck
else
    echo "file does not exist: $dck"
    exit 1
fi

create_network_if_not_exists

running="$(is_container_running $dbcontainername $restart)"
echo "db running: $running"
if [ "$running" == "0" ]; then
    # run db container
    echo "build"
    docker build -t $dbcontainername . --build-arg DATABASE_PASSWORD=$password
    echo "run"
    docker run -d --name $dbcontainername -p $port:$default_sql_port --network $network $dbcontainername

    # update db - from localhost
    pushd .
    cd "$update_tool_path"
    dotnet publish -c Release -o ./pub
    cd "pub"
    
    echo "dotnet $update_tool_dll \"dbup\" \"$localconnstring\" -h All"
    dotnet $update_tool_dll "dbup" "$localconnstring" "-h All"
    
    echo "dotnet $update_tool_dll \"gen\" \"$localconnstring\""
    dotnet $update_tool_dll "gen" "$localconnstring"
    popd
fi
