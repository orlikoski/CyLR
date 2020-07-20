BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

if [ "$USE_CORERT" = true ] ; then
	dotnet publish -c release -r $BUILD_ARCH
else
	dotnet publish -f netcoreapp3.1 -c release -r $BUILD_ARCH
fi
