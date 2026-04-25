using Newtonsoft.Json;
using RollPunk.Entities;
using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public class TreeState
    {
        [JsonProperty] public Guid ParentID;
        [JsonProperty] public EntityState State;
        [JsonProperty] public List<TreeState> Children = new();
    }
}
