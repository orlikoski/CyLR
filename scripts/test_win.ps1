if (Test-Path env:TRAVIS_BUILD_DIR) {
	$BUILD_DIR = $env:TRAVIS_BUILD_DIR
}else {
	$BUILD_DIR = "."
}

dotnet restore
dotnet test CyLRTests
