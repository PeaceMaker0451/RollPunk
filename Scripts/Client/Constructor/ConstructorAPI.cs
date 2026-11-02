using MoonSharp.Interpreter;
using RollPunk.Client.Fields;
using RollPunk.Client.Rules;
using RollPunk.Debug;
using RollPunk.Fields;
using RollPunk.Modding;
using RollPunk.Modding.APIs;
using RollPunk.Rules;
using System;
using System.Collections.Generic;

namespace RollPunk.Client
{
    internal sealed class ConstructorAPI : API
    {
        private FieldsConstructor _fieldConstructor;
        private RulesConstructor _rulesConstructor;

        public ConstructorAPI(FieldsConstructor fieldsConstructor, RulesConstructor rulesConstructor)
        {
            _fieldConstructor = fieldsConstructor;
            _rulesConstructor = rulesConstructor;
        }

        public FieldAPI createField(DynValue luaTable)
        {
            var api = _fieldConstructor.CreateField(ConvertDynValueToConstructParametersDictionary(luaTable)).GetAPI() as FieldAPI;
            RPDebug.DebugLog($"[color=salmon][i]Конструктор создал тип {api.GetType()}[/i][/color]");
            return api;
        }

        public RuleAPI createRule(DynValue luaTable)
        {
            var api = _rulesConstructor.CreateRule(ConvertDynValueToConstructParametersDictionary(luaTable)).GetAPI() as RuleAPI;
            RPDebug.DebugLog($"[color=salmon][i]Конструктор создал тип {api.GetType()}[/i][/color]");
            return api;
        }

        private static Dictionary<string, object> ConvertDynValueToConstructParametersDictionary(DynValue luaTable)
        {
            if (luaTable.Type != DataType.Table)
            {
                throw new ArgumentException("Provided DynValue is not a table.");

            }

            var fieldDict = new Dictionary<string, object>();

            Table table = luaTable.Table;

            foreach (var fieldPair in table.Pairs)
            {
                string key = fieldPair.Key.String;
                DynValue value = fieldPair.Value;

                fieldDict[key] = ConvertDynValue(value);
            }

            return fieldDict;
        }

        private static object ConvertDynValue(DynValue value)
        {
            switch (value.Type)
            {
                case DataType.String:
                    return value.String;
                case DataType.Number:
                    return value.Number;
                case DataType.Boolean:
                    return value.Boolean;
                case DataType.Table:
                    // Recursively convert nested tables
                    var nestedDict = new Dictionary<string, object>();
                    foreach (var pair in value.Table.Pairs)
                    {
                        string key = pair.Key.String;
                        if (key == null)
                        {
                            RPDebug.LogError($"невозможно прочитать пару - {pair.Key} - {pair.Value}");
                            continue;
                        } 

                        nestedDict[key] = ConvertDynValue(pair.Value);
                    }
                    return nestedDict;
                case DataType.Function:
                    return value.Function;
                default:
                    return null;
            }
        }
    }
}