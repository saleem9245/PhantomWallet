using System.Collections.Generic;
using Newtonsoft.Json;
using Phantasma.RpcClient.Helpers;
using Phantom.Wallet.DTOs;

namespace PhantomCli
{
    public class CliWalletConfig : WalletConfigDto
    {
        [JsonProperty("prompt")]
        public string Prompt { get; set; } = "phantom> ";

        new public static CliWalletConfig FromJson(string json) => JsonConvert.DeserializeObject<CliWalletConfig>(json, JsonUtils.Settings);
        new public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
