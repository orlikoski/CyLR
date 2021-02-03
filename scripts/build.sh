#!/usr/bin/env bash
BUILD_DIR="." && [[ "$TRAVIS_BUILD_DIR" != "" ]] && BUILD_DIR=$TRAVIS_BUILD_DIR

unameOut="$(uname -s)"

case "${unameOut}" in
	Linux*)	
		BUILD_ARCH="linux-x64"
		;;
	Darwin*) 
		BUILD_ARCH="osx-x64"
		;;
esac

if [ "$USE_CORERT" = true ] ; then
	dotnet publish -c release -r $BUILD_ARCH
else
	dotnet publish -f netcoreapp3.1 -c release -r $BUILD_ARCH
fi
