using Newtonsoft.Json;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.Players;
using System;
using System.Collections.Generic;

namespace NetcodeCommon
{
    public class SessionState
    {
        [JsonProperty] public List<FieldState> Fields = new();
        [JsonProperty] public List<EntityState> Logs = new();
        [JsonProperty] public Dictionary<Guid, EntityState> Players = new();
    }
}
