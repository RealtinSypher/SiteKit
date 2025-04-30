# Build and install SiteKit on Linux/MacOS.

dotnet pack -c Release
dotnet tool install --global --add-source ./bin/nupkg SiteKit --version 1.0.0
