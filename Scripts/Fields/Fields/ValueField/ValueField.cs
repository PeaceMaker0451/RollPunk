using Newtonsoft.Json.Linq;
using RollPunk.Entities;
using System;
using System.Collections.Generic;

namespace RollPunk.Fields
{
    public abstract class ValueField : Field
    {
        public event Action ValueChanged;

        public ValueField(string name, Type apiType, Dictionary<string, object> additionalData = null)
        : base(name, apiType, additionalData) { }

        public ValueField(EntityState data, Type apiType) : base(data, apiType) { }

        public abstract object GetRawValue();

        protected void RaiseValueChanged()
        {
            ValueChanged?.Invoke();
            RaiseChanged();
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            base.ApplyPayload(payload);
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            base.WritePayload(payload);
        }
    }
}