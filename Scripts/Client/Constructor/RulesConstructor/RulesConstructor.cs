using RollPunk.Modding;
using RollPunk.Rules;
using System;
using System.Collections.Generic;

namespace RollPunk.Client.Rules
{
    internal class RulesConstructor
    {
        private IRuleExecuter _modHooker;

        private const string NameFieldName = "name";
        private const string TypeFieldName = "type";
        private const string HookFieldName = "hook";
        private const string ArgumentsFieldName = "argument";
        private const string ReturnParametersFieldName = "return_parameters";

        private Dictionary<string, Func<Dictionary<string, object>, Rule>> _rulesConstructors;
        private Dictionary<string, Type> _types;

        public RulesConstructor(IRuleExecuter modHooker)
        {
            _modHooker = modHooker;

            _rulesConstructors = new Dictionary<string, Func<Dictionary<string, object>, Rule>>
            {
                { nameof(Rule), CreateBaseRule }
            };

            _types = new Dictionary<string, Type>
            {
                { "string", typeof(string) },
                { "int", typeof(int) },
                { "float", typeof(float) },
                { "bool", typeof(bool) },
            };
        }

        public Rule CreateRule(Dictionary<string, object> ruleData)
        {
            string type = GetStringValue(ruleData, TypeFieldName);

            if (_rulesConstructors.TryGetValue(type, out var constructor))
            {
                return constructor(ruleData);
            }

            throw new ArgumentException($"Unknown rule type: {type}");
        }

        private Rule CreateBaseRule(Dictionary<string, object> ruleData)
        {
            string name = GetStringValue(ruleData, NameFieldName);
            string hook = GetStringValue(ruleData, HookFieldName);

            var rawArguments = GetDictionaryValue(ruleData, ArgumentsFieldName);
            List<(string, Type)> arguments = new();
            foreach(var arg in rawArguments)
            {
                if (arg.Value is string typeString == false)
                    throw new ArgumentException($"Unnable to convert to string type of argument {arg.Key}");
                
                if (typeString == "any")
                    arguments.Add((arg.Key, typeof(object)));
                else if (_types.TryGetValue(typeString, out var type) == false)
                    throw new InvalidOperationException($"Unable to convert to type argument {arg.Key} ({typeString})");
                else
                    arguments.Add((arg.Key, type));
            }

            var rawReturnParameters = GetDictionaryValue(ruleData, ReturnParametersFieldName);
            List<(string, Type)> returnParameters = new();
            foreach (var parameter in rawReturnParameters)
            {
                if (parameter.Value is string typeString == false)
                    throw new ArgumentException($"Unnable to convert to string type of return parameter {parameter.Key}");

                if (typeString == "any")
                    returnParameters.Add((parameter.Key, typeof(object)));
                else if (_types.TryGetValue(typeString, out var type) == false)
                    throw new InvalidOperationException($"Unable to convert to type return parameter {parameter.Key} ({typeString})");
                else
                    returnParameters.Add((parameter.Key, type));
            }

            return new Rule(name, hook, _modHooker, arguments, returnParameters);
        }

        private string GetStringValue(Dictionary<string, object> fieldData, string key, string defaultValue = "")
        {
            return fieldData.TryGetValue(key, out object value) ? value as string ?? defaultValue : defaultValue;
        }

        private bool GetBoolValue(Dictionary<string, object> fieldData, string key, bool defaultValue = true)
        {
            return fieldData.TryGetValue(key, out object value) ? value is bool b ? b : defaultValue : defaultValue;
        }

        private int GetIntValue(Dictionary<string, object> fieldData, string key, int defaultValue = 0)
        {
            return fieldData.TryGetValue(key, out object value) ? value is double d ? Convert.ToInt32(d) : defaultValue : defaultValue;
        }

        private Dictionary<string, object> GetDictionaryValue(Dictionary<string, object> fieldData, string key)
        {
            return fieldData.TryGetValue(key, out object value) ? value as Dictionary<string, object> ?? new Dictionary<string, object>() : new Dictionary<string, object>();
        }
    }
}
