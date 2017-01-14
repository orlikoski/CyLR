#!/bin/bash

PLATFORM=$1

mkdir staging
cp CyLR/bin/$PLATFORM/Release/CyLR.exe staging/
cp CyLR/bin/$PLATFORM/Release/*.dll staging/
mono ./packages/ILRepack.2.0.12/tools/ILRepack.exe /wildcards /out:./CyLR.exe staging/CyLR.exe staging/*.dll
zip ./CyLR$PLATFORM.zip ./CyLR.exe
