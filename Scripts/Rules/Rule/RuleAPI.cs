using MoonSharp.Interpreter;
using RollPunk.Modding.APIs;

namespace RollPunk.Rules
{
    public sealed class RuleAPI : HeldAPI
    {
        private Rule _ruleHandler;

        public string name => _ruleHandler.Name;
        public string hook => _ruleHandler.Hook;
        public string id => _ruleHandler.ID.ToString();

        public API handler => (_ruleHandler.Handler is IAPIHandler apiHandler) ? apiHandler.GetAPI() : null;

        public string[] arguments => _ruleHandler.Arguments.Select(arg => arg.ToString()).ToArray();
        public string[] return_parameters => _ruleHandler.ReturnParameters.Select(arg => arg.ToString()).ToArray();


        public RuleAPI(Rule handler) : base(handler)
        {
            _ruleHandler = handler;
        }
        
        [MoonSharpHidden]
        public Rule GetRule()
        {
            return _ruleHandler;
        }

        public object[] execute(object[] arguments)
        {
            return _ruleHandler.Execute(arguments);
        }
    }
}
