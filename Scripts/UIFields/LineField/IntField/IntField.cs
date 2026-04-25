using Newtonsoft.Json.Linq;
using RollPunk.AccessPolicy;
using RollPunk.Entities;
using System;
using System.Collections.Generic;
namespace RollPunk.UIFields
{
    [EntityType("Int")]
    public sealed class IntField : LineField
    {
        public event Action MaxValueChanged;
        public event Action MinValueChanged;

        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }
        public int Value { get; private set; }

        public IntField(string name, string visibleName, PlayerRole viewAccessLevel, PlayerRole editAcessLevel, int value = 0, int minValue = 0, int maxValue = 100, int linePriority = 0, Dictionary<string, object> additionalData = null)
        : base(name, visibleName, viewAccessLevel, editAcessLevel, typeof(IntFieldAPI), linePriority, additionalData)
        {
            Value = value;
            MaxValue = maxValue;
            MinValue = minValue;
        }

        public IntField(EntityState fieldData) : base(fieldData, typeof(IntFieldAPI)) { }

        public override object GetRawValue()
        {
            return Value;
        }

        public void SetValue(int value)
        {
            value = Math.Clamp(value, MinValue, MaxValue);

            if (value == Value)
                return;

            Value = value;
            RaiseValueChanged();
        }

        public void SetMaxValue(int maxValue)
        {
            MaxValue = maxValue;
            MaxValueChanged?.Invoke();

            if (Value > MaxValue)
                SetValue(MaxValue);

            RaiseChanged();
        }

        public void SetMinValue(int minValue)
        {
            MinValue = minValue;
            MinValueChanged?.Invoke();

            if (Value < MinValue)
                SetValue(MinValue);

            RaiseChanged();
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            base.ApplyPayload(payload);

            Value = Get<int>(payload, nameof(Value));
            MaxValue = Get<int>(payload, nameof(MaxValue));
            MinValue = Get<int>(payload, nameof(MinValue));
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            base.WritePayload(payload);

            payload.Add(nameof(Value), Value);
            payload.Add(nameof(MaxValue), MaxValue);
            payload.Add(nameof(MinValue), MinValue);
        }
    }
}

