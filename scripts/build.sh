BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

dotnet restore \
	&& dotnet publish -f netcoreapp2.1 -c release -r $BUILD_ARCH
