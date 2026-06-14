using RollPunk.Fields;

namespace RollPunk.UIFields
{
    public class RuleFieldAPI : FieldAPI
    {
        RuleField _ruleField;

        public string RuleName => _ruleField.RuleName;
        
        public RuleFieldAPI(RuleField handler) : base(handler)
        {
            _ruleField = handler;
        }

        public void execute() => _ruleField.Execute();

        public void setRuleName(string ruleName) => _ruleField.SetRuleName(ruleName);
    }
}
