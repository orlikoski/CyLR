BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

[[ "$ZIP_COMMAND" == "" ]] && ZIP_COMMAND="zip -rj"

[[ "$BUILD_ARCH" == "linux-x64" ]] && WARP_ARCH=linux-x64
[[ "$BUILD_ARCH" == "osx-x64" ]] && WARP_ARCH=macos-x64

mkdir $BUILD_DIR/deployments

if [ "$USE_WARP" = true ] ; then
	echo "Packaging build with Warp"
	curl -Lo $BUILD_DIR/warp-packer https://github.com/dgiagio/warp/releases/download/v0.2.1/$WARP_ARCH.warp-packer
	chmod +x $BUILD_DIR/warp-packer
	$BUILD_DIR/warp-packer --arch $WARP_ARCH --input_dir $BUILD_DIR/CyLR/bin/Release/netcoreapp2.1/$BUILD_ARCH/publish/ --exec CyLR --output $BUILD_DIR/deployments/CyLR
	echo "Warp complete."
else
	cp -r $BUILD_DIR/CyLR/bin/Release/netcoreapp2.1/$BUILD_ARCH/publish/ $BUILD_DIR/deployments/CyLR
fi
echo "Zipping files:"
ls -R $BUILD_DIR/deployments
mkdir $BUILD_DIR/archive
$ZIP_COMMAND $BUILD_DIR/archive/CyLR_$BUILD_ARCH.zip $BUILD_DIR/deployments/