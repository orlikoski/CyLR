BUILD_DIR=$TRAVIS_BUILD_DIR

dotnet restore \
	&& dotnet build -c release -r $BUILD_ARCH \
	&& dotnet publish -c release -r $BUILD_ARCH
