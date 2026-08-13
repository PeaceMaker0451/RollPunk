using Newtonsoft.Json;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.Players;
using System;
using System.Collections.Generic;

namespace NetcodeCommon
{
    public class SessionPatch
    {
        [JsonProperty] public List<FieldState> PendingFields = new();
        [JsonProperty] public List<Guid> RemoveFields = new();

        [JsonProperty] public Dictionary<Guid, EntityState> PendingPlayers = new();
        [JsonProperty] public List<Guid> RemovePlayers = new();

        [JsonProperty] public Dictionary<Guid, EntityState> PendingOwnerships = new();
        [JsonProperty] public List<Guid> RemoveOwnerships = new();

        [JsonProperty] public List<EntityState> PendingLogs = new();
    }
}
