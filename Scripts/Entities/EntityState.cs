using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace RollPunk.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EntityState
    {
        [JsonProperty] public string EntityType;
        [JsonProperty] public string Name;
        [JsonProperty] public Guid ID;
        [JsonProperty] public Dictionary<string, JToken> Payload;

        public override string ToString()
        {
            return $"({EntityType}) {Name} [{ID}]";
        }
    }
}