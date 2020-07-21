if (Test-Path env:TRAVIS_BUILD_DIR) {
	$BUILD_DIR=$env:TRAVIS_BUILD_DIR
}else {
	$BUILD_DIR=Get-Location
}

Write-Output "Zipping files:"
mkdir -p "$BUILD_DIR/archive/" |Out-Null
if (-not (test-path "$env:ProgramFiles\7-Zip\7z.exe")) {throw "$env:ProgramFiles\7-Zip\7z.exe needed"}
set-alias sz "$env:ProgramFiles\7-Zip\7z.exe"

$Source = "$BUILD_DIR/CyLR/bin/release/netcoreapp3.1/win-x86/publish/CyLR.exe"
$Target = "$BUILD_DIR/archive/CyLR_win-x86.zip"

Write-Output "sz a -tzip -mx=9 $Target $Source"
sz a -tzip -mx=9 $Target $Source

$Source = "$BUILD_DIR/CyLR/bin/release/netcoreapp3.1/win-x64/publish/CyLR.exe"
$Target = "$BUILD_DIR/archive/CyLR_win-x64.zip"

Write-Output "sz a -tzip -mx=9 $Target $Source"
sz a -tzip -mx=9 $Target $Source
