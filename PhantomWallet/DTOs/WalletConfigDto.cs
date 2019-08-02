using System.Collections.Generic;
using Newtonsoft.Json;
using Phantasma.RpcClient.Helpers;

namespace Phantom.Wallet.DTOs
{
    public class WalletConfigDto
    {
        [JsonProperty("network")]
        public string Network { get; set; } = "simnet";

        [JsonProperty("currency")]
        public string Currency { get; set; } = "USD";

        [JsonProperty("theme")]
        public string Theme { get; set; } = "light";

        [JsonProperty("rpc_url")]
        public string RpcUrl { get; set; } = "http://localhost:7077/rpc";

        [JsonProperty("explorer_url")]
        public string ExplorerUrl { get; set; } = "http://localhost:7072";

        public static WalletConfigDto FromJson(string json) => JsonConvert.DeserializeObject<WalletConfigDto>(json, JsonUtils.Settings);
        public string ToJson() => JsonConvert.SerializeObject(this, JsonUtils.Settings);
    }
}
