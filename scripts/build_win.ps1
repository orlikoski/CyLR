if (Test-Path env:TRAVIS_BUILD_DIR) {
	$BUILD_DIR = $env:TRAVIS_BUILD_DIR
}else {
	$BUILD_DIR = "."
}
$env:BUILD_ARCH=win-x64

dotnet publish -f netcoreapp2.1 -c release -r $env:BUILD_ARCH
