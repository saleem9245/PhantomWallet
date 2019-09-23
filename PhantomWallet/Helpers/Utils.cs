using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Http;
using LunarLabs.Parser.JSON;
using Phantasma.Blockchain.Contracts.Native;
using Phantasma.Cryptography;
using Phantasma.Numerics;
using Phantasma.RpcClient.DTOs;
using Phantasma.Storage;
using Phantom.Wallet.DTOs;
using Phantom.Wallet.Models;
using Newtonsoft.Json;
using Phantasma.CodeGen.Assembler;
using Phantasma.VM;

namespace Phantom.Wallet.Helpers
{
    public static class Utils
    {
        public static string CfgPath { get; } = Path.Combine(Environment.GetFolderPath(
                                         Environment.SpecialFolder.ApplicationData), "phantom_wallet.cfg");

        public static string LogPath { get; } = Path.Combine(Environment.GetFolderPath(
                                         Environment.SpecialFolder.ApplicationData), "phantom_wallet.log");

        public static string GetTxAmount(TransactionDto tx, List<ChainDto> phantasmaChains, List<TokenDto> phantasmaTokens)
        {
            string amountsymbol = null;

            string senderToken = null;
            Address senderChain = Address.FromText(tx.ChainAddress);
            Address senderAddress = Address.Null;

            string receiverToken = null;
            Address receiverChain = Address.Null;
            Address receiverAddress = Address.Null;

            BigInteger amount = 0;

            foreach (var evt in tx.Events) //todo move this
            {
                switch (evt.EventKind)
                {
                    case EventKind.TokenSend:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            senderAddress = Address.FromText(evt.EventAddress);
                            senderToken = data.symbol;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == senderToken).Decimals);
                            amountsymbol = $"{amountDecimal.ToString("#,0.##########")} {senderToken}";
                        }
                        break;

                    case EventKind.TokenReceive:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            receiverAddress = Address.FromText(evt.EventAddress);
                            receiverChain = data.chainAddress;
                            receiverToken = data.symbol;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == receiverToken).Decimals);
                            amountsymbol = $"{amountDecimal.ToString("#,0.##########")} {receiverToken}";
                        }
                        break;

                    case EventKind.TokenEscrow:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            receiverAddress = Address.FromText(evt.EventAddress);
                            receiverChain = data.chainAddress;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == data.symbol).Decimals);
                            amountsymbol = $"{amountDecimal.ToString("#,0.##########")} {data.symbol}";
                        }
                        break;

                    case EventKind.TokenMint:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            receiverAddress = Address.FromText(evt.EventAddress);
                            receiverChain = data.chainAddress;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == data.symbol).Decimals);
                            amountsymbol = $"{amountDecimal.ToString("#,0.##########")} {data.symbol}";
                        }
                        break;

                    case EventKind.TokenStake:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            receiverAddress = Address.FromText(evt.EventAddress);
                            receiverChain = data.chainAddress;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == data.symbol).Decimals);
                            amountsymbol = $"{amountDecimal.ToString("#,0.##########")} {data.symbol}";
                        }
                        break;

                    case EventKind.TokenUnstake:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            receiverAddress = Address.FromText(evt.EventAddress);
                            receiverChain = data.chainAddress;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == data.symbol).Decimals);
                            amountsymbol = $"{amountDecimal.ToString("#,0.##########")} {data.symbol}";
                        }
                        break;

                }
            }

            return amountsymbol;
        }

        public static string GetTxType(TransactionDto tx, List<ChainDto> phantasmaChains, List<TokenDto> phantasmaTokens)
        {
            string typetx = null;

            string senderToken = null;
            Address senderChain = Address.FromText(tx.ChainAddress);
            Address senderAddress = Address.Null;

            string receiverToken = null;
            Address receiverChain = Address.Null;
            Address receiverAddress = Address.Null;

            BigInteger amount = 0;

            foreach (var evt in tx.Events) //todo move this
            {
                switch (evt.EventKind)
                {
                    case EventKind.TokenSend:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            senderAddress = Address.FromText(evt.EventAddress);
                            senderToken = data.symbol;
                        }
                        break;

                    case EventKind.TokenReceive:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            receiverAddress = Address.FromText(evt.EventAddress);
                            receiverChain = data.chainAddress;
                            receiverToken = data.symbol;
                        }
                        break;

                    case EventKind.TokenEscrow:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == data.symbol).Decimals);
                            receiverAddress = Address.FromText(evt.EventAddress);
                            receiverChain = data.chainAddress;
                            var chain = GetChainName(receiverChain.Text, phantasmaChains);
                            typetx = $"Custom";
                        }
                        break;

                    case EventKind.TokenStake:
                        {
                            typetx = $"Stake";
                        }
                        break;

                    case EventKind.TokenUnstake:
                        {
                            typetx = $"Unstake";
                        }
                        break;

                    case EventKind.TokenMint:
                        {
                            typetx = $"Mint";
                        }
                        break;
                }
            }

            if (typetx == null)
            {
                if (amount > 0 && senderAddress != Address.Null && receiverAddress != Address.Null &&
                    senderToken != null && senderToken == receiverToken)
                {
                    typetx = $"{senderAddress.ToString()}";
                }
                else if (amount > 0 && receiverAddress != Address.Null && receiverToken != null)
                {
                    typetx = $"{receiverAddress.ToString()}";
                }
                else
                {
                    typetx = $"Custom";
                }

                if (receiverChain != Address.Null && receiverChain != senderChain)
                {
                    typetx = $"Custom";
                }
            }

            return typetx;
        }

        public static string GetTxDescription(TransactionDto tx, List<ChainDto> phantasmaChains, List<TokenDto> phantasmaTokens, string addressfrom)
        {
            string description = null;

            string senderToken = null;
            Address senderChain = Address.FromText(tx.ChainAddress);
            Address senderAddress = Address.Null;

            string receiverToken = null;
            Address receiverChain = Address.Null;
            Address receiverAddress = Address.Null;

            BigInteger amount = 0;

            foreach (var evt in tx.Events) //todo move this
            {
                switch (evt.EventKind)
                {
                    case EventKind.TokenSend:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            senderAddress = Address.FromText(evt.EventAddress);
                            senderToken = data.symbol;
                        }
                        break;

                    case EventKind.TokenReceive:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            receiverAddress = Address.FromText(evt.EventAddress);
                            receiverChain = data.chainAddress;
                            receiverToken = data.symbol;
                        }
                        break;

                    case EventKind.TokenEscrow:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == data.symbol).Decimals);
                            receiverAddress = Address.FromText(evt.EventAddress);
                            receiverChain = data.chainAddress;
                            var chain = GetChainName(receiverChain.Text, phantasmaChains);
                            description =
                                $"Escrowed for address {receiverAddress} in {chain}";
                        }
                        break;

                    case EventKind.TokenStake:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == data.symbol).Decimals);
                            description = $"Stake transaction";
                        }
                        break;

                    case EventKind.TokenUnstake:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == data.symbol).Decimals);
                            description = $"Unstake transaction";
                        }
                        break;

                    case EventKind.TokenMint:
                        {
                            var data = Serialization.Unserialize<TokenEventData>(evt.Data.Decode());
                            amount = data.value;
                            var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == data.symbol).Decimals);
                            description = $"Claim transaction";
                        }
                        break;

                    case EventKind.AddressRegister:
                        {
                            var name = Serialization.Unserialize<string>(evt.Data.Decode());
                            description = $"{evt.EventAddress} registered the name {name}";
                        }
                        break;

                    case EventKind.AddressLink:
                        {
                            var address = Serialization.Unserialize<Address>(evt.Data.Decode());
                            description = $"{evt.EventAddress} linked to {address.ToString()}";
                        }
                        break;

                    case EventKind.AddressUnlink:
                        {
                            var address = Serialization.Unserialize<Address>(evt.Data.Decode());
                            description = $"{evt.EventAddress} unlinked from {address.ToString()}";
                        }
                        break;
                }
            }

            if (description == null)
            {
                if (amount > 0 && senderAddress != Address.Null && receiverAddress != Address.Null &&
                    senderToken != null && senderToken == receiverToken)
                {
                    var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == senderToken).Decimals);

                    if (addressfrom == senderAddress.ToString())
                    {
                      description =
                          $"Sent to {receiverAddress.ToString()}";
                    }
                    else
                    {
                      description =
                          $"Received from {senderAddress.ToString()}";
                    }

                }
                else if (amount > 0 && receiverAddress != Address.Null && receiverToken != null)
                {
                    var amountDecimal = UnitConversion.ToDecimal(amount, phantasmaTokens.Single(p => p.Symbol == receiverToken).Decimals);

                    description = $"Received from {senderAddress.Text} ";
                }
                else
                {
                    description = "Custom transaction";
                }

                if (receiverChain != Address.Null && receiverChain != senderChain)
                {
                    //description +=
                        //$"From {GetChainName(senderChain.Text, phantasmaChains)} chain to {GetChainName(receiverChain.Text, phantasmaChains)} chain";
                }
            }

            return description;
        }

        public static string DisassembleScript(byte[] script)
        {
            return string.Join('\n', new Disassembler(script).Instructions);
        }

        public static MultisigSettings GetMultisigSettings(string scriptString)
        {
            //HACK TODO (!!!) this method is a very ugly hack, this needs to improve badly
            List<Address> addressList = new List<Address>();
            string[] str = scriptString.Split(' ');
            Regex regex = new Regex(@"LOAD\s*r4,\s(\d*)");
            Match match = regex.Match(scriptString);
            int minSignees = Int32.Parse(match.Value.Split(" ").Last());
            foreach (var s in str)
            {
                if (s.StartsWith("\""))
                {
                    var tempStr = Regex.Replace(s.Replace('"', ' ').Trim(), Environment.NewLine, " ").Split(' ').First();
                    if (tempStr.Length == 45)
                    {
                        try
                        {
                            addressList.Add(Address.FromText(tempStr));
                        }
                        catch {}
                    }
                }
            }

            return new MultisigSettings
            {
                addressCount = addressList.Count,
                signeeCount = minSignees,
                addressArray = addressList.ToArray()
            };
        }
        private static string GetChainName(string address, List<ChainDto> phantasmaChains)
        {
            foreach (var element in phantasmaChains)
            {
                if (element.Address == address) return element.Name;
            }

            return string.Empty;
        }

        public static T ReadConfig<T>(string path)
        {
            path = FixPath(path, true);
            if (!File.Exists(path))
            {
                var file = File.CreateText(path);
                file.Close();
            }

            return (T) JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        public static void WriteConfig<T>(T walletConfig, string path)
        {
            path = FixPath(path, true);
            if (path == null || path == "")
            {
                Console.WriteLine("Path cannot be empty!");
                return;
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(walletConfig));
        }

        public static string FixPath(string path, bool final)
        {
            String platform = System.Environment.OSVersion.Platform.ToString();

            if (platform != "Unix")
            {
                path = path.Replace(@"/", @"\");
                if (!final && !path.EndsWith(@"\"))
                {
                    path += @"\";
                }
            }
            else
            {
                path = path.Replace(@"\", @"/");
                if (!final && !path.EndsWith(@"/"))
                {
                    path += @"/";
                }
            }

            return path;
        }

        public static decimal GetCoinRate(string ticker, string symbol)
        {
            var url = $"https://api.coingecko.com/api/v3/simple/price?ids={ticker}&vs_currencies={symbol}";

            string json;

            try
            {
                using (var httpClient = new HttpClient())
                {
                  json = httpClient.GetStringAsync(new Uri(url)).Result;
                }
                var root = JSONReader.ReadFromString(json);

                root = root[ticker];

                var price = root.GetDecimal(symbol.ToLower());

                return price;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex}");
                return 0;
            }
        }
    }
}
