BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

[[ "$BUILD_ARCH" == "linux-x64" ]] && WARP_ARCH=linux-x64
[[ "$BUILD_ARCH" == "osx-x64" ]] && WARP_ARCH=macos-x64

mkdir $BUILD_DIR/deployments

curl -Lo $BUILD_DIR/warp-packer https://github.com/dgiagio/warp/releases/download/v0.2.1/$WARP_ARCH.warp-packer \
	&& chmod +x $BUILD_DIR/warp-packer \
	&& $BUILD_DIR/warp-packer --arch $WARP_ARCH --input_dir $BUILD_DIR/CyLR/bin/Release/netcoreapp2.1/$BUILD_ARCH/publish/ --exec CyLR --output $BUILD_DIR/deployments/CyLR

zip -j $BUILD_DIR/CyLR_$BUILD_ARCH.zip $BUILD_DIR/deployments/CyLR