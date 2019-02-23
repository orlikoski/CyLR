$BUILD_ARCH="win-x64"

echo "dotnet publish -f netcoreapp2.1 -c release -r $BUILD_ARCH"
dotnet publish -f netcoreapp2.1 -c release -r $BUILD_ARCH
