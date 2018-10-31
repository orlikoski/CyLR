BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

dotnet restore \
	&& dotnet publish -c release -r $BUILD_ARCH
