using Newtonsoft.Json.Linq;
using RollPunk.Entities;

namespace RollPunk.Logs
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
            : base(date.ToString())
        {
            Source = source;
            Type = type;
            Data = data;
            Date = date;
        }

        public Event(EntityState state) : base(state) { }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            Source = Get<string>(payload, nameof(Source));
            Type = (SourceType)Get<int>(payload, nameof(Type));
            Data = Get<string>(payload, nameof(Data));
            Date = DateTime.FromBinary(Get<long>(payload, nameof(Date)));
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            Set<string>(payload, nameof(Source), Source);
            Set<int>(payload, nameof(Type), (int)Type);
            Set<string>(payload, nameof(Data), Data);
            Set<long>(payload, nameof(Date), Date.ToBinary());
        }
    }
}
