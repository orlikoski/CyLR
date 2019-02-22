BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

[[ "$ZIP_COMMAND" == "" ]] && ZIP_COMMAND="zip -r -j"

[[ "$BUILD_ARCH" == "linux-x64" ]] && WARP_ARCH=linux-x64
[[ "$BUILD_ARCH" == "osx-x64" ]] && WARP_ARCH=macos-x64
[[ "$BUILD_ARCH" == "win-x64" ]] && WARP_ARCH=windows-x64
[[ "$BUILD_ARCH" == "win-x86" ]] && WARP_ARCH=windows-x86

WARP_COMMAND=$WARP_ARCH.warp-packer
[[ "$BUILD_ARCH" == "win-x64" ]] && WARP_COMMAND=$WARP_ARCH.warp-packer.exe

CYLR_EXE="CyLR" && [[ "$BUILD_ARCH" == "win-x64" ]] && CYLR_EXE="CyLR.exe"

mkdir -p $BUILD_DIR/deployments

if [ "$USE_WARP" = true ] ; then
	echo "Packaging build with Warp" \
		&& curl -sSLo $BUILD_DIR/$WARP_COMMAND https://github.com/dgiagio/warp/releases/download/v0.3.0/$WARP_COMMAND \
		&& chmod +x $BUILD_DIR/$WARP_COMMAND \
		&& $BUILD_DIR/$WARP_COMMAND --arch $WARP_ARCH --input_dir $BUILD_DIR/CyLR/bin/Release/netcoreapp2.1/$BUILD_ARCH/publish/ --exec $CYLR_EXE --output $BUILD_DIR/deployments/$CYLR_EXE \
		&& echo "Warp complete."
else
	cp -r $BUILD_DIR/CyLR/bin/Release/netcoreapp2.1/$BUILD_ARCH/publish/ $BUILD_DIR/deployments/CyLR
fi
echo "Zipping files:"
ls -R $BUILD_DIR/deployments
mkdir -p "$BUILD_DIR/archive"

if [ $BUILD_ARCH = "linux-x64" ] || [ $BUILD_ARCH = "osx-x64" ] ; then
	$ZIP_COMMAND $BUILD_DIR/archive/CyLR_$BUILD_ARCH.zip $BUILD_DIR/deployments/CyLR/CyLR
elif [ $BUILD_ARCH = "win-x64" ] ; then
	$ZIP_COMMAND $BUILD_DIR/archive/CyLR_$BUILD_ARCH.zip --output $BUILD_DIR/deployments/$CYLR_EXE
else
	$ZIP_COMMAND $BUILD_DIR/archive/CyLR_$BUILD_ARCH.zip $BUILD_DIR/deployments/CyLR/*
fi
