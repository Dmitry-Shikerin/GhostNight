using Newtonsoft.Json;
using Sources.Frameworks.GameServices.Loads.Domain.Data;

namespace Sources.BoundedContexts.Tutorials.Domain.Data
{
    public class TutorialDto : IDto
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("hasCompleted")]
        public bool HasCompleted { get; set; }
    }
}