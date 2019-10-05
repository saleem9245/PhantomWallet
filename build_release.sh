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

rm -rf bin/PhantomWallet-linux
rm -rf bin/PhantomWallet-osx
rm -rf bin/PhantomWallet-win

# create directories
mkdir -p bin/PhantomWallet-linux
mkdir -p bin/PhantomWallet-win
mkdir -p bin/PhantomWallet-osx

mkdir -p bin/PhantomWallet-linux/www
mkdir -p bin/PhantomWallet-win/www
mkdir -p bin/PhantomWallet-osx/www

mkdir -p bin/release

# package wallet
third-party/warp-packer_$platform --arch linux-x64 --input_dir PhantomWallet/bin/Release/netcoreapp2.0/linux-x64/publish/ --exec PhantomWallet --output bin/PhantomWallet-linux/PhantomWallet
third-party/warp-packer_$platform --arch macos-x64 --input_dir PhantomWallet/bin/Release/netcoreapp2.0/osx-x64/publish/ --exec PhantomWallet --output bin/PhantomWallet-osx/PhantomWallet
third-party/warp-packer_$platform --arch windows-x64 --input_dir PhantomWallet/bin/Release/netcoreapp2.0/win-x64/publish/ --exec PhantomWallet.exe --output bin/PhantomWallet-win/PhantomWallet.exe

# copy html/css stuff
cp -r PhantomWallet/www/views  bin/PhantomWallet-linux/www
cp -r PhantomWallet/www/views  bin/PhantomWallet-win/www
cp -r PhantomWallet/www/views  bin/PhantomWallet-osx/www

cp -r PhantomWallet/www/public bin/PhantomWallet-linux/www
cp -r PhantomWallet/www/public bin/PhantomWallet-win/www
cp -r PhantomWallet/www/public bin/PhantomWallet-osx/www

cp third-party/start.command bin/PhantomWallet-osx/
cp third-party/start.sh bin/PhantomWallet-linux/
cp third-party/start.bat bin/PhantomWallet-win/

# package cli
#third-party/warp-packer_$platform --arch linux-x64 --input_dir PhantomCli/bin/Release/netcoreapp2.0/linux-x64/publish/ --exec PhantomCli --output bin/PhantomWallet-linux/PhantomCli
#third-party/warp-packer_$platform --arch macos-x64 --input_dir PhantomCli/bin/Release/netcoreapp2.0/osx-x64/publish/ --exec PhantomCli --output bin/PhantomWallet-osx/PhantomCli
#third-party/warp-packer_$platform --arch windows-x64 --input_dir PhantomCli/bin/Release/netcoreapp2.0/win-x64/publish/ --exec PhantomCli.exe --output bin/PhantomWallet-win/PhantomCli.exe

# zip
zip -r bin/release/PhantomWallet-0-2-9-linux.zip bin/PhantomWallet-linux
zip -r bin/release/PhantomWallet-0-2-9-windows.zip bin/PhantomWallet-win
zip -r bin/release/PhantomWallet-0-2-9-osx.zip bin/PhantomWallet-osx

