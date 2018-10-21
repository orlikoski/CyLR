Set-PSDebug -Trace 1
cd ../

dotnet restore
dotnet test ./CyLRTest/
dotnet build -c release -r win-x86
dotnet publish -c release -r  win-x86
dotnet build -c release -r win-x64
dotnet publish -c release -r  win-x64


[Net.ServicePointManager]::SecurityProtocol = "tls12, tls11, tls" ; Invoke-WebRequest https://github.com/dgiagio/warp/releases/download/v0.2.1/windows-x64.warp-packer.exe -OutFile warp-packer.exe
.\warp-packer.exe --arch windows-x64 --input_dir CyLR/bin/release/netcoreapp2.1/win-x86/publish --exec CyLR.exe --output deployments/win-x86/CyLR.exe
.\warp-packer.exe --arch windows-x64 --input_dir CyLR/bin/release/netcoreapp2.1/win-x64/publish --exec CyLR.exe --output deployments/win-x64/CyLR.exe

del CyLR_windowsx86.zip
.\7za.exe a -y -tzip CyLR_windowsx86.zip deployments/win-x86/CyLR.exe
del CyLR_windowsx64.zip
.\7za.exe a -y -tzip CyLR_windowsx64.zip deployments/win-x64/CyLR.exe
