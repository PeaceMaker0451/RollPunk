using Newtonsoft.Json.Linq;
using RollPunk.Entities;
using System;
using System.Collections.Generic;

namespace RollPunk.Scripts.UI.SessionConsole
{
    public enum SourceType
    {
        User,
        System,
        Error
    }
    
    [EntityType("Event")]
    public class Event: Entity
    {
        public string Source;
        public SourceType Type;
        public string Data;
        public DateTime Date;

        public Event(string source, SourceType type, string data, DateTime date)
        {
            Source = source;
            Type = type;
            Data = data;
            Date = date;
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            Source = Get<string>(payload, nameof(Source));
            Data = Get<string>(payload, nameof(Data));
            Date = DateTime.FromBinary(Get<long>(payload, nameof(Date)));
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            Set<string>(payload, nameof(Source), Source);
            Set<string>(payload, nameof(Data), Data);
            Set<long>(payload, nameof(Date), Date.ToBinary());
        }
    }
}
