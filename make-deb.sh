#!/bin/bash

PACKAGE="iznakurnozbot"
#VERSION="0.1-1"

DIST="dist"
TARGET="$DIST/target"
DEBIAN="$TARGET/debian"
ETC="$TARGET/etc/$PACKAGE"
OPT="$TARGET/opt/$PACKAGE"
DEB="$DIST/"$PACKAGE"_"$VERSION"_amd64.deb"

rm -vfR $DIST
mkdir -p $TARGET
mkdir -p $ETC
mkdir -p $OPT
mkdir -p $DEBIAN

chmod -R 755 $TARGET

cp -R package/debian/** $DEBIAN
cp Resources/iznakurnozbot.conf $ETC
cp bin/Debug/netcoreapp3.1/linux-x64/publish/** $OPT
cp package/iznakurnozbot.service $DEBIAN

cd $TARGET
dpkg-buildpackage --build=any -us -uc
cd ../..

rm -vfR $TARGET

#dpkg --contents $DEB
#lintian $DEB
