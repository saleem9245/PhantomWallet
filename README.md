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
- View wallet and smart contract balance (SOUL, KCAL, other token, unclaimed KCAL, staked SOUL, rewards SOULMASTER)
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

Dependency:

- .NET Core 2.2 (https://dotnet.microsoft.com/download/dotnet-core/2.2)

Dependencies (if using on Simnet):

- PhantasmaSpook
- PhantasmaChain
- PhantasmaExplorer
- PhantasmaRpcClient

### Launch from binaries

Phantom Wallet:

1) macOS

`chmod +x phantom-wallet`

`./phantom-wallet --path=PhantomWallet/www --port=7071`

2) Windows

3) Linux

Phantom Cli:

1) macOS

2) Windows

3) Linux

### Manual build

1) Phantom Wallet

`dotnet Phantom.Wallet.dll --path=/pathtowallet/PhantomWallet/www/ --port=7071`

2) Phantom Cli
