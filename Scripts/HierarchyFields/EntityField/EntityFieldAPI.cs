using RollPunk.Fields;
using RollPunk.Rules;
using System;

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
            return _fieldHandler.GetRule(name)?.GetAPI() as RuleAPI;
        }

        public RuleAPI? getRuleByID(string id)
        {
            Guid guid = Guid.Parse(id);

            if (guid == Guid.Empty)
                return null;
            
            return _fieldHandler.GetRuleById(guid)?.GetAPI() as RuleAPI;
        }
    }
}