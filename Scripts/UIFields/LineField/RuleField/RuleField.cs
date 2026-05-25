using Newtonsoft.Json.Linq;
using RollPunk.AccessPolicy;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.HierarchyFields;
using RollPunk.Rules;
using System;
using System.Collections.Generic;

namespace RollPunk.UIFields
{
    [EntityType("RuleField")]
    public class RuleField : LineField
    {
        private string _ruleName;

        public string RuleName => _ruleName;
        
        public RuleField(string name, string visibleName, PlayerRole viewAccessLevel, PlayerRole editAcessLevel, string ruleName, int linePriority = 0, Dictionary<string, object> additionalData = null) : base(name, visibleName, viewAccessLevel, editAcessLevel, typeof(RuleFieldAPI), linePriority, additionalData)
        {
            _ruleName = ruleName;
        }

        public RuleField(EntityState entityState) : base(entityState, typeof(RuleFieldAPI)) { }

        public void Execute()
        {
            EntityField entityField = (this as Field).GetEntityAncestor();

            if (entityField == null)
                throw new InvalidOperationException();

            if (entityField.TryGetRule(_ruleName, out Rule rule) == false)
                throw new InvalidOperationException();

            rule.Execute();
        }

        public override object GetRawValue()
        {
            return _ruleName;
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            base.ApplyPayload(payload);

            _ruleName = Get<string>(payload, nameof(RuleName));
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            base.WritePayload(payload);

            Set<string>(payload, nameof(RuleName), RuleName);
        }
    }
}
