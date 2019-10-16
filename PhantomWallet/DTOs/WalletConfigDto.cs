using System.Collections.Generic;
using Newtonsoft.Json;
using Phantasma.RpcClient.Helpers;

namespace Phantom.Wallet.DTOs
{
    public class WalletConfigDto
    {
        [JsonProperty("network")]
        public string Network { get; set; } = "mainnet";

        [JsonProperty("currency")]
        public string Currency { get; set; } = "USD";

        [JsonProperty("theme")]
        public string Theme { get; set; } = "light";

        [JsonProperty("rpc_url")]
        public string RpcUrl { get; set; } = "http://207.148.17.86:7071/rpc";

        [JsonProperty("explorer_url")]
        public string ExplorerUrl { get; set; } = "https://explorer.phantasma.io";

        public static WalletConfigDto FromJson(string json) => JsonConvert.DeserializeObject<WalletConfigDto>(json, JsonUtils.Settings);
        public string ToJson() => JsonConvert.SerializeObject(this, JsonUtils.Settings);
    }
}
