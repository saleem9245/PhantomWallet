using System;
using Phantom.Wallet.Controllers;

namespace Phantom.Wallet.Helpers
{
    internal static class Settings
    {
        internal static string RpcServerUrl = "http://localhost:7077/rpc";

        internal static void SetRPCServerUrl()
        {
            RpcServerUrl = AccountController.WalletConfig != null ? AccountController.WalletConfig.RpcUrl : "http://localhost:7077/rpc";
        }

    }
}

