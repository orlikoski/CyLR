BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

unameOut="$(uname -s)"

case "${unameOut}" in
	Linux*)	
		BUILD_ARCH="linux-x64"
		PACKER_ARCH="linux-x64"
		;;
	Darwin*) 
		BUILD_ARCH="osx-x64"
		PACKER_ARCH="macos-x64"
		;;
esac

if [ ! -d $BUILD_DIR/deployments/CyLR/$BUILD_ARCH ]; then
	mkdir -p $BUILD_DIR/deployments/CyLR/$BUILD_ARCH
fi

if [ ! -f "./warp-packer" ] ; then

	curl -Lo warp-packer "https://github.com/dgiagio/warp/releases/download/v0.3.0/$PACKER_ARCH.warp-packer"
	chmod +x warp-packer
fi

# Pack into a single binary
echo "Packing $PACKER_ARCH"
./warp-packer --arch $PACKER_ARCH --input_dir $BUILD_DIR/CyLR/bin/Release/netcoreapp2.1/$BUILD_ARCH/publish --exec CyLR --output ./deployments/CyLR/$BUILD_ARCH/CyLR.$PACKER_ARCH
chmod +x $BUILD_DIR/deployments/CyLR/$BUILD_ARCH/CyLR.$PACKER_ARCH

[[ "$ZIP_COMMAND" == "" ]] && ZIP_COMMAND="zip -r -j"

echo "Zipping files:"
mkdir -p "$BUILD_DIR/archive"

$ZIP_COMMAND $BUILD_DIR/archive/CyLR_$BUILD_ARCH.zip $BUILD_DIR/deployments/CyLR/$BUILD_ARCH

