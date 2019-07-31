Linux:
dotnet publish -c Release -r linux-x64
./third-party/warp-packer --arch linux-x64 --input_dir PhantomCli/bin/Release/netcoreapp2.2/linux-x64/publish/ --exec PhantomCli --output bin/cli/linux/phantom-cli
cp PhantomCli/.config bin/cli/linux/

dotnet publish -c Release -r linux-x64
./third-party/warp-packer --arch linux-x64 --input_dir PhantomWallet/bin/Release/netcoreapp2.2/linux-x64/publish/ --exec PhantomWallet --output bin/wallet/linux/phantom-wallet


OSX:
dotnet publish -c Release -r osx-x64
./third-party/warp-packer --arch macos-x64 --input_dir PhantomCli/bin/Release/netcoreapp2.2/osx-x64/publish/ --exec PhantomCli --output bin/cli/osx/phantom-cli
cp PhantomCli/.config bin/cli/osx/

dotnet publish -c Release -r osx-x64
./third-party/warp-packer --arch macos-x64 --input_dir PhantomWallet/bin/Release/netcoreapp2.2/osx-x64/publish/ --exec PhantomWallet --output bin/wallet/osx/phantom-wallet


Windows:
dotnet publish -c Release -r win-x64
./third-party/warp-packer --arch windows-x64 --input_dir PhantomCli/bin/Release/netcoreapp2.2/win-x64/publish/ --exec PhantomCli.exe --output bin/cli/windows/phantom-cli.exe
cp PhantomCli/.config bin/cli/osx/

dotnet publish -c Release -r win-x64
./third-party/warp-packer --arch windows-x64 --input_dir PhantomWallet/bin/Release/netcoreapp2.2/win-x64/publish/ --exec PhantomWallet.exe --output bin/wallet/windows/phantom-wallet.exe

mkdir -p bin/cli/windows 
mkdir -p bin/wallet/windows 

mkdir -p bin/cli/osx
mkdir -p bin/wallet/osx

mkdir -p bin/cli/linux
mkdir -p bin/wallet/linux

