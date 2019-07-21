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

        public static WalletConfigDto FromJson(string json) => JsonConvert.DeserializeObject<WalletConfigDto>(json, JsonUtils.Settings);
        public string ToJson() => JsonConvert.SerializeObject(this, JsonUtils.Settings);
    }
}
