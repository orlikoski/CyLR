#!/bin/bash

PLATFORM=$1

xbuild /p:Configuration=Release /p:Platform="Framework $PLATFORM" CyLR.sln
