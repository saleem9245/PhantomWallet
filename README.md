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
- Login with an existing wallet (via private key)
- View wallet/smart contract balance (SOUL, KCAL, token, unclaimed KCAL, staked SOUL)
- View portfolio split in percentage and portfolio historical value over time
- Claim KCAL
- Stake/Unstake SOUL
- Claim SOULMASTER rewards
- Send/receive SOUL, KCAL and any token
- View wallet transactions history
- Switch between network type (Simnet/Testnet/Mainnet)
- Switch between Dark theme / Light theme
- Switch between multiple currencies (USD/CAD/EUR/etc.)
- Connect to custom Explorer URL and custom RPC URL
- Read/Write generic contract calls, for all chain/sidechain
- PhantomCli, which does everything PhantomWallet does, but from CLI

### Upcoming features

- Migrate to a full native UI, or keep a web based UI like Electron
- Improve logic RPC selection (load balance based on response time?)
- Finalize & improve multisignature (revoke conversion / set a max amount per day with no multisig required / trigger events)
- Add feature account Import/Export/login with user/pw (requires db + logic + view)
- Add feature contacts addresses
- Add feature swap to/from NEO/native SOUL
- Add dashboard statistic swapping progress (supply on NEO/native)
- Add dashboard statistics soulmasters / BP
_ Build generic script parser
- Fix when new ABI: with complex types, adjust TokenInfo contract parameter as it does not work yet
- Fix when new ABI: Overall logic check when/if call require signed tx (currently it does call both all the time because of that)

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

If using the wallet on Simnet, you have to go to the Settings Page, and define your own explorer and RPC URLs (which should be http://localhost:7077/rpc for the rpc and http://localhost:7072 for the explorer).

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
