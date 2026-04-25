using Newtonsoft.Json.Linq;
using RollPunk.AccessPolicy;
using RollPunk.Entities;
using System.Collections.Generic;

namespace RollPunk.UIFields
{
    [EntityType("String")]
    public sealed class StringField : LineField
    {
        public string Value { get; private set; }

        public StringField(string name, string visibleName, PlayerRole viewAccessLevel, PlayerRole editAcessLevel, string value = "", int linePriority = 0, Dictionary<string, object> additionalData = null)
        : base(name, visibleName, viewAccessLevel, editAcessLevel, typeof(StringFieldAPI), linePriority, additionalData)
        {
            Value = value;
        }

        public StringField(EntityState fieldData) : base(fieldData, typeof(StringFieldAPI)) { }

        public override object GetRawValue()
        {
            return Value;
        }

        public void SetValue(string value)
        {
            this.Value = value;
            RaiseValueChanged();
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            base.ApplyPayload(payload);

            Value = Get<string>(payload, nameof(Value));
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            base.WritePayload(payload);

            payload.Add(nameof(Value), Value);
        }
    }
}

