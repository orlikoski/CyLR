BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

[[ "$ZIP_COMMAND" == "" ]] && ZIP_COMMAND="zip -r -j"

CYLR_EXE="CyLR" && [[ "$BUILD_ARCH" == "win-x64" ]] && CYLR_EXE="CyLR.exe"

mkdir -p $BUILD_DIR/deployments

cp -r $BUILD_DIR/CyLR/bin/Release/netcoreapp3.1/$BUILD_ARCH/publish/ $BUILD_DIR/deployments/CyLR

echo "Zipping files:"
mkdir -p "$BUILD_DIR/archive"

if [ $BUILD_ARCH = "linux-x64" ] || [ $BUILD_ARCH = "osx-x64" ] ; then
	$ZIP_COMMAND $BUILD_DIR/archive/CyLR_$BUILD_ARCH.zip $BUILD_DIR/deployments/CyLR/CyLR
else
	$ZIP_COMMAND $BUILD_DIR/archive/CyLR_$BUILD_ARCH.zip $BUILD_DIR/deployments/CyLR/publish/*
fi
