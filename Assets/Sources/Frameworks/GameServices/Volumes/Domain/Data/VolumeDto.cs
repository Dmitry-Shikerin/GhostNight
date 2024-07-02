using Newtonsoft.Json;
using Sources.Frameworks.GameServices.Loads.Domain.Data;

namespace Sources.Frameworks.GameServices.Volumes.Domain.Data
{
    public class VolumeDto : IDto
    {
        [JsonProperty("musicValue")]
        public float MusicValue { get; set; }
        
        [JsonProperty("miniGunVolumeValue")]
        public float MiniGunVolumeValue { get; set; }
        
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}