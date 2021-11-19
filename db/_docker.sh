#!/bin/bash

# author: martin@affolter.net

# print a log a message: https://stackoverflow.com/questions/3524978/logging-functions-in-bash-and-stdout
log ()
{
    echo "start_container: $1" >&2
}

stop_wait() {
    local ctname=$1
    echo "stopping $ctname..."
    docker container stop $ctname
    while [[ ! -z $(docker ps | grep $ctname) && $count -lt 60 ]]; do count=$[ count + 1] && echo "waiting... $count $(docker container ls -la | grep $ctname)" && sleep 2; done
    sleep 1
}

is_container_running() {
    # running?
    local ctname=$1
    local restart=${2:-"0"}
    local runs=$(docker ps | grep $ctname)
    if [ -z "$runs" ]; then
        # start
        log "$ctname not running - START"
        local container=$(docker container ls -la | grep $ctname)
        if [[ ! -z "$container" ]]; then 
            log "container exists - REMOVE"
            docker container rm $ctname
        fi
        echo "0"
    else
        if [ "$restart" == "0" ]; then
            log "$ctname runs - OK"   
            echo "1"
        else 
            # start
            log "$ctname already running - RESTART"
            log "stop"
            stop_wait $ctname
            log "remove"
            docker container rm $ctname
            echo "0"
        fi
    fi
}

create_network_if_not_exists() {
    if [ -z "$(docker network ls | grep $network)" ]; then
        echo "create network"
        docker network create "$network"
    else
        echo "network $network exists"
    fi
}