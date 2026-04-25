using Newtonsoft.Json.Linq;
using RollPunk.Entities;
using RollPunk.Modding.APIs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollPunk.Rules
{
    [EntityType("Rule")]
    public sealed class Rule : Entity, IAPIHandler
    {
        private RuleAPI _api;

        private (string Name, Type Type)[] _argumentPairs;
        private (string Name, Type Type)[] _returnPairs;

        public string Hook { get; private set; }
        public IRuleOwner Handler { get; private set; }
        public IRuleExecuter Executer { get; private set; }
        public IReadOnlyCollection<(string Name, Type Type)> Arguments => _argumentPairs;
        public IReadOnlyCollection<(string Name, Type Type)> ReturnParameters => _returnPairs;

        public Rule(string name, string hook, IRuleExecuter hooker, ICollection<(string Name, Type Type)> argPairs = null, ICollection<(string Name, Type Type)> returnPairs = null)
        {
            Name = name;
            Hook = hook;

            _api = new RuleAPI(this);
            Executer = hooker;

            _argumentPairs = argPairs?.ToArray() ?? Array.Empty<(string, Type)>();
            _returnPairs = returnPairs?.ToArray() ?? Array.Empty<(string, Type)>();
        }

        public Rule(EntityState data, IRuleExecuter hooker) : base(data)
        {
            _api = new RuleAPI(this);
            Executer = hooker;
        }

        public Rule(EntityState data) : base(data)
        {
            _api = new RuleAPI(this);
        }

        public void SetHandler(IRuleOwner handler)
        {
            Handler = handler;
        }

        public void ClearHandler()
        {
            Handler = null;
        }

        public void SetExecuter(IRuleExecuter executer)
        {
            Executer = executer;
        }

        public object[] Execute(params object[] arguments)
        {
            if (Executer == null)
                throw new InvalidOperationException("Unable to execute rule: Rule Executer is not set or is null");

            object[] finalArgs = new object[(arguments != null) ? (arguments.Length + 1) : 1];
            if (Handler is IAPIHandler apiHandler)
                finalArgs[0] = apiHandler.GetAPI();

            ValidateArguments(arguments);

            for (int i = 0; i < arguments.Length; i++)
                finalArgs[i + 1] = arguments[i];

            object[] returnValues = Executer.Execute(Hook, finalArgs);

            ValidateReturnValues(returnValues);

            return returnValues;
        }

        public API GetAPI()
        {
            return _api;
        }

        private void ValidateReturnValues(object[] returnValues)
        {
            if (_returnPairs == null)
                return;

            if (returnValues.Length != _returnPairs.Length)
                throw new Exception($"Invalid number of return parameters - {returnValues.Length}/{_returnPairs.Length}");

            for (int i = 0; i < returnValues.Length; i++)
            {
                Type realArgType = returnValues[i].GetType();
                Type neededType = _returnPairs[i].Type;

                if (realArgType != neededType)
                    throw new Exception($"Invalid type for return parameter '{_returnPairs[i].Name}' - {realArgType} (expected {neededType})");
            }
        }

        private void ValidateArguments(object[] arguments)
        {
            if (_argumentPairs == null)
                return;

            if (arguments.Length != _argumentPairs.Length)
                throw new Exception($"Invalid number of arguments - {arguments.Length}/{_argumentPairs.Length}");

            for (int i = 0; i < arguments.Length; i++)
            {
                Type realArgType = arguments[i].GetType();
                Type neededType = _argumentPairs[i].Type;

                if (realArgType != neededType)
                    throw new Exception($"Invalid type for argument '{_argumentPairs[i].Name}' - {realArgType} (expected {neededType})");
            }
        }

        protected override void ApplyPayload(Dictionary<string, JToken> payload)
        {
            // Parse argument types and names
            var args = Get<string[]>(payload, nameof(Arguments));
            _argumentPairs = new (string, Type)[args.Length / 2];
            for (int i = 0; i < args.Length; i += 2)
            {
                _argumentPairs[i / 2] = (args[i], Type.GetType(args[i + 1]));
            }

            // Parse return parameter types and names
            var returnParameters = Get<string[]>(payload, nameof(ReturnParameters));
            _returnPairs = new (string, Type)[returnParameters.Length / 2];
            for (int i = 0; i < returnParameters.Length; i += 2)
            {
                _returnPairs[i / 2] = (returnParameters[i], Type.GetType(returnParameters[i + 1]));
            }
        }

        protected override void WritePayload(Dictionary<string, JToken> payload)
        {
            // Convert arguments and return parameters to name-type pairs
            var argumentPairs = _argumentPairs.Select(pair => new[] { pair.Name, pair.Type.AssemblyQualifiedName }).SelectMany(x => x).ToArray();
            Set(payload, nameof(Arguments), argumentPairs);

            var returnPairs = _returnPairs.Select(pair => new[] { pair.Name, pair.Type.AssemblyQualifiedName }).SelectMany(x => x).ToArray();
            Set(payload, nameof(ReturnParameters), returnPairs);
        }
    }
}