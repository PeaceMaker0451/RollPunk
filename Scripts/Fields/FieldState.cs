using Newtonsoft.Json;
using RollPunk.Entities;
using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public class FieldState
    {
        [JsonProperty] public Guid? ParentID;
        [JsonProperty] public EntityState State;
        [JsonProperty] public List<FieldState> Children = new();
    }
}
