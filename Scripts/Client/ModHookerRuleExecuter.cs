using RollPunk.Modding;
using RollPunk.Rules;

namespace RollPunk.Client
{
    internal class ModHookerRuleExecuter : IRuleExecuter
    {
        private ModHooker _modHooker;

        public ModHookerRuleExecuter(ModHooker modHooker)
        {
            _modHooker = modHooker;
        }
        
        public object[] Execute(string eventName, params object[] args)
        {
            return _modHooker.CallHook(eventName, args);
        }
    }
}
