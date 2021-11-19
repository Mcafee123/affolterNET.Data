#!/bin/bash

# author: martin@affolter.net

SA_PASSWORD=$(cat /tmp/_pw.sh)
echo "SA_PASSWORD: $SA_PASSWORD"
/usr/src/app/run-initialization.sh "$SA_PASSWORD" & /opt/mssql/bin/sqlservr