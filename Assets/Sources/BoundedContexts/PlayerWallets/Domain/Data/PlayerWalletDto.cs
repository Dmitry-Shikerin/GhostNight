using Newtonsoft.Json;
using Sources.Frameworks.GameServices.Loads.Domain.Data;

namespace Sources.BoundedContexts.PlayerWallets.Domain.Data
{
    public class PlayerWalletDto : IDto
    {
        [JsonProperty("coins")]
        public int Coins { get; set; }
        
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}