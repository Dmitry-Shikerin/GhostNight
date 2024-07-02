using Newtonsoft.Json;
using Sources.Frameworks.GameServices.Loads.Domain.Data;

namespace Sources.Frameworks.Domain.Implementation.Data
{
    public class ScoreCounterDto : IDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("totalScore")]
        public int TotalScore { get; set; }
    }
}