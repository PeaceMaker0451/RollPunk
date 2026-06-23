using Newtonsoft.Json;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.Players;

namespace NetcodeCommon
{
    public class SessionState
    {
        [JsonProperty] public List<FieldState> Fields = new();
        [JsonProperty] public Dictionary<Guid, EntityState> Players = new();
    }
}
