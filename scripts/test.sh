BUILD_DIR=$TRAVIS_BUILD_DIR

dotnet restore \
	&& dotnet test CyLRTests/