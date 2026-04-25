using RollPunk.Fields;
using RollPunk.Rules;

namespace RollPunk.HierarchyFields
{
    public sealed class EntityFieldAPI : FieldAPI
    {
        private EntityField _fieldHandler;

        public EntityFieldAPI(EntityField handler) : base(handler)
        {
            _fieldHandler = handler;
        }

        public void addRule(RuleAPI rule)
        {
            _fieldHandler.AddRule(rule.GetRule());
        }

        public bool removeRule(RuleAPI rule)
        {
            return _fieldHandler.RemoveRule(rule.GetRule());
        }

        public RuleAPI? getRule(string name)
        {
            if (_fieldHandler.TryGetRule(name, out var rule) == false)
                return null;

            return (RuleAPI)rule.GetAPI();
        }

        public RuleAPI? getRuleByID(string id)
        {
            if (_fieldHandler.TryGetRule(Guid.Parse(id), out var rule) == false)
                return null;

            return (RuleAPI)rule.GetAPI();
        }
    }
}