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
- View wallet/smart contract balance (SOUL, KCAL, other, unclaimed KCAL, staked SOUL, rewards SOULMASTER)
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

- PhantasmaSpook (https://github.com/phantasma-io/PhantasmaSpook)
- PhantasmaChain (https://github.com/phantasma-io/PhantasmaChain)
- PhantasmaExplorer (https://github.com/phantasma-io/PhantasmaExplorer)
- PhantasmaRpcClient (https://github.com/phantasma-io/PhantasmaRpcClient)

### Launch from binaries

*Phantom Wallet:*

- macOS

1) Download here (https://)

2) Make the file executable

`chmod +x phantom-wallet`

3) Launch the program 

`./phantom-wallet --path=PhantomWallet/www`

4) Open a browser and go to http://localhost

- Windows

- Linux

*Phantom Cli:*

- macOS

- Windows

- Linux

### Manual build

*Phantom Wallet:*

`dotnet Phantom.Wallet.dll --path=/pathtowallet/PhantomWallet/www/ --port=7071`

*Phantom Cli:*
