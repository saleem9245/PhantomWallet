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

## Phantom Wallet Overview

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
- PhantomCli

### Roadmap

- Migrate to a full native UI, or keep a web based UI like Electron
- Improve logic RPC selection (load balance based on response time?)
- Finalize & improve multisignature (revoke conversion / set a max amount per day with no multisig required / trigger events)
- Add feature account Import/Export/login with user/pw (requires db + logic + view)
- Add feature contacts addresses
- Add feature swap to/from NEO/native SOUL
- Add dashboard statistic swapping progress (supply on NEO/native)
- Add dashboard statistics soulmasters / BP
- Build generic script parser
- Fix when new ABI: with complex types, adjust TokenInfo contract parameter as it does not work yet
- Fix when new ABI: Overall logic check when/if call require signed tx (currently it does call both all the time because of that)
- Offline mode

## Phantom Cli Overview

### Current features

PhantomCli is a cli interface that allows quick queries on transactions or to check a contract abi. It has a history and autocompletion for the built in commands and on historized patterns. Not everything might work smoothly as I abused it a bit to test a few things, but in the future PhantomCli will be a full featured Phantasma CLI wallet with extended functionalities.

### Roadmap

- Simple scripting within the cli
- File read write
- Transaction/block export
- Standard wallet features (send/receive)
- Passthrough to your OS shell (Linux and OSX, not sure about Windows yet)
- Output in nicely formatted text tables

## Phantom Wallet & CLI Installation

The wallet can be launched either from the latest binaries, or can be built manually

### Required Tools and Dependencies

Dependencies (to build with testnet or mainnet):

- .NET Core 2.0 (current release: https://dotnet.microsoft.com/download/dotnet-core/)
- PhantasmaChain (https://github.com/phantasma-io/PhantasmaChain)
- PhantasmaRpcClient (https://github.com/phantasma-io/PhantasmaRpcClient)

Dependencies (to build with simnet):

- .NET Core 2.0 (current release: https://dotnet.microsoft.com/download/dotnet-core/)
- PhantasmaChain (https://github.com/phantasma-io/PhantasmaChain)
- PhantasmaRpcClient (https://github.com/phantasma-io/PhantasmaRpcClient)
- PhantasmaSpook (https://github.com/phantasma-io/PhantasmaSpook)
- PhantasmaExplorer (https://github.com/phantasma-io/PhantasmaExplorer)

In any network type selected (Simnet/Testnet/Mainnet), you have the possibility to define your own RPC node and Explorer. Just go to the Settings Page, and define your own explorer and RPC URLs (which should be http://localhost:7077/rpc for the rpc and http://localhost:7072 for the explorer by default, which are used for a local spook and local explorer).

### Launch from binaries

*Phantom Wallet:*

1) Download here (https://github.com/merl111/PhantomWallet/releases)

2.1) One step launch

`start.[sh,command,bat]`

The wallet runs locally, but its ui is currently in the browser, executing the start script opens a shell or cmd, and a browser with the wallets login page, you have to close the shell, cmd window after you closed the browser window to fully close the wallet.

2.2) Two steps launch

Within the unzipped directory type the following command:

on Linux or MacOS:

`./PhantomWallet --path=www --port=7071`

on Windows:

`.\PhantomWallet.exe --path=www --port=7071`

Then open a browser and go to http://localhost:7071
If using on Windows, you might need to disable your antivirus before launching the wallet, as sometimes it gets detected as a false positive.

*Phantom Cli:*

1) Download here (https://github.com/merl111/PhantomWallet/releases)

2) Launch the program

on Linux or MacOS:

`./PhantomCli`

on Windows:

`.\PhantomCli.exe --path=www --port=7071`

### Manual build

*Phantom Wallet:*

Go to PhantomWallet directory and type:

`dotnet build`

`dotnet PhantomWallet/www/bin/netcoreapp2.0/PhantomWallet.dll --path=/pathtowallet/PhantomWallet/www/ --port=7071`

*Phantom Cli:*

Go to PhantomCli directory and type:

`dotnet build`

`dotnet PhantomCli/bin/netcoreapp2.0/PhantomCli.dll`

## Multisignature

Currently, multisignature addresses are being tested and we are almost there. After finishing we will provide a seamless experience almost identical to standard transactions.

If you want to run the current multisignature tests feel free to head over to `Phantom.Tests` and type:

`dotnet test`

### How does multisig work with PhantomWallet

Phantasma addresses have a feature which allows you to attach a script onto your address which is executed before every action (receive, send, mint, burn). To convert you address to a multisig address you have to attach a script that does the verification, means it verifies how many participants have signed the transaction.

If enough have signed, the script returns normally and the transaction is allowed to be executed. If not enough wallets have signed the transaction, the script will return with an error which results in a fail during transaction processing.

Currently address scripts are immutable, so think twice or better three times if you want to attach a script to your address, it could result in a loss of funds if the script is faulty.

The current multisig script used for the tests:

```
 load r1 0
 load r2 0x7125C3AE45BE83F2CD9BA8162B11E151C88D92AB042C75F81626C7BD92A066D9
 put r2 r3 r1
 load r1 1
 load r2 0x722FD6C9100C35B20C942F4292ECCDC229763654D39135B3A3D44B4D4455EA73
 put r2 r3 r1
 load r1 2
 load r2 0x8FD2B3A13DFA9F79CE80A9EBA2FCC1EED2689B5BE45E21590E6BD6F84E5D79E2
 put r2 r3 r1
 alias r4 $minSigned
 alias r5 $addrCount
 alias r6 $i
 alias r7 $loopResult
 alias r8 $interResult
 alias r9 $result
 alias r10 $signed
 alias r11 $temp
 alias r12 $true
 alias r13 $triggerSend
 alias r14 $currentTrigger
 alias r15 $false
 load $false False
 load $triggerSend, "OnSend"
 pop $currentTrigger
 equal $triggerSend, $currentTrigger, $result
 jmpnot $result, @finishNotSend
 load $minSigned 3
 load $addrCount 3
 load $signed 0
 load $true True
 load $i 0
 @loop:
 lt $i $addrCount $loopResult
 jmpnot $loopResult @checkSigned
 get r3 $temp $i
 push $temp
 extcall "Address()"
 call @checkWitness
 equal $interResult, $true, $result
 jmpif $result @increm
 inc $i
 jmp @loop
 @increm:
 inc $signed
 inc $i
 jmp @loop
 @checkSigned:
 gte $signed $minSigned $result
 jmpif $result @finish
 jmpnot $result @break
 ret
 @finish:
 push $result
 ret
 @finishNotSend:
 push $true
 ret
 @break:
 throw
 @checkWitness:
 extcall "CheckWitness()"
 pop $interResult
 ret
```
