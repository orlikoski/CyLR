$BUILD_ARCH="win-x64"

Write-Output "dotnet publish -f netcoreapp2.1 -c release -r $BUILD_ARCH"
dotnet publish -f netcoreapp2.1 -c release -r $BUILD_ARCH
