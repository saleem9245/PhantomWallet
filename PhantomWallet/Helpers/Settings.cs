using System;
using Phantom.Wallet.Controllers;

namespace Phantom.Wallet.Helpers
{
    internal static class Settings
    {
        internal static string RpcServerUrl = "http://207.148.17.86:7071/rpc";

        internal static void SetRPCServerUrl()
        {
            RpcServerUrl = AccountController.WalletConfig != null ? AccountController.WalletConfig.RpcUrl : "http://207.148.17.86:7071/rpc";
        }

    }
}

