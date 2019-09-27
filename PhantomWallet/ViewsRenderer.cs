using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using LunarLabs.WebServer.HTTP;
using LunarLabs.WebServer.Templates;
using Phantasma.Blockchain.Contracts.Native;
using Phantasma.Cryptography;
using Phantasma.RpcClient.DTOs;
using Phantasma.Numerics;
using Phantom.Wallet.Controllers;
using Phantom.Wallet.Helpers;
using Phantom.Wallet.DTOs;
using Phantom.Wallet.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;

namespace Phantom.Wallet
{
    public class ViewsRenderer
    {
        private static Logger Log = new LoggerConfiguration().MinimumLevel.Debug()
                                    .WriteTo.File(Utils.LogPath).CreateLogger();

        public ViewsRenderer(HTTPServer server, string viewsPath)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));
            TemplateEngine = new TemplateEngine(server, viewsPath);
            Log.Information(TemplateEngine.ToString());
            Log.Information(server.Settings.Path);
        }

        public void SetupControllers()
        {
            AccountController = new AccountController();
        }

        public void InitAccountController()
        {
            AccountController.ReInit();
        }

        private AccountController AccountController { get; set; }

        public TemplateEngine TemplateEngine { get; set; }

        public string RendererView(Dictionary<string, object> context, params string[] templateList)
        {
            return TemplateEngine.Render(context, templateList);
        }

        static KeyPair GetLoginKey(HTTPRequest request)
        {
            var wif = request.session.GetString("wif");
            var keyPair = KeyPair.FromWIF(wif);
            return keyPair;
        }

        static bool HasLogin(HTTPRequest request)
        {
            return request.session.Contains("login");
        }

        static void PushError(HTTPRequest request, string msg, string code = "0")
        {
            var temp = new ErrorContext() { ErrorDescription = msg, ErrorCode = code };
            request.session.SetStruct<ErrorContext>("error", temp);
        }

        void UpdateHistoryContext(Dictionary<string, object> context, HTTPRequest request)
        {
            if (request.session.Contains("confirmedHash"))
            {
                context["confirmedHash"] = request.session.GetString("confirmedHash");
            }
        }

        void UpdateSendContext(Dictionary<string, object> context, KeyPair keyPair, HTTPRequest request)
        {
            var cache = FindCache(keyPair.Address);

            var availableChains = new List<string>();
            foreach (var token in cache.Tokens)
            {
                if (!availableChains.Contains(token.ChainName))
                {
                    availableChains.Add(token.ChainName);
                }
            }

            context["chainTokens"] = AccountController.PrepareSendHoldings();
            context["availableChains"] = availableChains;
            if (request.session.Contains("error"))
            {
                var error = request.session.GetStruct<ErrorContext>("error");
                context["error"] = error;
                request.session.Remove("error");
            }
        }

        #region Cache

        private static readonly Dictionary<Address, AccountCache> _accountCaches = new Dictionary<Address, AccountCache>();

        private void InvalidateCache(Address address)
        {
            if (_accountCaches.ContainsKey(address))
            {
                _accountCaches.Remove(address);
            }
        }

        private AccountCache FindCache(Address address)
        {
            AccountCache cache;

            var currentTime = DateTime.UtcNow;

            if (_accountCaches.ContainsKey(address))
            {
                cache = _accountCaches[address];
                var diff = currentTime - cache.LastUpdated;

                if (diff.TotalMinutes < 5)
                {
                    return cache;
                }
            }

            cache = new AccountCache()
            {
                LastUpdated = currentTime,
                Holdings = AccountController.GetAccountHoldings(address.Text).Result,
                Tokens = AccountController.GetAccountTokens(address.Text).Result.ToArray(),
                Transactions = AccountController.GetAccountTransactions(address.Text).Result,
                Interops = AccountController.GetAccountInterops(address.Text).Result.ToArray()
            };

            _accountCaches[address] = cache;
            return cache;
        }

        #endregion


        private Dictionary<string, object> InitContext(HTTPRequest request)
        {
            var context = request.session.Data.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (request.session.Contains("error.ErrorDescription")) // TODO this is stupid
            {
                var error = request.session.GetStruct<ErrorContext>("error");
                context["error"] = error;
                request.session.Remove("error");
            }

            var config = AccountController.WalletConfig;

            context["menu"] = MenuEntries;
            context["networks"] = Networks;
            context["explorer"] =  config != null ? config.ExplorerUrl : "http://localhost:7072";

            if (HasLogin(request))
            {
                var keyPair = GetLoginKey(request);

                context["login"] = true;

                context["address"] = keyPair.Address;

                context["chains"] = AccountController.PhantasmaChains;
                context["tokens"] = AccountController.PhantasmaTokens;

                var cache = FindCache(keyPair.Address);

                var entry = MenuEntries.FirstOrDefault(e => e.Id == "history");
                entry.Count = cache.Transactions.Length;

                context["transactions"] = cache.Transactions;
                context["holdings"] = AccountController.GetAccountHoldings(keyPair.Address.Text).Result;
                context["interops"] = cache.Interops;

                if (string.IsNullOrEmpty(AccountController.AccountName))
                {
                    context["name"] = "Anonymous";
                }
                else
                {
                    context["name"] = AccountController.AccountName;
                }
            }

            return context;
        }

        public void SetupHandlers()
        {
            TemplateEngine.Server.Get("/", RouteHome);

            TemplateEngine.Server.Get("/login/{key}", RouteLoginWithParams);

            TemplateEngine.Server.Get("/login", RouteLogin);

            TemplateEngine.Server.Get("/create", RouteCreateAccount);

            TemplateEngine.Server.Post("/sendrawtx", RouteSendRawTx);

            TemplateEngine.Server.Get("/error", RouteError);

            TemplateEngine.Server.Get("/waiting/{txhash}", RouteWaitingTx);

            TemplateEngine.Server.Get("/confirmations/{txhash}", RouteConfirmations);

            TemplateEngine.Server.Post("/register", RouteRegisterName);

            TemplateEngine.Server.Get("/lookupname", RouteLookUpName);

            TemplateEngine.Server.Post("/stake", RouteStake);

            TemplateEngine.Server.Post("/settle/tx", RouteInvokeSettleTx);

            TemplateEngine.Server.Post("/contract", RouteInvokeContract);

            TemplateEngine.Server.Post("/contract/tx", RouteInvokeContractTx);

            TemplateEngine.Server.Post("/contract/abi", RouteContractABI);

            TemplateEngine.Server.Get("/chains", RouteChains);

            TemplateEngine.Server.Get("/platforms", RoutePlatforms);

            TemplateEngine.Server.Post("/registerlink", RouteRegisterLink);

            TemplateEngine.Server.Post("/config", RouteConfig);

            TemplateEngine.Server.Get("/tx/{txhash}", RouteTransaction);

            foreach (var entry in MenuEntries)
            {
                var url = $"/{entry.Id}";

                if (entry.Id == "logout")
                {
                    TemplateEngine.Server.Get(url, RouteLogout);
                }
                else
                {
                    TemplateEngine.Server.Get(url, request => RouteMenuItems(request, url, entry.Id));
                }
            }
        }

        #region Routes
        private HTTPResponse RouteHome(HTTPRequest request)
        {
            return HTTPResponse.Redirect(HasLogin(request) ? "/portfolio" : "/login");
        }

        private object RouteError(HTTPRequest request)
        {
            if (!HasLogin(request))
            {
                return HTTPResponse.Redirect("/login");
            }

            var context = InitContext(request);
            return RendererView(context, "layout", "error");
        }

        private HTTPResponse RouteLoginWithParams(HTTPRequest request)
        {
            var key = request.GetVariable("key");

            AccountController.UpdateConfig(Utils.ReadConfig<WalletConfigDto>(Utils.CfgPath));

            // very ugly needs to be changed
            Settings.SetRPCServerUrl();
            InitAccountController();
            SetupControllers();

            try
            {
                request.session.SetString("wif", key);
                request.session.SetBool("login", true);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                PushError(request, "Error decoding key.");
                return HTTPResponse.Redirect("/login");
            }

            return HTTPResponse.Redirect("/portfolio");
        }

        private string RouteLogin(HTTPRequest request)
        {
            var context = InitContext(request);
            return RendererView(context, "login");
        }

        private string RouteCreateAccount(HTTPRequest request)
        {
            var keyPair = KeyPair.Generate();

            var context = InitContext(request);
            context["WIF"] = keyPair.ToWIF();
            context["address"] = keyPair.Address;


            return RendererView(context, "login");
        }

        private string RouteMultisig(HTTPRequest request)
        {
            //var json  = "{ \"addressCount\": 3, \"signeeCount\": 2, \"addressArray\": [\" ksdlfjksadjfkljsadlkfjasdklf \"]}";
            var settingsJson = request.GetVariable("settings");

            var settings = JsonConvert.DeserializeObject<MultisigSettings>(settingsJson);
            var keyPair = GetLoginKey(request);
            var result = AccountController.CreateMultisigWallet(keyPair, settings);

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        private HTTPResponse RouteLogout(HTTPRequest request)
        {
            var keyPair = GetLoginKey(request);
            InvalidateCache(keyPair.Address);
            request.session.Destroy();
            return HTTPResponse.Redirect("/login");
        }

        private object RouteMenuItems(HTTPRequest request, string url, string entry)
        {
            if (!HasLogin(request))
            {
                return HTTPResponse.Redirect("/login");
            }

            var keyPair = GetLoginKey(request);

            UpdateMenus(entry, url, request);

            var context = InitContext(request);

            context["active"] = entry;

            switch (entry)
            {
                case "history":
                    UpdateHistoryContext(context, request);
                    break;

                case "send":
                    UpdateSendContext(context, keyPair, request);
                    break;

                case "swap":
                    UpdateSendContext(context, keyPair, request);
                    break;

                default: break;
            }

            return RendererView(context, "layout", entry);
        }

        private object RouteSendRawTx(HTTPRequest request)
        {
            if (!HasLogin(request))
            {
                return HTTPResponse.Redirect("/login");
            }

            var isFungible = bool.Parse(request.GetVariable("fungible"));
            var addressTo = request.GetVariable("dest");

            var chainName = request.GetVariable("chain");
            var destinationChain = request.GetVariable("destChain");

            var symbol = request.GetVariable("token");
            var amountOrId = request.GetVariable(isFungible ? "amount" : "id");

            var keyPair = GetLoginKey(request);
            string result;

            // Multisig code, NOT YET DONE
            // HACK if the address has a script it's multisig
            //byte[] addressScript = AccountController.GetAddressScript(keyPair);

            //if (addressScript != null && addressScript.Length > 0)
            //{
            //    if (chainName != destinationChain)
            //    {
            //        // cross chain multisg not supported yet
            //        return null;
            //    }

            //    MultisigSettings settings = AccountController.CheckMultisig(keyPair, addressScript);
            //    result = AccountController.TransferTokens(isFungible
            //                                             ,keyPair
            //                                             ,addressTo
            //                                             ,chainName
            //                                             ,symbol
            //                                             ,amountOrId
            //                                             ,settings).Result;

            //    return "";
            //}

            if (chainName == destinationChain)
            {
                result = AccountController.TransferTokens(isFungible, keyPair, addressTo, chainName, symbol, amountOrId).Result;

                ResetSessionSendFields(request);
            }
            else //cross chain requires 2 txs
            {
                var pathList = AccountController.GetShortestPath(chainName, destinationChain).ToArray();

                request.session.SetInt("txNumber", pathList.Length);

                if (pathList.Length > 2)
                {
                    chainName = pathList[0].Name;
                    destinationChain = pathList[1].Name;

                    // save tx
                    request.session.SetStruct<TransferTx>("transferTx", new TransferTx
                    {
                        IsFungible = isFungible,
                        FromChain = chainName,
                        ToChain = destinationChain,
                        FinalChain = pathList[pathList.Length - 1].Name,
                        AddressTo = addressTo,
                        Symbol = symbol,
                        AmountOrId = amountOrId
                    });

                    result = AccountController.CrossChainTransferToken(isFungible, keyPair, keyPair.Address.Text, chainName, destinationChain, symbol, amountOrId).Result;
                }
                else
                {
                    result = AccountController.CrossChainTransferToken(isFungible, keyPair, addressTo, chainName, destinationChain, symbol, amountOrId).Result;
                }
                if (SendUtils.IsTxHashValid(result))
                {
                    request.session.SetBool("isCrossTransfer", true);
                    request.session.SetStruct<SettleTx>("settleTx",
                        new SettleTx
                        {
                            ChainName = chainName,
                            ChainAddress = AccountController.PhantasmaChains.Find(p => p.Name == chainName).Address,
                            DestinationChainAddress = AccountController.PhantasmaChains.Find(p => p.Name == destinationChain).Address,
                        });
                }
            }

            if (!SendUtils.IsTxHashValid(result))
            {
                PushError(request, result);
                return "";
            }

            return result;
        }

        private object RouteWaitingTx(HTTPRequest request)
        {
            if (!HasLogin(request))
            {
                return HTTPResponse.Redirect("/login");
            }

            var context = InitContext(request);
            context["confirmingTxHash"] = request.GetVariable("txhash");
            context["transferTx"] = request.session.GetStruct<TransferTx>("transferTx");
            return RendererView(context, "layout", "waiting");
        }

        private object RouteConfirmations(HTTPRequest request)
        {
            if (!HasLogin(request))
            {
                return HTTPResponse.Redirect("/login");
            }

            var context = InitContext(request);
            var txHash = request.GetVariable("txhash");

            request.session.SetStruct<ErrorContext>("error", new ErrorContext { ErrorCode = "", ErrorDescription = $"{txHash} is still not confirmed." });
            var result = AccountController.GetTxConfirmations(txHash).Result;

            if (result.GetType() == typeof(ErrorResult))
            {
                return JsonConvert.SerializeObject(result, Formatting.Indented);
            }

            var txObject = (TransactionDto) result;

            if (txObject.Confirmations > 0)
            {
                request.session.SetString("confirmedHash", txHash);
                if (request.session.GetBool("isCrossTransfer"))
                {
                    var settle = request.session.GetStruct<SettleTx>("settleTx");

                    var settleTx = AccountController.SettleBlockTransfer(
                        GetLoginKey(request),
                        settle.ChainAddress,
                        txObject.Txid, settle.DestinationChainAddress).Result;

                    // clear
                    request.session.SetBool("isCrossTransfer", false);

                    if (SendUtils.IsTxHashValid(settleTx))
                    {
                        context["confirmingTxHash"] = settleTx;
                        return "settling";
                    }
                    PushError(request, settleTx);
                    return "unconfirmed";
                }
                else
                {
                    if (request.session.GetInt("txNumber") > 2)
                    {
                        return "continue";
                    }

                    //if it gets here, there are no more txs to process
                    var keyPair = GetLoginKey(request);
                    InvalidateCache(keyPair.Address);

                    ResetSessionSendFields(request);
                    return "confirmed";
                }
            }
            PushError(request, "Error sending tx.");
            return "unconfirmed";
        }

        private object RouteTransaction(HTTPRequest request)
        {
            if (!HasLogin(request))
            {
                return HTTPResponse.Redirect("/login");
            }

            var context = InitContext(request);
            var txHash = request.GetVariable("txhash");

            request.session.SetStruct<ErrorContext>("error", new ErrorContext { ErrorCode = "", ErrorDescription = $"{txHash} is still not confirmed." });
            var result = AccountController.GetTxConfirmations(txHash).Result;

            if (result.GetType() == typeof(ErrorResult))
            {
                return JsonConvert.SerializeObject(result, Formatting.Indented);
            }

            var txObject = (TransactionDto) result;

            if (txObject.Confirmations > 0)
            {
                return JsonConvert.SerializeObject(txObject, Formatting.Indented);
            }

            PushError(request, "Error sending tx.");
            return JsonConvert.SerializeObject(new TransactionDto() {}, Formatting.Indented);
        }

        private object RouteInvokeContractTx(HTTPRequest request)
        {
            var chain = request.GetVariable("chain");
            var contract = request.GetVariable("contract");
            var method = request.GetVariable("method");
            var param = request.GetVariable("params");
            var context = InitContext(request);

            if (param == null) {
                PushError(request, "Parameters cannot be null!");
                return null;
            }

            List<object> paramList = SendUtils.BuildParamList(param);

            if (context["holdings"] is Holding[] balance)
            {
                var kcalBalance = balance.SingleOrDefault(b => b.Symbol == "KCAL" && b.Chain == chain);
                if (kcalBalance.Amount > 0.1m)
                {
                    var keyPair = GetLoginKey(request);
                    InvalidateCache(keyPair.Address);
                    var result = AccountController.InvokeContractTxGeneric(
                            keyPair, chain, contract, method, paramList.ToArray()
                            ).Result;

                    if (result.GetType() == typeof(ErrorResult))
                    {
                        return JsonConvert.SerializeObject(result, Formatting.Indented);
                    }

                    var contractTx = (string)result;

                    if (SendUtils.IsTxHashValid(contractTx))
                    {
                        return contractTx;
                    }

                    PushError(request, contractTx);
                }
                else
                {
                    PushError(request, "You need a small drop of KCAL (+0.1) to call a contract.");
                }
            }
            return null;
        }

        private object RouteStake(HTTPRequest request)
        {
            var stakeAmount = request.GetVariable("stakeAmount");
            var context = InitContext(request);

            if (stakeAmount == null) {
                PushError(request, "stakeAmount cannot be null!");
                return null;
            }

            if (context["holdings"] is Holding[] balance)
            {
              var keyPair = GetLoginKey(request);
              InvalidateCache(keyPair.Address);
              var result = AccountController.CreateStakeSoulTransactionWithClaim(
                      keyPair, stakeAmount
                      ).Result;

              if (result.GetType() == typeof(ErrorResult))
              {
                  return JsonConvert.SerializeObject(result, Formatting.Indented);
              }

              var contractTx = (string)result;

              if (SendUtils.IsTxHashValid(contractTx))
              {
                  return contractTx;
              }

              PushError(request, contractTx);

            }
            return null;
        }

        private object RouteInvokeContract(HTTPRequest request)
        {
            var chain = request.GetVariable("chain");
            var contract = request.GetVariable("contract");
            var method = request.GetVariable("method");
            var param = request.GetVariable("params");
            var context = InitContext(request);

            if (param == null)
	        {
                PushError(request, "Parameters cannot be null!");
                return null;
            }

            List<object> paramList = SendUtils.BuildParamList(param);

            var keyPair = GetLoginKey(request);
            var result = AccountController.InvokeContractGeneric(keyPair, chain, contract, method, paramList.ToArray()).Result;

            if (result != null && result.GetType() == typeof(BigInteger))
	        {
                return result.ToString();
            }

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        private object RouteChains(HTTPRequest request)
        {
            var chains = AccountController.PhantasmaChains;
            string json = JsonConvert.SerializeObject(chains, Formatting.Indented);
            return json;
        }

        private object RoutePlatforms(HTTPRequest request)
        {
            var platforms = AccountController.PhantasmaPlatforms;
            string json = JsonConvert.SerializeObject(platforms, Formatting.Indented);
            return json;
        }

        private object RouteLookUpName(HTTPRequest request)
        {
            var addressName = request.GetVariable("addressName");
            return "addressName";
        }

        private object RouteConfig(HTTPRequest request)
        {
            var mode = request.GetVariable("mode");
            var configStr = request.GetVariable("config");

            WalletConfigDto config = new WalletConfigDto();

            if (mode == "set")
            {
                config = JsonConvert.DeserializeObject<WalletConfigDto>(configStr);
                Utils.WriteConfig<WalletConfigDto>(config, Utils.CfgPath);

                AccountController.UpdateConfig(config);

                // very ugly needs to be changed
                Settings.SetRPCServerUrl();
                InitAccountController();
                SetupControllers();

                var keyPair = GetLoginKey(request);
                InvalidateCache(keyPair.Address);

                return JsonConvert.SerializeObject(config);
            }
            else if (mode == "get")
            {

                config = Utils.ReadConfig<WalletConfigDto>(Utils.CfgPath);
            }

            return JsonConvert.SerializeObject(config);
        }

        private object RouteContractABI(HTTPRequest request)
        {
            var chain = request.GetVariable("chain");
            var contract = request.GetVariable("contract");
            var result = JsonConvert.SerializeObject(AccountController
                    .GetContractABI(chain, contract).Result, Formatting.Indented);
            return result;
        }

        private object RouteInvokeSettleTx(HTTPRequest request)
        {
            var neoTxHash = request.GetVariable("neoTxHash");
            var context = InitContext(request);
            if (context["holdings"] is Holding[] balance)
            {
              var keyPair = GetLoginKey(request);
              var result = AccountController.InvokeSettleTx(keyPair, neoTxHash).Result;
              return result;
            }
            return null;
        }

        private object RouteRegisterLink(HTTPRequest request)
        {
            var targetAddr = request.GetVariable("target");
            var context = InitContext(request);

            if (context["holdings"] is Holding[] balance)
            {
                var kcalBalance = balance.SingleOrDefault(b => b.Symbol == "KCAL" && b.Chain == "main");
                if (kcalBalance.Amount > 0.1m) //RegistrationCost
                {
                    var keyPair = GetLoginKey(request);
                    var result = AccountController.RegisterLink(keyPair, targetAddr).Result;

                    // invalidate cache as account has changed
                    InvalidateCache(keyPair.Address);

                    if (result.GetType() == typeof(ErrorResult))
                    {
                        return JsonConvert.SerializeObject(result, Formatting.Indented);
                    }

                    var registerTx = (string) result;

                    if (SendUtils.IsTxHashValid(registerTx))
                    {
                        return registerTx;
                    }

                    PushError(request, registerTx);
                }
                else
                {
                    PushError(request, "You need a small drop of KCAL (+0.1) to register a name.");
                }
            }
            else
            {
                PushError(request, "Error while registering name.");
            }
            return "";
        }

        private object RouteRegisterName(HTTPRequest request)
        {
            var name = request.GetVariable("name");
            var context = InitContext(request);

            if (context["holdings"] is Holding[] balance)
            {
                var kcalBalance = balance.SingleOrDefault(b => b.Symbol == "KCAL" && b.Chain == "main");
                if (kcalBalance.Amount > 0.1m) //RegistrationCost
                {
                    var keyPair = GetLoginKey(request);
                    var result = AccountController.RegisterName(keyPair, name).Result;

                    if (result.GetType() == typeof(ErrorResult))
                    {
                        return JsonConvert.SerializeObject(result, Formatting.Indented);
                    }

                    var registerTx = (string) result;

                    if (SendUtils.IsTxHashValid(registerTx))
                    {
                        return registerTx;
                    }

                    PushError(request, registerTx);
                }
                else
                {
                    PushError(request, "You need a small drop of KCAL (+0.1) to register a name.");
                }
            }

            else
            {
                PushError(request, "Error while registering name.");
            }
            return "";
        }

        #endregion

        private void ResetSessionSendFields(HTTPRequest request)
        {
            if (request.session.Contains("txNumber"))
            {
                request.session.Remove("txNumber");
            }

            if (request.session.Contains("transferTx"))
            {
                request.session.Remove("transferTx");
            }

            if (request.session.Contains("settleTx"))
            {
                request.session.Remove("settleTx");
            }

            if (request.session.Contains("isCrossTransfer"))
            {
                request.session.Remove("isCrossTransfer");
            }
        }

        private void UpdateMenus(string id, string url, HTTPRequest request)
        {
            request.session.SetString("active", url);
            foreach (var menuEntry in MenuEntries)
            {
                menuEntry.IsSelected = menuEntry.Id == id;
            }
            request.session.SetString("selectedMenu", MenuEntries.SingleOrDefault(m => m.IsSelected).Caption);
        }

        #region UI
        private static readonly MenuEntry[] MenuEntries = new MenuEntry[]
        {
            new MenuEntry(){ Id = "portfolio", Icon = "fa-wallet", Caption = "Portfolio", Enabled = true, IsSelected = true},
            new MenuEntry(){ Id = "send", Icon = "fa-paper-plane", Caption = "Send", Enabled = true, IsSelected = false},
            new MenuEntry(){ Id = "receive", Icon = "fa-qrcode", Caption = "Receive", Enabled = true, IsSelected = false},
            new MenuEntry(){ Id = "swap", Icon = "fa-random", Caption = "Swap", Enabled = true, IsSelected = false},
            new MenuEntry(){ Id = "history", Icon = "fa-receipt", Caption = "History", Enabled = true, IsSelected = false},
            //new MenuEntry(){ Id = "storage", Icon = "fa-hdd", Caption = "Storage", Enabled = true, IsSelected = false},
            //new MenuEntry(){ Id = "exchange", Icon = "fa-chart-bar", Caption = "Exchange", Enabled = true, IsSelected = false},
            //new MenuEntry(){ Id = "sales", Icon = "fa-certificate", Caption = "Crowdsales", Enabled = true, IsSelected = false},
            //new MenuEntry(){ Id = "offline", Icon = "fa-file-export", Caption = "Offline Operation", Enabled = true, IsSelected = false},
            new MenuEntry(){ Id = "contracts", Icon = "fa-file-signature", Caption = "Contracts", Enabled = true, IsSelected = false},
            new MenuEntry(){ Id = "settings", Icon = "fa-cog", Caption = "Settings", Enabled = true, IsSelected = false},
            new MenuEntry(){ Id = "logout", Icon = "fa-sign-out-alt", Caption = "Log Out", Enabled = true, IsSelected = false},
        };

        private static readonly Net[] Networks =
        {
            new Net{Name = "simnet", IsEnabled = true, Value = 1},
            new Net{Name = "testnet", IsEnabled = false, Value = 2},
            new Net{Name = "mainnet", IsEnabled = false, Value = 3},
        };
        #endregion
    }
}
