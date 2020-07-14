if (Test-Path env:TRAVIS_BUILD_DIR) {
	$BUILD_DIR=$env:TRAVIS_BUILD_DIR
}else {
	$BUILD_DIR=Get-Location
}
$env:BUILD_ARCH="win-x64"
$WARP_ARCH = "windows-x64"

if (-not (Test-Path $BUILD_DIR/deployments/$env:BUILD_ARCH/)){
	mkdir -p $BUILD_DIR/deployments/$env:BUILD_ARCH/ |Out-Null
}

Write-Output "Packaging build with Warp"
if (-not (Test-Path .\warp-packer.exe)){
	[Net.ServicePointManager]::SecurityProtocol = "tls12, tls11, tls" ; Invoke-WebRequest https://github.com/dgiagio/warp/releases/download/v0.3.0/windows-x64.warp-packer.exe -OutFile warp-packer.exe
}
.\warp-packer.exe --arch $WARP_ARCH --input_dir $BUILD_DIR/CyLR/bin/release/netcoreapp2.1/$env:BUILD_ARCH/publish --exec CyLR.exe --output $BUILD_DIR/deployments/$env:BUILD_ARCH/CyLR.exe

Write-Output "Zipping files:"
mkdir -p "$BUILD_DIR/archive/" |Out-Null
if (-not (test-path "$env:ProgramFiles\7-Zip\7z.exe")) {throw "$env:ProgramFiles\7-Zip\7z.exe needed"}
set-alias sz "$env:ProgramFiles\7-Zip\7z.exe"

$Source = "$BUILD_DIR/deployments/$env:BUILD_ARCH/CyLR.exe"
$Target = "$BUILD_DIR/archive/CyLR_$env:BUILD_ARCH.zip"

Write-Output "sz a -tzip -mx=9 $Target $Source"
sz a -tzip -mx=9 $Target $Source
