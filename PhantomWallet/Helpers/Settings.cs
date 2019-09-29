using System;
using Phantom.Wallet.Controllers;

namespace Phantom.Wallet.Helpers
{
    internal static class Settings
    {
        internal static string RpcServerUrl = "http://45.76.88.140:7076/rpc";

        internal static void SetRPCServerUrl()
        {
            RpcServerUrl = AccountController.WalletConfig != null ? AccountController.WalletConfig.RpcUrl : "http://45.76.88.140:7076/rpc";
        }

    }
}

