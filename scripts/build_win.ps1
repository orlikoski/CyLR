$BUILD_ARCH="win-x64"

echo "dotnet publish -f netcoreapp3.1 -c release -r $BUILD_ARCH"
dotnet publish -f netcoreapp3.1 -c release -r $BUILD_ARCH
