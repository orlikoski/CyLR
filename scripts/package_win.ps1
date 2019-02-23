if (Test-Path env:TRAVIS_BUILD_DIR) {
	$BUILD_DIR = $env:TRAVIS_BUILD_DIR
}else {
	$BUILD_DIR = "."
}
$env:BUILD_ARCH="win-x64"
$WARP_ARCH = "windows-x64"

mkdir -p $BUILD_DIR/deployments/$env:BUILD_ARCH/

echo "Packaging build with Warp"
[Net.ServicePointManager]::SecurityProtocol = "tls12, tls11, tls" ; Invoke-WebRequest https://github.com/dgiagio/warp/releases/download/v0.3.0/windows-x64.warp-packer.exe -OutFile warp-packer.exe
.\warp-packer.exe --arch $WARP_ARCH --input_dir $env:BUILD_DIR/CyLR/bin/release/netcoreapp2.1/$env:BUILD_ARCH/publish --exec CyLR.exe --output $env:BUILD_DIR/deployments/$env:BUILD_ARCH/CyLR.exe

echo "Zipping files:"
mkdir -p "$BUILD_DIR/archive/$env:BUILD_ARCH"
if (-not (test-path "$env:ProgramFiles\7-Zip\7z.exe")) {throw "$env:ProgramFiles\7-Zip\7z.exe needed"}
set-alias sz "$env:ProgramFiles\7-Zip\7z.exe"

$Source = "$env:BUILD_DIR/deployments/$env:BUILD_ARCH/CyLR.exe"
$Target = "$BUILD_DIR/archive/$env:BUILD_ARCH/CyLR-$env:BUILD_ARCH.zip"

sz a -tzip -mx=9 $Target $Source
