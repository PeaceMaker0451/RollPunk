using Godot;
using MoonSharp.Interpreter;
using RollPunk.Fields;
using RollPunk.Modding.APIs;
using System;
using System.Collections.Generic;

namespace RollPunk.Client.Fields
{
    internal sealed class FieldsConstructorAPI : HeldAPI
    {
        private FieldsConstructor _fieldConstructor;

        public FieldsConstructorAPI(FieldsConstructor handler) : base(handler)
        {
            _fieldConstructor = handler;
        }

        //public FieldAPI CreateField(DynValue luaTable)
        //{
        //    try
        //    {
        //        return _fieldConstructor.CreateField(ConvertDynValueToFieldData(luaTable)).GetFieldAPI();
        //    }
        //    catch (Exception e)
        //    {
        //        GD.PushError(e);
        //        throw new ScriptRuntimeException(
        //        $"CreateField failed: {e.Message}"
        //    );
        //    }
        //}

        //private static Dictionary<string, object> ConvertDynValueToFieldData(DynValue luaTable)
        //{
        //    if (luaTable.Type != DataType.Table)
        //    {
        //        throw new ArgumentException("Provided DynValue is not a table.");

        //    }

        //    var fieldDict = new Dictionary<string, object>();

        //    Table table = luaTable.Table;

        //    foreach (var fieldPair in table.Pairs)
        //    {
        //        string key = fieldPair.Key.String;
        //        DynValue value = fieldPair.Value;

        //        fieldDict[key] = ConvertDynValue(value);
        //    }

        //    return fieldDict;
        //}

        //private static object ConvertDynValue(DynValue value)
        //{
        //    switch (value.Type)
        //    {
        //        case DataType.String:
        //            return value.String;
        //        case DataType.Number:
        //            return value.Number;
        //        case DataType.Boolean:
        //            return value.Boolean;
        //        case DataType.Table:
        //            // Recursively convert nested tables
        //            var nestedDict = new Dictionary<string, object>();
        //            foreach (var pair in value.Table.Pairs)
        //            {
        //                nestedDict[pair.Key.String] = ConvertDynValue(pair.Value);
        //            }
        //            return nestedDict;
        //        case DataType.Function:
        //            return value.Function;
        //        default:
        //            return null;
        //    }
        //}
    }
}
