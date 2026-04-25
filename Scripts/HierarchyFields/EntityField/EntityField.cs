using Newtonsoft.Json.Linq;
using RollPunk.Entities;
using RollPunk.Fields;
using RollPunk.Rules;
using System.Runtime.InteropServices;

namespace RollPunk.HierarchyFields
{
    [EntityType("EntityField")]
    public sealed class EntityField : Field, IRuleOwner
    {
        private Dictionary<string, Guid> _rulesByNames = new();
        private Dictionary<Guid, Rule> _rules = new();

        private IRuleExecuter _ruleExecuter;

        public event Action<Rule> RuleAdded;
        public event Action<Rule> RuleRemoved;

        public IReadOnlyList<Rule> Rules => _rules.Values.ToList();

        public EntityField(string name, Dictionary<string, object> additionalData = null)
            : base(name, typeof(EntityFieldAPI), additionalData) { }

        public EntityField(EntityState data) : base(data, typeof(EntityFieldAPI)) { }

        public void SetRulesExecuter(IRuleExecuter ruleExecuter)
        {
            _ruleExecuter = ruleExecuter;
            
            foreach(var rule in _rules.Values)
                rule.SetExecuter(ruleExecuter);
        }

        public void AddRule(Rule rule)
        {
            if (_rulesByNames.ContainsKey(rule.Name))
                throw new InvalidOperationException($"Entity field already contains rule with name {rule.Name}");

            if (_rules.ContainsKey(rule.ID))
                throw new InvalidOperationException($"Entity field already contains rule with ID {rule.ID}");

            _rulesByNames.Add(rule.Name, rule.ID);
            _rules.Add(rule.ID, rule);

            rule.SetExecuter(_ruleExecuter);
            rule.SetHandler(this);

            RuleAdded?.Invoke(rule);
        }

        public bool RemoveRule(Rule rule)
        {
            if(_rules.ContainsKey(rule.ID) == false)
                return false;

            _rules.Remove(rule.ID);

            if (_rulesByNames.ContainsKey(rule.Name) == false)
                throw new Exception($"Entity field contained rule with ID {rule.ID}, but don't contains rule with name {rule.Name}");

            rule.SetExecuter(null);
            rule.ClearHandler();

            _rulesByNames.Remove(rule.Name);
            RuleRemoved?.Invoke(rule);
            return true;
        }

        public bool TryGetRule(string name, out Rule? rule)
        {
            rule = null;

            if (_rulesByNames.TryGetValue(name, out var id) == false)
                return false;

            return _rules.TryGetValue(id, out rule);
        }

        public bool TryGetRule(Guid id, out Rule? rule)
        {
            rule = null;
            return _rules.TryGetValue(id, out rule);
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            base.ApplyPayload(payload);

            _rulesByNames = Get<Dictionary<string, Guid>>(payload, nameof(_rulesByNames));
            var rules = Get<List<EntityState>>(payload, nameof(_rules));

            foreach (var rule in rules)
                _rules.Add(rule.ID, new(rule));
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            base.WritePayload(payload);

            Set(payload, nameof(_rulesByNames), _rulesByNames);

            List<EntityState> rules = new();

            foreach (var rule in _rules.Values)
                rules.Add(rule.GetState());

            Set(payload, nameof(_rules), rules);
        }
    }
}