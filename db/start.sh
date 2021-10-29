#!/bin/bash

# author: martin@affolter.net

echo
echo
echo "starting zadr db in docker"
echo
echo

projectname="affolterNET.Example"
default_sql_port="1433"
password="2xp0KRpi9c"
update_tool_path="../src/$projectname.DbUpdate"
update_tool_dll="$projectname.DbUpdate.dll"
connstring_param="CONNSTRING"

# set variables if not set "outside"
if [ -z "$restart" ]; then
    restart=${1:-"0"}
fi
if [ -z "$network" ]; then
    network=${2:-"$projectname-network"}
fi
if [ -z "$port" ]; then
    port=${3:-"1434"}
fi

dbcontainername="${projectname,,}-db-$port" #commas make it lowercase

echo
echo "start $dbcontainername / network: $network / db-port: $port"
echo

localconnstring="Server=localhost,$port;Database=zadr;User Id=sa;Password=${password};"
connstring="Server=$dbcontainername,$default_sql_port;Database=zadr;User Id=sa;Password=${password};"
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
    dotnet "./pub/$update_tool_dll" "$connstring_param=$localconnstring"
    popd
fi
