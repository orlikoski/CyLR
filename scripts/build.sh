BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

dotnet add package Microsoft.DotNet.ILCompiler --version 1.0.5-prerelease-00002 \
       --source https://www.myget.org/F/dotnet/api/v3/index.json

if [ "$USE_CORERT" = true ] ; then
	dotnet publish -c release -r $BUILD_ARCH
else
	dotnet publish -f netcoreapp2.1 -c release -r $BUILD_ARCH
fi
