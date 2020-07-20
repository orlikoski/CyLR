echo "dotnet publish -f netcoreapp3.1 -c release -r win-x86"
dotnet publish -c release -r win-x86

echo "dotnet publish -f netcoreapp3.1 -c release -r win-x64"
dotnet publish -c release -r win-x64
