namespace RollPunk.Rules
{
    public interface IRuleOwner
    {
        public event Action<Rule> RuleAdded;
        public event Action<Rule> RuleRemoved;

        public IReadOnlyList<Rule> Rules { get; }

        public void AddRule(Rule rule);
        public bool RemoveRule(Rule rule);
    }
}
