using RollPunk.Client.Runtime;
using RollPunk.Modding;
using RollPunk.Rules;

namespace RollPunk.Client
{
    internal class ModHookerRuleExecuter : IRuleExecuter
    {
        private ModHooker _modHooker;
        private MutationCatcher _mutationCatcher;

        public ModHookerRuleExecuter(ModHooker modHooker, MutationCatcher mutationCatcher)
        {
            _modHooker = modHooker;
            _mutationCatcher = mutationCatcher;
        }
        
        public object[] Execute(string eventName, params object[] args)
        {
            if (_mutationCatcher != null)
                return _modHooker.BatchHook(_mutationCatcher, eventName, args);
            else
                return _modHooker.CallHook(eventName, args);
        }
    }
}
