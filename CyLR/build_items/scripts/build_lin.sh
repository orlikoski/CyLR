#!/usr/bin/env bash
set -ex
#build_dir="../.."
build_dir="./"
test_dir="../CyLRTests"
netcore_version="2.1"
deployment_root_dir="./deployments/"

# Move to Build Directory
cd $build_dir

# # Download Warp
# warp_version="0.2.1"
# curl -Lo warp-packer https://github.com/dgiagio/warp/releases/download/v$warp_version/linux-x64.warp-packer
# chmod +x warp-packer

# Start Build Process
dotnet restore
platform="linux-x64"

# Build and Publish Linux and MacOs Versions
deployments_dir="$deployment_root_dir/$platform"
mkdir -p $deployments_dir
dotnet test $test_dir
dotnet build -c release -r $platform
dotnet publish -c release -r $platform
./warp-packer --arch $platform --input_dir bin/release/netcoreapp$netcore_version/linux-x64/publish --exec CyLR --output $deployments_dir/CyLR
zip -j ./CyLR_linux.zip $deployments_dir/CyLR
