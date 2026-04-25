using RollPunk.Client.Fields;
using RollPunk.Client.Rules;
using RollPunk.Modding.APIs;
using RollPunk.Rules;

namespace RollPunk.Client
{
    internal class Constructor : IAPIHandler
    {
        private FieldsConstructor _fieldsConstructor;
        private RulesConstructor _rulesConstructor;

        private ConstructorAPI _api;
        
        public Constructor(IRuleExecuter modHooker)
        {
            _fieldsConstructor = new FieldsConstructor();
            _rulesConstructor = new RulesConstructor(modHooker);

            _api = new ConstructorAPI(_fieldsConstructor, _rulesConstructor);
        }

        public API GetAPI()
        {
            return _api;
        }
    }
}