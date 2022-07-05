#!/bin/bash

# author: martin@affolter.net

. __api_key.sh

version=$1
dotnet nuget delete "Example.Update" $version -s https://www.nuget.org -k $APIKEY --non-interactive
# dotnet nuget delete "ExampleHistory.Data" $version -s https://www.nuget.org -k $APIKEY --non-interactive
# dotnet nuget delete "ExampleHistory.Update" $version -s https://www.nuget.org -k $APIKEY --non-interactive
# dotnet nuget delete "ExampleUserDate.Data" $version -s https://www.nuget.org -k $APIKEY --non-interactive
# dotnet nuget delete "ExampleUserDate.Update" $version -s https://www.nuget.org -k $APIKEY --non-interactive
# dotnet nuget delete "ExampleVersion.Data" $version -s https://www.nuget.org -k $APIKEY --non-interactive
# dotnet nuget delete "ExampleVersion.Update" $version -s https://www.nuget.org -k $APIKEY --non-interactive
# dotnet nuget delete "ExampleVersionUserDate.Data" $version -s https://www.nuget.org -k $APIKEY --non-interactive
# dotnet nuget delete "ExampleVersionUserDate.Update" $version -s https://www.nuget.org -k $APIKEY --non-interactive
# dotnet nuget delete "ExampleVersionUserDateHistory.Data" $version -s https://www.nuget.org -k $APIKEY --non-interactive
# dotnet nuget delete "ExampleVersionUserDateHistory.Update" $version -s https://www.nuget.org -k $APIKEY --non-interactive