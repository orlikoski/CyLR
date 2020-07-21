BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

dotnet restore \
	&& dotnet test CyLRTests/ /p:CollectCoverage=true /p:CoverletOutputFormat="lcov" /p:CoverletOutput=../lcov