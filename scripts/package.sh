#!/usr/bin/env bash
BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

ZIP_COMMAND="zip -r -j"

mkdir -p $BUILD_DIR/deployments

cp -r $BUILD_DIR/CyLR/bin/Release/netcoreapp3.1/$BUILD_ARCH/publish/ $BUILD_DIR/deployments/CyLR

echo "Zipping files:"
mkdir -p "$BUILD_DIR/archive"

$ZIP_COMMAND $BUILD_DIR/archive/CyLR_$BUILD_ARCH.zip $BUILD_DIR/deployments/CyLR/CyLR
