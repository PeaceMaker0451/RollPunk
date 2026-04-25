using RollPunk.Entities;

namespace RollPunk.Rules
{
    public class RulesContainer : EntityContainer<Rule>
    {
        private bool _allowEqualNames;

        private IRuleOwner _rulesOwner;
        private Dictionary<Guid, Rule> _rules = new();
        private Dictionary<string, Rule> _rulesByNames = new();

        public event Action<Rule> RuleAdded;
        public event Action<Rule> RuleRemoved;

        public IReadOnlyList<Rule> Rules => _rules.Values.ToList();
        public IReadOnlyDictionary<Guid, Rule> RulesDictionary => _rules;

        public RulesContainer(IRuleOwner ruleOwner, bool allowEqualNames = false)
        {
            _rulesOwner = ruleOwner;
            _allowEqualNames = allowEqualNames;

            Added += (rule) => RuleAdded?.Invoke(rule);
            Removed += (rule) => RuleRemoved?.Invoke(rule);
        }

        protected override bool ValidateAdding(Rule rule)
        {
            if(_allowEqualNames == false && _rulesByNames.ContainsKey(rule.Name))
                throw new Exception($"This container already contains rule with name {rule.Name}");

            if (rule.Handler != null && rule.Handler.RemoveRule(rule) == false)
                throw new Exception("Rule re-ownering gone wrong");

            if(_allowEqualNames == false)
                _rulesByNames.Add(rule.Name, rule);

            if(_rulesOwner != null)
                rule.SetHandler(_rulesOwner);
            return true;
        }

        protected override bool ValidateRemoving(Rule rule)
        {
            if (_rulesOwner != null)
                rule.ClearHandler();

            if (_allowEqualNames == false)
                _rulesByNames.Remove(rule.Name);

            return true;
        }
    }
}
