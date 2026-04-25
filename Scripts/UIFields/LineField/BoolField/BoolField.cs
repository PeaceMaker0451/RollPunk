using Newtonsoft.Json.Linq;
using RollPunk.AccessPolicy;
using RollPunk.Entities;
using System.Collections.Generic;

namespace RollPunk.UIFields
{
    [EntityType("Bool")]
    public sealed class BoolField : LineField
    {
        public bool Value { get; private set; }

        public BoolField(string name, string visibleName, PlayerRole viewAccessLevel, PlayerRole editAcessLevel, bool value = false, int linePriority = 0, Dictionary<string, object> additionalData = null)
        : base(name, visibleName, viewAccessLevel, editAcessLevel, typeof(BoolFieldAPI), linePriority, additionalData)
        {
            Value = value;
        }

        public BoolField(EntityState fieldData) : base(fieldData, typeof(BoolFieldAPI)) { }

        public void SetValue(bool value)
        {
            Value = value;
            RaiseValueChanged();
        }

        public override object GetRawValue()
        {
            return Value;
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            base.ApplyPayload(payload);

            Value = Get<bool>(payload, nameof(Value));
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            base.WritePayload(payload);

            payload.Add(nameof(Value), Value);
        }
    }
}

