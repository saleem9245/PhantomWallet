#/bin/bash

platform=$1

if [ -z $platform ]; then
    echo "Please add platform parameter [osx|linux]";
    exit 1
fi

if [ $platform  = "win" ]; then
    echo Build release on linux or osx, no windows support for now
    exit 1
fi

rm -rf bin/linux
rm -rf bin/osx
rm -rf bin/win

# create directories
mkdir -p bin/linux
mkdir -p bin/win
mkdir -p bin/osx

mkdir -p bin/linux/www
mkdir -p bin/win/www
mkdir -p bin/osx/www

mkdir -p bin/release

# package wallet
third-party/warp-packer_$platform --arch linux-x64 --input_dir PhantomWallet/bin/Release/netcoreapp2.0/linux-x64/publish/ --exec PhantomWallet --output bin/linux/PhantomWallet
third-party/warp-packer_$platform --arch macos-x64 --input_dir PhantomWallet/bin/Release/netcoreapp2.0/osx-x64/publish/ --exec PhantomWallet --output bin/osx/PhantomWallet
third-party/warp-packer_$platform --arch windows-x64 --input_dir PhantomWallet/bin/Release/netcoreapp2.0/win-x64/publish/ --exec PhantomWallet.exe --output bin/win/PhantomWallet.exe

# copy html/css stuff
cp -r PhantomWallet/www/views  bin/linux/www
cp -r PhantomWallet/www/views  bin/win/www
cp -r PhantomWallet/www/views  bin/osx/www

cp -r PhantomWallet/www/public bin/linux/www
cp -r PhantomWallet/www/public bin/win/www
cp -r PhantomWallet/www/public bin/osx/www

cp third-party/start.command bin/osx/
cp third-party/start.sh bin/linux/
cp third-party/start.bat bin/win/

# package cli
third-party/warp-packer_$platform --arch linux-x64 --input_dir PhantomCli/bin/Release/netcoreapp2.0/linux-x64/publish/ --exec PhantomCli --output bin/linux/PhantomCli
third-party/warp-packer_$platform --arch macos-x64 --input_dir PhantomCli/bin/Release/netcoreapp2.0/osx-x64/publish/ --exec PhantomCli --output bin/osx/PhantomCli
third-party/warp-packer_$platform --arch windows-x64 --input_dir PhantomCli/bin/Release/netcoreapp2.0/win-x64/publish/ --exec PhantomCli.exe --output bin/win/PhantomCli.exe

# zip
zip -r bin/release/linux.zip bin/linux
zip -r bin/release/windows.zip bin/win
zip -r bin/release/osx.zip bin/osx

