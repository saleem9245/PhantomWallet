<p align="center">
  <img src="./PhantomWallet/www/public/img/phantasma_logo.png" width="125px;">
</p>

<h1 align="center">Phantom Wallet</h1>

<p align="center">
  An extended light wallet for the <b>Phantasma</b> blockchain.
</p>

<p align="center">
  <img src="./PhantomWallet/www/public/img/wallet2.png">
  <img src="./PhantomWallet/www/public/img/wallet1.png">
</p>

## Overview

### Current features

- Create a new wallet
- Login with an existing wallet (private key)
- View wallet balance (SOUL, KCAL, any token) and smart contract balance (unclaimed KCAL, staked SOUL, rewards SOULMASTER)
- View portfolio split in percentage and portfolio historical value over time
- Claim KCAL
- Stake/Unstake SOUL
- Claim SOULMASTER rewards
- Send/receive SOUL, KCAL and any token
- View wallet transactions history
- Switch between network type (Simnet/Testnet/Mainnet)
- Switch between Dark theme / Light theme
- Switch between multiple currencies (USD/CAD/EUR/etc.)
- Read/Write generic contract calls, for all chain/sidechain
- PhantomCli, which does everything PhantomWallet does, but from CLI

### Upcoming features

- Multisignature wallet conversion/support
- Login with a local account
- Import/Export account
- Swap to/from NEO and native SOUL

## Phantom Wallet Installation

The wallet can be launched either from the latest binaries, or can be built manually

### Required Tools and Dependencies

- .NET Core 2.2 (https://dotnet.microsoft.com/download/dotnet-core/2.2)
- PhantomWallet
- PhantasmaSpook (only required for Simnet)
- PhantasmaChain (only required for Simnet)
- PhantasmaExplorer (only required for Simnet)
- PhantasmaRpcClient (only required for Simnet)

### Launch from binaries

1) MAC
`chmod +x phantom-wallet`
`./phantom-wallet --path=PhantomWallet/www --port=7071`

2) Windows

3) Linux

### Manual build

1) PhantasmaSpook
`dotnet Spook.CLI/bin/Debug/netcoreapp2.0/Spook.dll -node.wif=L2LGgkZAdupN2ee8Rs6hpkc65zaGcLbxhbSDGq8oh6umUxxzeW25 -nexus.name=simnet -rpc.enabled=true -gui.enabled=false`

2) PhantasmaExplorer
`dotnet Phantasma.Explorer.dll --path=/pathtoexplorer/PhantasmaExplorer/PhantasmaExplorer/www --port=7072`

3) PhantomWallet
`dotnet Phantom.Wallet.dll --path=/pathtowallet/PhantomWallet/www/ --port=7071`

## Phantom Cli Installation

The wallet can be launched either from the latest binaries, or can be built manually
